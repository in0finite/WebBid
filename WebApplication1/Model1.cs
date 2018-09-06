namespace WebApplication1
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
        }

        public virtual DbSet<Auction> Auctions { get; set; }
        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Bid> Bids { get; set; }

        //public virtual DbSet<TokenOrder> TokenOrder { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auction>()
                .Property(e => e.GUID)
                .IsUnicode(false);

            modelBuilder.Entity<Auction>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Auction>()
                .Property(e => e.ImageLink)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Surname)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.NumTokens)
                .HasPrecision(18, 0);



            modelBuilder.Entity<TokenOrder>()
                .Property(e => e.GUID)
                .IsUnicode(false);

            modelBuilder.Entity<TokenOrder>()
                .Property(e => e.PackagePrice)
                .HasPrecision(18, 0);

            modelBuilder.Entity<TokenOrder>()
                .Property(e => e.Status)
                .IsUnicode(false);

        }

        public System.Data.Entity.DbSet<WebApplication1.TokenOrder> TokenOrders { get; set; }
    }
}
