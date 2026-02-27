namespace Core.DTOs
{
    public class ProjectGalleryImageDto
    {
        public string Id { get; set; } = string.Empty;
        public string ProjectId { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public int DisplayOrder { get; set; }
    }
}
