using Control.Endeavour.FrontEnd.Models.Models.Administration.User.Request;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Signature.Request
{
    public class SignatureCreateDtoRequest
    {
        public int UserId { get; set; }
        public List<UsersSignatureDtoRequest> NewSignatures { get; set; }
    }
}