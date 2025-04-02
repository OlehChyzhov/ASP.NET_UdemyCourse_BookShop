using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Models
{
    [Table("company")]
    public class Company
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name")]
        public string Name { get; set; }
        [Column("street_address")]
        public string? StreetAddress { get; set; }
        [Column("city")]
        public string? City { get; set; }
        [Column("state")]
        public string? State { get; set; }
        [Column("postal_code")]
        public string? PostalCode { get; set; }
        [Column("phone_number")]
        public string? PhoneNumber { get; set; }
    }
}
