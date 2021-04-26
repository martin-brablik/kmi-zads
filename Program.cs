using System;
using System.Collections.Generic;

namespace B_Stromy
{
    class Btree
    {
        public Node Root; // Pointer to root node
        public int T; // Minimum degree

        // Constructor (Initializes tree as empty)
        public Btree(int t)
        {
            this.Root = null;
            this.T = t;
        }

        // function to search a key in this tree
        public Node Search(int k)
        {
            if (this.Root == null)
                return null;
            else
                return this.Root.Search(k);
        }

        public void Insert(int k)
        {
            if (Root == null)
            {
                Root = new Node(T, true);
                Root.Keys[0] = k;
                Root.N = 1;
            }
            else
            {
                if(Root.N == 2 * T - 1)
                {
                    var s = new Node(T, false);
                    s.Children[0] = Root;
                    s.SplitChild(0, Root);
                    var i = 0;
                    if(s.Keys[0] < k)
                    {
                        i++;
                    }
                    if (s.Children[i] == null)
                    {
                        s.Children[i] = new Node(T, true);
                    }
                    s.Children[i].InsertNotFull(k);
                    Root = s;
                }
                else
                {
                    Root.InsertNotFull(k);
                }
            }
        }
        public void Remove(int k)
        {
            if(Root == null || Root.N == 0 || Root.Keys.Length == 0)
            {
                Console.WriteLine("Tree is Empty");
                return;
            }
            Root.Remove(k);
            if(Root.N == 0)
            {
                if(Root.Leaf)
                {
                    Root = new Node(T, true);
                }
                else
                {
                    Root = Root.Children[0];
                }
            }
        }
        public void traverse()
        {
            if (Root.N != null)
                Root.traverse();
            Console.WriteLine();
        }
    }
    class Node
    {
        public int[] Keys; // An array of keys
        public int T; // Minimum degree (defines the range for number of keys)
        public Node[] Children; // An array of child pointers
        public int N; // Current number of keys
        public bool Leaf; // Is true when node is leaf. Otherwise false

        // Constructor
        public Node(int t, bool leaf)
        {
            this.T = t;
            this.Leaf = leaf;
            this.Keys = new int[2 * t - 1];
            this.Children = new Node[2 * t];
            this.N = 0;
        }

        // A function to search a key in the subtree rooted with this node.
        public Node Search(int k)
        { // returns NULL if k is not present.

            // Find the first key greater than or equal to k
            var i = 0;
            while (i < N && k > Keys[i])
            {
                i++;
            }

            // If the found key is equal to k, return this node
            if (Keys[i] == k)
            {
                return this;
            }

            // If the key is not found here and this is a leaf node
            if (Leaf == true)
            {
                return null;
            }

            // Go to the appropriate child
            return Children[i].Search(k);
        }

