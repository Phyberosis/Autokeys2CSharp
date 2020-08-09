using System;
using System.Collections.Generic;
using System.Text;

namespace Data
{
    public class Tree<T>
    {
        public List<T> Children = new List<T>();
        public string Name;

        public Tree(string name)
        {
            Name = name;
        }
    }
}
