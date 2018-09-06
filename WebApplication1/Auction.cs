namespace WebApplication1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    [Table("Auction")]
    public partial class Auction
    {
        [Key]
        [StringLength(256)]
        public string GUID { get; set; }

        [StringLength(50)]
        [Required]
        public string Name { get; set; }

        [StringLength(50)]
        public string ImageLink { get; set; }

        [Column(TypeName = "image")]
        public byte[] Image { get; set; }
        
        public int? Duration { get; set; }

        [Required]
        public decimal? StartingPrice { get; set; }

        public decimal? CurrentPrice { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateCreated { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateOpened { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateClosed { get; set; }
        
        public int? Status { get; set; }

        public int? CreatorId { get; set; }

        public string StatusText { get {
                int? s = this.Status;
                if (!s.HasValue)
                    return "READY";
                if (1 == s.Value)
                    return "OPENED";
                if (2 == s.Value)
                    return "CLOSED";
                return "READY";
            }
        }

        public bool IsOpened()
        {
            return this.Status == 1;
        }

        public bool IsClosed()
        {
            return this.Status == 2;
        }

        public IEnumerable<Bid> GetBids() { return new Model1().Bids.ToList().Where(b => b.AuctionID == GUID); }

    }
}
