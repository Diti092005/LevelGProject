namespace PermutationGeneratorAPI.Models
{
    public class PermutationSessionState
    {
        public int N { get; set; }
        public long CurrentIndex { get; set; }
        public int[]? CurrentPermutation { get; set; }
        public long TotalPermutations { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastAccessedAt { get; set; }

        public PermutationSessionState()
        {
            CreatedAt = DateTime.UtcNow;
            LastAccessedAt = DateTime.UtcNow;
            CurrentIndex = 0;
        }
    }
}
