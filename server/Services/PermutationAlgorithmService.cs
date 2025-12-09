using PermutationGeneratorAPI.Services.Interfaces;

namespace PermutationGeneratorAPI.Services
{
    public class PermutationAlgorithmService : IPermutationAlgorithmService
    {
        private static readonly long[] FactorialCache = new long[21];

        static PermutationAlgorithmService()
        {
            FactorialCache[0] = 1;
            FactorialCache[1] = 1;
            for (int i = 2; i <= 20; i++)
            {
                FactorialCache[i] = FactorialCache[i - 1] * i;
            }
        }

        public int[] GetPermutationByIndex(int n, long index)
        {
            if (n < 1 || n > 20)
                throw new ArgumentException("n must be between 1 and 20");

            long totalPerms = CalculateFactorial(n);
            if (index < 0 || index >= totalPerms)
                throw new ArgumentException("index is out of range");

            int[] result = new int[n];
            FenwickTree tree = new FenwickTree(n);
            
            for (int i = 0; i < n; i++)
            {
                long fact = CalculateFactorial(n - 1 - i);
                int pos = (int)(index / fact);
                
                int selectedIdx = tree.FindKthAvailable(pos + 1);
                result[i] = selectedIdx;
                
                tree.Remove(selectedIdx);
                
                index = index % fact;
            }

            return result;
        }

        public long CalculateFactorial(int n)
        {
            if (n < 0 || n > 20) return 0;
            return FactorialCache[n];
        }
    }
}
