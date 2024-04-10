using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{
	private SortedDictionary<int, Queue<T>> dict = new SortedDictionary<int, Queue<T>>();

	public int Count
	{
		get
		{
			int count = 0;
			foreach (var pair in dict)
			{
				count += pair.Value.Count;
			}
			return count;
		}
	}

	public void Enqueue(T item, int priority)
	{
		if (!dict.ContainsKey(priority))
		{
			dict[priority] = new Queue<T>();
		}
		dict[priority].Enqueue(item);
	}

	public T Dequeue()
	{
		if (Count == 0)
		{
			throw new InvalidOperationException("Priority queue is empty.");
		}

		foreach (var pair in dict)
		{
			if (pair.Value.Count > 0)
			{
				return pair.Value.Dequeue();
			}
		}
		throw new InvalidOperationException("Priority queue is empty.");
	}
}
