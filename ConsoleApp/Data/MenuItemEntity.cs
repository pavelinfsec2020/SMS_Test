using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ConsoleApp.Data
{
    [Table("menu_items")]
    public class MenuItemEntity
    {
        [Key]
        [Column("id")]
        [StringLength(50)]
        public string Id { get; set; } = string.Empty;

        [Required]
        [Column("article")]
        [StringLength(50)]
        public string Article { get; set; } = string.Empty;

        [Required]
        [Column("name")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Column("price", TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Column("is_weighted")]
        public bool IsWeighted { get; set; }

        [Column("full_path")]
        [StringLength(500)]
        public string? FullPath { get; set; }

        [Column("barcodes")]
        public string? Barcodes { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
