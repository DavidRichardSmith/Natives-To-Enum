using System;
using System.IO;

using Formatter.Native;

namespace Formatter
{
    class Program
    {
        static void Main(string[] args)
        {
            // must provide at least two arguments.
            if(args.Length < 2)
            {
                Console.WriteLine("\nsyntax error!");
                Console.WriteLine("required arguments: string <natives_path> bool <use_padding>");
                return;
            }

            try
            {
                var parser = new NativeHeaderParser(args[0]);

                parser.Initialize();
                parser.Start();

                using (StreamWriter outputFile = new StreamWriter("natives_output.h"))
                {
                    outputFile.WriteLine("enum class eNatives : std::uint64_t");
                    outputFile.WriteLine("{");

                    bool applyPadding = Convert.ToBoolean(args[1]);
                    foreach (var native in parser.GetResults())
                    {
                        // amount of padding to appened to the right side of the native name.
                        int paddingCount = applyPadding ? GetNativeNamePadding(native.Name, parser.GetLongestNativeName()) : 0;

                        // write formatted string to output file.
                        outputFile.WriteLine("\t" + native.Name.PadRight(paddingCount) + " = " + native.Hash + ",");
                    }

                    outputFile.WriteLine("};");
                }

            }
            catch(FileNotFoundException)
            {
                Console.WriteLine("failed to find input natives.h file.");
                Console.ReadLine();
                return;
            }
        }

        /// <summary>
        /// Retrieve the amount of padding that should be applied to the right side of the native name.
        /// </summary>
        /// <param name="nativeName">The native name.</param>
        /// <param name="maxPadding">The max amount of padding that should be applied.</param>
        /// <returns></returns>
        static int GetNativeNamePadding(string nativeName, int maxPadding)
        {
            // if the native name is less then the max padding we must append the max
            // amount to keep everything in a perfect row.
            if (nativeName.Length < maxPadding)
                return maxPadding;

            return nativeName.Length - maxPadding;
        }
    }
}
