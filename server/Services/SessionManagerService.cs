using PermutationGeneratorAPI.Services.Interfaces;
using PermutationGeneratorAPI.Models;

namespace PermutationGeneratorAPI.Services
{
    public class SessionManagerService : ISessionManagerService
    {
        private readonly Dictionary<string, PermutationSessionState> _sessions = new();
        private readonly object _lockObject = new object();
        private readonly IPermutationAlgorithmService _algorithm;
        private const int SESSION_TIMEOUT_MINUTES = 30;

        public SessionManagerService(IPermutationAlgorithmService algorithm)
        {
            _algorithm = algorithm;
        }

        public (string sessionId, PermutationSessionState session) CreateSession(int n)
        {
            lock (_lockObject)
            {
                CleanupExpiredSessions();
                string sessionId = Guid.NewGuid().ToString();
                
                var session = new PermutationSessionState
                {
                    N = n,
                    TotalPermutations = _algorithm.CalculateFactorial(n),
                    CurrentIndex = 0
                };
                
                _sessions[sessionId] = session;

                return (sessionId, session);
            }
        }

        public PermutationSessionState GetSession(string sessionId)
        {
            lock (_lockObject)
            {
                if (_sessions.TryGetValue(sessionId, out var session))
                {
                    session.LastAccessedAt = DateTime.UtcNow;
                    return session;
                }
                return null;
            }
        }

        public void UpdateSession(string sessionId, PermutationSessionState state)
        {
            lock (_lockObject)
            {
                if (_sessions.ContainsKey(sessionId))
                {
                    state.LastAccessedAt = DateTime.UtcNow;
                    _sessions[sessionId] = state;
                }
            }
        }

        public void ClearSession(string sessionId)
        {
            lock (_lockObject)
            {
                _sessions.Remove(sessionId);
            }
        }

        private void CleanupExpiredSessions()
        {
            var expiredSessions = _sessions
                .Where(kvp => DateTime.UtcNow.Subtract(kvp.Value.LastAccessedAt).TotalMinutes > SESSION_TIMEOUT_MINUTES)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var sessionId in expiredSessions)
            {
                _sessions.Remove(sessionId);
            }
        }
    }
}
