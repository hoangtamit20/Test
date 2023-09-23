using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetShop.Entity;
using serverapi.Entity;

namespace PetShop.Data
{
    public class PetShopDbContext : IdentityDbContext<NguoiDung>
    {
        public PetShopDbContext(DbContextOptions<PetShopDbContext> options) : base(options)
        {
        }

        #region 
        public DbSet<Order>? Orders { get; set; }
        public DbSet<Product>? Products { get; set; }
        public DbSet<Brand>? Brands { get; set; }
        public DbSet<OrderDetail>? OrderDetails { get; set; }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Payment>? Payments { get; set; }
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