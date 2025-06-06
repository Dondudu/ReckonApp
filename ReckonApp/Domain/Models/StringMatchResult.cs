namespace ReckonApp.Domain.Models
{
    public class StringMatchResult
    {
        public required Dictionary<string, List<int>> SubStringIndices { get; set; }
    }
}
