using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace project_dnc_se1.Models;

public partial class CompanyWebContext : DbContext
{
    public CompanyWebContext()
    {
    }

    public CompanyWebContext(DbContextOptions<CompanyWebContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CompanyInfo> CompanyInfos { get; set; }

    public virtual DbSet<ContactInfo> ContactInfos { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventsCategory> EventsCategories { get; set; }

    public virtual DbSet<EventsProduct> EventsProducts { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<NewsCategory> NewsCategories { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-3SQ6IJF\\MSSQLSERVER01;Initial Catalog=CompanyWeb;Integrated Security=True;Encrypt=True;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CompanyInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__COMPANY___3214EC27762C619F");

            entity.ToTable("COMPANY_INFO");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<ContactInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CONTACT___3214EC277BA27380");

            entity.ToTable("CONTACT_INFO");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.EventsId).HasColumnName("EventsID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Events).WithMany(p => p.ContactInfos)
                .HasForeignKey(d => d.EventsId)
                .HasConstraintName("FK__CONTACT_I__Event__5535A963");

            entity.HasOne(d => d.Product).WithMany(p => p.ContactInfos)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__CONTACT_I__Produ__5441852A");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EVENTS__3214EC2760121F20");

            entity.ToTable("EVENTS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("UpdatedAT");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Category).WithMany(p => p.Events)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__EVENTS__Category__46E78A0C");

            entity.HasOne(d => d.User).WithMany(p => p.Events)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__EVENTS__UserID__47DBAE45");
        });

        modelBuilder.Entity<EventsCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EVENTS_C__3214EC2775A034BE");

            entity.ToTable("EVENTS_CATEGORY");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<EventsProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EVENTS_P__3214EC2794E11EB2");

            entity.ToTable("EVENTS_PRODUCT");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.EventsId).HasColumnName("EventsID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Events).WithMany(p => p.EventsProducts)
                .HasForeignKey(d => d.EventsId)
                .HasConstraintName("FK__EVENTS_PR__Event__5070F446");

            entity.HasOne(d => d.Product).WithMany(p => p.EventsProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__EVENTS_PR__Produ__5165187F");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NEWS__3214EC27AF029F59");

            entity.ToTable("NEWS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.IsDeleted).HasDefaultValue(false);
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("UpdatedAT");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Category).WithMany(p => p.News)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__NEWS__CategoryID__403A8C7D");

            entity.HasOne(d => d.User).WithMany(p => p.News)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__NEWS__UserID__412EB0B6");
        });

        modelBuilder.Entity<NewsCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__NEWS_CAT__3214EC2739D6E984");

            entity.ToTable("NEWS_CATEGORY");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PRODUCTS__3214EC27D140E22D");

            entity.ToTable("PRODUCTS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.ImageUrls).HasColumnName("ImageURLs");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("UpdatedAT");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__PRODUCTS__Catego__4D94879B");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PRODUCT___3214EC27DC8F5620");

            entity.ToTable("PRODUCT_CATEGORY");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__USERS__3214EC276A66A655");

            entity.ToTable("USERS");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("CreatedAT");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("UpdatedAT");
            entity.Property(e => e.UserName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
