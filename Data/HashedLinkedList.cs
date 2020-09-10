using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml.XPath;

namespace Data
{
    public class HashedLinkedList<T> : IEnumerable<T>
    {
        private LinkedList<T> list;
        private Dictionary<T, LinkedListNode<T>> hash;

        public int Count
        {
            get { return list.Count; }
        }

        public LinkedListNode<T> First
        {
            get { return list.First; }
        }
        public LinkedListNode<T> Last
        {
            get { return list.Last; }
        }

        public HashedLinkedList()
        {
            list = new LinkedList<T>();
            hash = new Dictionary<T, LinkedListNode<T>>();
        }

        public void AddFirst(T val)
        {
            var n = list.AddFirst(val);
            hash.Add(val, n);
        }

        public void AddLast(T val)
        {
            var n = list.AddLast(val);
            hash.Add(val, n);
        }

        public void AddAfter(LinkedListNode<T> after, T val)
        {
            var n = list.AddAfter(after, val);
            hash.Add(val, n);
        }

        public void AddAfter(T after, T val)
        {
            var n = hash[after];
            var newN = list.AddAfter(n, val);
            hash.Add(val, newN);
        }

        public LinkedListNode<T> GetNode(T val)
        {
            return hash[val];
        }

        public void Remove(T val)
        {
            var n = hash[val];
            list.Remove(n);
            hash.Remove(val);
        }

        public void Remove(LinkedListNode<T> n)
        {
            hash.Remove(n.Value);
            list.Remove(n);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new HashedLinkedListEnumerator<T>(First);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class HashedLinkedListEnumerator<T> : IEnumerator<T>
    {
        private LinkedListNode<T> curr, first;
        private bool pre = true;

        public T Current
        {
            get
            {
                return curr.Value;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public HashedLinkedListEnumerator(LinkedListNode<T> first)
        {
            curr = first;
            this.first = first;
        }

        #region dispose
        private bool disposedValue = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(!this.disposedValue)
            {
                if(disposing)
                {
                    curr = null;
                    first = null;
                }
            }

            disposedValue = true;
        }

        ~HashedLinkedListEnumerator()
        {
            Dispose(false);
        }
        #endregion

        public bool MoveNext()
        {
            if (pre)
            {
                curr = first;
                pre = false;
            }else
            {
                curr = curr.Next;
            }

            return curr != null;
        }

        public void Reset()
        {
            pre = true;
        }
    }
}
