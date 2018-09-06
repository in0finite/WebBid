namespace WebApplication1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        public int Id { get; set; }

        [StringLength(30)]
        [Required]
        public string Name { get; set; }

        [StringLength(30)]
        [Required]
        public string Surname { get; set; }

        [StringLength(30)]
        [DataType(DataType.EmailAddress)]
        [Required]
        public string Email { get; set; }

        [StringLength(256)]
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        public decimal? NumTokens { get; set; }

        public int? PermissionLevel { get; set; }

        public bool IsAdmin()
        {
            return this.PermissionLevel != null;
        }

        public string DisplayName { get { return this.Name + " " + this.Surname; } }

    }
}
