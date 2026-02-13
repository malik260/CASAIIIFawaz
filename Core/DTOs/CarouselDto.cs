using Core.Enum;
using Microsoft.AspNetCore.Http;

namespace Core.DTOs
{
    public class CarouselDto
    {
        public string Title { get; set; }
        public string? Subtitle { get; set; }
        public CarouselPageType PageType { get; set; }
        public IFormFile? BackgroundImage { get; set; } // Nullable only during updates
        public string? BadgeText { get; set; } // For Home only
        public IFormFile? Brochure { get; set; } // For Home only (PDF)
        public string? ButtonLink { get; set; } // For Vendor only
        public string ButtonText { get; set; }
    }

    public class CarouselUpdateDto : CarouselDto
    {
        public string Id { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsActive { get; set; }
    }
}
