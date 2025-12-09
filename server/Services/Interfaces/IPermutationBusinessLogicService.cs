using PermutationGeneratorAPI.Models.DTOs;

namespace PermutationGeneratorAPI.Services.Interfaces
{
    public interface IPermutationBusinessLogicService
    {
        (string sessionId, long totalPermutations, int n) StartNewSession(int n);
        PermutationResponse GetNextPermutation(string sessionId);
        PaginatedPermutationResponse GetPermutationsPage(string sessionId, int pageSize, long startIndex = -1);
        void ResetSession(string sessionId);
    }
}
