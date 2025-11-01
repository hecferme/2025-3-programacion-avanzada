using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProgramacionAvanzada.Books.Model;

public partial class BooksDbContext : DbContext
{
    public BooksDbContext(DbContextOptions<BooksDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<BookCopy> BookCopies { get; set; }

    public virtual DbSet<Borrow> Borrows { get; set; }

    public virtual DbSet<Person> Persons { get; set; }

    public virtual DbSet<Theme> Themes { get; set; }

    public virtual DbSet<BookAuthor> BookAuthors { get; set; }

    public virtual DbSet<BookTheme> BookThemes { get; set; }

    // demo/system table `xyz` excluded from the model

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Edition).HasMaxLength(100);
            entity.Property(e => e.EnglishTitle).HasMaxLength(255);
            entity.Property(e => e.OriginalTitle).HasMaxLength(255);

            entity.HasMany(d => d.Authors).WithMany(p => p.Books)
                .UsingEntity<BookAuthor>(
                    j => j.HasOne(ba => ba.Author).WithMany().HasForeignKey(ba => ba.AuthorId),
                    j => j.HasOne(ba => ba.Book).WithMany().HasForeignKey(ba => ba.BookId)
                );

            entity.HasMany(d => d.Themes).WithMany(p => p.Books)
                .UsingEntity<BookTheme>(
                    j => j.HasOne(bt => bt.Theme).WithMany().HasForeignKey(bt => bt.ThemeId),
                    j => j.HasOne(bt => bt.Book).WithMany().HasForeignKey(bt => bt.BookId)
                );
        });

    modelBuilder.Entity<BookCopy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.BookId, "BookId");

            entity.Property(e => e.ISBN).HasMaxLength(20);
            entity.Property(e => e.IsLost).HasDefaultValueSql("'0'");
            entity.Property(e => e.Language).HasMaxLength(50);
            entity.Property(e => e.Provider).HasMaxLength(100);
            entity.Property(e => e.Serial).HasMaxLength(100);

            entity.HasOne(d => d.Book).WithMany(p => p.BookCopies)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookcopies_ibfk_1");
        });

    modelBuilder.Entity<Borrow>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.BookCopyId, "BookCopyId");

            entity.HasIndex(e => e.PersonId, "PersonId");

            entity.HasOne(d => d.BookCopy).WithMany(p => p.Borrows)
                .HasForeignKey(d => d.BookCopyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("borrows_ibfk_1");

            entity.HasOne(d => d.Person).WithMany(p => p.Borrows)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("borrows_ibfk_2");
        });

    modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Sex)
                .HasMaxLength(1)
                .IsFixedLength();
        });

        modelBuilder.Entity<Theme>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Subject).HasMaxLength(255);
            entity.Property(e => e.ThemeName).HasMaxLength(255);
        });

        modelBuilder.Entity<BookAuthor>(entity =>
        {
            entity.HasKey(e => new { e.BookId, e.AuthorId }).HasName("PRIMARY");
            entity.ToTable("bookauthors");

            entity.HasOne(d => d.Book)
                .WithMany()
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookauthors_ibfk_1");

            entity.HasOne(d => d.Author)
                .WithMany()
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookauthors_ibfk_2");
        });

        modelBuilder.Entity<BookTheme>(entity =>
        {
            entity.HasKey(e => new { e.BookId, e.ThemeId }).HasName("PRIMARY");
            entity.ToTable("bookthemes");

            entity.HasOne(d => d.Book)
                .WithMany()
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookthemes_ibfk_1");

            entity.HasOne(d => d.Theme)
                .WithMany()
                .HasForeignKey(d => d.ThemeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookthemes_ibfk_2");
        });

        // xyz table removed from model - demo data only

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
