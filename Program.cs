using System;
using System.IO;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCat
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var x = Directory.GetCurrentDirectory();
            if (args.Length == 1)
            {
                if (Path.IsPathRooted(args[0]))
                {
                    x = args[0];
                }
                else
                {
                    x = x + @"\" + args[0];
                }


            }
            Console.WriteLine(x);

            var textFilePath = x; 
            var filestream = new System.IO.FileStream(textFilePath,
                                          System.IO.FileMode.Open,
                                          System.IO.FileAccess.Read,
                                          System.IO.FileShare.ReadWrite);
            var file = new System.IO.StreamReader(filestream, System.Text.Encoding.UTF8, true, 128);
            var lineOfText = string.Empty;
            while ((lineOfText = file.ReadLine()) != null)
            {
                var lastPos = 0;
                var colorCode = getBetween(lineOfText, "<color ", ">", out lastPos);

                if (lastPos == -1)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(lineOfText);

                    continue;
                }

                // If here then we have some section parsing to do
                //
                {
                    // Setup the color for output
                    //
                    switch (colorCode.ToLower())
                    {
                        case "red":
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        case "yellow":
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                    }

                    Console.WriteLine(lineOfText.Substring(lastPos));
                }
            }

        }
        public static string getBetween(string strSource, string strStart, string strEnd, out int lastPos)
        {
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                lastPos = End + 1;
                return strSource.Substring(Start, End - Start);
            }
            lastPos = -1;
            return "";
        }
    }
}
