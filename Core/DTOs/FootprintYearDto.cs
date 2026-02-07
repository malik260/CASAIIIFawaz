namespace Core.DTOs
{
    public class FootprintYearDto
    {
        public int Year { get; set; }
        public List<FootprintProjectDto> Projects { get; set; } = new();
    }
}

