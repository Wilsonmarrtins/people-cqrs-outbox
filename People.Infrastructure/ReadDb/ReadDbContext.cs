using Microsoft.EntityFrameworkCore;
using People.Infrastructure.ReadDb.Models;

namespace People.Infrastructure.ReadDb;

public sealed class ReadDbContext : DbContext
{
    public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options) { }

    public DbSet<PersonReadModel> People => Set<PersonReadModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PersonReadModel>(e =>
        {
            e.ToTable("people_read");

            e.HasKey(x => x.Id);

            e.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType("char(36)");

            e.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(200)
                .IsRequired();

            e.Property(x => x.Age)
                .HasColumnName("age")
                .IsRequired();

            e.Property(x => x.Sex)
                .HasColumnName("sex")
                .HasMaxLength(20)
                .IsRequired();

            e.Property(x => x.Rg)
                .HasColumnName("rg")
                .HasMaxLength(30)
                .IsRequired();

            e.Property(x => x.Cpf)
                .HasColumnName("cpf")
                .HasMaxLength(14)
                .IsRequired();

            e.Property(x => x.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            e.HasIndex(x => x.Rg).IsUnique();
            e.HasIndex(x => x.Cpf).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
