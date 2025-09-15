using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.Domain
{
    public class Books
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsRead { get; set; }
        public DateTime? DateRead { get; set; }
        public int? Rate { get; set; }
        public string Genre { get; set; }
        public string? CoverUrl { get; set; }
        public DateTime DateAdded { get; set; }
        //Navigation propperties - One Publisher has many book
        public int PublisherID { get; set; }
        public Publishers Publisher { get; set; }
        //navigation Propperties- One Book has many book_author
        public List<Book_Authors> Book_Author { get; set; }
    }
}
