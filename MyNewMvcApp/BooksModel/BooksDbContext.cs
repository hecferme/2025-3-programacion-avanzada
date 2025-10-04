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

    public virtual DbSet<author> authors { get; set; }

    public virtual DbSet<book> books { get; set; }

    public virtual DbSet<bookcopy> bookcopies { get; set; }

    public virtual DbSet<borrow> borrows { get; set; }

    public virtual DbSet<person> persons { get; set; }

    public virtual DbSet<theme> themes { get; set; }

    public virtual DbSet<xyz> xyzs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<author>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Country).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<book>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Edition).HasMaxLength(100);
            entity.Property(e => e.EnglishTitle).HasMaxLength(255);
            entity.Property(e => e.OriginalTitle).HasMaxLength(255);

            entity.HasMany(d => d.Authors).WithMany(p => p.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "bookauthor",
                    r => r.HasOne<author>().WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("bookauthors_ibfk_2"),
                    l => l.HasOne<book>().WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("bookauthors_ibfk_1"),
                    j =>
                    {
                        j.HasKey("BookId", "AuthorId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("bookauthors");
                        j.HasIndex(new[] { "AuthorId" }, "AuthorId");
                    });

            entity.HasMany(d => d.Themes).WithMany(p => p.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "booktheme",
                    r => r.HasOne<theme>().WithMany()
                        .HasForeignKey("ThemeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("bookthemes_ibfk_2"),
                    l => l.HasOne<book>().WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("bookthemes_ibfk_1"),
                    j =>
                    {
                        j.HasKey("BookId", "ThemeId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("bookthemes");
                        j.HasIndex(new[] { "ThemeId" }, "ThemeId");
                    });
        });

        modelBuilder.Entity<bookcopy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.BookId, "BookId");

            entity.Property(e => e.ISBN).HasMaxLength(20);
            entity.Property(e => e.IsLost).HasDefaultValueSql("'0'");
            entity.Property(e => e.Language).HasMaxLength(50);
            entity.Property(e => e.Provider).HasMaxLength(100);
            entity.Property(e => e.Serial).HasMaxLength(100);

            entity.HasOne(d => d.Book).WithMany(p => p.bookcopies)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookcopies_ibfk_1");
        });

        modelBuilder.Entity<borrow>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.BookCopyId, "BookCopyId");

            entity.HasIndex(e => e.PersonId, "PersonId");

            entity.HasOne(d => d.BookCopy).WithMany(p => p.borrows)
                .HasForeignKey(d => d.BookCopyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("borrows_ibfk_1");

            entity.HasOne(d => d.Person).WithMany(p => p.borrows)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("borrows_ibfk_2");
        });

        modelBuilder.Entity<person>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Sex)
                .HasMaxLength(1)
                .IsFixedLength();
        });

        modelBuilder.Entity<theme>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Subject).HasMaxLength(255);
            entity.Property(e => e.ThemeName).HasMaxLength(255);
        });

        modelBuilder.Entity<xyz>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PRIMARY");

            entity.ToTable("xyz");

            entity.HasIndex(e => e.name, "idx_name");

            entity.Property(e => e.id).ValueGeneratedNever();
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp");
            entity.Property(e => e.name).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