        public void InsertNotFull(int k)
        {
            var i = N - 1;
            if (Leaf)
            {
                while (i >= 0 && Keys[i] > k)
                {
                    Keys[i + 1] = Keys[i];
                    i--;
                }
                Keys[i + 1] = k;
                N++;
            }
            else
            {
                while (i >= 0 && Keys[i] > k)
                {
                    i--;
                }
                if (Children[i + 1] == null)
                {
                    Children[i + 1] = new Node(T, false);
                }
                if (Children[i + 1].N == 2 * T - 1)
                {
                    SplitChild(i + 1, Children[i + 1]);
                    if (Keys[i + 1] < k)
                    {
                        i++;
                    }
                }
                Children[i + 1].InsertNotFull(k);
            }
        }
        public void SplitChild(int i, Node y)
        {
            Node z = new Node(y.T, y.Leaf);
            z.N = T - 1;
            for (int j = 0; j < T - 1; j++)
            {
                z.Keys[j] = y.Keys[j + 1];
            }
            if (!y.Leaf)
            {
                for (int j = 0; j < T; j++)
                {
                    z.Children[j] = y.Children[j + 1];
                }
            }
            y.N = T - 1;
            for (int j = N - 1; j >= i; j--)
            {
                Keys[j + 1] = Keys[j];
            }
            Keys[i] = y.Keys[T - 1];
            N++;
        }
        private int FindKey(int k)
        {
            var i = 0;
            while (i < N && Keys[i] < k)
            {
                ++i;
            }
            return i;
        }
        private int GetPred(int i)
        {
            var current = Children[i];
            while (!current.Leaf)
            {
                current = current.Children[current.N];
            }
            return current.Keys[current.N - 1];
        }
        private void BorrowFromPrev(int i)
        {
            var child = Children[i];
            var leftSibling = Children[i - 1];
            for (int j = child.N - 1; j >= 0; --j)
            {
                child.Keys[j + 1] = child.Keys[j];
            }
            if (!child.Leaf)
            {
                for (int j = child.N; j >= 0; --j)
                {
                    child.Children[j + 1] = child.Children[j];
                }
            }
            child.Keys[0] = Keys[i - 1];
            if (!child.Leaf)
            {
                child.Children[0] = leftSibling.Children[leftSibling.N - 1];
            }
            Keys[i - 1] = leftSibling.Keys[leftSibling.N - 1];
            child.N++;
            leftSibling.N--;
        }
        private void BorrowFromNext(int i)
        {
            var child = Children[i];
            var rightSibling = Children[i + 1];
            child.Keys[child.N] = Keys[i];
            if (!child.Leaf)
            {
                child.Children[child.N + 1] = rightSibling.Children[0];
            }
            Keys[i] = rightSibling.Keys[0];
            for (int j = 1; j < rightSibling.N; ++j)
            {
                rightSibling.Keys[j - 1] = rightSibling.Keys[j];
            }
            if (!rightSibling.Leaf)
            {
                for (int j = 1; j <= rightSibling.N; ++j)
                {
                    rightSibling.Children[j - 1] = rightSibling.Children[j];
                }
            }
            child.N++;
            rightSibling.N--;
        }
        private void Merge(int i)
        {
            var child = Children[i];
            var rightSibling = Children[i + 1];
            child.Keys[T - 1] = Keys[i];
            for (int j = 0; j < rightSibling.N; ++j)
            {
                child.Keys[j + T] = rightSibling.Keys[i];
            }
            if (!child.Leaf)
            {
                for (int j = 0; j <= rightSibling.N; ++j)
                {
                    child.Children[j + T] = rightSibling.Children[j];
                }
            }
            for (int j = i + 1; j < N; ++j)
            {
                Keys[j - 1] = Keys[j];
            }
            for (int j = i + 2; j <= N; ++j)
            {
                Children[j - 1] = Children[j];
            }
            child.N += rightSibling.N + 1;
            N--;
        }
        private int GetSucc(int i)
        {
            var current = Children[i + 1];
            while (!current.Leaf)
            {
                current = current.Children[0];
            }
            return current.Keys[0];
        }
        private void Fill(int i)
        {
            if (i != 0 && Children[i - 1].N >= T)
            {
                BorrowFromPrev(i);
            }
            else if (i != N && Children[i + 1].N >= T)
            {
                BorrowFromNext(i);
            }
            else
            {
                if (i != N)
                {
                    Merge(i);
                }
                else
                {
                    Merge(i - 1);
                }
            }
        }
        public void Remove(int k)
        {
            var i = FindKey(k);
            if (i < N && Keys[i] == k)
            {
                if (Leaf)
                {
                    RemoveFromLeaf(i);
                }
                else
                {
                    RemoveFromNormal(i);
                }
            }
            else
            {
                if (Leaf)
                {
                    Console.WriteLine("Key does not exist");
                    return;
                }
                var flag = i == N ? true : false;
                if (Children[i].N < T)
                {
                    Fill(i);
                }
                if (flag && i > N)
                {
                    Children[i - 1].Remove(k);
                }
                else
                {
                    Children[i].Remove(k);
                }
            }
        }
        private void RemoveFromLeaf(int i)
        {
            for (int j = i + 1; j < i; ++j)
            {
                Keys[j - 1] = Keys[j];
            }
            N--;
        }
        private void RemoveFromNormal(int i)
        {
            var k = Keys[i];
            if (Children[i].N >= T)
            {
                var pred = GetPred(i);
                Keys[i] = pred;
                Children[i].Remove(pred);
            }
            else if (Children[i + 1].N >= T)
            {
                var succ = GetSucc(i);
                Keys[i] = succ;
                Children[i + 1].Remove(succ);
            }
            else
            {
                Merge(i);
                Children[i].Remove(k);
            }
        }
        public void traverse()
        {
            int i;
            for(i = 0; i < N; i++)
            {
                if(!Leaf)
                {
                    Children[i].traverse();
                }
                Console.Write(" " + Keys[i].ToString());
            }
            if(!Leaf)
            {
                Children[i].traverse();
            }
        }
    }

    class Program
    {
        private static void Main()
        {
            var t = new Btree(2);
            t.Insert(10);
            t.Insert(20);
            t.Insert(5);
            t.Insert(6);
            t.Insert(12);
            t.Insert(30);
            t.Insert(7);
            t.Insert(17);

            Console.WriteLine("Trvaersal:");
            t.traverse();

            var k = 20;
            if (t.Search(k) == null)
                Console.WriteLine("Not Present");
            else
                Console.WriteLine("Present");
        }
    }
}
