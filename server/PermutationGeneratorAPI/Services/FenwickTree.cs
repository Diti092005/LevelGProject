namespace PermutationGeneratorAPI.Services
{
    internal class FenwickTree
    {
        private readonly int[] tree;
        private readonly int size;

        public FenwickTree(int n)
        {
            size = n;
            tree = new int[n + 1];
            for (int i = 1; i <= n; i++)
            {
                Update(i, 1);
            }
        }

        private void Update(int idx, int delta)
        {
            while (idx <= size)
            {
                tree[idx] += delta;
                idx += idx & -idx;
            }
        }

        public int FindKthAvailable(int k)
        {
            int pos = 0;
            int bitMask = 1;
            
            while (bitMask <= size) bitMask <<= 1;
            bitMask >>= 1;
            
            while (bitMask > 0)
            {
                int newPos = pos + bitMask;
                
                if (newPos <= size && tree[newPos] < k)
                {
                    pos = newPos;
                    k -= tree[newPos];
                }
                
                bitMask >>= 1;
            }
            
            return pos + 1;
        }

        public void Remove(int idx)
        {
            Update(idx, -1);
        }
    }
}
