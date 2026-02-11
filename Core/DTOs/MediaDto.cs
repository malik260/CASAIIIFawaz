namespace Core.DTOs
{
    public class MediaDto
    {
        public string Id { get; set; } = string.Empty;
        public string StoredPath { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long? FileSize { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public string OwnerType { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
    }
}
