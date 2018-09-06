namespace WebApplication1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TokenOrder")]
    public partial class TokenOrder
    {
        [Key]
        [StringLength(256)]
        public string GUID { get; set; }

        public int? UserId { get; set; }

        public int? NumTokens { get; set; }

        public decimal? PackagePrice { get; set; }

        [StringLength(20)]
        public string Status { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateSubmitted { get; set; }
    }
}
