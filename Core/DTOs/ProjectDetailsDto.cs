namespace Core.DTOs
{
    public class ProjectDetailsDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? HeroImageUrl { get; set; }
        public string? Description { get; set; }
        public string? BrochurePdfUrl { get; set; }
        public List<BuildingDesignDto>? BuildingDesigns { get; set; }
    }
}
