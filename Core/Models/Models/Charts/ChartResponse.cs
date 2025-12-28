namespace Core.Models.Charts
{
    public class ChartResponse
    {
        public string PngFilePath { get; set; } = string.Empty;
        public string PdfFilePath { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}