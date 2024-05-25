using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Png;

namespace StegoCoder;

/// <summary>
/// A class for en- and decoding steganography images.
/// </summary>
public class StegoConverter
{
    /// <summary>
    /// Decodes an image at the provided path.
    /// </summary>
    /// <param name="imagePath">The path of the image that should be decoded.</param>
    /// <returns>A <see cref="string"/> that represents the result of the decoding.</returns>
    /// <exception cref="Exception">Throws a general exception if the decoding fails.</exception>
    /// <remarks>This process of decoding only works if the image was incoded in a way that this package can understand. This package manipulates the lsb. If the image was encoded in a different way, the decoding won't work as expected.</remarks>
    public static string DecodeImage(string imagePath)
    {
        try
        {
            return DecodeBitsAsString(GetLBSFromImage(GetImage(imagePath)));
        }
        catch (Exception ex)
        {
            throw new Exception($"There was an error trying to decode the image...\n{ex.Message}\n{ex.StackTrace}");
        }
    }

    /// <summary>
    /// Decodes an <see cref="Image{Rgba32}"/>.
    /// </summary>
    /// <param name="image">The <see cref="Image{Rgba32}"/> that should be decoded.</param>
    /// <returns>A <see cref="string"/> that represents the result of the decoding.</returns>
    /// <exception cref="ArgumentException">Throws an <see cref="ArgumentException"/> if the decoding fails.</exception>
    /// <remarks>This process of decoding only works if the image was incoded in a way that this package can understand. This package manipulates the lsb. If the image was encoded in a different way, the decoding won't work as expected.</remarks>
    public static string DecodeImage(Image<Rgba32> image)
    {
        try
        {
            return DecodeBitsAsString(GetLBSFromImage(image));
        }
        catch (Exception ex)
        {
            throw new ArgumentException($"There was an error trying to decode the image...\n{ex.Message}\n{ex.StackTrace}");
        }
    }

    /// <summary>
    /// Encodes a string in the image at the given path.
    /// </summary>
    /// <param name="imagePath">The path of the image that should be encoded.</param>
    /// <param name="text">The text that should be encoded in the image</param>
    /// <param name="encodedImagePath">The path where the encoded image should be saved. Leave this empty if the image should be overwritten, which is the default.</param>
    /// <returns>A <see cref="bool"/> representing the seuccess of encoding the image.</returns>
    /// <remarks>This package encodes strings by manipulating the lsb of the image bytes. At the end of the string, a EM (25) is appended to signal the End of Medium.</remarks>
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

            EncodeLSBInImageBytes(TextToBits(text).ToList(), GetImage(imagePath)).SaveAsPng(encodedImagePath == "" ? imagePath : encodedImagePath, pngEncoder);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"There was an error trying to encode the image...\n{ex.Message}\n{ex.StackTrace}");
            return false;
        }
    }

    /// <summary>
    /// Encodes a string in the <see cref="Image{Rgba32}"/>.
    /// </summary>
    /// <param name="image">The <see cref="Image{Rgba32}"/> that should be encoded.</param>
    /// <param name="text">The text that should be encoded in the image</param>
    /// <returns>An <see cref="Image{Rgba32}"/> representing the result of the encoding.</returns>
    /// <remarks>This package encodes strings by manipulating the lsb of the image bytes. At the end of the string, a EM (25) is appended to signal the End of Medium.</remarks>
    public static Image<Rgba32> EncodeImage(Image<Rgba32> image, string text)
    {
        try
        {
            return EncodeLSBInImageBytes(TextToBits(text).ToList(), image);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"There was an error trying to encode the image...\n{ex.Message}\n{ex.StackTrace}");
            return new(0, 0);
        }
    }

    /// <summary>
    /// Gets an <see cref="Image{Rgba32}"/> at the given path.
    /// </summary>
    /// <param name="imagePath">The path from where the image should be loaded.</param>
    /// <returns>An <see cref="Image{Rgba32}"/> representing the loaded image.</returns>
    public static Image<Rgba32> GetImage(string imagePath)
    {
        return Image.Load<Rgba32>(imagePath);
    }

    /// <summary>
    /// Gets the Least Significant Bit from all the image bytes until EM (25) is found.
    /// </summary>
    /// <param name="image">The <see cref="Image{Rgba32}"/> from which the LSB should be taken.</param>
    /// <returns>A <see cref="bool"/>[] with all the LSBs from the image.</returns>
    /// <remarks>The Method checks if the last byte is equal to 25, which signals the End of Medium. If the EM is found, it returns.</remarks>
    public static bool[] GetLBSFromImage(Image<Rgba32> image)
    {
        List<bool> bits = new();

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

    /// <summary>
    /// Decodes a <see cref="bool"/>[] to a <see cref="string"/>.
    /// </summary>
    /// <param name="bits">The <see cref="bool"/>[] with all the bits that should be decoded as a string.</param>
    /// <returns>A <see cref="string"/> representing the result of the decoding.</returns>
    public static string DecodeBitsAsString(bool[] bits)
    {
        StringBuilder sb = new();

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

    /// <summary>
    /// Encodes bits in an <see cref="Image{Rgba32}"/>.
    /// </summary>
    /// <param name="lsb">A <see cref="bool"/>[] representing the bits that should be encoded in the image.</param>
    /// <param name="image">A <see cref="Image{Rgba32}"/> representing the image the bytes should be encoded in.</param>
    /// <returns>A <see cref="Image{Rgba32}"/> with the encoded bits.</returns>
    /// <exception cref="ArgumentException">Throws an <see cref="ArgumentException"/> when the bits provided would not fit in the image's bytes</exception>
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

    /// <summary>
    /// Converts a <see cref="string"/> to bits.
    /// </summary>
    /// <param name="text">A <see cref="string"/> representing the text that should be converted to bits.</param>
    /// <returns>A <see cref="bool"/>[] representing the result of the conversion.</returns>
    public static bool[] TextToBits(string text)
    {
        byte[] bytes = Encoding.ASCII.GetBytes(text);
        List<bool> lsb = new();

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
