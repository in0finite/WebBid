namespace WebApplication1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Bid")]
    public partial class Bid
    {
        [Key]
        [Column(Order = 0)]
        public int UserID { get; set; }

        [Key]
        [StringLength(256)]
        [Column(Order = 1)]
        public string AuctionID { get; set; }

        [Key]
        [Column(TypeName = "date", Order = 2)]
        public DateTime DateOfBidding { get; set; }

        public decimal? TokensOffered { get; set; }

        [StringLength(200)]
        public string UserName { get; set; }

    }
}
