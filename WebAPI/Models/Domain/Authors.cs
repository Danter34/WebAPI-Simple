using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.Domain
{
    public class Authors
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        // Navigation properties - One author has many Book_author
        public List <Book_Authors> Book_Author { get; set; }
    }
}
