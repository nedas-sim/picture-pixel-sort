using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MergeSortFileLinkedList
{
    class FileClass
    {
        private BinaryReader Reader;
        private BinaryWriter Writer;

        private void Constructor(string file)
        {
            using(Writer = new BinaryWriter(File.Open(file, FileMode.Create, FileAccess.Write)))
            {
                byte[] arr = new byte[4];
                arr = BitConverter.GetBytes(-1);
                Writer.Write(arr, 0, arr.Length);
                Writer.Write(arr, 0, arr.Length);
            }
        }

        public int GetStart(string file)
        {
            using(Reader = new BinaryReader(File.Open(file, FileMode.Open, FileAccess.Read)))
            {
                Reader.BaseStream.Seek(0, SeekOrigin.Begin);
                int value = Reader.ReadInt32();
                Reader.Close();
                return value;
            }
        }

        public int GetEnd(string file)
        {
            using(Reader = new BinaryReader(File.Open(file, FileMode.Open, FileAccess.Read)))
            {
                Reader.BaseStream.Seek(4, SeekOrigin.Begin);
                int value = Reader.ReadInt32();
                Reader.Close();
                return value;
            }
        }

        public void SetStart(string file, int value)
        {
            using(Writer = new BinaryWriter(File.Open(file, FileMode.Open, FileAccess.Write)))
            {
                byte[] values = BitConverter.GetBytes(value);
                Writer.Seek(0, SeekOrigin.Begin);
                Writer.Write(values, 0, 4);
                Writer.Close();
            }

        }

        public void SetEnd(string file, int value)
        {
            using (Writer = new BinaryWriter(File.Open(file, FileMode.Open, FileAccess.Write)))
            {
                byte[] values = BitConverter.GetBytes(value);
                Writer.Seek(4, SeekOrigin.Begin);
                Writer.Write(values, 0, 4);
                Writer.Close();
            }
        }

        public int Count(string file)
        {
            using(Reader = new BinaryReader(File.Open(file, FileMode.Open, FileAccess.Read)))
            {
                int bytes = (int) Reader.BaseStream.Length;
                bytes -= 8;
                Reader.Close();
                return bytes / 11;
            }
        }

        public void SetPrevious(string file, int node, int prev)
        {
            using(Writer = new BinaryWriter(File.Open(file, FileMode.Open, FileAccess.Write)))
            {
                byte[] prev_ = BitConverter.GetBytes(prev);
                Writer.Seek(8 + 11 * node, SeekOrigin.Begin);
                Writer.Write(prev_, 0, 4);
                Writer.Close();
            }
        }

        public void SetNext(string file, int node, int next)
        {
            using (Writer = new BinaryWriter(File.Open(file, FileMode.Open, FileAccess.Write)))
            {
                byte[] next_ = BitConverter.GetBytes(next);
                Writer.Seek(8 + 11 * node + 4, SeekOrigin.Begin);
                Writer.Write(next_, 0, 4);
                Writer.Close();
            }
        }

        public int GetPrevious(string file, int node)
        {
            using (Reader = new BinaryReader(File.Open(file, FileMode.Open, FileAccess.Read)))
            {
                Reader.BaseStream.Seek(8 + 11 * node, SeekOrigin.Begin);
                int prev = Reader.ReadInt32();
                Reader.Close();
                return prev;
            }
        }

        public int GetNext(string file, int node)
        {
            using(Reader = new BinaryReader(File.Open(file, FileMode.Open, FileAccess.Read)))
            {
                Reader.BaseStream.Seek(8 + 11 * node + 4, SeekOrigin.Begin);
                int next = Reader.ReadInt32();
                Reader.Close();
                return next;
            }
        }

        public int GetValueByNode(string file, int node)
        {
            using(Reader = new BinaryReader(File.Open(file, FileMode.Open, FileAccess.Read)))
            {
                Reader.BaseStream.Seek(8 + 11 * node + 8, SeekOrigin.Begin);
                byte[] values = Reader.ReadBytes(3);
                int value = (((values[2] << 8) + values[1]) << 8) + values[0];
                Reader.Close();
                return value;
            }
        }

        public void Add(string file, int value)
        {
            if (!File.Exists(file))
            {
                Constructor(file);
            }

            byte[] array = new byte[11];
            byte[] end = BitConverter.GetBytes(GetEnd(file));
            byte[] null_ = BitConverter.GetBytes(-1);
            byte[] values = BitConverter.GetBytes(value);

            for(int i = 0; i < 4; i++)
                array[i] = end[i];

            for (int i = 0; i < 4; i++)
                array[i + 4] = null_[i];

            for (int i = 0; i < 3; i++)
                array[i + 8] = values[i];


            
            if(GetStart(file) == -1)
            {
                SetStart(file, Count(file));
            }
            else
            {
                SetNext(file, GetEnd(file), Count(file));
            }
            SetEnd(file, Count(file));


            using(Writer = new BinaryWriter(File.Open(file, FileMode.Open, FileAccess.Write)))
            {
                Writer.Seek(0, SeekOrigin.End);
                Writer.Write(array, 0, 11);
                Writer.Close();
            }

        }

        public int Get(string file, int index)
        {
            int pointer = GetStart(file);

            for(int i = 0; i < index; i++)
            {
                pointer = GetNext(file, pointer);
                if (pointer == -1)
                    break;
            }

            if (pointer != -1)
                return GetValueByNode(file, pointer);

            return -1;
        }

        public int GetNode(string file, int index)
        {
            int pointer = GetStart(file);

            for (int i = 0; i < index; i++)
            {
                pointer = GetNext(file, pointer);
                if (pointer == -1)
                    break;
            }

            return pointer;
        }

        public void Print(string file)
        {
            int node = GetStart(file);
            while (node != -1)
            {
                Console.WriteLine(GetValueByNode(file, node));
                node = GetNext(file, node);
            }
            Console.WriteLine();
        }

        public void Swap(string file, int index1, int index2)
        {
            if (index1 == index2)
                return;

            if (index1 > index2)
            {
                int temp = index1;
                index1 = index2;
                index2 = temp;
            }
            
            int first = -1;
            int second = -1;

            int i = 0;
            int iter = GetStart(file);


            while(iter != -1)
            {
                if (i == index1)
                    first = iter;
                if (i == index2)
                    second = iter;

                iter = GetNext(file, iter);
                i++;
                if (first != -1 && second != -1)
                    break;
            }


            if (first == -1 || second == -1)
                return;
            

            if (first == GetStart(file))
            {
                SetStart(file, second);
            }
            if (second == GetEnd(file))
            {
                SetEnd(file, first);
            }
                

            
            if(GetNext(file, first) == second)
            {
                int firstPrev1 = GetPrevious(file, first);
                int secondNext1 = GetNext(file, second);

                SetPrevious(file, first, second);
                SetNext(file, second, first);
                SetNext(file, first, secondNext1);
                SetPrevious(file, second, firstPrev1);

                if (secondNext1 != -1)
                    SetPrevious(file, secondNext1, first);

                if (firstPrev1 != -1)
                    SetNext(file, firstPrev1, second);

                return;
            }

            
            int firstPrev = GetPrevious(file, first);
            int firstNext = GetNext(file, first);
            int secondPrev = GetPrevious(file, second);
            int secondNext = GetNext(file, second);
            
            SetNext(file, secondPrev, first);
            if (secondNext != -1)
                SetPrevious(file, secondNext, first);

            if (firstPrev != -1)
                SetNext(file, firstPrev, second);
            SetPrevious(file, firstNext, second);

            SetPrevious(file, first, secondPrev);
            SetNext(file, first, secondNext);
            SetPrevious(file, second, firstPrev);
            SetNext(file, second, firstNext);



        }

        public int GetMiddle(string file)
        {
            int slow = GetStart(file);
            int fast = GetStart(file);
            Program.actions += 2;

            while (GetNext(file, fast) != -1 && GetNext(file, GetNext(file, fast)) != -1)
            {
                Program.actions += 4;
                slow = GetNext(file, slow);
                fast = GetNext(file, fast);
                fast = GetNext(file, fast);
            }
            Program.actions++;
            return slow;
        }

        public string LeftHalf(string file)
        {
            string left = Program.saveDir + Program.fileIter + ".txt";
            Program.fileIter++;

            Program.actions += 2;
            if(Count(file) == 1)
            {
                Program.actions++;
                return file;
            }

            int middle = GetMiddle(file);
            int iter = GetStart(file);
            Program.actions++;

            while (iter != GetNext(file, middle))
            {
                Program.actions += 3;
                Add(left, GetValueByNode(file, iter));
                iter = GetNext(file, iter);
            }
            Program.actions++;
            return left;
        }

        public string RightHalf(string file)
        {
            string right = Program.saveDir + Program.fileIter + ".txt";
            Program.fileIter++;

            Program.actions += 2;
            if (Count(file) == 1)
            {
                Program.actions++;
                return file;
            }

            int middle = GetMiddle(file);
            int iter = GetNext(file, middle);
            Program.actions++;

            while (iter != -1)
            {
                Program.actions += 3;
                Add(right, GetValueByNode(file, iter));
                iter = GetNext(file, iter);
            }
            Program.actions++;
            return right;
        }

        public void ChangeValue(string file, int node, int value)
        {
            using(Writer = new BinaryWriter(File.Open(file, FileMode.Open, FileAccess.Write)))
            {
                Writer.Seek(8 + 11 * node + 8, SeekOrigin.Begin);
                byte[] values = BitConverter.GetBytes(value);
                Writer.Write(values, 0, 3);
                Writer.Close();
            }
        }

        public void ChangeValueByIndex(string file, int index, int value)
        {
            int node = GetNode(file, index);
            ChangeValue(file, node, value);
        }

        public string Clone(string fileToCopy)
        {
            string newFile = Program.saveDir + Path.GetFileNameWithoutExtension(fileToCopy) + "New.txt";
            Constructor(newFile);

            int iter = GetStart(fileToCopy);
            while(iter != -1)
            {
                int value = GetValueByNode(fileToCopy, iter);
                Add(newFile, value);
                iter = GetNext(fileToCopy, iter);
            }
            
            return newFile;
        }
    }
}
