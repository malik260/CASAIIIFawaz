namespace Core.DTOs
{
    public class ProjectDetailsDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? HeroImageUrl { get; set; }
        public string? Description { get; set; }
        public string? BrochurePdfUrl { get; set; }
        public List<BuildingDesignDto> BuildingDesigns { get; set; } = new();
    }
}
