using System;
using System.Collections.Generic;

namespace B_Stromy
{
    class Tree
    {
        public readonly int T;
        public Node Root;
        public List<Tree> Children = new List<Tree>();

        public Tree(int tParameter, List<int> keys)
        {
            this.T = tParameter;
            this.Root = new Node(keys);
            if(keys.Count >= 2 * T - 1)
            {
                List<int> rootKeys = new List<int>();
                List<int> childrenKeys = new List<int>();

                for (int i = 0; i < 2 * T - 1; i++)
                    rootKeys.Add(keys[i]);
                for (int i = 2 * T - 1; i < keys.Count; i++)
                    childrenKeys.Add(keys[i]);

                rootKeys.Sort();
                this.Root = new Node(rootKeys);

                if (childrenKeys.Count != 0)
                {
                    List<List<int>> nextRootsKeys = new List<List<int>>();
                    for (int i = 0; i < rootKeys.Count + 1; i++)
                    {
                        nextRootsKeys.Add(new List<int>());
                    }

                    foreach (int childrenKey in childrenKeys)
                    {
                        int cKeyPosition = 0;
                        foreach (int rootKey in rootKeys)
                        {
                            if (childrenKey > rootKey)
                            {
                                cKeyPosition++;
                            }
                        }
                        nextRootsKeys[cKeyPosition].Add(childrenKey);
                    }

                    foreach (List<int> nextRoot in nextRootsKeys)
                    {
                        this.Children.Add(new Tree(T, nextRoot));
                    }
                }
            }
            
        }
    }
    class Node
    {
        public List<int> Keys = new List<int>();
        public int Level = 1;
        public int Index;

        public Node(List<int> keys)
        {
            this.Keys = keys;
        }
    }
    class Program
    {
        private static void Main()
        {
        }
    }
}
