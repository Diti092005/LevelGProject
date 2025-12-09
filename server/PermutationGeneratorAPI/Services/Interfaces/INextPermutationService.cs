namespace PermutationGeneratorAPI.Services.Interfaces
{
    public interface INextPermutationService
    {
        bool GetNextPermutation(int[] array);
        int[] GenerateSequentialPermutations(int[] startPermutation, int count, int n);
    }
}
