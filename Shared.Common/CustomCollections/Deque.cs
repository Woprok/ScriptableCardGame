using System;
using System.Collections;
using System.Collections.Generic;

namespace Shared.Common.CustomCollections
{
    public static class DequeTest
    {
        public static IList<T> GetReverseView<T>(Deque<T> d)
        {
            if (d == null)
                throw new ArgumentException("Deque is null.", nameof(d));
            return new ReverseDeque<T>(d);
        }
    }

    /// <summary>
    /// Implementation of IList interface for reversie view on Deque.
    /// Note: If you wish to use IDeque interface, you will need to provide implementation for it.
    /// </summary>

    [Serializable]
    public class ReverseDeque<T> : IList<T>
    {
        /// <summary>
        /// Returns Deque, which is used as base for this.
        /// </summary>
        public Deque<T> Deque { get; private set; }

        public ReverseDeque(Deque<T> deque)
        {
            Deque = deque;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
            if (array == null) throw new ArgumentNullException();
            if (arrayIndex + Count > array.Length) throw new ArgumentException();

            foreach (T item in this)
                array[arrayIndex++] = item;
        }

        public IEnumerator<T> GetEnumerator() => Deque.GetDirectionEnumerator(EnumerationDirection.ToLowerIndexes);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item) => Deque.Insert(0, item);

        public void Clear() => Deque.Clear();

        public bool Contains(T item) => Deque.Contains(item);


        public int Count { get => Deque.Count; }

        public bool IsReadOnly { get => Deque.IsReadOnly; }

        public void RemoveAt(int index) => Deque.RemoveAt(ReverseIndex(index));

