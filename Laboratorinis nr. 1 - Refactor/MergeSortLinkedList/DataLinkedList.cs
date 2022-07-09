using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MergeSortLinkedList
{
    class DataLinkedList<T> where T : IComparable
    {
        public Node<T> Start;
        private Node<T> End;
        private Node<T> Iter;
        public int Count { get; private set; }

        public DataLinkedList()
        {
            Start = null;
            End = null;
            Iter = null;
            Count = 0;
        }

        public void ChangeList(DataLinkedList<T> list)
        {
            Start = list.Start;
            End = list.End;
            Iter = null;
            Count = list.Count;
        }

        public void Add(T data)
        {
            Node<T> toAdd = new Node<T>(data, End, null);

            if(Start == null)
            {
                Start = toAdd;
            }
            else
            {
                End.Next = toAdd;
            }
            End = toAdd;
            Count++;
        }
        
        public void Print()
        {
            for(Iter = Start; Iter != null; Iter = Iter.Next)
            {
                Console.WriteLine(Iter.Data);
            }
        }

        public bool SizeMoreThanOne()
        {
            return Start != End;
        }

        public void Change(int index, T data)
        {
            Iter = Start;
            for (int i = 0; i < index; i++)
            {
                if (Iter == null)
                    break;
                Iter = Iter.Next;
            }
                
            if (Iter != null)
                Iter.Data = data;
        }

        public T Get(int index)
        {
            Iter = Start;
            for (int i = 0; i < index; i++)
                Iter = Iter.Next;

            return Iter.Data;
        }

        public DataLinkedList<T> LeftHalf()
        {
            if (!SizeMoreThanOne())
            {
                Program.actions++;
                return this;
            }
            DataLinkedList<T> left = new DataLinkedList<T>();
            Node<T> middle = GetMiddle();
            Iter = Start;
            while(Iter != middle.Next)
            {
                left.Add(Iter.Data);
                Iter = Iter.Next;
                Program.actions += 3;
            }
            Program.actions += 5;
            return left;
        }

        public DataLinkedList<T> RightHalf()
        {
            if (!SizeMoreThanOne())
            {
                Program.actions++;
                return this;
            }
            DataLinkedList<T> right = new DataLinkedList<T>();
            Iter = GetMiddle().Next;
            while (Iter != null)
            {
                right.Add(Iter.Data);
                Iter = Iter.Next;
                Program.actions += 3;
            }
            Program.actions += 4;
            return right;
        }

        public Node<T> GetMiddle()
        {
            Node<T> slow = Start;
            Node<T> fast = Start;

            while(fast.Next != null && fast.Next.Next != null)
            {
                slow = slow.Next;
                fast = fast.Next.Next;
                Program.actions += 3;
            }
            Program.actions += 3;
            return slow;
        }

        public void Swap(int index1, int index2)
        {
            if (index1 == index2)
                return;

            if(index1 > index2)
            {
                int temp = index1;
                index1 = index2;
                index2 = temp;
            }

            Node<T> first = null;
            Node<T> second = null;

            int i = 0;
            Iter = Start;
            while (Iter != null)
            {
                if (i == index1)
                    first = Iter;
                if (i == index2)
                    second = Iter;

                Iter = Iter.Next;
                i++;
                if (first != null && second != null)
                    break;
            }

            if (first == null || second == null)
                return;

            if (first == Start)
                Start = second;
            if (second == End)
                End = first;

            if (first.Next == second)
            {
                Node<T> firstPrev1 = first.Previous;
                Node<T> secondNext1 = second.Next;
                
                first.Previous = second;
                second.Next = first;
                first.Next = secondNext1;
                second.Previous = firstPrev1;

                if (secondNext1 != null)
                    secondNext1.Previous = first;

                if (firstPrev1 != null)
                    firstPrev1.Next = second;
                
                return;
            }



            Node<T> firstPrev = first.Previous;
            Node<T> firstNext = first.Next;
            Node<T> secondPrev = second.Previous;
            Node<T> secondNext = second.Next;

            


            secondPrev.Next = first;
            if(secondNext != null)
                secondNext.Previous = first;

            if(firstPrev != null)
                firstPrev.Next = second;
            firstNext.Previous = second;

            first.Previous = secondPrev;
            first.Next = secondNext;
            second.Previous = firstPrev;
            second.Next = firstNext;


        }

        public DataLinkedList<T> Clone()
        {
            DataLinkedList<T> clone = new DataLinkedList<T>();
            
            Iter = Start;

            while(Iter != null)
            {
                clone.Add(Iter.Data);
                Iter = Iter.Next;
            }

            return clone;
        }

        public void SetFromArray(T[] arr)
        {
            DataLinkedList<T> list = new DataLinkedList<T>();
            foreach(T item in arr)
            {
                list.Add(item);
            }
            ChangeList(list);
        }

        public Node<T> GetNode(int index)
        {
            Iter = Start;
            for (int i = 0; i < index; i++)
            {
                if (Iter == null)
                    return null;

                Iter = Iter.Next;
            }

            return Iter;
        }
    }

    sealed class Node<T>
    {
        public Node<T> Previous { get; set; }
        public Node<T> Next { get; set; }
        public T Data;
        
        public Node(T data, Node<T> prev, Node<T> next)
        {
            Data = data;
            Previous = prev;
            Next = next;
        }

        public void Print()
        {
            if (Next != null)
            {
                Console.WriteLine(Data);
                Next.Print();
            }
        }
    }
}
