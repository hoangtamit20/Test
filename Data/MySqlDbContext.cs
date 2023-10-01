using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using serverapi.Entity;

namespace serverapi.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class MySqlDbContext : IdentityDbContext<AppUser>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public MySqlDbContext(DbContextOptions<MySqlDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach (var entityTypes in builder.Model.GetEntityTypes())
                if (entityTypes.GetTableName()!.StartsWith("AspNet"))
                    entityTypes.SetTableName(entityTypes.GetTableName()!.Substring(6));
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
        #endregion
    }
}