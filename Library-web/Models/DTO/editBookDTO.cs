namespace Library_web.Models.DTO
{
    public class editBookDTO
    {
        public int Id { get; set; }
        public string title { get; set; }
        public string? Description { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public int Rate { get; set; }
        public string? Genre { get; set; }
        public string? CoverUrl { get; set; }
        public DateTime DateAdded { get; set; }
        public int PublisherID { get; set; }
        public List<string> AuthorIds { get; set; }
    }
}
