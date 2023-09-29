using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using serverapi.Entity;

namespace PetShop.Data
{
    public class PetShopDbContext : IdentityDbContext<AppUser>
    {
        public PetShopDbContext(DbContextOptions<PetShopDbContext> options) : base(options)
        {
        }

        #region dbset
        public virtual DbSet<Contact> Contacts { get; set; } = null!;
        public virtual DbSet<Cart> Carts { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<CategoryTranslation> CategoryTranslations { get; set; } = null!;
        public virtual DbSet<Language> Languages { get; set; } = null!;
        public virtual DbSet<Merchant> Merchants { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<PaymentDestination> PaymentDestinations { get; set; } = null!;
        public virtual DbSet<PaymentNotification> PaymentNotifications { get; set; } = null!;
        public virtual DbSet<PaymentTransaction> PaymentTransactions { get; set; } = null!;
        public virtual DbSet<PaymentSignature> PaymentSignatures { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
        public virtual DbSet<ProductTranslation> ProductTranslations { get; set; } = null!;
        public virtual DbSet<Promotion> Promotions { get; set; } = null!;
        #endregion
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var entityType in builder.Model.GetEntityTypes ()) {
                var tableName = entityType.GetTableName();
                if (tableName!.StartsWith ("AspNet")) {
                    entityType.SetTableName (tableName.Substring (6));
                }
            }

            // builder.Entity<OrderDetail>(entity => entity.HasKey(or => new {or.ProductId, or.OrderId}));
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}