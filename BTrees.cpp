#include<iostream>
using namespace std;

class Node
{
	friend class BTree;

	int* keys;
	int t;
	Node** children;
	int n;
	bool isLeaf;

public:

	Node(int tParam, bool leaf)
	{
		t = tParam;
		isLeaf = leaf;
		keys = new int[2 * t - 1];
		children = new Node * [2 * t];
		n = 0;
	}

	int findKey(int k)
	{
		int i = 0;
		while (i < n && keys[i] < k)
			i++;
		return i;
	}

	void splitChild(int i, Node* y)
	{
		Node* z = new Node(y->t, y->isLeaf);
		z->n = t - 1;
		for (int j = 0; j < t - 1; j++)
			z->keys[j] = y->keys[j + t];
		if (y->isLeaf == false)
		{
			for (int j = 0; j < t; j++)
				z->children[j] = y->children[j + t];
		}
		y->n = t - 1;
		for (int j = n; j >= i + 1; j--)
			children[j + 1] = children[j];
		children[i + 1] = z;
		for (int j = n - 1; j >= i; j--)
			keys[j + 1] = keys[j];
		keys[i] = y->keys[t - 1];
		n++;
	}

	int getPred(int index)
	{
		Node* current = children[index];
		while (!current->isLeaf)
			current = current->children[current->n];
		return current->keys[current->n - 1];
	}

	int getSucc(int index)
	{
		Node* current = children[index + 1];
		while (!current->isLeaf)
			current = current->children[0];
		return current->keys[0];
	}

	void fill(int index)
	{
		if (index != 0 && children[index - 1]->n >= t)
			borrowFromPrev(index);
		else if (index != n && children[index + 1]->n >= t)
			borrowFromNext(index);
		else
		{
			if (index != n)
				merge(index);
			else
				merge(index - 1);
		}
	}

	void borrowFromPrev(int index)
	{
		Node* child = children[index];
		Node* sibling = children[index - 1];
		for (int i = child->n - 1; i >= 0; i--)
			child->keys[i + 1] = child->keys[i];
		if (!child->isLeaf)
		{
			for (int i = child->n; i >= 0; i--)
				child->children[i + 1] = child->children[i];
		}
		child->keys[0] = keys[index - 1];
		if (!child->isLeaf)
			child->children[0] = sibling->children[sibling->n];
		keys[index - 1] = sibling->keys[sibling->n - 1];
		child->n++;
		sibling->n--;
	}

	void borrowFromNext(int index)
	{

		Node* child = children[index];
		Node* sibling = children[index + 1];
		child->keys[(child->n)] = keys[index];
		if (!(child->isLeaf))
			child->children[(child->n) + 1] = sibling->children[0];
		keys[index] = sibling->keys[0];
		for (int i = 1; i < sibling->n; ++i)
			sibling->keys[i - 1] = sibling->keys[i];
		if (!sibling->isLeaf)
		{
			for (int i = 1; i <= sibling->n; ++i)
				sibling->children[i - 1] = sibling->children[i];
		}
		child->n++;
		sibling->n--;
	}

	void merge(int index)
	{
		Node* child = children[index];
		Node* sibling = children[index + 1];
		child->keys[t - 1] = keys[index];
		for (int i = 0; i < sibling->n; i++)
			child->keys[i + t] = sibling->keys[i];
		if (!child->isLeaf)
		{
			for (int i = 0; i <= sibling->n; i++)
				child->children[i + t] = sibling->children[i];
		}
		for (int i = index + 1; i < n; i++)
			keys[i - 1] = keys[i];
		for (int i = index + 2; i <= n; i++)
			children[i - 1] = children[i];
		child->n += sibling->n + 1;
		n--;
		delete(sibling);
	}

	void listKeys()
	{
		int i;
		for (i = 0; i < n; i++)
		{
			if (!isLeaf)
				children[i]->listKeys();
			cout << " " << keys[i];
		}
		if (!isLeaf)
			children[i]->listKeys();
	}

