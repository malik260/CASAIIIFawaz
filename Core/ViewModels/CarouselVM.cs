using Core.Enum;
namespace Core.ViewModels
{
    public class CarouselVM
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string Title { get; set; }
        public string? Subtitle { get; set; }
        public CarouselPageType PageType { get; set; }
        public string PageTypeDisplay { get; set; }
        public string BackgroundImageUrl { get; set; }
        public string? BadgeText { get; set; }
        public string? BrochureUrl { get; set; }
        public string? ButtonLink { get; set; }
        public string ButtonText { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
