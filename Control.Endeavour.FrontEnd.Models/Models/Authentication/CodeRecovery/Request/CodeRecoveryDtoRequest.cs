namespace Control.Endeavour.FrontEnd.Models.Models.Authentication.CodeRecovery.Request
{
    public class CodeRecoveryDtoRequest
    {
        public string? Code { get; set; }
        public string? Uuid { get; set; }
        public string? Ip { get; set; }
        public string? UserNameOrEmail { get; set; }
        public int TypeValidation { get; set; }
    }
}
