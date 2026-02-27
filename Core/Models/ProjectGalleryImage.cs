using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Model
{
    public class ProjectGalleryImage : BaseModel
    {
        public string ProjectId { get; set; } = string.Empty;

        [ForeignKey("ProjectId")]
        public virtual Project? Project { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public string? Caption { get; set; }

        public int DisplayOrder { get; set; }
    }
}
