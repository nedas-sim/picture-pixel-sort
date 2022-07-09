using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace MergeSortFile
{
    class Program
    {
        public static FileClass file = new FileClass();
        public static int iter = 1;
        public static string saveDir = "..\\..\\nuotraukosOutput\\";
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
            

            for (int i1 = 10; i1 <= (image.Width < image.Height ? image.Width : image.Height); i1 += 10)
            {
                string finalFile = saveDir + Path.GetFileNameWithoutExtension(file1) + i1 + "Sorted.bmp";
                                
                using(FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    for (int i = 0; i < 54; i++)
                    {
                        byte value = (byte)stream.ReadByte();
                        file.WriteByte(finalFile, value);
                    }

                    for (int i = 0; i < i1; i++)
                    {
                        for (int j = 0; j < image.Width; j++)
                        {
                            byte one = (byte)stream.ReadByte();
                            byte two = (byte)stream.ReadByte();
                            byte three = (byte)stream.ReadByte();
                            if (j < i1)
                            {
                                file.WriteByte(temp, one);
                                file.WriteByte(temp, two);
                                file.WriteByte(temp, three);
                            }
                            else
                            {
                                file.WriteByte(other, one);
                                file.WriteByte(other, two);
                                file.WriteByte(other, three);
                            }
                        }
                    }
                    for (int i = 0; i < image.Width * (image.Height - i1); i++)
                    {
                        byte one = (byte)stream.ReadByte();
                        byte two = (byte)stream.ReadByte();
                        byte three = (byte)stream.ReadByte();
                        file.WriteByte(other, one);
                        file.WriteByte(other, two);
                        file.WriteByte(other, three);
                    }
                    stream.Close();
                }
                

                Stopwatch watch;

                Console.WriteLine("Pradeda mergesort");
                watch = Stopwatch.StartNew();
                MergeSortBinary(temp);
                watch.Stop();
                int ms = (int)watch.ElapsedMilliseconds;
                Console.WriteLine("Pabaige mergesort per " + ms + " milisekundziu");

                PrintData(ms, i1 * i1, "File_Array.csv");
                actions = 0;

                Console.WriteLine("Pradeda perkelima");
                string newTemp = ReshuffleBinary(temp, i1, i1);
                Replace(newTemp, i1, i1);
                Console.WriteLine("Pabaige perkelima");



                Console.WriteLine("Pradeda irasyma");
                int sort = 0;
                int rest = 0;
                for(int i = 0; i < i1; i++)
                {
                    for(int j = 0; j < image.Width; j++)
                    {
                        int value = 0;
                        if(j < i1)
                        {
                            value = file.Read(newTemp, sort);
                            sort++;
                        }
                        else
                        {
                            value = file.Read(other, rest);
                            rest++;
                        }
                        file.Write(finalFile, value);
                    }
                }
                for(int i = rest; i < image.Width * image.Height - i1 * i1; i++)
                {
                    int value = file.Read(other, i);
                    file.Write(finalFile, value);
                }
                for(int i = 54 + 3 * image.Width * image.Height; i < file.ByteLength(fileName); i++)
                {
                    byte value = file.ReadByte(fileName, i);
                    file.WriteByte(finalFile, value);
                }
                Console.WriteLine("Pabaige irasyma");

                
                DeleteFiles();

                
                iter = 1;

                Console.WriteLine();

                if (ms > 60 * 1000)
                    break;

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
            for(int i = 0; i < files.Length; i++)
            {
                if (File.Exists(files[i]))
                    File.Delete(files[i]);
            }
        }

        static void MergeSortBinary(string file1)
        {
            if(file.Length(file1) > 1)
            {
                string left = LeftHalfBinary(file1);
                MergeSortBinary(left);
                string right = RightHalfBinary(file1);
                MergeSortBinary(right);
                MergeBinary(file1, left, right);
                actions += 5;
            }
        }

        static string LeftHalfBinary(string file1)
        {
            string filename = saveDir + iter + ".txt";
            iter++;
            int limit = file.Length(file1) / 2;
            for(int i = 0; i < limit; i++)
            {
                int toWrite = file.Read(file1, i);
                file.Write(filename, toWrite);
            }
            actions += (5 + limit);
            return filename;
        }

        static string RightHalfBinary(string file1)
        {
            string filename = saveDir + iter + ".txt";
            iter++;
            int limit = file.Length(file1) / 2;
            int count = file.Length(file1);
            for(int i = limit; i < count; i++)
            {
                int toWrite = file.Read(file1, i);
                file.Write(filename, toWrite);
            }
            actions += (5 + count - limit);
            return filename;
        }

        static void MergeBinary(string result, string left, string right)
        {
            int i1 = 0;
            int i2 = 0;
            for (int i = 0; i < file.Length(result); i++)
            {
                if (i2 >= file.Length(right) ||
                    (i1 < file.Length(left) && file.Read(left, i1) <= file.Read(right, i2)))
                {
                    file.Replace(result, file.Read(left, i1++), i);
                }
                else
                {
                    file.Replace(result, file.Read(right, i2++), i);
                }
            }
            actions += 3 + 3 * file.Length(result);
        }


        // Istrizainems:

        public static string ReshuffleBinary(string file1, int height, int width)
        {
            int n = height; // eilutes
            int m = width; // stulpeliai
            int toWrite = 0;

            string toReturn = file.Clone(file1);

            int number = 0;

            while (toWrite < n * m)
            {
                ArrowDown(number, toReturn, m, n, ref toWrite, file1);
                if (toWrite >= n * m)
                    break;
                ArrowUp(number, toReturn, m, n, ref toWrite, file1);
                number++;
            }


            return toReturn;
        }

        public static void ArrowDown(int num, string array, int m, int n, ref int toWrite, string pixels)
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
                file.Replace(array, file.Read(pixels, toWrite), y * m + i);
                y++;
                toWrite++;
                if (y >= n)
                    return;
            }

        }

        public static void ArrowUp(int num, string array, int m, int n, ref int toWrite, string pixels)
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
                file.Replace(array, file.Read(pixels, toWrite), y * m + i);
                y--;
                toWrite++;
                if (y < 0)
                    return;
            }
        }

        // Sutaisymui:
        public static void Replace(string array, int height, int width)
        {
            for (int i = 0; i < height / 2; i++)
            {
                SwapRows(array, i, height - 1 - i, width);
            }
        }

        public static void SwapRows(string array, int a, int b, int width)
        {
            for (int i = 0; i < width; i++)
            {
                int one = file.Read(array, a * width + i);
                int two = file.Read(array, b * width + i);

                file.Replace(array, one, b * width + i);
                file.Replace(array, two, a * width + i);
            }
        }
    }
}
