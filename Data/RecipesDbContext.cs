using Microsoft.EntityFrameworkCore;
using RecipesAPI.Models;

namespace RecipesAPI.Data
{
    public class RecipesDbContext : DbContext
    {
        public RecipesDbContext(DbContextOptions<RecipesDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeStep> RecipeSteps { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<RecipeTag> RecipeTags { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<RecipeComment> RecipeComments { get; set; }
        public DbSet<RecipeLike> RecipeLikes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.ToTable("ingredients");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Category)
                    .HasColumnName("category")
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Índices definidos en tu schema
                entity.HasIndex(e => e.Name)
                    .HasDatabaseName("idx_ingredients_name_search");

                entity.HasIndex(e => e.Category)
                    .HasDatabaseName("idx_ingredients_category");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Description)
                    .HasColumnName("description");

                entity.Property(e => e.IconName)
                    .HasColumnName("icon_name")
                    .HasMaxLength(50);

                entity.Property(e => e.DisplayOrder)
                    .HasColumnName("display_order");

                entity.Property(e => e.IsActive)
                    .HasColumnName("is_active")
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("tags");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Slug)
                    .HasColumnName("slug")
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Color)
                    .HasColumnName("color")
                    .HasMaxLength(7);

                entity.Property(e => e.IsActive)
                    .HasColumnName("is_active")
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.PasswordHash)
                    .HasColumnName("password_hash")
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.Bio)
                    .HasColumnName("bio");

                entity.Property(e => e.ProfileImageUrl)
                    .HasColumnName("profile_image_url")
                    .HasMaxLength(500);

                entity.Property(e => e.BannerImageUrl)
                    .HasColumnName("banner_image_url")
                    .HasMaxLength(500);

                entity.Property(e => e.IsEmailVerified)
                    .HasColumnName("is_email_verified")
                    .HasDefaultValue(false);

                entity.Property(e => e.IsActive)
                    .HasColumnName("is_active")
                    .HasDefaultValue(true);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.LastLoginAt)
                    .HasColumnName("last_login_at");

                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("idx_users_email");

                entity.HasIndex(e => e.Username)
                    .IsUnique()
                    .HasDatabaseName("idx_users_username");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.ToTable("refresh_tokens");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();

                entity.Property(e => e.Token)
                    .HasColumnName("token")
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.ExpiresAt)
                    .HasColumnName("expires_at")
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.RevokedAt)
                    .HasColumnName("revoked_at");

                entity.Property(e => e.ReplacedByToken)
                    .HasColumnName("replaced_by_token")
                    .HasMaxLength(500);

                entity.Property(e => e.IsActive)
                    .HasColumnName("is_active")
                    .HasComputedColumnSql("(revoked_at IS NULL AND expires_at > CURRENT_TIMESTAMP)", stored: true);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Token)
                    .IsUnique()
                    .HasDatabaseName("idx_refresh_tokens_token");
            });

            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.ToTable("password_reset_tokens");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();

                entity.Property(e => e.Token)
                    .HasColumnName("token")
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.ExpiresAt)
                    .HasColumnName("expires_at")
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UsedAt)
                    .HasColumnName("used_at");

                entity.Property(e => e.IsValid)
                    .HasColumnName("is_valid")
                    .HasComputedColumnSql("(used_at IS NULL AND expires_at > CURRENT_TIMESTAMP)", stored: true);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.Token)
                    .IsUnique()
                    .HasDatabaseName("idx_password_reset_tokens_token");
            });

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.ToTable("recipes");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();

                entity.Property(e => e.CategoryId)
                    .HasColumnName("category_id");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasColumnName("description");

                entity.Property(e => e.CoverImageUrl)
                    .HasColumnName("cover_image_url")
                    .HasMaxLength(500);

                entity.Property(e => e.PrepTimeMinutes)
                    .HasColumnName("prep_time_minutes");

                entity.Property(e => e.CookTimeMinutes)
                    .HasColumnName("cook_time_minutes");

                entity.Property(e => e.TotalTimeMinutes)
                    .HasColumnName("total_time_minutes")
                    .HasComputedColumnSql("(prep_time_minutes + cook_time_minutes)", stored: true);

                entity.Property(e => e.DifficultyLevel)
                    .HasColumnName("difficulty_level")
                    .HasMaxLength(20)
                    .HasDefaultValue("easy");

                entity.Property(e => e.IsPublic)
                    .HasColumnName("is_public")
                    .HasDefaultValue(false);

                entity.Property(e => e.FavoritesCount)
                    .HasColumnName("favorites_count")
                    .HasDefaultValue(0);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.PublishedAt)
                    .HasColumnName("published_at");

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Category)
                    .WithMany()
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("idx_recipes_user_id");

                entity.HasIndex(e => e.CategoryId)
                    .HasDatabaseName("idx_recipes_category_id");

                entity.HasIndex(e => e.IsPublic)
                    .HasDatabaseName("idx_recipes_is_public");
            });

            modelBuilder.Entity<RecipeStep>(entity =>
            {
                entity.ToTable("recipe_steps");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.RecipeId)
                    .HasColumnName("recipe_id")
                    .IsRequired();

                entity.Property(e => e.StepNumber)
                    .HasColumnName("step_number")
                    .IsRequired();

                entity.Property(e => e.StepType)
                    .HasColumnName("step_type")
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasDefaultValue("preparation");

                entity.Property(e => e.Title)
                    .HasColumnName("title")
                    .HasMaxLength(200);

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .IsRequired();

                entity.Property(e => e.ImageUrl)
                    .HasColumnName("image_url")
                    .HasMaxLength(500);

                entity.Property(e => e.DurationMinutes)
                    .HasColumnName("duration_minutes");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Recipe)
                    .WithMany(r => r.Steps)
                    .HasForeignKey(e => e.RecipeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.RecipeId, e.StepNumber })
                    .IsUnique()
                    .HasDatabaseName("idx_recipe_steps_recipe_id");
            });

            modelBuilder.Entity<RecipeIngredient>(entity =>
            {
                entity.ToTable("recipe_ingredients");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.RecipeId)
                    .HasColumnName("recipe_id")
                    .IsRequired();

                entity.Property(e => e.IngredientId)
                    .HasColumnName("ingredient_id")
                    .IsRequired();

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.Unit)
                    .HasColumnName("unit")
                    .HasMaxLength(50);

                entity.Property(e => e.Notes)
                    .HasColumnName("notes");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Recipe)
                    .WithMany(r => r.RecipeIngredients)
                    .HasForeignKey(e => e.RecipeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Ingredient)
                    .WithMany()
                    .HasForeignKey(e => e.IngredientId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.RecipeId, e.IngredientId })
                    .IsUnique()
                    .HasDatabaseName("idx_recipe_ingredients_recipe_id");
            });

            modelBuilder.Entity<RecipeTag>(entity =>
            {
                entity.ToTable("recipe_tags");
                entity.HasKey(e => new { e.RecipeId, e.TagId });

                entity.Property(e => e.RecipeId)
                    .HasColumnName("recipe_id");

                entity.Property(e => e.TagId)
                    .HasColumnName("tag_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Recipe)
                    .WithMany(r => r.RecipeTags)
                    .HasForeignKey(e => e.RecipeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Tag)
                    .WithMany()
                    .HasForeignKey(e => e.TagId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.RecipeId)
                    .HasDatabaseName("idx_recipe_tags_recipe_id");

                entity.HasIndex(e => e.TagId)
                    .HasDatabaseName("idx_recipe_tags_tag_id");
            });

            modelBuilder.Entity<Favorite>(entity =>
            {
                entity.ToTable("favorites");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();

                entity.Property(e => e.RecipeId)
                    .HasColumnName("recipe_id")
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Recipe)
                    .WithMany()
                    .HasForeignKey(e => e.RecipeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.RecipeId })
                    .IsUnique();

                entity.HasIndex(e => new { e.UserId, e.CreatedAt })
                    .HasDatabaseName("idx_favorites_user_id");

                entity.HasIndex(e => e.RecipeId)
                    .HasDatabaseName("idx_favorites_recipe_id");
            });

            modelBuilder.Entity<RecipeLike>(entity =>
            {
                entity.ToTable("recipe_likes");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();

                entity.Property(e => e.RecipeId)
                    .HasColumnName("recipe_id")
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Recipe)
                    .WithMany()
                    .HasForeignKey(e => e.RecipeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.UserId, e.RecipeId })
                    .IsUnique();

                entity.HasIndex(e => e.RecipeId)
                    .HasDatabaseName("idx_recipe_likes_recipe_id");

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("idx_recipe_likes_user_id");
            });

            modelBuilder.Entity<RecipeComment>(entity =>
            {
                entity.ToTable("recipe_comments");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.RecipeId)
                    .HasColumnName("recipe_id")
                    .IsRequired();

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();

                entity.Property(e => e.Content)
                    .HasColumnName("content")
                    .IsRequired()
                    .HasMaxLength(1000);

                entity.Property(e => e.ParentCommentId)
                    .HasColumnName("parent_comment_id");

                entity.Property(e => e.IsEdited)
                    .HasColumnName("is_edited")
                    .HasDefaultValue(false);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at");

                entity.HasOne(e => e.Recipe)
                    .WithMany()
                    .HasForeignKey(e => e.RecipeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ParentComment)
                    .WithMany(c => c.Replies)
                    .HasForeignKey(e => e.ParentCommentId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.RecipeId)
                    .HasDatabaseName("idx_recipe_comments_recipe_id");

                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("idx_recipe_comments_user_id");

                entity.HasIndex(e => e.ParentCommentId)
                    .HasDatabaseName("idx_recipe_comments_parent_comment_id");
            });

            modelBuilder.Entity<Follow>(entity =>
            {
                entity.ToTable("follows");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("uuid_generate_v4()");

                entity.Property(e => e.FollowerId)
                    .HasColumnName("follower_id")
                    .IsRequired();

                entity.Property(e => e.FollowingId)
                    .HasColumnName("following_id")
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(e => e.Follower)
                    .WithMany()
                    .HasForeignKey(e => e.FollowerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Following)
                    .WithMany()
                    .HasForeignKey(e => e.FollowingId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.FollowerId, e.FollowingId })
                    .IsUnique();

                entity.HasIndex(e => e.FollowerId)
                    .HasDatabaseName("idx_follows_follower_id");

                entity.HasIndex(e => e.FollowingId)
                    .HasDatabaseName("idx_follows_following_id");
            });
        }
    }
}
