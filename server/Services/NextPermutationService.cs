using PermutationGeneratorAPI.Services.Interfaces;

namespace PermutationGeneratorAPI.Services
{
    public class NextPermutationService : INextPermutationService
    {
        public bool GetNextPermutation(int[] array)
        {
            int i = array.Length - 2;
            while (i >= 0 && array[i] >= array[i + 1])
            {
                i--;
            }
            
            if (i < 0)
            {
                return false;
            }
            
            int j = array.Length - 1;
            while (array[j] <= array[i])
            {
                j--;
            }
            
            Swap(array, i, j);
            
            Array.Reverse(array, i + 1, array.Length - i - 1);
            
            return true;
        }
        
        public int[] GenerateSequentialPermutations(int[] startPermutation, int count, int n)
        {
            int[] current = (int[])startPermutation.Clone();
            
            for (int i = 1; i < count; i++)
            {
                if (!GetNextPermutation(current))
                {
                    break;
                }
            }
            
            return current;
        }
        
        private static void Swap(int[] array, int i, int j)
        {
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }
}
