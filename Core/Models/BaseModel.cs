using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Model
{
    public class BaseModelMain : BaseModel
    {
        public string CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public virtual AppUser CreatedBy { get; set; }
    }
    public class BaseModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
    }
}
