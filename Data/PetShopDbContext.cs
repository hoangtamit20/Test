using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using serverapi.Entity;

namespace PetShop.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class PetShopDbContext : IdentityDbContext<AppUser>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public PetShopDbContext(DbContextOptions<PetShopDbContext> options) : base(options)
        {
        }

        #region dbset
        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<Contact> Contacts { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<Cart> Carts { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<CartItems> CartItems { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<Category> Categories { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<CategoryTranslation> CategoryTranslations { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<Language> Languages { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<Merchant> Merchants { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<Order> Orders { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<Payment> Payments { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<PaymentDestination> PaymentDestinations { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<PaymentNotification> PaymentNotifications { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<PaymentSignature> PaymentSignatures { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<Product> Products { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<ProductTranslation> ProductTranslations { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<Promotion> Promotions { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<PromotionCategory> PromotionCategories { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public virtual DbSet<PromotionProduct> PromotionProducts { get; set; } = null!;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>(b =>
            {
                b.HasIndex(u => u.Email).IsUnique();
                b.HasIndex(u => u.UserName).IsUnique();
            });

            builder.Entity<Cart>(c =>
            {
                c.HasIndex(c => c.UserId).IsUnique();
            });

            builder.Entity<ProductTranslation>(pp => 
            {
                pp.HasIndex(e => new { e.LanguageId, e.ProductId })
                .IsUnique();
            });

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName!.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
}

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionsBuilder"></param>

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    base.OnConfiguring(optionsBuilder);
}
    }
}