namespace PermutationGeneratorAPI.Services.Interfaces
{
    public interface ISessionManagerService
    {
        (string sessionId, Models.PermutationSessionState session) CreateSession(int n);
        Models.PermutationSessionState GetSession(string sessionId);
        void UpdateSession(string sessionId, Models.PermutationSessionState state);
        void ClearSession(string sessionId);
    }
}
