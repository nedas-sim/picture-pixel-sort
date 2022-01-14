using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;

namespace MergeSortLinkedList
{
    class Program
    {
        public static int actions = 0;
        public static string saveDir = "..\\..\\nuotraukosOutput\\";

        static void Main(string[] args)
        {
            Delete();

            string dir = "..\\..\\nuotraukosInput\\";
            

            string file1 = Directory.GetFiles(dir, "*.jpg")[0];

            Console.WriteLine("Failas: " + file1);
            Bitmap image = new Bitmap(file1);
            image.Save(saveDir + Path.GetFileNameWithoutExtension(file1) + ".bmp", ImageFormat.Bmp);

            int width = image.Width;
            int height = image.Height;

            Console.WriteLine("Dydis: " + width + " " + height);
            Console.WriteLine();

            using (FileStream file = new FileStream(saveDir + Path.GetFileNameWithoutExtension(file1) + ".bmp", FileMode.Open, FileAccess.Read))
            {
                for (int i1 = 10; i1 <= (image.Width < image.Height ? image.Width : image.Height); i1 += 10)
                {
                    file.Seek(0, SeekOrigin.Begin);
                    
                    DataLinkedList<int> bs = new DataLinkedList<int>();

                    file.Seek(54, SeekOrigin.Begin);
                    for (int i = 0; i < i1; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            if(j < i1)
                            {
                                byte b_ = (byte)file.ReadByte();
                                byte g = (byte)file.ReadByte();
                                byte r = (byte)file.ReadByte();
                                bs.Add(((r << 8) + g << 8) + b_);
                            }
                        }
                    }

                    Stopwatch watch;

                    
                    watch = Stopwatch.StartNew();
                    MergeSort(bs);
                    watch.Stop();
                    int ms = (int)watch.ElapsedMilliseconds;
                    
                    Console.WriteLine("{0} -> {1}", i1, ms);
                    PrintData(ms, i1 * i1, "LinkedList.csv");

                    actions = 0;

                    Console.WriteLine("Pradeda istrizaines");
                    bs = Reshuffle(bs, i1, i1);
                    Replace(bs, i1, i1);
                    Console.WriteLine("Pabaige istrizaines");


                    Console.WriteLine("Pradeda irasyma");
                    image.Save(saveDir + Path.GetFileNameWithoutExtension(file1) + i1 + "Sorted.bmp", ImageFormat.Bmp);
                    using (FileStream file2 = new FileStream(saveDir + Path.GetFileNameWithoutExtension(file1) + i1 + "Sorted.bmp", FileMode.Open, FileAccess.Write))
                    {
                        int k = 54;
                        Node<int> node = bs.Start;
                        for (int i = 0; i < i1; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                if (j < i1)
                                {
                                    DataLinkedList<byte> bytes = new DataLinkedList<byte>();
                                    bytes.SetFromArray(BitConverter.GetBytes(node.Data));
                                    file2.Seek(k, SeekOrigin.Begin);
                                    file2.WriteByte(bytes.Get(0));
                                    file2.Seek(k + 1, SeekOrigin.Begin);
                                    file2.WriteByte(bytes.Get(1));
                                    file2.Seek(k + 2, SeekOrigin.Begin);
                                    file2.WriteByte(bytes.Get(2));
                                    node = node.Next;
                                }
                                k += 3;
                            }
                        }

                        file2.Close();

                    }
                    Console.WriteLine("Pabaige irasyma");
                    Console.WriteLine();


                    if (ms > 5 * 1000)
                        break;

                }
                
                file.Close();
            }

            Console.WriteLine();


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

        static void MergeSort(DataLinkedList<int> list)
        {
            if(list.SizeMoreThanOne())
            {
                DataLinkedList<int> left = list.LeftHalf();
                DataLinkedList<int> right = list.RightHalf();

                MergeSort(left);
                MergeSort(right);

                Merge(list, left, right);
                actions += 5;
            }
            actions++;
        }

        static void Merge(DataLinkedList<int> list,
                          DataLinkedList<int> left,
                          DataLinkedList<int> right)
        {

            Node<int> nodeL = left.Start;
            Node<int> nodeR = right.Start;
            Node<int> iter = list.Start;
            actions += 3;

            while(true)
            {
                actions += 6;
                if(nodeR == null || (nodeL != null && nodeL.Data <= nodeR.Data))
                {
                    iter.Data = nodeL.Data;
                    nodeL = nodeL.Next;
                }
                else
                {
                    iter.Data = nodeR.Data;
                    nodeR = nodeR.Next;
                }

                iter = iter.Next;

                if (nodeL == null && nodeR == null)
                {
                    actions++;
                    break;
                }
            }
        }



        public static void Replace(DataLinkedList<int> list, int height, int width)
        {
            for (int i = 0; i < height / 2; i++)
            {
                SwapRows(list, i, height - 1 - i, width);
            }
        }

        public static void SwapRows(DataLinkedList<int> list, int a, int b, int width)
        {
            for (int i = 0; i < width; i++)
                list.Swap(a * width + i, b * width + i);
        }

        public static DataLinkedList<int> Reshuffle(DataLinkedList<int> list, int height, int width)
        {
            int n = height; // eilutes
            int m = width; // stulpeliai
            int toWrite = 0;
            
            DataLinkedList<int> copy = list.Clone();

            Node<int> iter = copy.Start;

            int number = 0;

            while (toWrite < n * m)
            {
                ArrowDown(number, list, m, n, ref toWrite, ref iter);
                if (toWrite >= n * m)
                    break;
                ArrowUp(number, list, m, n, ref toWrite, ref iter);
                number++;
            }


            return list;
        }
        
        public static void ArrowDown(int num, DataLinkedList<int> list, int m, int n, ref int toWrite, ref Node<int> node)
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
                list.Change(y * m + i, node.Data);
                node = node.Next;
                y++;
                toWrite++;
                if (y >= n)
                    return;
            }

        }
        
        public static void ArrowUp(int num, DataLinkedList<int> list, int m, int n, ref int toWrite, ref Node<int> node)
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
                list.Change(y * m + i, node.Data);
                node = node.Next;
                y--;
                toWrite++;
                if (y < 0)
                    return;
            }
        }
    }
}
