using Core.Enum;
using Core.Model;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Carousel : BaseModelMain
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(500)]
        public string? Subtitle { get; set; }

        [Required]
        public CarouselPageType PageType { get; set; }

        [Required]
        public string BackgroundImageUrl { get; set; }

        // For Home: Badge text (e.g., "PROJECT OF THE MONTH")
        [StringLength(100)]
        public string? BadgeText { get; set; }

        // For Home: PDF brochure for download
        public string? BrochureUrl { get; set; }

        // For Vendor: External link (e.g., "Get Started")
        [StringLength(500)]
        public string? ButtonLink { get; set; }

        [Required]
        [StringLength(100)]
        public string ButtonText { get; set; }

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}
