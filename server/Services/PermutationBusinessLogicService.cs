using PermutationGeneratorAPI.Models.DTOs;
using PermutationGeneratorAPI.Services.Interfaces;
using Microsoft.Extensions.Logging;
using PermutationGeneratorAPI.Models;

namespace PermutationGeneratorAPI.Services
{
    public class PermutationBusinessLogicService : IPermutationBusinessLogicService
    {
        private readonly IPermutationAlgorithmService _algorithm;
        private readonly ISessionManagerService _sessionManager;
        private readonly INextPermutationService _nextPermutation;
        private readonly ILogger<PermutationBusinessLogicService> _logger;

        public PermutationBusinessLogicService(
            IPermutationAlgorithmService algorithm,
            ISessionManagerService sessionManager,
            INextPermutationService nextPermutation,
            ILogger<PermutationBusinessLogicService> logger)
        {
            _algorithm = algorithm;
            _sessionManager = sessionManager;
            _nextPermutation = nextPermutation;
            _logger = logger;
        }

        public (string sessionId, long totalPermutations, int n) StartNewSession(int n)
        {
            if (n < 1 || n > 20)
                throw new ArgumentException("N must be between 1 and 20");

            var (sessionId, session) = _sessionManager.CreateSession(n);

            _logger.LogInformation($"Created session {sessionId} with N={n}, Total={session.TotalPermutations}");

            return (sessionId, session.TotalPermutations, n);
        }

        public PermutationResponse GetNextPermutation(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
                throw new ArgumentException("SessionId is required");

            var session = _sessionManager.GetSession(sessionId);
            if (session == null)
                throw new InvalidOperationException("Session not found");

            if (session.CurrentIndex >= session.TotalPermutations)
            {
                return new PermutationResponse
                {
                    Permutation = null,
                    SequenceNumber = session.CurrentIndex,
                    Success = false,
                    Message = "אין יותר קומבינציות. הגעת לסוף."
                };
            }

            int[] currentPerm;
            
            if (session.CurrentPermutation == null)
            {
                currentPerm = _algorithm.GetPermutationByIndex(session.N, session.CurrentIndex);
            }
            else
            {
                currentPerm = (int[])session.CurrentPermutation.Clone();
                if (!_nextPermutation.GetNextPermutation(currentPerm))
                {
                    return new PermutationResponse
                    {
                        Permutation = null,
                        SequenceNumber = session.CurrentIndex,
                        Success = false,
                        Message = "אין יותר קומבינציות. הגעת לסוף."
                    };
                }
            }

            var response = new PermutationResponse
            {
                Permutation = currentPerm,
                SequenceNumber = session.CurrentIndex + 1, 
                Success = true,
                Message = $"קומבינציה {session.CurrentIndex + 1} מתוך {session.TotalPermutations}"
            };

            session.CurrentIndex++;
            session.CurrentPermutation = currentPerm;
            _sessionManager.UpdateSession(sessionId, session);

            return response;
        }

        public PaginatedPermutationResponse GetPermutationsPage(string sessionId, int pageSize, long startIndex = -1)
        {
            if (string.IsNullOrEmpty(sessionId))
                throw new ArgumentException("SessionId is required");

            if (pageSize < 1 || pageSize > 1000)
                throw new ArgumentException("PageSize must be between 1 and 1000");

            var session = _sessionManager.GetSession(sessionId);
            if (session == null)
                throw new InvalidOperationException("Session not found");

            long actualStartIndex = startIndex >= 0 ? startIndex : session.CurrentIndex;
            
            if (actualStartIndex < 0 || actualStartIndex >= session.TotalPermutations)
                throw new ArgumentException($"StartIndex must be between 0 and {session.TotalPermutations - 1}");

            List<int[]> permutations = new List<int[]>();
            
            bool useNextPermutation = session.CurrentPermutation != null && 
                                      actualStartIndex == session.CurrentIndex;

            if (useNextPermutation)
            {
                int[] current = (int[])session.CurrentPermutation!.Clone();
                
                for (int i = 0; i < pageSize && actualStartIndex + i < session.TotalPermutations; i++)
                {
                    permutations.Add((int[])current.Clone());
                    
                    if (i < pageSize - 1 && actualStartIndex + i + 1 < session.TotalPermutations)
                    {
                        if (!_nextPermutation.GetNextPermutation(current))
                            break;
                    }
                }
                
                session.CurrentIndex = actualStartIndex + permutations.Count;
                session.CurrentPermutation = permutations.Count > 0 ? permutations[^1] : session.CurrentPermutation;
            }
            else
            {
                for (long i = 0; i < pageSize && actualStartIndex + i < session.TotalPermutations; i++)
                {
                    try
                    {
                        int[] perm = _algorithm.GetPermutationByIndex(session.N, actualStartIndex + i);
                        permutations.Add(perm);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Failed to generate permutation at index {actualStartIndex + i}: {ex.Message}");
                        continue;
                    }
                }
                
                session.CurrentIndex = actualStartIndex + permutations.Count;
                session.CurrentPermutation = permutations.Count > 0 ? permutations[^1] : null;
            }
            
            _sessionManager.UpdateSession(sessionId, session);

            return new PaginatedPermutationResponse
            {
                Permutations = permutations,
                StartSequenceNumber = actualStartIndex, 
                PageSize = permutations.Count,
                TotalPermutations = session.TotalPermutations,
                Success = true,
                Message = $"הוחזרו {permutations.Count} קומבינציות החל ממספר {actualStartIndex}"
            };
        }

        public void ResetSession(string sessionId)
        {
            if (string.IsNullOrEmpty(sessionId))
                throw new ArgumentException("SessionId is required");

            _sessionManager.ClearSession(sessionId);
            _logger.LogInformation($"Cleared session {sessionId}");
        }
    }
}
