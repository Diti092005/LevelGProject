namespace PermutationGeneratorAPI.Models.DTOs
{
    public class PaginatedPermutationResponse
    {
        public List<int[]> Permutations { get; set; }
        public long StartSequenceNumber { get; set; }
        public int PageSize { get; set; }
        public long TotalPermutations { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
