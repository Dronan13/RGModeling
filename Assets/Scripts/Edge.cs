using System;

public class Edge<T> where T : IEdgeItem<T> {
	
	T[] items;
	int currentItemCount;
	
	public Edge(int maxEdgeSize) {
		items = new T[maxEdgeSize];
	}
	
	public void Add(T item) {
		item.EdgeIndex = currentItemCount;
		items[currentItemCount] = item;
		SortUp(item);
		currentItemCount++;
	}

	public T RemoveFirst() {
		T firstItem = items[0];
		currentItemCount--;
		items[0] = items[currentItemCount];
		items[0].EdgeIndex = 0;
		SortDown(items[0]);
		return firstItem;
	}

	public void UpdateItem(T item) {
		SortUp(item);
	}

	public int Count {
		get {
			return currentItemCount;
		}
	}

	public bool Contains(T item) {
		return Equals(items[item.EdgeIndex], item);
	}

	void SortDown(T item) {
		while (true) {
			int childIndexLeft = item.EdgeIndex * 2 + 1;
			int childIndexRight = item.EdgeIndex * 2 + 2;
			int swapIndex = 0;

			if (childIndexLeft < currentItemCount) {
				swapIndex = childIndexLeft;

				if (childIndexRight < currentItemCount) {
					if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0) {
						swapIndex = childIndexRight;
					}
				}

				if (item.CompareTo(items[swapIndex]) < 0) {
					Swap (item,items[swapIndex]);
				}
				else {
					return;
				}

			}
			else {
				return;
			}

		}
	}
	
	void SortUp(T item) {
		int parentIndex = (item.EdgeIndex-1)/2;
		
		while (true) {
			T parentItem = items[parentIndex];
			if (item.CompareTo(parentItem) > 0) {
				Swap (item,parentItem);
			}
			else {
				break;
			}

			parentIndex = (item.EdgeIndex-1)/2;
		}
	}
	
	void Swap(T itemA, T itemB) {
		items[itemA.EdgeIndex] = itemB;
		items[itemB.EdgeIndex] = itemA;
		int itemAIndex = itemA.EdgeIndex;
		itemA.EdgeIndex = itemB.EdgeIndex;
		itemB.EdgeIndex = itemAIndex;
	}
	
	
	
}

public interface IEdgeItem<T> : IComparable<T> {
	int EdgeIndex {
		get;
		set;
	}
}
