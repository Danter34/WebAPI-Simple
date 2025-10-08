using Microsoft.EntityFrameworkCore;
using WebAPI.Models.Domain;

namespace WebAPI.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions): base(dbContextOptions) 
        {

        }
        // Define C# model
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // You can define the relationships between tables using the Fluent API
            modelBuilder.Entity<Book_Authors>()
                .HasOne(b => b.Book)
                .WithMany(ba => ba.Book_Author)
                .HasForeignKey(bi => bi.BookId);

            modelBuilder.Entity<Book_Authors>()
                .HasOne(b => b.Author)
                .WithMany(ba => ba.Book_Author)
                .HasForeignKey(bi => bi.AuthorId);
        }

        public DbSet<Books> Books { get; set; }
        public DbSet<Authors> Authors { get; set; }
        public DbSet<Book_Authors> Books_Authors { get; set; }
        public DbSet<Publishers> Publishers { get; set; }
        public DbSet<Image> Images { get; set; }
    }
}
