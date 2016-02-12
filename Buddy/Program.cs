using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace Buddy
{
    class Block
    {
        public string processName { get; set; }
        public int size { get; set; }
        public bool isAvailable { get; set; }
    }
    class Program
    {
        public static int TraverseBuddy(int size, int currentindex, Block[] freeBlocks)
        {
            int blockSize = 0;
            for (int i = currentindex; i < freeBlocks.Length; i++)
            {
                if (freeBlocks[i].isAvailable == false)
                {
                    return 0;
                }
                else
                {
                    blockSize += freeBlocks[i].size;
                    if (blockSize >= size)
                        return i;
                }
            }
            return 0;
        }
        public static void AllocateBlock(string ProcessName, int size, Block[] freeBlocks)
        {
            for (int i = 0; i < freeBlocks.Length; i++)
            {
                if (freeBlocks[i].isAvailable == true && freeBlocks[i].size >= size)
                {
                    freeBlocks[i].isAvailable = false;
                    freeBlocks[i].processName = ProcessName;
                    break;
                }
                else if (freeBlocks[i].isAvailable == true && freeBlocks[i].size <= size)
                {
                    //Traverse to allblocks till you get required space.
                    int nextAvailableIndexes = TraverseBuddy(size, i, freeBlocks);
                    if(nextAvailableIndexes !=0)
                    {
                        for (; i <= nextAvailableIndexes; i++)
                        {
                            freeBlocks[i].isAvailable = false;
                            freeBlocks[i].processName = ProcessName;
                        }
                        break;
                    }
                }
            }
        }
        public static void DeAllocateBlock(string ProcessName, Block[] freeBlocks)
        {
            for (int i = 0; i < freeBlocks.Length; i++)
            {
                if (freeBlocks[i].processName == ProcessName)
                {
                    freeBlocks[i].processName = "";
                    freeBlocks[i].isAvailable = true;
                }
            }

        }
        public static void MergeAndUnMergeBlocks(Block[] freeBlocks, List<Block> ActualBlocks)
        {
            ActualBlocks.Clear();
            int memory = 0;
            for (int i = 0; i < freeBlocks.Length; i++)
            {
                if (freeBlocks[i].isAvailable == false)
                {
                    if (memory != 0)
                    {
                        Block freeBlock = new Block();
                        freeBlock.isAvailable = true;
                        freeBlock.processName = "";
                        freeBlock.size = memory;
                        ActualBlocks.Add(freeBlock);
                        memory = 0;
                    }
                    Block occupiedBlock = new Block();
                    occupiedBlock.isAvailable = false;
                    occupiedBlock.processName = freeBlocks[i].processName;
                    occupiedBlock.size = freeBlocks[i].size;
                    ActualBlocks.Add(occupiedBlock);

                }
                else
                {
                    memory += freeBlocks[i].size;
                }
            }
            if (memory != 0)
            {
                Block freeBlock = new Block();
                freeBlock.isAvailable = true;
                freeBlock.processName = "";
                freeBlock.size = memory;
                ActualBlocks.Add(freeBlock);
                memory = 0;
            }
        }
        public static void printMemory(List<Block> ActualBlocks)
        {
            foreach (var item in ActualBlocks)
            {
                if(item.isAvailable==true)
                    Console.Write(item.size.ToString() + "KB free |");
                else
                    Console.Write(item.processName.ToString() +"-"+ item.size.ToString() + "KB"+"  |");
            }
            Console.WriteLine("");
        }
        public static void intializeMemory(Block[] freeBlocks,int size)
        {
            for (int i = 0; i < freeBlocks.Length; i++)
            {
                freeBlocks[i] = new Block();
                freeBlocks[i].isAvailable = true;
                freeBlocks[i].size = size;
                freeBlocks[i].processName = "";
            }
        }
        static void Main(string[] args)
        {
            int maxLimit = 0;
            int minLimit = 0;
            while (true)
            {
                Console.WriteLine("Enter Max Limit");
                string max = Console.ReadLine();
                Console.WriteLine("Enter Min Limit");
                string min = Console.ReadLine();
                if (int.Parse(max) % int.Parse(min) == 0)
                {
                    maxLimit = int.Parse(max);
                    minLimit = int.Parse(min);
                    break;
                }
                else
                {
                    Console.WriteLine("Enter Again!.");
                }
            }
            int sizeOfArray = maxLimit / minLimit;
            Block[] freeBlocks = new Block[sizeOfArray];
            intializeMemory(freeBlocks, minLimit);
            List<Block> ActualBlocks = new List<Block>();
            Block freeMemory = new Block();
            freeMemory.isAvailable = true;
            freeMemory.processName = "";
            freeMemory.size = maxLimit;
            ActualBlocks.Add(freeMemory);
            Console.WriteLine("Intial State of Memory");
            printMemory(ActualBlocks);
            string fileName = "input.txt";
            using (StreamReader sr = new StreamReader(fileName))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] splittedlines = line.Split('\t');
                    int milliseconds = 2000;
                    Thread.Sleep(milliseconds);

                    if (splittedlines[0] == "E")
                    {
                        Console.WriteLine("Process " + splittedlines[1] + " Enter " + splittedlines[2] + "KB");
                        AllocateBlock(splittedlines[1], int.Parse(splittedlines[2]), freeBlocks);
                        MergeAndUnMergeBlocks(freeBlocks, ActualBlocks);
                        printMemory(ActualBlocks);
                    }
                    else
                    {
                        Console.WriteLine("Process " + splittedlines[1] + " Leave" );
                        DeAllocateBlock(splittedlines[1], freeBlocks);
                        MergeAndUnMergeBlocks(freeBlocks, ActualBlocks);
                        printMemory(ActualBlocks);
                    }
                }
            }

            //Process A Enter
            //Console.WriteLine("Process A Enter 7Kb");
            //AllocateBlock("A", 7, freeBlocks);
            //MergeAndUnMergeBlocks(freeBlocks, ActualBlocks);
            //printMemory(ActualBlocks);

            //Console.WriteLine("Process B Enter 3Kb");
            //AllocateBlock("B", 3, freeBlocks);
            //MergeAndUnMergeBlocks(freeBlocks, ActualBlocks);
            //printMemory(ActualBlocks);

            //Console.WriteLine("Process A Leaves 7Kb");
            //DeAllocateBlock("A", freeBlocks);
            //MergeAndUnMergeBlocks(freeBlocks, ActualBlocks);
            //printMemory(ActualBlocks);

            //Console.WriteLine("Process C Enter 6Kb");
            //AllocateBlock("C", 6, freeBlocks);
            //MergeAndUnMergeBlocks(freeBlocks, ActualBlocks);
            //printMemory(ActualBlocks);

            //Console.WriteLine("Process B Leaves 3Kb");
            //DeAllocateBlock("B", freeBlocks);
            //MergeAndUnMergeBlocks(freeBlocks, ActualBlocks);
            //printMemory(ActualBlocks);

            //Console.WriteLine("Process C Leaves 4Kb");
            //DeAllocateBlock("C", freeBlocks);
            //MergeAndUnMergeBlocks(freeBlocks, ActualBlocks);
            //printMemory(ActualBlocks);
        }
    }
}
