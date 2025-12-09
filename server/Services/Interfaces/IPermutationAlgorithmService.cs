namespace PermutationGeneratorAPI.Services.Interfaces
{
    public interface IPermutationAlgorithmService
    {
        int[] GetPermutationByIndex(int n, long index);
        long CalculateFactorial(int n);
    }
}
