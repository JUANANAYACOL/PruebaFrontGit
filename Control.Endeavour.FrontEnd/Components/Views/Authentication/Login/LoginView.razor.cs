using Control.Endeavour.FrontEnd.Components.Components.Captcha;
using Control.Endeavour.FrontEnd.Components.Components.DropDownList;
using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.Login.Request;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.PasswordRecovery.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Interfaces.Authentication;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Net.Http.Json;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Components.Views.Authentication.Login
{
    public partial class LoginView
    {
        #region Variables

        #region Inject

        [Inject]
        private ProfileStateContainer? AuthenticationStateContainer { get; set; }

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        public IAuthenticationJWT? AuthenticationJWT { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        //TODO: Comentar para pruebas y eliminar para produccion
        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        [Inject]
        private IConfiguration? Configuration { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Components

        private InputComponent? NameInput { get; set; }
        private InputComponent? PasswordInput { get; set; }
        private CaptchaComponent? Captcha { get; set; }
        private NotificationsComponentModal? NotificationModal { get; set; }

        #region RecaptchaGoogle

        private ReCaptchaGoogleComponent? reCaptchaGoogle;
        private string siteKey = "";
        private string captchaResponse = string.Empty;
        private bool ValidReCAPTCHA = false;
        private bool DisablePostButton => !ValidReCAPTCHA;

        private void OnSuccess() => ValidReCAPTCHA = true;

        private void OnExpired() => ValidReCAPTCHA = false;

        #endregion RecaptchaGoogle

        #endregion Components

        #region DTOs

        private LoginUserRequest LoginUserRequest { get; set; } = new();

        #endregion DTOs

        #region Entorno

        private bool formSubUser = false;
        private bool formSubPassw = false;
        private string userEnteredCaptcha = string.Empty;
        public object FormModel { get; set; } = new();

        #endregion Entorno

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            await AuthenticationJWT.LogoutToken();
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            AuthenticationStateContainer.ComponentChange += StateHasChanged;
            siteKey = Configuration["RecaptchaSiteKey"].ToString();
        }

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion OnInitializedAsync

        #region Components

        private void CambiarComponente()
        {
            AuthenticationStateContainer.SelectedComponentChanged("PasswordRecovery");
        }

        #endregion Components

        #region Valid Submit

        private void validateFields()
        {
            NameInput?.ValidateInput();
            PasswordInput?.ValidateInput();

            formSubUser = NameInput!.IsInvalid;
            formSubPassw = PasswordInput!.IsInvalid;
        }

        private async Task HandleValidSubmit()
        {
            captchaResponse = await reCaptchaGoogle.GetResponseAsync();
            //captchaResponse = await reCaptchaGoogle.GetResponseAsync();

            if (string.IsNullOrEmpty(captchaResponse))
            {
                NotificationModal?.UpdateModal(ModalType.Warning, Translation["ValidateCaptcha"], true);
            }
            else
            {
                await HandleLoginSubmit();
                NameInput?.Reset();
                PasswordInput?.Reset();
            }
        }

        #endregion Valid Submit

        #region Handle Login Submit

        private async Task HandleLoginSubmit()
        {
            Guid myuuid = Guid.NewGuid();

            LoginUserRequest.UserName = NameInput?.InputValue ?? string.Empty;
            LoginUserRequest.Password = PasswordInput?.InputValue ?? string.Empty;
            LoginUserRequest.Ip = "1.1.1.1";
            LoginUserRequest.Uuid = myuuid.ToString();
            LoginUserRequest.CompanyId = 17;
            LoginUserRequest.ReCaptchaResponse = captchaResponse;

            try
            {
                var answer = await HttpClient.PostAsJsonAsync("security/Session/CreateLoginGoogle", LoginUserRequest);

                var loginResponse = await answer.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();

                if (loginResponse!.Succeeded)
                {
                    switch (loginResponse.Data)
                    {
                        case "SLS2A1":
                            AuthenticationStateContainer?.SelectedComponentChanged("Multisession");
                            break;

                        case "SLS2A2":
                            AuthenticationStateContainer?.Parametros(LoginUserRequest.UserName, LoginUserRequest.Uuid, LoginUserRequest.Ip);
                            AuthenticationStateContainer?.SelectedComponentChanged("CodeRecovery");
                            Captcha!.Dispose();
                            break;

                        default:

                            NotificationModal?.UpdateModal(ModalType.Error, Translation["LogInProblems"], true, Translation["Accept"]);
                            break;
                    }
                }
                else
                {
                    NotificationModal?.UpdateModal(ModalType.Error, Translation["WrongUserOrPassword"], true, Translation["Accept"]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al intentar iniciar sesión: {ex.Message}");
            }
        }

        #endregion Handle Login Submit

        /* private string SetKeyName(string key)
         {
             return Translation[key];
         }*/

        private void OnCaptchaEntered(string captcha)
        {
            userEnteredCaptcha = captcha;
        }
    }
}