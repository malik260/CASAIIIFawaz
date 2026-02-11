namespace Core.DTOs
{
    public class BuildingDesignCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? ImageUrl { get; set; }
        public string? BrochurePdfUrl { get; set; }
        public string? FloorPlanPdfUrl { get; set; }
    }
}
