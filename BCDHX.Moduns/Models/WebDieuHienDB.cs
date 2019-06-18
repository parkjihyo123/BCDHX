namespace BCDHX.Moduns.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class WebDieuHienDB : DbContext
    {
        public WebDieuHienDB()
            : base("name=WebDieuHienDB")
        {
        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<About> Abouts { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<AdminUser> AdminUsers { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<BestDeal> BestDeals { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CuponCode> CuponCodes { get; set; }
        public virtual DbSet<FeedBack> FeedBacks { get; set; }
        public virtual DbSet<ImageForProduct> ImageForProducts { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<InvoiceAccount> InvoiceAccounts { get; set; }
        public virtual DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public virtual DbSet<LinkSystem> LinkSystems { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Silder> Silders { get; set; }
        public virtual DbSet<StockInOut> StockInOuts { get; set; }
        public virtual DbSet<StockInOutDetail> StockInOutDetails { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<WishList> WishLists { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .Property(e => e.Username)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<Account>()
                .Property(e => e.Amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUser>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<Category>()
                .Property(e => e.Name_Category)
                .IsUnicode(false);

            modelBuilder.Entity<CuponCode>()
                .Property(e => e.ValueSale)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Invoice>()
                .Property(e => e.ID_Invoice)
                .IsUnicode(false);

            modelBuilder.Entity<InvoiceAccount>()
                .Property(e => e.ID_Invoice)
                .IsUnicode(false);

            modelBuilder.Entity<InvoiceAccount>()
                .Property(e => e.ID_InvoicePaymentAccount)
                .IsUnicode(false);

            modelBuilder.Entity<InvoiceAccount>()
                .Property(e => e.Amount)
                .HasPrecision(19, 4);

            modelBuilder.Entity<InvoiceDetail>()
                .Property(e => e.ID_Invoice)
                .IsUnicode(false);

            modelBuilder.Entity<InvoiceDetail>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<InvoiceDetail>()
                .Property(e => e.Sale)
                .HasPrecision(19, 4);

            modelBuilder.Entity<InvoiceDetail>()
                .Property(e => e.ShippingFee)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Product>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Product>()
                .Property(e => e.Sale)
                .HasPrecision(19, 4);

            modelBuilder.Entity<StockInOut>()
                .HasMany(e => e.StockInOutDetails)
                .WithRequired(e => e.StockInOut)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<StockInOutDetail>()
                .Property(e => e.StockLeft)
                .IsFixedLength();
        }
    }
}
