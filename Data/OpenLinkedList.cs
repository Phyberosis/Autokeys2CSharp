using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class OpenLinkedListNode<T>
    {
        public OpenLinkedListNode<T> Next = null;
        public OpenLinkedListNode<T> Previous = null;

        public T Value { get; set; }

        public OpenLinkedListNode(T value)
        {
            Value = value;
        }
    }

    public class OpenLinkedListEnumerator<T> : IEnumerator<T>
    {
        private OpenLinkedListNode<T> current;

        public OpenLinkedListEnumerator(OpenLinkedListNode<T> current)
        {
            this.current = current;
        }

        public T Current => current.Value;

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            if (current == null) return false;
            current = current.Next;
            return (current != null);
        }

        public void Dispose()
        {
            
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }


    public class OpenLinkedList<T> : IEnumerable<T>
    {
        private OpenLinkedListNode<T> first;
        private OpenLinkedListNode<T> last;
        private int count = 0;

        public OpenLinkedListNode<T> First() { return first; }
        public OpenLinkedListNode<T> Last() { return last; }
        public int Count() { return count; }
        public OpenLinkedList()
        {
            first = null; first = null;
        }

        public void AddLast(T val)
        {
            count++;
            var v = new OpenLinkedListNode<T>(val);
            if (first == null)
            {
                first = v;
                last = first;
            }
            else
            {
                last.Next = v;
                v.Previous = last;
                last = v;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new OpenLinkedListEnumerator<T>(first);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
