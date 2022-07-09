using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeSortArray
{
    class DataArray<T> where T : IComparable
    {
        private T[] Array;
        private int Count;

        public DataArray(int size)
        {
            Array = new T[size];
            Count = 0;
        }

        public void Add(T data)
        {
            if (Count < Size())
            {
                Array[Count] = data;
                Count++;
            }
        }

        public void Replace(int index, T data)
        {
            Array[index] = data;
        }

        public T this[int index]
        {
            get { return Array[index]; }
        }

        public void Swap(int index1, int index2)
        {
            T temp = Array[index1];
            Array[index1] = Array[index2];
            Array[index2] = temp;
        }

        public T[] GetArray()
        {
            return Array;
        }

        public void SetArray(T[] arr)
        {
            Array = arr;
        }

        public int Size()
        {
            return Array.Length;
        }
    }
}
