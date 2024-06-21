namespace Control.Endeavour.FrontEnd.Models.Models.Authentication.Login.Request
{
    public class LoginUserRequest
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Ip { get; set; }
        public string? Uuid { get; set; }
        public int? CompanyId { get; set; }
        public string? ReCaptchaResponse { get; set; }
    } 
}
