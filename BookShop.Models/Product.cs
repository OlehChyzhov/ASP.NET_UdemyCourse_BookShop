using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Models
{
    [Table("product")]
    public class Product
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("product_title")]
        public string Title { get; set; }
        [Column("product_description")]
        public string Description { get; set; }
        [Column("product_isbn")]
        public string ISBN { get; set; }
        [Column("product_author")]
        public string Author { get; set; }
        [Range(1, 1000)]
        [Display(Name = "List Price")]
        [Column("product_list_price")]
        public double ListPrice { get; set; }

        [Range(1, 1000)]
        [Display(Name = "Price for 1-50")]
        [Column("price_per_product")]
        public double Price { get; set; }

        [Range(1, 1000)]
        [Display(Name = "Price for 50+")]
        [Column("price_50")]
        public double Price50 { get; set; }

        [Range(1, 1000)]
        [Display(Name = "Price for 100+")]
        [Column("price_100")]
        public double Price100 { get; set; }

        [Column("category_id")]

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category? Category { get; set; }

        [ValidateNever]
        public List<ProductImage> ProductImages { get; set; }
    }
}
