

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.User.Request
{
    public class UsersSignatureDtoRequest
    {
        public int? UserSignatureId {  get; set; }
        public string FileExt { get; set; } = null!;

        public string FileName { get; set; } = null!;

        public byte[] DataFile { get; set; } = null!;

        public string? SignatureType { get; set; }

        public string? SignatureName { get; set; }

        public string? SignatureFunction { get; set; }

        public string? SignatureDescription { get; set; }
        
    }
}
