using FormBuilder.Pages;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.Persiste;

public class FormBuilderDbContext(DbContextOptions<FormBuilderDbContext> builder) : DbContext(builder)
{
    public DbSet<FormPage> FormPages { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new FormPageConfiguration());
    }
}