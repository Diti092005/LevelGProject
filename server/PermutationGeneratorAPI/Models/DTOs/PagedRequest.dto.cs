namespace PermutationGeneratorAPI.Models.DTOs
{
    public class PagedRequest
    {
        public string SessionId { get; set; }
        public int PageSize { get; set; }
        public long StartIndex { get; set; } = 0;
    }
}
