namespace PermutationGeneratorAPI.Models.DTOs
{
    public class PermutationResponse
    {
        public int[] Permutation { get; set; }
        public long SequenceNumber { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
}
