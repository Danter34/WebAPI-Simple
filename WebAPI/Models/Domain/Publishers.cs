using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models.Domain
{
    public class Publishers
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        //Navigation properties - One author has many book
        public List<Books> Books { get; set; }
    }
}
