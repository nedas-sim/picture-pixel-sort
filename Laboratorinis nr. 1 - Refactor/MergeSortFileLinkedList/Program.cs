using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace MergeSortFileLinkedList
{
    class Program
    {
        public static string saveDir = "..\\..\\nuotraukosOutput\\";
        public static int fileIter = 1;
        public static FileClass f = new FileClass();
        public static int actions = 0;

        static void Main(string[] args)
        {
            Delete();

            string dir = "..\\..\\nuotraukosInput\\";

            string file1 = Directory.GetFiles(dir, "*.jpg")[0];

            Console.WriteLine("Failas: " + file1);
            Bitmap image = new Bitmap(file1);
            
            string fileName = saveDir + Path.GetFileNameWithoutExtension(file1) + ".bmp";
            string temp = saveDir + "temp.txt";
            string other = saveDir + "other.txt";
            image.Save(fileName, ImageFormat.Bmp);
            Console.WriteLine("Dydis: " + image.Height + " " + image.Width);
            int width = image.Width;
            int height = image.Height;



            for (int i1 = 10; i1 <= (image.Width < image.Height ? image.Width : image.Height); i1 += 10)
            {
                Console.WriteLine("Rikiuojamas: " + i1 + " " + i1);
                string finalFile = saveDir + Path.GetFileNameWithoutExtension(file1) + i1 + "Sorted.bmp";

                using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    using (FileStream stream2 = new FileStream(finalFile, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        stream2.Seek(0, SeekOrigin.Begin);

                        for (int i = 0; i < 54; i++)
                        {
                            byte value = (byte)stream.ReadByte();
                            stream2.WriteByte(value);
                        }
                        stream2.Close();
                    }
                    
                    for (int i = 0; i < i1; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            byte b = (byte)stream.ReadByte();
                            byte g = (byte)stream.ReadByte();
                            byte r = (byte)stream.ReadByte();
                            if (j < i1)
                            {
                                f.Add(temp, (((r << 8) + g) << 8) + b);
                            }
                            else
                            {
                                f.Add(other, (((r << 8) + g) << 8) + b);
                            }
                        }
                    }

                    for (int i = 0; i < image.Width * (image.Height - i1); i++)
                    {
                        byte b = (byte)stream.ReadByte();
                        byte g = (byte)stream.ReadByte();
                        byte r = (byte)stream.ReadByte();
                        f.Add(other, (((r << 8) + g) << 8) + b);
                    }
                    
                    stream.Close();
                }

                Stopwatch watch;

                Console.WriteLine("Pradeda mergesort");
                watch = Stopwatch.StartNew();
                MergeSort(temp);
                watch.Stop();
                int ms = (int)watch.ElapsedMilliseconds;
                Console.WriteLine("Pabaige mergesort per " + ms + " milisekundziu");

                PrintData(ms, i1 * i1, "File_LinkedList.csv");
                actions = 0;

                Console.WriteLine("Pradeda perkelima");
                string newTemp = Reshuffle(temp, i1, i1);
                Console.WriteLine("Pabaige perkelima");

                Console.WriteLine("Pradeda taisyti");
                Replace(newTemp, i1, i1);
                Console.WriteLine("Pabaige taisyti");

                

                Console.WriteLine("Pradeda irasyma i galutini faila");
                using(FileStream stream = new FileStream(finalFile, FileMode.Open, FileAccess.Write))
                {
                    int sortNode = f.GetStart(newTemp);
                    int otherNode = f.GetStart(other);

                    for(int i = 0; i < i1; i++)
                    {
                        for(int j = 0; j < width; j++)
                        {
                            if(j < i1)
                            {
                                byte[] arr = BitConverter.GetBytes(f.GetValueByNode(newTemp, sortNode));
                                stream.Seek(0, SeekOrigin.End);
                                stream.Write(arr, 0, 3);
                                sortNode = f.GetNext(newTemp, sortNode);
                            }
                            else
                            {
                                byte[] arr = BitConverter.GetBytes(f.GetValueByNode(other, otherNode));
                                stream.Seek(0, SeekOrigin.End);
                                stream.Write(arr, 0, 3);
                                otherNode = f.GetNext(other, otherNode);
                            }
                        }
                    }
                    for(int i = 0; i < width * (height - i1); i++)
                    {
                        byte[] arr = BitConverter.GetBytes(f.GetValueByNode(other, otherNode));
                        stream.Seek(0, SeekOrigin.End);
                        stream.Write(arr, 0, 3);
                        otherNode = f.GetNext(other, otherNode);
                    }
                    using (FileStream stream1 = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        stream1.Seek(54 + 3 * width * height, SeekOrigin.Begin);
                        for (int i = 54 + 3 * width * height; i < stream1.Length; i++)
                        {
                            byte value = (byte)stream1.ReadByte();
                            stream.WriteByte(value);
                        }
                    }

                    stream.Close();
                }
                Console.WriteLine("Pabaige irasyma i faila");

                Console.WriteLine("Pradeda failu istrynima");
                DeleteFiles();
                Console.WriteLine("Pabaige failu istrynima");


                Console.WriteLine("Prireike " + fileIter + " failu");
                fileIter = 1;

                if (ms > 100 * 1000)
                    break;

                Console.WriteLine();

            }

            Console.WriteLine("Pabaiga");
            Console.ReadKey();
        }

        public static void Delete()
        {
            string[] files = Directory.GetFiles(saveDir);
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        public static void PrintData(int ms, int size, string fileName)
        {
            using (StreamWriter writer = new StreamWriter(saveDir + fileName, true))
            {
                writer.WriteLine(size + "," + ms + "," + actions);
            }
        }

        static void DeleteFiles()
        {
            string[] files = Directory.GetFiles(saveDir, "*.txt");
            for (int i = 0; i < files.Length; i++)
            {
                if (File.Exists(files[i]))
                    File.Delete(files[i]);
            }
        }

        static void MergeSort(string file1)
        {
            if (f.Count(file1) > 1)
            {
                string left = f.LeftHalf(file1);
                MergeSort(left);
                string right = f.RightHalf(file1);
                MergeSort(right);
                Merge(file1, left, right);
                actions += 5;
            }
        }
        
        

        static void Merge(string result, string left, string right)
        {
            int nodeL = f.GetStart(left);
            int nodeR = f.GetStart(right);
            int iter = f.GetStart(result);
            actions += 3;

            while(true)
            {
                if(nodeR == -1 || (nodeL != -1 && f.GetValueByNode(left, nodeL) <= f.GetValueByNode(right, nodeR)))
                {
                    f.ChangeValue(result, iter, f.GetValueByNode(left, nodeL));
                    nodeL = f.GetNext(left, nodeL);
                }
                else
                {
                    f.ChangeValue(result, iter, f.GetValueByNode(right, nodeR));
                    nodeR = f.GetNext(right, nodeR);
                }

                iter = f.GetNext(result, iter);

                actions += 6;
                if(nodeL == -1 && nodeR == -1)
                {
                    actions++;
                    break;
                }
            }

        }

        // Istrizainems:
        public static void Replace(string list, int height, int width)
        {
            for (int i = 0; i < height / 2; i++)
            {
                SwapRows(list, i, height - 1 - i, width);
            }
        }

        public static void SwapRows(string list, int a, int b, int width)
        {
            for (int i = 0; i < width; i++)
                f.Swap(list, a * width + i, b * width + i);
        }

        public static string Reshuffle(string list, int height, int width)
        {
            int n = height; // eilutes
            int m = width; // stulpeliai
            int toWrite = 0;
            
            string copy = f.Clone(list);

            int iter = f.GetStart(copy);

            int number = 0;

            while (toWrite < n * m)
            {
                ArrowDown(number, list, m, n, ref toWrite, ref iter, copy);
                if (toWrite >= n * m)
                    break;
                ArrowUp(number, list, m, n, ref toWrite, ref iter, copy);
                number++;
            }


            return list;
        }
        
        public static void ArrowDown(int num, string list, int m, int n, ref int toWrite, ref int node, string copy)
        {
            int x = num;
            int y = 0;
            if (num >= m)
            {
                x = m - 1;
                y = num - m + 1;
            }

            for (int i = x; i >= 0; i--)
            {
                f.ChangeValueByIndex(list, y * m + i, f.GetValueByNode(copy, node));
                node = f.GetNext(copy, node);
                y++;
                toWrite++;
                if (y >= n)
                    return;
            }

        }
        
        public static void ArrowUp(int num, string list, int m, int n, ref int toWrite, ref int node, string copy)
        {
            int x = m - num - 1;
            int y = n - 1;
            if (x < 0)
            {
                x = 0;
                y = n - 2 - num + m;
            }

            for (int i = x; i < m; i++)
            {
                f.ChangeValueByIndex(list, y * m + i, f.GetValueByNode(copy, node));
                node = f.GetNext(copy, node);
                y--;
                toWrite++;
                if (y < 0)
                    return;
            }
        }
    }
}
