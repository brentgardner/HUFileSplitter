using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace HUFileSplitter
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = string.Empty;
            int recordsPerFile = 0;
            List<Stack<string>> blockCollection = new List<Stack<string>>();

            Console.WriteLine("This program allows you to split large HU files into several smaller file.");
            Console.WriteLine("Type 'EXIT' to quit");

            while (fileName == string.Empty)
            {
                fileName = getFileName().ToLower();
                if (fileName == "exit")
                {
                    Environment.Exit(0);
                }
            }

            Console.WriteLine("This file will be the source file: {0}", fileName);

            while (recordsPerFile == 0)
            {
                recordsPerFile = getNumOfSubFiles();
            }

            Console.WriteLine("{0} subfiles will be created", recordsPerFile);

            Console.WriteLine("Working....");

            //read the file and return a collection of stacks, each stack represents once 10,20,25 block
            blockCollection = readFileIntoList(fileName);
            writeListIntoFiles(recordsPerFile, blockCollection, fileName);

            
        }

        static string getFileName()
        {
            string file;
            Console.WriteLine("Which file would you like to process?:");
            file = Console.ReadLine();

            //does file exit?
            if (!File.Exists(file))
            {
                Console.WriteLine("Sorry, could not find {0} in the current directory", file);
                return string.Empty;
            }
            else
            {
                return file;
            }
        }

        static int getNumOfSubFiles()
        {
            var subFileCount = 0;

            Console.WriteLine("How many records would you like in each sub file?:");
            bool result = false;
            result = int.TryParse(Console.ReadLine(), out subFileCount);

            if (result)
            {
                return Math.Abs(subFileCount);
            }
            else
            {
                Console.WriteLine("Sorry, that is not a vaild file");
                return 0;
            }
            
        }

        static List<Stack<string>> readFileIntoList(string fileName)
        {
            List<Stack<string>> blocks = new List<Stack<string>>();

            string[] lines = File.ReadAllLines(fileName);
            bool twentyFiveFound = false;

            var s = new Stack<string>();

            foreach (var line in lines)
            {
                if (twentyFiveFound == false)
                {
                    
                    if (line.StartsWith("25|"))
                    {
                        twentyFiveFound = true;
                    }

                    s.Push(line);
                    
                }
                else
                {
                    
                    if (line.StartsWith("10|"))
                    {
                        //this indicates that the beginning of a new block has been found
                        //time to add the stack to the collection and start a new stack.
                        twentyFiveFound = false;

                        blocks.Add(s);
                        s.Clear();
                        s.Push(line);
                    }
                    else
                    {
                        s.Push(line);
                    }
                }
            }

             return blocks;
        }

        static void writeListIntoFiles(int fileCount, List<Stack<string>> blocks, string fileName)
        {
            int splitSize = blocks.Count / fileCount;
            List<List<Stack<string>>> splitLists = new List<List<Stack<string>>>();
            int filesWritten = 1;
            int splitsWritten = 1;


            var files = new Stack<string>();

            for (int i = 1; i < fileCount; i++)
            {
                files.Push(string.Format("{0}_{1}.{2}", fileName.Split('.')[0], i.ToString(), fileName.Split('.')[1]));
            }

            splitLists = blocks.Select((x, i) => new { Index = i, Value = x }).GroupBy(x => x.Index / fileCount).Select(x => x.Select(v => v.Value).ToList()).ToList();

            foreach (var lists in splitLists)
            {
                
            }


            
        }

        

    }
}
