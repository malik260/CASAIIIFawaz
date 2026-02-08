namespace Core.ViewModels
{
    public class HeplerResponseVM
    {
        public bool success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public object Data { get; set; }
    }
}
