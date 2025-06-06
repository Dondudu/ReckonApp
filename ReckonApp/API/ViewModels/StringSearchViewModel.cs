namespace ReckonApp.API.ViewModels
{
    public class StringSearchViewModel
    {
        public required string Text { get; set; }
        public required string Candidate { get; set; }
        public required List<Item> Results { get; set; }
    }

    public class Item
    {
        public required string Subtext { get; set; }
        public required string Result { get; set; }
    }
}
