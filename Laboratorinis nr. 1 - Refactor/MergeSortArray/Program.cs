using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;

namespace MergeSortArray
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

            using (FileStream file = new FileStream(saveDir + Path.GetFileNameWithoutExtension(file1) + ".bmp", FileMode.Open, FileAccess.Read))
            {
                for (int i1 = 10; i1 <= (image.Width < image.Height ? image.Width : image.Height); i1 += 10)
                {
                    file.Seek(0, SeekOrigin.Begin);


                    DataArray<byte> b = new DataArray<byte>((int)file.Length);

                    file.Read(b.GetArray(), 0, (int)file.Length);

                    DataArray<int> bs = new DataArray<int>(i1 * i1);

                    int k = 54;
                    for(int i = 0; i < i1; i++)
                    {
                        for(int j = 0; j < width; j++)
                        {
                            if(j < i1)
                            {
                                file.Seek(k, SeekOrigin.Begin);
                                byte b_ = (byte)file.ReadByte();
                                byte g = (byte)file.ReadByte();
                                byte r = (byte)file.ReadByte();
                                bs.Add(((r << 8) + g << 8) + b_);
                            }

                            k += 3;
                        }
                    }
                    
                    Stopwatch watch;
                    
                    watch = Stopwatch.StartNew();
                    MergeSort(bs);
                    watch.Stop();
                    int ms = (int) watch.ElapsedMilliseconds;

                    Console.WriteLine("{0, 5} -> {1} {2}", i1, ms, actions);
                    PrintData(ms, i1 * i1, "Array.csv");

                    actions = 0;
                    
                    bs = Reshuffle(bs, i1, i1);
                    Replace(bs, i1, i1);


                    k = 54;
                    int m = 0;
                    for (int i = 0; i < i1; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            if (j < i1)
                            {
                                DataArray<byte> p = new DataArray<byte>(BitConverter.GetBytes(bs[m]).Length);
                                p.SetArray(BitConverter.GetBytes(bs[m]));

                                b.Replace(k, p[0]);
                                b.Replace(k + 1, p[1]);
                                b.Replace(k + 2, p[2]);

                                m++;
                            }
                               
                            k += 3;
                        }
                    }

                    using (FileStream file2 = new FileStream(saveDir + Path.GetFileNameWithoutExtension(file1) + i1 + "Sorted.bmp", FileMode.Create, FileAccess.Write))
                    {
                        file2.Seek(0, SeekOrigin.Begin);
                        file2.Write(b.GetArray(), 0, b.Size());
                        file2.Close();
                    }

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
            foreach(string file in files)
            {
                File.Delete(file);
            }
        }

        public static void PrintData(int ms, int size, string fileName)
        {
            using(StreamWriter writer = new StreamWriter(saveDir + fileName, true))
            {
                writer.WriteLine(size + "," + ms + "," + actions);
            }
        }

        public static void Replace(DataArray<int> arr, int height, int width)
        {
            for (int i = 0; i < height / 2; i++)
            {
                SwapRows(arr, i, height - 1 - i, width);
            }
        }

        public static void SwapRows(DataArray<int> arr, int a, int b, int width)
        {
            for (int i = 0; i < width; i++)
                arr.Swap(a * width + i, b * width + i);
        }

        public static DataArray<int> Reshuffle(DataArray<int> arr, int height, int width)
        {
            int n = height; // eilutes
            int m = width; // stulpeliai
            int toWrite = 0;

            DataArray<int> toReturn = new DataArray<int>(arr.Size());

            int number = 0;

            while (toWrite < n * m)
            {
                ArrowDown(number, toReturn, m, n, ref toWrite, arr);
                if (toWrite >= n * m)
                    break;
                ArrowUp(number, toReturn, m, n, ref toWrite, arr);
                number++;
            }


            return toReturn;
        }

        public static void ArrowDown(int num, DataArray<int> array, int m, int n, ref int toWrite, DataArray<int> pixels)
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
                array.Replace(y * m + i, pixels[toWrite]);
                y++;
                toWrite++;
                if (y >= n)
                    return;
            }

        }

        public static void ArrowUp(int num, DataArray<int> array, int m, int n, ref int toWrite, DataArray<int> pixels)
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
                array.Replace(y * m + i, pixels[toWrite]);
                y--;
                toWrite++;
                if (y < 0)
                    return;
            }
        }

        public static DataArray<int> LeftHalf(DataArray<int> array)
        {
            int size1 = array.Size() / 2;
            DataArray<int> left = new DataArray<int>(size1);
            for (int i = 0; i < size1; i++)
            {
                left.Replace(i, array[i]);
            }
            actions += (3 + size1 + size1);
            return left;
        }
        public static DataArray<int> RightHalf(DataArray<int> array)
        {
            int size1 = array.Size() / 2;
            int size2 = array.Size() - size1;
            DataArray<int> right = new DataArray<int>(size2);
            for (int i = 0; i < size2; i++)
            {
                right.Replace(i, array[i + size1]);
            }
            actions += (4 + size2 + size2);
            return right;
        }

        public static void Merge(DataArray<int> result,
                         DataArray<int> left, DataArray<int> right)
        {
            int i1 = 0;
            int i2 = 0;
            for (int i = 0; i < result.Size(); i++)
            {
                if (i2 >= right.Size() ||
                        (i1 < left.Size() &&
                        left[i1] <= right[i2]))
                {
                    result.Replace(i, left[i1++]);
                }
                else
                {
                    result.Replace(i, right[i2++]);
                }
            }
            actions += (3 + 3 * result.Size());
        }

        public static void MergeSort(DataArray<int> array)
        {
            if (array.Size() > 1)
            {
                DataArray<int> left = LeftHalf(array);
                DataArray<int> right = RightHalf(array);
                
                MergeSort(left);
                MergeSort(right);
                
                Merge(array, left, right);
                actions += 5;
            }
            actions++;
        }
    }
}
