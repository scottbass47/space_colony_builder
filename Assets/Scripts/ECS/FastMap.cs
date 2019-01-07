using System;

namespace ECS
{
    // FastMap uses array indices as keys. This results in O(1) get,
    // O(n) remove, and O(1) contains. FastMap is most efficient if
    // the indices used are close together and small. Large indices
    // will cause the internal array to grow to that size leading to 
    // a lot of wasted space.
    public class FastMap<T> where T : class
    {
        private T[] data;

        public int Capacity => data.Length; 
        public int Size { get; private set; } // Number of non-null elements in the map
        public bool Empty => Size == 0;

        public FastMap(int capacity)
        {
            data = new T[capacity];
            Size = 0;
        }

        public FastMap() : this(4)
        {
        }

        public T Get(int index)
        {
            if (OutOfBounds(index)) throw new IndexOutOfRangeException();
            return data[index];
        }

        public void Put(int index, T item)
        {
            if (index >= Capacity) Grow(index + 1);
            if (data[index] == null && item != null) Size++;
            data[index] = item;
        }

        public T Remove(int index)
        {
            if (OutOfBounds(index)) return null;
            T item = Get(index);
            Put(index, null);
            if (item != null) Size--; 
            return item;
        }

        // This remove is O(n) because all values must be checked. Use
        // remove(index) for O(1) time.
        public bool Remove(T item)
        {
            for(int i = 0; i < Capacity; i++)
            {
                if(data[i] == item)
                {
                    Remove(i);
                    return true;
                }
            }
            return false;
        }

        public bool OutOfBounds(int index)
        {
            return index < 0 || index >= Capacity;
        }

        private void Grow()
        {
            Grow(Capacity * 2);
        }

        private void Grow(int newCapacity)
        {
            T[] newData = new T[newCapacity];
            Array.Copy(data, newData, Capacity);
            data = newData;
        }

        public T this[int i]
        {
            get { return Get(i);  }
            set { Put(i, value); }
        }

        //public static FastArray<T> operator+(FastArray<T> arr, T item)
        //{
        //    arr.Add(item);
        //    return arr;
        //} 
    } 
}