	Node* search(int k)
	{
		int i = 0;
		while (i < n && k > keys[i])
			i++;
		if (keys[i] == k)
			return this;
		if (isLeaf)
			return NULL;
		return children[i]->search(k);
	}

	void insertNotFull(int k)
	{
		int i = n - 1;
		if (isLeaf)
		{
			while (i >= 0 && keys[i] > k)
			{
				keys[i + 1] = keys[i];
				i--;
			}
			keys[i + 1] = k;
			n = n + 1;
		}
		else
		{
			while (i >= 0 && keys[i] > k)
				i--;
			if (children[i + 1]->n == 2 * t - 1)
			{
				splitChild(i + 1, children[i + 1]);
				if (keys[i + 1] < k)
					i++;
			}
			children[i + 1]->insertNotFull(k);
		}
	}

	void delFromLeaf(int index)
	{
		for (int i = index + 1; i < n; i++)
			keys[i - 1] = keys[i];
		n--;
	}

	void delFromNotLeaf(int index)
	{
		int k = keys[index];
		if (children[index]->n >= t)
		{
			int pred = getPred(index);
			keys[index] = pred;
			children[index]->del(pred);
		}
		else if (children[index + 1]->n >= t)
		{
			int succ = getSucc(index);
			keys[index] = succ;
			children[index + 1]->del(succ);
		}
		else
		{
			merge(index);
			children[index]->del(k);
		}
	}

	void del(int k)
	{
		int index = findKey(k);
		if (index < n && keys[index] == k)
		{
			if (isLeaf)
				delFromLeaf(index);
			else
				delFromNotLeaf(index);
		}
		else
		{
			if (isLeaf)
			{
				cout << "Klic " << k << " se ve stromu nenachazi.\n";
				return;
			}
			bool sign = index == n;
			if (children[index]->n < t)
				fill(index);
			if (sign && index > n)
				children[index - 1]->del(k);
			else
				children[index]->del(k);
		}
	}
};

class BTree
{
	Node* root;
	int t;

public:

	BTree(int tParam)
	{
		root = NULL;
		t = tParam;
	}

	void listKeys()
	{
		if (root != NULL)
			root->listKeys();
	}

	Node* search(int k)
	{
		if (root == NULL)
			return NULL;
		return root->search(k);
	}

	void insert(int k)
	{
		if (root == NULL)
		{
			root = new Node(t, true);
			root->keys[0] = k;
			root->n = 1;
		}
		else
		{
			if (root->n == 2 * t - 1)
			{
				Node* s = new Node(t, false);
				s->children[0] = root;
				s->splitChild(0, root);
				int i = 0;
				if (s->keys[0] < k)
					i++;
				s->children[i]->insertNotFull(k);
				root = s;
			}
			else
				root->insertNotFull(k);
		}
	}

	void del(int k)
	{
		if (!root)
		{
			cout << "Strom je prázdný.\n";
			return;
		}
		root->del(k);
		if (root->n == 0)
		{
			if (root->isLeaf)
				root = NULL;
			else
				root = root->children[0];
		}
	}
};

// Funkce pro testování
int main()
{
	//Vytvoření stromu
	BTree t(2);

	//Vložení hodnot do stromu
	t.insert(26);
	t.insert(6);
	t.insert(12);
	t.insert(42);
	t.insert(51);
	t.insert(62);
	t.insert(1);
	t.insert(2);
	t.insert(4);
	t.insert(7);
	t.insert(8);
	t.insert(13);
	t.insert(15);
	t.insert(18);
	t.insert(25);
	t.insert(27);
	t.insert(29);
	t.insert(45);
	t.insert(48);
	t.insert(48);
	t.insert(53);
	t.insert(55);
	t.insert(64);
	t.insert(70);
	t.insert(90);

	//Vypsání všech klíčů před odstraněním
	cout << "Pred odstranenim klicu:";
	t.listKeys();
	cout << endl;

	//Odstranění klíče ze stromu
	t.del(91);
	t.del(45);

	//Vypsání všech klíčů po odstranění
	cout << "Po odstraneni klicu:";
	t.listKeys();
	cout << endl;

	getchar();
	return 0;
}
