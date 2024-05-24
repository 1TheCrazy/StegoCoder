using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;

namespace StenoCoderCmd.Converter;

public class StenoConverter
{
    public static string DecodeImage(string imagePath)
    {
        try
        {
            return DecodeBitsAsString(GetLBSFromImage(GetImage(imagePath)));
        }
        catch (Exception ex)
        {
            return $"There was an error trying to decode the image...\n{ex.Message}\n{ex.StackTrace}";
        }
    }

    public static bool EncodeImage(string imagePath, string text, string encodedImagePath = "")
    {
        try
        {
            var pngEncoder = new PngEncoder
            {
                CompressionLevel = PngCompressionLevel.NoCompression,
                BitDepth = PngBitDepth.Bit8,
                ColorType = PngColorType.RgbWithAlpha
            };

            EncodeLSBInImageBytes(TextToBits(text).ToList(), GetImage(imagePath)).SaveAsPng(imagePath, pngEncoder);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"There was an error trying to encode the image...\n{ex.Message}\n{ex.StackTrace}");
            return false;
        }
    }

    public static Image<Rgba32> GetImage(string imagePath)
    {
        return Image.Load<Rgba32>(imagePath);
    }

    public static bool[] GetLBSFromImage(Image<Rgba32> image)
    {
        List<bool> bits = new List<bool>();

        image.ProcessPixelRows(d => {
            for (int y = 0; y < d.Height; y++)
            {
                Span<Rgba32> pixelRow = d.GetRowSpan(y);

                for (int x = 0; x < pixelRow.Length; x++)
                {
                    if ((bits.Count % 8 == 0) && (bits.Count >= 8))
                    {
                        byte value = 0;

                        for (int c = 8; c > 0; c--)
                        {
                            if (bits[bits.Count - c])
                            {
                                value |= (byte)(1 << (c - 1));
                            }
                        }

                        if (value == 25)
                            return;
                    }
                    Rgba32 pixel = pixelRow[x];

                    bits.Add((pixel.R & 1) == 1);
                    bits.Add((pixel.G & 1) == 1);
                    bits.Add((pixel.B & 1) == 1);
                    bits.Add((pixel.A & 1) == 1);
                }
            }
        });
        return bits.ToArray();
    }

    public static string DecodeBitsAsString(bool[] bits)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < bits.Length; i += 8)
        {
            byte value = 0;
            for (int j = 0; j < 8; j++)
            {
                if (bits[i + j])
                {
                    value |= (byte)(1 << (7 - j));
                }
            }

            if (value == 25)
                return sb.ToString();

            sb.Append((char)value);
        }

        return sb.ToString();
    }

    public static Image<Rgba32> EncodeLSBInImageBytes(List<bool> lsb, Image<Rgba32> image)
    {
        lsb.AddRange([false, false, false, true, true, false, false, true]);

        if (lsb.Count > image.Width * image.Height * 4)
            throw new ArgumentException("The Bytes (as Bits) that should be encoded in the image are too many and would not fit into the image's Bytes, which would lead to data corruption.");

        image.ProcessPixelRows(d => {

            for (int y = 0; y < d.Height; y++)
            {
                Span<Rgba32> pixelRow = d.GetRowSpan(y);

                for (int x = 0; x < pixelRow.Length; x++)
                {
                    ref Rgba32 pixel = ref pixelRow[x];

                    if ((y * d.Width * 4) + x * 4 == lsb.Count)
                        return;

                    pixel.R = (byte)((pixel.R & ~1) | (lsb[(y * d.Width * 4) + x * 4 + 0] ? 1 : 0));
                    pixel.G = (byte)((pixel.G & ~1) | (lsb[(y * d.Width * 4) + x * 4 + 1] ? 1 : 0));
                    pixel.B = (byte)((pixel.B & ~1) | (lsb[(y * d.Width * 4) + x * 4 + 2] ? 1 : 0));
                    pixel.A = (byte)((pixel.A & ~1) | (lsb[(y * d.Width * 4) + x * 4 + 3] ? 1 : 0));
                }
            }
        });
        return image;
    }

    public static bool[] TextToBits(string text)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(text);
        List<bool> lsb = new List<bool>();

        foreach (byte b in bytes)
        {
            for (int i = 7; i >= 0; i--)
            {
                bool bit = ((b >> i) & 1) == 1;

                lsb.Add(bit);
            }
        }

        return lsb.ToArray();
    }
}

