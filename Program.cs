using System;
using System.IO;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace CCat
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Sloppy code, really bad style here
            //
            //
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



                Console.WriteLine(x);

                var textFilePath = x; 
                if(File.Exists(textFilePath))
                {
                    var filestream = new System.IO.FileStream(textFilePath,
                                                  System.IO.FileMode.Open,
                                                  System.IO.FileAccess.Read,
                                                  System.IO.FileShare.ReadWrite);
                    var file = new System.IO.StreamReader(filestream, System.Text.Encoding.UTF8, true, 128);
                    var lineOfText = string.Empty;
                    while ((lineOfText = file.ReadLine()) != null)
                    {
                        processLine(lineOfText);
                        Console.WriteLine();
                        continue;

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
                                case "blue":
                                    Console.ForegroundColor = ConsoleColor.Blue;
                                    break;
                                case "black":
                                    Console.ForegroundColor = ConsoleColor.Black;
                                    break;
                            }

                            Console.WriteLine(lineOfText.Substring(lastPos));
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Usage: ccat.exe <file name>");
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

        struct CharOut
        {
            public string c;
            public string color;
        }
        private static void write(CharOut charOut)
        {

            switch (charOut.color.ToLower())
            {
                case "red":
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case "yellow":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case "blue":
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case "black":
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                case "white":
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            
            Console.Write(charOut.c);
        }
        private static void processLine(string src)
        {
            var COLOR_TAG_START = "<color";
            var COLOR_TAG_END = "</color>";
            var currentColor = "White";

            // each char is written with a color,
            // the default color is white
            // 
            //
            var currentPosition = 0;
            var tagStartPos = src.IndexOf(COLOR_TAG_START, currentPosition);


            // Read a char at a time until we get a tag match
            //
            while (currentPosition < src.Length)
            {
                char c = src[currentPosition];

                var co = new CharOut();


                if (currentPosition < tagStartPos || tagStartPos == -1)
                {
                    co.color = "White";
                    co.c = c.ToString();
                    write(co);  
                  //  logSingle($"{c}");
                }
                else
                {
                    tagStartPos = src.IndexOf(COLOR_TAG_START, currentPosition);
                    if (tagStartPos == -1) { break; }
                    var startSearchPos = tagStartPos + COLOR_TAG_START.Length;

                    currentColor = GetColor(src, startSearchPos, out currentPosition);

                    var tagEndPos = src.IndexOf(COLOR_TAG_END, currentPosition);
                    var textValue = src.Substring(currentPosition, tagEndPos - currentPosition);
                    co.color = currentColor;
                    co.c = textValue;
                  //  log("");
                  //  log($"COLOR  {currentColor}");
                  //  log($"VALUE {textValue}");
                  //  log($"CURRENT POS : {currentPosition}");
                  //  log($"VALUE AT CURRENT POS: {src[currentPosition]}");
                  //  log("  ");
                    currentPosition = tagEndPos + COLOR_TAG_END.Length - 1;
                    write(co);


                }


                currentPosition++;
                tagStartPos = src.IndexOf(COLOR_TAG_START, currentPosition);
                //  log($"current :{currentPosition}");
                //  log($"tagstart :{tagStartPos}");


            }
        }
        private static string GetColor(string src, int startPos, out int currentPos)
        {
            var retVal = string.Empty;
            var found = false;
            currentPos = startPos;
            var sb = new StringBuilder();
            // Read chars until a '>' is encountered
            //
            while (!found && currentPos < src.Length - 1)
            {
                if (src[currentPos] == '>')
                {
                    found = true;
                    retVal = sb.ToString();

                }
                else
                {
                    if (src[currentPos] != ' ')
                    {
                        sb.Append(src[currentPos]);
                    }

                }
                currentPos++;
            }

            return retVal;
        }


    }
}
