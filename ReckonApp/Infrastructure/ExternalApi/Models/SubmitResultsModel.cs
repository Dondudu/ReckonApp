namespace ReckonApp.Infrastructure.ExternalApi.Models
{
    public class SubmitResultsModel
    {
        public required string Candidate { get; set; }
        public required string Text { get; set; }
        public required List<SubmitResultsItem> Results { get; set; }
    }

    public class SubmitResultsItem
    {
        public string Subtext { get; set; }
        public string Result { get; set; }

        public SubmitResultsItem(string subtext, string result)
        {
            Subtext = subtext;
            Result = result;
        }
    }
}
