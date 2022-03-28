using Microsoft.EntityFrameworkCore;
using Database.Models;

namespace Database.DbContexts;

public partial class MasterContext : DbContext
{
    public MasterContext()
    {
    }

    public MasterContext(DbContextOptions<MasterContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Comment> Comment { get; set; } = null!;
    public virtual DbSet<ProductGroup> ProductGroup { get; set; } = null!;
    public virtual DbSet<Review> Review { get; set; } = null!;
    public virtual DbSet<ReviewLike> ReviewLike { get; set; } = null!;
    public virtual DbSet<ReviewTag> ReviewTag { get; set; } = null!;
    public virtual DbSet<ReviewUserRating> ReviewUserRating { get; set; } = null!;
    public virtual DbSet<Role> Role { get; set; } = null!;
    public virtual DbSet<StatusReview> StatusReview { get; set; } = null!;
    public virtual DbSet<Tag> Tag { get; set; } = null!;
    public virtual DbSet<User> User { get; set; } = null!;
    public virtual DbSet<UserSocial> UserSocial { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Cyrillic_General_CI_AS");

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("comment", "main");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(3000)
                .IsUnicode(false)
                .HasColumnName("content");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.ReviewId).HasColumnName("reviewId");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Review)
                .WithMany(p => p.Comment)
                .HasForeignKey(d => d.ReviewId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comment_review_id_fk");

            entity.HasOne(d => d.User)
                .WithMany(p => p.Comment)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("comment_user_id_fk");
        });

        modelBuilder.Entity<ProductGroup>(entity =>
        {
            entity.ToTable("productGroup", "main");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("review", "main");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AuthorAssessment).HasColumnName("authorAssessment");
            entity.Property(e => e.AuthorId).HasColumnName("authorId");
            entity.Property(e => e.AverageUserRating).HasColumnName("averageUserRating");
            entity.Property(e => e.Content)
                .IsUnicode(false)
                .HasColumnName("content");
            entity.Property(e => e.CreationDate).HasColumnName("creationDate");
            entity.Property(e => e.DeletionDate).HasColumnName("deletionDate");
            entity.Property(e => e.ProductId).HasColumnName("productId");
            entity.Property(e => e.ProductName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("productName");
            entity.Property(e => e.RedactionDate).HasColumnName("redactionDate");
            entity.Property(e => e.StatusId).HasColumnName("statusId");
            entity.Property(e => e.Title)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("title");

            entity.HasOne(d => d.Author)
                .WithMany(p => p.Review)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("review_user_id_fk");

            entity.HasOne(d => d.Product)
                .WithMany(p => p.Review)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("review_productGroup_id_fk");

            entity.HasOne(d => d.Status)
                .WithMany(p => p.Review)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("review_statusReview_id_fk");
        });

        modelBuilder.Entity<ReviewLike>(entity =>
        {
            entity.ToTable("reviewLike", "main");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ReviewId).HasColumnName("reviewId");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Review)
                .WithMany(p => p.ReviewLike)
                .HasForeignKey(d => d.ReviewId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reviewLike_review_id_fk");

            entity.HasOne(d => d.User)
                .WithMany(p => p.ReviewLike)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reviewLike_user_id_fk");
        });

        modelBuilder.Entity<ReviewTag>(entity =>
        {
            entity.ToTable("reviewTag", "main");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ReviewId).HasColumnName("reviewId");
            entity.Property(e => e.TagId).HasColumnName("tagId");

            entity.HasOne(d => d.Review)
                .WithMany(p => p.ReviewTag)
                .HasForeignKey(d => d.ReviewId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reviewTag_review_id_fk");

            entity.HasOne(d => d.Tag)
                .WithMany(p => p.ReviewTag)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reviewTag_tag_id_fk");
        });

        modelBuilder.Entity<ReviewUserRating>(entity =>
        {
            entity.ToTable("reviewUserRating", "main");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Assessment).HasColumnName("assessment");
            entity.Property(e => e.ReviewId).HasColumnName("reviewId");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Review)
                .WithMany(p => p.ReviewUserRating)
                .HasForeignKey(d => d.ReviewId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reviewUserRating_review_id_fk");

            entity.HasOne(d => d.User)
                .WithMany(p => p.ReviewUserRating)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reviewUserRating_user_id_fk");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("role", "main");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<StatusReview>(entity =>
        {
            entity.ToTable("statusReview", "main");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("tag", "main");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user", "main");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Avatar)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("avatar");
            entity.Property(e => e.LastLoginDate).HasColumnName("lastLoginDate");
            entity.Property(e => e.Nickname)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("nickname");
            entity.Property(e => e.RegistrationDate).HasColumnName("registrationDate");
            entity.Property(e => e.ReviewsLikes).HasColumnName("reviewsLikes");
            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.SocialId).HasColumnName("socialId");

            entity.HasOne(d => d.Role)
                .WithMany(p => p.User)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_role_id_fk");

            entity.HasOne(d => d.Social)
                .WithMany(p => p.User)
                .HasForeignKey(d => d.SocialId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_userSocial_id_fk");
        });

        modelBuilder.Entity<UserSocial>(entity =>
        {
            entity.ToTable("userSocial", "main");

            entity.HasIndex(e => e.Uid, "userSocial_uid_uindex")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.Network)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("network");
            entity.Property(e => e.Uid)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("uid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}