        public T this[int index]
        {
            get => Deque[ReverseIndex(index)];
            set => Deque[ReverseIndex(index)] = value;
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index == -1) return false;
            RemoveAt(index);
            return true;
        }

        public int IndexOf(T item)
        {
            int index = 0;
            if ((Object)item == null)
            {
                foreach (T currentItem in this)
                {
                    if ((Object)currentItem == null)
                        return index;
                    index++;
                }
                return -1;
            }
            else
            {
                EqualityComparer<T> c = EqualityComparer<T>.Default;
                foreach (T currentItem in this)
                {
                    if (c.Equals(currentItem, item))
                        return index;
                    index++;
                }
                return -1;
            }
        } //.
        public void Insert(int index, T item) => Deque.Insert(ReverseIndex(index) + 1, item); // +1?

        /// <summary>
        /// Returns Count - index - 1, which is equal to index in original Deque
        /// </summary>
        /// <param name="index">is index in reverse deque</param>
        private int ReverseIndex(int index) => Count - index - 1;
    }

    // index is 0 ... currentN * q - 1
    // Map is F to B, where F,B == null and F+1 is first real, B-1 is last real
    //https://referencesource.microsoft.com/#System/compmod/system/collections/generic/queue.cs,6dba415a0e1792b0
    //https://referencesource.microsoft.com/#mscorlib/system/collections/generic/list.cs,cf7f4095e4de7646

    [Serializable]
    public partial class Deque<T> : IList<T>
    {
        private const int n = 8;
        private const int q = 8;
        private const int MultiplicationValue = 2;

        private int currentMultiplicator = 1;

        private int currentN = n;

        private int F = n * q / 2 - 1;
        private int B = n * q / 2;

        private bool ValidateBackIndexMove(int newB) => newB < currentN * q && newB != F;
        private bool ValidateFrontIndexMove(int newF) => newF >= 0 && newF != B;

        private T[][] Map = new T[n][] { new T[q], new T[q], new T[q], new T[q], new T[q], new T[q], new T[q], new T[q] };

        private int count = 0;
        /// <summary>	
        /// Gets the number of elements contained in the Deque.
        /// Operations that modify Count, will modify Version as well.
        /// </summary>
        public int Count
        {
            get => count;
            private set
            {
                count = value;
                Version++;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Deque is read-only.
        /// Default implementation Deque returns always false.
        /// </summary>
        public bool IsReadOnly { get; } = false;
    }

    /// <summary>
    /// Enumeration
    /// </summary>
    public partial class Deque<T>
    {
        public virtual IEnumerator<T> GetEnumerator() => GetDirectionEnumerator(EnumerationDirection.ToHigherIndexes);

        public IEnumerator<T> GetDirectionEnumerator(EnumerationDirection direction) => DequeYield(direction);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<T> DequeYield(EnumerationDirection direction)
        {
            int localVersionCopy = Version;

            if (direction > 0)
            {
                for (int i = 0; i < Count; i = i + (int)direction)
                {
                    if (localVersionCopy != Version) throw new InvalidOperationException();
                    yield return this[i];
                }
            }

            if (direction < 0)
            {
                for (int i = Count - 1; i >= 0; i = i + (int)direction)
                {
                    if (localVersionCopy != Version) throw new InvalidOperationException();
                    yield return this[i];
                }
            }

            if (direction == 0) throw new ArgumentException("I told you to never create 0 based EnumDirection...");
        }

        public sealed class DequeEnumerator<T> : IEnumerator<T>
        {
            private readonly Deque<T> enumeratedDeque;
            private readonly EnumerationDirection choosedDirection;
            private bool isEnumerating;

            private readonly int storedVersion;
            private bool CheckVersion => storedVersion != enumeratedDeque.Version ? throw new InvalidOperationException("Accessing an iterator to a deque that has been modified.") : true;

            private int LocalEnumeration { get; set; }

            public DequeEnumerator(Deque<T> deque, EnumerationDirection direction)
            {
                storedVersion = deque.Version;
                enumeratedDeque = deque;
                choosedDirection = direction;

                Reset();
            }

            public T Current
            {
                get
                {
                    if (!isEnumerating)
                        throw new InvalidOperationException("Accessing an iterator to a deque that was not enumerated yet.");
                    return enumeratedDeque[LocalEnumeration];
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if (!isEnumerating)
                        throw new InvalidOperationException("Accessing an iterator to a deque that was not enumerated yet.");
                    return (object)enumeratedDeque[LocalEnumeration];
                }
            }

            public void Dispose()
            {
                //enumeratedDeque = null;
            }

            public bool MoveNext()
            {
                if (!CheckVersion || (LocalEnumeration == enumeratedDeque.Count - 1 && choosedDirection > 0) || (LocalEnumeration == 0 && choosedDirection < 0))
                    return false;

                LocalEnumeration += (int)choosedDirection; //Don't forget that this actually moves index to first real item.
                isEnumerating = true;
                return true;
            }

            public void Reset()
            {
                isEnumerating = false;

                if (choosedDirection > 0) LocalEnumeration = -1;
                else if (choosedDirection < 0) LocalEnumeration = enumeratedDeque.Count;
                else throw new ArgumentException("I told you to never create 0 based EnumDirection...");
            }
        }
    }

    /// <summary>
    /// Generic interface for Deque
    /// </summary>
    public interface IDeque<T>
    {
        void PushFront(T item);
        void PushBack(T item);
        T PopFront();
        T PopBack();
        T PeekFront();
        T PeekBack();
        int Capacity { get; }
        /// <summary>
        /// This should provide option to release resources, if current Capacity is too big for current usage.
        /// </summary>
        void TrimExcess();
        /// <summary>
        /// Providing manual option to increase capacity.
        /// </summary>
        void ReAllocate();
        //These are here just for overview, and should not be used, like they are most of the time completly useless and ineffective...
        //int Version { get; }
        //void PushLinear(int index, T item);
        //T PopLinear(int index);
    }

    /// <summary>
    /// Please be sane person and never set it to 0, it makes no sense!
    /// </summary>
    public enum EnumerationDirection
    {
        ToHigherIndexes = 1,
        ToLowerIndexes = -1,
    }

    public partial class Deque<T> : IDeque<T>
    {
        /// <summary>
        /// Initialize internal blocks of mapArray, by default block size
        /// </summary>
        private void InitializeArray(T[][] mapArray)
        {
            for (var index = 0; index < mapArray.Length; index++)
            {
                mapArray[index] = new T[q];
            }
        }

        public void ReAllocate()
        {
            currentMultiplicator *= MultiplicationValue;
            int newCurrentN = n * currentMultiplicator;
            T[][] newMap = new T[newCurrentN][];
            InitializeArray(newMap);

            Capacity = newCurrentN * q;

            int updatedF = (Capacity - Count) / 2;

            for (int i = 0; i < Count; i++) //Viac sa mi pacilo ked som to kopiroval cez bloky, ale uz sa mi to nechce dalej debugovat
            {
                newMap[(updatedF + i) / n][(updatedF + i) % q] = this[i];
            }

            Map = newMap;
            F = updatedF - 1;
            B = updatedF + Count;
            currentN = newCurrentN;
        }

        /// <summary>
        /// ReAllocates Map into smaller one, if possible
        /// </summary>
        public void TrimExcess()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the total number of elements the internal data structure can hold without resizing.
        /// </summary>
        public int Capacity { get; private set; } = n * q;

        /// <summary>
        /// Current Version of deque, based on changes of items.
        /// </summary>
        public int Version { get; private set; } = 0;

        /// <summary>
        /// Removing item from inside of deque and correctly removing space, created in process.
        /// </summary>
        public T PopLinear(int index) //RemoveAt Index
        {
            T insider = this[index];

            if (F + index < B / 2)
            {
                for (int i = F + index + 1; i > F + 1; i--)
                {
                    Map[i / n][i % q] = Map[(i - 1) / n][(i - 1) % q];
                }

                F++;
                Map[F / n][F % q] = default(T);
            }
            else
            {
                for (int i = B - (Count - index); i < B - 1; i++)
                {
                    Map[i / n][i % q] = Map[(i + 1) / n][(i + 1) % q];
                }

                B--;
                Map[B / n][B % q] = default(T);
            }

            Count--;

            return insider;
        }

        /// <summary>
        /// Insert item at index inside of deque and correctly moving F,B indexers..
        /// </summary>
        public void PushLinear(int index, T item)
        {
            T nextItem = item;
            if (F + index < B / 2)
            {
                for (int i = F + index; i > F; i--)
                {
                    SwapByReference(ref Map[i / n][i % q], ref nextItem);
                }
                PushFront(nextItem); //Map[F / n][F % q] = nextItem; //F--;
            }
            else
            {
                for (int i = B - (Count - index); i < B; i++)
                {
                    SwapByReference(ref Map[i / n][i % q], ref nextItem);
                }
                PushBack(nextItem); //Map[B / n][B % q] = nextItem; //B++;
            }
            //Count++;
        }

        private void SwapByReference(ref T first, ref T second)
        {
            T temp = first;
            first = second;
            second = temp;
        }


        /// <summary>
        /// Removes the first occurrence of a specific object from the Deque.
        /// ----------------------------------------------------------------
        /// Internally finds item with help of IndexOf, then calls PopLinear
        /// </summary>
        /// <returns> true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the Deque.</returns>
        public bool Remove(T item)
        {
            var itemIndex = IndexOf(item);
            if (itemIndex == -1) return false;
            else
            {
                PopLinear(itemIndex);
                return true;
            }
        }

        /// <summary>
        /// Adds an object to the end of the Deque.
        /// Internally calls PushBack(item).
        /// </summary>
        public void Add(T item) => PushBack(item);

        /// <summary>
        /// Adds an object to the beginning of the Deque.
        /// </summary>
        public void PushFront(T item)
        {
            Map[F / n][F % q] = item;
            Count++;
            if (ValidateFrontIndexMove(F - 1))
                F--;
            else
            {
                F--;
                ReAllocate();
            }
        }

        /// <summary>
        /// Adds an object to the end of the Deque.
        /// </summary>
        public void PushBack(T item)
        {
            Map[B / n][B % q] = item;
            Count++;
            if (ValidateBackIndexMove(B + 1))
                B++;
            else
            {
                B++;
                ReAllocate();
            }
        }

        /// <summary>
        /// Removes and returns the object at the beginning of the Deque.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public T PopFront()
        {
            if (ValidateFrontIndexMove(F + 1))
            {
                F++;

                var T = Map[F / n][F % q];
                Map[F / n][F % q] = default(T);

                Count--;
                return T;
            }
            else throw new InvalidOperationException();
        }

        /// <summary>
        /// Removes and returns the object at the end of the Deque.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public T PopBack()
        {
            if (ValidateBackIndexMove(B - 1))
            {
                B--;

                var T = Map[B / n][B % q];
                Map[B / n][B % q] = default(T);

                Count--;
                return T;
            }
            else throw new InvalidOperationException();
        }

        /// <summary>
        /// Returns the object at the beginning of the Deque without removing it.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public T PeekFront()
        {
            if (ValidateFrontIndexMove(F + 1))
                return Map[(F + 1) / n][(F + 1) % q];
            else throw new InvalidOperationException();
        }

        /// <summary>
        /// Returns the object at the end of the Deque without removing it.
        /// </summary>
        /// <exception cref="InvalidOperationException"/>
        public T PeekBack()
        {
            if (ValidateBackIndexMove(B - 1))
                return Map[(B - 1) / n][(B - 1) % q];
            else throw new InvalidOperationException();
        }

        /// <summary>
        /// Removes all elements from the Deque.
        /// ----------------------------------------------------------------
        /// We don't want to be different, so this is not making map field smaller => we should implement TrimExcess() for that.
        /// </summary>
        public void Clear()
        {
            //if found bug rewrite it by: for (int i = 0; i < Count; i++), otherwise <F+1, B-1>
            for (int i = F + 1; i < B; i++)
            {
                Map[i / n][i % q] = default(T);
            }

            F = currentN * q / 2 - 1;
            B = currentN * q / 2;
            Count = 0;
        }

        /// <summary>
        /// Copies the entire Deque to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"> index out of interval (-1,Count)</exception>
        /// <exception cref="ArgumentNullException">array is null</exception>
        /// <exception cref="ArgumentException">The number of elements in the source Deque is greater than the available space from arrayIndex to the end of the destination array.</exception>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
            if (array == null) throw new ArgumentNullException();
            if (arrayIndex + Count > array.Length) throw new ArgumentException();

            foreach (T item in this)
                array[arrayIndex++] = item;
        }

        /// <summary>
        /// Determines whether an element is in the Deque.
        /// Uses EqualityComparer
        /// </summary>
        /// <returns>True if item is found in the Deque; otherwise, false.</returns>
        public bool Contains(T item)
        {
            if ((Object)item == null)
            {
                for (int index = 0; index < Count; index++)
                {
                    if ((Object)this[index] == null)
                        return true;
                }
                return false;
            }
            else
            {
                for (int index = 0; index < Count; index++)
                {
                    EqualityComparer<T> c = EqualityComparer<T>.Default;
                    if (c.Equals(this[index], item))
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire Deque.
        /// Uses EqualityComparer
        /// </summary> 
        /// <returns>The zero-based index of the first occurrence of item within the entire Deque, if found; otherwise, -1.</returns>
        public int IndexOf(T item)
        {
            if ((Object)item == null)
            {
                for (int index = 0; index < Count; index++)
                {
                    var fullIndex = F + 1 + index;
                    if ((Object)Map[fullIndex / n][fullIndex % q] == null)
                        return index;
                }
                return -1;
            }
            else
            {
                EqualityComparer<T> c = EqualityComparer<T>.Default;
                for (int index = 0; index < Count; index++)
                {
                    var fullIndex = F + 1 + index;
                    if (c.Equals(Map[fullIndex / n][fullIndex % q], item))
                        return index;
                }

                return -1;
            }
        }

        /// <summary>
        /// Inserts an element into the Deque at the specified index.
        /// ----------------------------------------------------------------
        /// Based on index internally calls PushFront(i == 0), PushBack(i == Count) or PushLinear.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public void Insert(int index, T item)
        {
            if (index == 0) PushFront(item);
            else if (index == Count) PushBack(item); //reconsider this
            else if (index < 0 || index > Count) throw new ArgumentOutOfRangeException("");
            else
            {
                if (Capacity == Count || B >= currentN * q - 1 || 0 >= F)
                {
                    ReAllocate(); //ToDo verify if this is really needed to check all 3
                }
                PushLinear(index, item);
            }
        }

        /// <summary>
        /// Removes the element at the specified index of the Deque.
        /// ----------------------------------------------------------------
        /// Based on index internally calls PopFront(i == 0), PopBack(i == Count - 1) or PopLinear.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public void RemoveAt(int index)
        {
            if (index == 0) PopFront();
            else if (index == Count - 1) PopBack();
            else if (index < 0 || index >= Count) throw new ArgumentOutOfRangeException();
            else PopLinear(index);
        }

        /// <summary>
        /// Sets or Gets the element at the given index.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"/>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException();
                return Map[(F + 1 + index) / n][(F + 1 + index) % q];
            }
            set
            {
                if (index < 0 || index >= Count)
                    throw new ArgumentOutOfRangeException();
                Map[(F + 1 + index) / n][(F + 1 + index) % q] = value;
                Version++;
            }
        }
    }
}