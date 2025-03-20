// Copied from https://stackoverflow.com/a/1937706
using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    SortedList<Pair<int>, T> _list;
    int id;

    public PriorityQueue()
    {
        _list = new SortedList<Pair<int>, T>(new PairComparer<int>());
    }

    public void Enqueue(T item, int priority)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item), "Item cannot be null.");
        }
        _list.Add(new Pair<int>(priority, id), item);
        id++;
    }

    public T Dequeue()
    {
        T item = Peek();
        _list.RemoveAt(0);
        return item;
    }

    public int Count()
    {
        return _list.Count;
    }

    public T Peek()
    {
        if (_list.Count == 0)
        {
            throw new InvalidOperationException("The priority queue is empty.");
        }
        return _list[_list.Keys[0]];
    }

    public bool Remove(T item)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            if (EqualityComparer<T>.Default.Equals(_list.Values[i], item))
            {
                _list.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public bool Contains(T item)
    {
        foreach (T value in _list.Values)
        {
            if (EqualityComparer<T>.Default.Equals(value, item))
            {
                return true;
            }
        }
        return false;
    }
}

class Pair<T>
{
    public T First { get; private set; }
    public T Second { get; private set; }

    public Pair(T first, T second)
    {
        First = first;
        Second = second;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + First.GetHashCode();
            hash = hash * 31 + Second.GetHashCode();
            return hash;
        }
    }

    public override bool Equals(object other)
    {
        Pair<T> pair = other as Pair<T>;
        if (pair == null)
        {
            return false;
        }
        return this.First.Equals(pair.First) && this.Second.Equals(pair.Second);
    }
}

class PairComparer<T> : IComparer<Pair<T>> where T : IComparable
{
    public int Compare(Pair<T> x, Pair<T> y)
    {
        if (x.First.CompareTo(y.First) < 0)
        {
            return -1;
        }
        else if (x.First.CompareTo(y.First) > 0)
        {
            return 1;
        }
        else
        {
            return x.Second.CompareTo(y.Second);
        }
    }
}