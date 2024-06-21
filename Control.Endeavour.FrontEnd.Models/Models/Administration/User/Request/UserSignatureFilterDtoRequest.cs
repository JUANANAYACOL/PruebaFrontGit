
namespace Control.Endeavour.FrontEnd.Models.Models.Administration.User.Request
{
    public class UserSignatureFilterDtoRequest
    {
        public int UserId { get; set; }
        public string? SignatureType { get; set; }
        public string? SignatureName { get; set;}
        public string? SignatureFunction { get; set; }
        public string? SignatureDescription { get; set; }
    }
}
