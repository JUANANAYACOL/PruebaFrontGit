

namespace Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles
{
    public class FileInfoData
    {
        public string? Name { get; set; }
        public string? Extension { get; set; }
        public long? Size { get; set; }
        public string? IconPath { get; set; }
        public byte[]? Base64Data { get; set; }
        public string Description { get; set; } = string.Empty;

        public string PathView
        {
            get
            {
                var allowedImageExtensions = new HashSet<string> { ".png", ".jpg", ".jpeg", ".gif", ".bmp" };
                if (allowedImageExtensions.Contains(Extension.ToLowerInvariant()))
                {
                    return $"data:image/{Extension[1..].ToLowerInvariant()};base64,{Convert.ToBase64String(Base64Data)}";
                }
                else
                {
                    return string.Empty;
                }
            }
        }
    }
}
