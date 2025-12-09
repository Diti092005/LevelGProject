namespace PermutationGeneratorAPI.Models
{
    public class PermutationSettings
    {
        public int MinN { get; set; } = 1;
        public int MaxN { get; set; } = 20;
        public int MaxPageSize { get; set; } = 1000;
        public int DefaultPageSize { get; set; } = 100;
    }
}
