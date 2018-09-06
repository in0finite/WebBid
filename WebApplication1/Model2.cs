namespace WebApplication1
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    //public partial class Model2 : DbContext
    //{
    //    public Model2()
    //        : base("name=Model2")
    //    {
    //    }

    //    public virtual DbSet<TokenOrder> TokenOrder { get; set; }

    //    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    //    {
    //        modelBuilder.Entity<TokenOrder>()
    //            .Property(e => e.GUID)
    //            .IsUnicode(false);

    //        modelBuilder.Entity<TokenOrder>()
    //            .Property(e => e.PackagePrice)
    //            .HasPrecision(18, 0);

    //        modelBuilder.Entity<TokenOrder>()
    //            .Property(e => e.Status)
    //            .IsUnicode(false);
    //    }
    //}
}
