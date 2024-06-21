namespace Control.Endeavour.FrontEnd.Models.Models.Authentication.Multisession.Request
{
    public class MultisessionDtoRequest
    {
        public string? Email { get; set; }
        public string? Code { get; set; }

        public string? ReCaptchaResponse { get; set; }
    }
}
