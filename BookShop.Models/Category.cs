using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookShop.Models
{
    [Table("category")]
    public class Category
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }
        [Column("name")]
        [MaxLength(30)]
        [DisplayName("Category Name")]
        public string Name { get; set; }
        [Column("display_order")]
        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Display Order must be in range [1, 100]")]
        public int DisplayOrder { get; set; }
    }
}
