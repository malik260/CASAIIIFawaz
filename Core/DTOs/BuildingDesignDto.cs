namespace Core.DTOs
{
    public class BuildingDesignDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? FloorPlanPdfUrl { get; set; }
    }
}
