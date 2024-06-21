namespace Control.Endeavour.FrontEnd.Services.Interfaces.Authentication
{
    public interface IAuthenticationJWT
    {
        Task LoginToken(string token);
        Task LogoutToken();
        Task TokenRenewalManagement();
    }
}
