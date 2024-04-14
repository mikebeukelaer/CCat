using System;
using System.IO;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Data.SqlTypes;

namespace CCat
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Sloppy code, really bad style here
            //
            //
            var textFilePath = Directory.GetCurrentDirectory();
            var pageSize = 32;
            if (args.Length == 1)
            {
                if (Path.IsPathRooted(args[0]))
                {
                    textFilePath = args[0];
                }
                else
                {
                    textFilePath = textFilePath + @"\" + args[0];
                }



                Console.WriteLine($"File :  {textFilePath}");

                
                if(File.Exists(textFilePath))
                {
                    var filestream = new System.IO.FileStream(textFilePath,
                                                  System.IO.FileMode.Open,
                                                  System.IO.FileAccess.Read,
                                                  System.IO.FileShare.ReadWrite);
                    var file = new System.IO.StreamReader(filestream, System.Text.Encoding.UTF8, true, 128);
                    var lineOfText = string.Empty;

                    var currentLineCount = 0;

                    while ((lineOfText = file.ReadLine()) != null)
                    {
                        try
                        {
                            processLine(lineOfText,0);
                        }
                        catch (Exception ex)
                        {
                            // Do nothing
                        }
                        currentLineCount++;
                        if(currentLineCount == pageSize)
                        {
                            if(!PromptForContinue())
                            {
                                break;
                            }
                            currentLineCount = 0;
                        }
                        
                        Console.WriteLine();

                    }
                }
            }
            else
            {
                Console.WriteLine("Usage: ccat.exe <file name>");
            }

        }

        private static bool PromptForContinue()
        {
            var retVal = false;

            ConsoleColor backgroundColor = Console.BackgroundColor;
            ConsoleColor foregroundColor = Console.ForegroundColor;

            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("");
            Console.Write("Continue:");
            var key = Console.ReadKey();
            if(key.Key == ConsoleKey.Spacebar)
            {
                retVal = true;
                //var x = Console.CursorLeft;
              
            }else if(key.Key == ConsoleKey.Q )
            {
                return false;
            }
            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = foregroundColor;

            ClearLine();

            return retVal;
        }

        private static void ClearLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write("                ");
            Console.SetCursorPosition(0, Console.CursorTop - 1);
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

        private static void processLineII(string src)
        {


            var words = src.Split(' ');

            foreach(var word in words)
            {
                Console.WriteLine(word); ;
            }

            Console.WriteLine("--------------------------------"); ;

        }

        private static void processLineII(string src, int currentPosition)
        {
            var COLOR_TAG_START = "<color";
            var COLOR_TAG_END = "</color>";
            var currentColor = "White";

            // each char is written with a color,
            // the default color is white
            // 
            //
            //var currentPosition = 0;
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

                    currentPosition = tagEndPos + COLOR_TAG_END.Length - 1;
                    write(co);


                }


                currentPosition++;
                tagStartPos = src.IndexOf(COLOR_TAG_START, currentPosition);


            }
        }

        private static void processLine(string src, int currentPosition)
        {
            var COLOR_TAG_START = "<color";
            var COLOR_TAG_END = "</color>";
            var currentColor = "White";

            // each char is written with a color,
            // the default color is white
            // 
            //
            //var currentPosition = 0;
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

                    currentPosition = tagEndPos + COLOR_TAG_END.Length - 1;
                    write(co);


                }


                currentPosition++;
                tagStartPos = src.IndexOf(COLOR_TAG_START, currentPosition);


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
