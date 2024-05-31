using System;
using System.Linq;

namespace image_to_ascii
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ASCII art generator!");

            Console.WriteLine(Environment.NewLine);

            Console.WriteLine("----------Usage----------");
            Console.WriteLine("<inputFilename> -o <outputFilepath> -n <outputFilename>");
            Console.WriteLine("Example: C:\\Users\\Alex\\Pictures\\waterfall.png -o C:\\Users\\Alex\\Downloads -n waterfall_ascii");
            Console.WriteLine("^ Creates a \"waterfall_ascii.txt\" file in C:\\Users\\Alex\\Downloads\\ ^");

            Console.WriteLine(Environment.NewLine);

            Console.WriteLine("To exit the program at any time, type \"EXIT\"");

            Console.WriteLine(Environment.NewLine);

            string input = "";
            while (input.Trim() != "EXIT") {
                input = Console.ReadLine();

                if (!input.Contains(" -o ")) {
                    Console.WriteLine("You forgot to include \" -o \"");
                    continue;
                } else if (!input.Contains(" -n ")) {
                    Console.WriteLine("You forgot to include \" -n \"");
                    continue;
                }

                string[] inputSeperated = input.Trim().Split(new string[] { " -o " }, StringSplitOptions.None);
                string[] inputParams = new string[3];
                inputParams[0] = inputSeperated[0];
                try {
                    string[] inputSeperatedSubstrings = inputSeperated[1].Split(new string[] { " -n " }, StringSplitOptions.None);
                    inputParams[1] = inputSeperatedSubstrings[0];
                    inputParams[2] = inputSeperatedSubstrings[1];
                } catch (IndexOutOfRangeException) {
                    Console.WriteLine("Please use the given format");
                    continue;
                }

                if (inputParams.Length != 3) {
                    Console.WriteLine("Please use the given format");
                    continue;
                }

                string inPath = inputParams[0];
                string thirdParam = inputParams[2];
                string name;
                if (thirdParam.Substring(thirdParam.Length - 4) == ".txt") {
                    name = thirdParam.Remove(thirdParam.Length - 4);
                } else {
                    name = thirdParam;
                }

                string secondParam = inputParams[1];
                string outPath;
                if (secondParam.Last() == '\\' || secondParam.Last() == '/')
                {
                    outPath = $"{secondParam}{name}.txt";
                } else {
                    Console.WriteLine($"You forgot a \\ or / at the end of {secondParam}");
                    continue;
                }

                Img image = new Img(inPath);
                image.ToAscii(outPath);
            }   
        }
    }
}
