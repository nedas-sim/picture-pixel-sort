namespace Models.Array;

public class ArrayRAM<T> : IArrayHandler<T> where T : IComparable
{
    T[] _array;
    int _count;

    public ArrayRAM(int size)
    {
        _array = new T[size];
        _count = 0;
    }

    public void Add(T data)
    {
        if (_count < _array.Length)
        {
            _array[_count] = data;
            _count++;
        }
    }

    public T Get(int index)
    {
        return _array[index];
    }

    public T[] GetArray()
    {
        return _array;
    }

    public void Replace(int index, T data)
    {
        _array[index] = data;
    }

    public void SetArray(T[] array)
    {
        _array = array;
    }

    public int Size()
    {
        return _array.Length;
    }

    public void Swap(int index1, int index2)
    {
        (_array[index1], _array[index2]) = (_array[index2], _array[index1]);
    }

    #region Merge Sort
    public void MergeSort()
    {
        if (Size() > 1)
        {
            ArrayRAM<T> left = LeftHalf();
            ArrayRAM<T> right = RightHalf();

            left.MergeSort();
            right.MergeSort();

            Merge(left, right);
        }
    }

    private void Merge(ArrayRAM<T> left, ArrayRAM<T> right)
    {
        int leftIndex = 0, rightIndex = 0;
        for (int i = 0; i < Size(); i++)
        {
            if (rightIndex >= right.Size() ||
                (leftIndex < left.Size() && 
                 left.Get(leftIndex).CompareTo(right.Get(rightIndex)) <= 0))
            {
                Replace(i, left.Get(leftIndex));
                leftIndex++;
            }
            else
            {
                Replace(i, right.Get(rightIndex));
                rightIndex++;
            }
        }
    }

    private ArrayRAM<T> LeftHalf()
    {
        int leftHalfSize = Size() / 2;
        ArrayRAM<T> left = new(leftHalfSize);
        for (int i = 0; i < leftHalfSize; i++)
        {
            left.Add(_array[i]);
        }
        return left;
    }

    private ArrayRAM<T> RightHalf()
    {
        int leftHalfSize = Size() / 2;
        int rightHalfSize = Size() - leftHalfSize;
        ArrayRAM<T> right = new(rightHalfSize);
        for (int i = 0; i < rightHalfSize; i++)
        {
            right.Add(_array[leftHalfSize + i]);
        }
        return right;
    }
    #endregion

    #region Reshuffle
    /// <summary>
    /// Reshuffles elements in the array in a diagonal. More about it in the readme file.
    /// </summary>
    /// <param name="height">Sorting image's height</param>
    /// <param name="width">Sorting image's width</param>
    public void Reshuffle(int height, int width)
    {
        int toWrite = 0; // what is this?
        ArrayRAM<T> returnValue = new(Size());
        int number = 0; //what is this?

        while (toWrite < width * height)
        {
            ArrowDown(number, returnValue, width, height, ref toWrite);
            if (toWrite >= width * height)
            {
                break;
            }
            ArrowUp(number, returnValue, width, height, ref toWrite);
            number++;
        }
        _array = returnValue.GetArray();
    }

    private void ArrowDown(int num, ArrayRAM<T> array, int width, int height, ref int toWrite)
    {
        int x = num;
        int y = 0;
        if (num >= width)
        {
            x = width - 1;
            y = num - width + 1;
        }

        for (int i = x; i >= 0; i--)
        {
            array.Replace(y * width + i, Get(toWrite));
            y++;
            toWrite++;
            if (y >= height)
            {
                return;
            }
        }
    }

    private void ArrowUp(int num, ArrayRAM<T> array, int width, int height, ref int toWrite)
    {
        int x = width - num - 1;
        int y = height - 1;
        if (x < 0)
        {
            x = 0;
            y = height - 2 - num + width;
        }

        for (int i = x; i < width; i++)
        {
            array.Replace(y * width + i, Get(toWrite));
            y--;
            toWrite++;
            if (y < 0)
            {
                return;
            }
        }
    }
    #endregion

    #region TurnUpsideDown
    /// <summary>
    /// Fixes reshuffle method. When I did the reshuffle, I realised it was upside down. Couldn't bother rethinking the 'reshuffle' logic,
    /// so I created TurnUpsideDown method to fix it.
    /// </summary>
    /// <param name="height">Sorting image's height</param>
    /// <param name="width">Sorting image's width</param>
    public void TurnUpsideDown(int height, int width)
    {
        for (int i = 0; i < height / 2; i++)
        {
            SwapRows(i, height - 1 - i, width);
        }
    }

    private void SwapRows(int rowIndex1, int rowIndex2, int width)
    {
        rowIndex1 *= width;
        rowIndex2 *= width;

        for (int i = 0; i < width; i++)
        {
            Swap(rowIndex1, rowIndex2);
            rowIndex1++;
            rowIndex2++;
        }
    }
    #endregion
}
