using static StenoCoderCmd.Converter.StenoConverter;

namespace StenoCoderCmd;

class StenocoderCmd()
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine($"Please provide arguments:\n" +
                $"stenocoder decode filePath ---> Decodes an image that was encoded with the same method this tool uses.\n" +
                $"stenocoder encode filePath string ---> Encodes an image with the provided string.\n" +
                $"Visit https://github.com/1TheCrazy/StenoCoder if you have questions or issues using this package!");
            return;
        }

        if (args[0] == "decode")
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Please provide an image path!");
                return;
            }

            if (!File.Exists(args[1]))
            {
                Console.WriteLine($"There is no image located at '{args[1]}'");
                return;
            }
            else
            {
                string result = DecodeImage(args[1]);

                Console.WriteLine(result);
            }
        }
        else if (args[0] == "encode")
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Please provide an image path and a string to encode!");
                return;
            }

            if (!File.Exists(args[1]))
            {
                Console.WriteLine($"There is no Image located at '{args[1]}'");
                return;
            }
            else
            {
                bool result = EncodeImage(args[1], args[2]);

                if (result)
                    Console.WriteLine("Successfully encoded the image!");
                else
                    Console.WriteLine("There was an error trying to encode the image...");
            }
        }
        else
        {
            Console.WriteLine($"Please provide valid arguments:\n" +
                $"stenocoder decode filePath ---> Decodes an image that was encoded with the same method this tool uses.\n" +
                $"stenocoder encode filePath string ---> Encodes an image with the provided string.\n" +
                $"Visit https://github.com/1TheCrazy/StenoCoder if you have questions or issues using this package!");
        }
    }
}
