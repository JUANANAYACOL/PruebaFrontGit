using Control.Endeavour.FrontEnd.Components.Components.Captcha;
using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Inputs;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.Login.Request;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.Multisession.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Interfaces.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http;
using System;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;
using Control.Endeavour.FrontEnd.StateContainer.Authentication;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.CodeRecovery.Request;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.DropDownList;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.Resources;
using Microsoft.Extensions.Localization;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;

namespace Control.Endeavour.FrontEnd.Components.Views.Authentication.Multisession
{
    public partial class MultisessionView
    {
        #region Variables

        #region Inject

        [Inject]
        public IAuthenticationJWT? AuthenticationJWT { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private ProfileStateContainer? AuthenticationStateContainer { get; set; }

        [Inject]
        private IConfiguration? Configuration { get; set; }

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion

        #region Components

        private InputComponent? emailInput;

        #region RecaptchaGoogle

        private ReCaptchaGoogleComponent? reCaptchaGoogle;
        private string siteKey = "";
        private string captchaResponse = string.Empty;
        private bool ValidReCAPTCHA = false;
        private bool DisablePostButton => !ValidReCAPTCHA;

        private void OnSuccess() => ValidReCAPTCHA = true;

        private void OnExpired() => ValidReCAPTCHA = false;

        #endregion RecaptchaGoogle

        private NotificationsComponentModal? NotificationModal { get; set; }

        #endregion Components

        #region Models

        private MultisessionDtoRequest multisessionDtoRequest = new();
        private LoginUserRequest loginUserDtoRequest = new();

        private CodeInputComponent CodeInputComponent { set; get; } = new();

        #endregion Models

        #region Entorno

        public string ComponenteRenderizar { get; set; } = "CodeRecovery";
        private bool formSubmitted = true;
        private bool formSubEmail = false;
        private string userEnteredCaptcha = string.Empty;

        #endregion Entorno

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            AuthenticationStateContainer.ComponentChange += StateHasChanged;
            siteKey = Configuration["RecaptchaSiteKey"].ToString();
        }

        #endregion OnInitializedAsync

        #region HandleLanguageChanged

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.ModalOrigin.Equals("SuccesModal"))
            {
                AuthenticationStateContainer?.SelectedComponentChanged("Login");
            }
        }

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #region Components

        private void CambiarComponente()
        {
            AuthenticationStateContainer?.SelectedComponentChanged("PasswordRecovery");
        }

        #endregion Components

        #region Valid Submit

        private void ValidateEmail()
        {
            emailInput?.ValidateInput();

            if ((emailInput?.InputValue == "") || (emailInput?.IsInvalid == true))
            {
                formSubEmail = true;
                formSubmitted = true;
            }
            else
            {
                formSubEmail = false;
                formSubmitted = false;
            }
        }

        private async Task HandleValidSubmit()
        {
            captchaResponse = await reCaptchaGoogle.GetResponseAsync(); 
            if (string.IsNullOrEmpty(captchaResponse))
            {
                NotificationModal?.UpdateModal(ModalType.Warning, Translation["ValidateCaptcha"], true);
            }
            else
            {
                await HandleSingOff();
            }
            ResetForm();
        }

        #endregion Valid Submit

        #region Handle Login Submit

        private async Task HandleSingOff()
        {
            try
            {
                multisessionDtoRequest.Email = emailInput.InputValue;
                multisessionDtoRequest.ReCaptchaResponse = captchaResponse;

                var answer = await HttpClient.PostAsJsonAsync("security/Session/SignOffGoogle", multisessionDtoRequest);
                var multisessionResponse = await answer.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();

                if (multisessionResponse.Succeeded)
                {
                    NotificationModal?.UpdateModal(ModalType.Information, Translation["otherSessionHasBeenClosed"], true, Translation["Accept"],buttonTextCancel:"", modalOrigin: "SuccesModal");
                }
                else
                {
                    NotificationModal?.UpdateModal(ModalType.Error, Translation["NoActiveSessionsMessage"], true, Translation["Accept"]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al intentar iniciar sesión: {ex.Message}");
            }
        }

        #endregion Handle Login Submit

        #region Render Component

        public void ClickCallBack(string ComponenteRenderizarnew)
        {
            ComponenteRenderizar = ComponenteRenderizarnew;
        }

        #endregion Render Component

        #region Cleanup

        private void ResetForm()
        {
            emailInput?.Reset();
            formSubmitted = false;
        }

        #endregion Cleanup

        #region Captcha
        private void OnCaptchaEntered(string captcha)
        {
            userEnteredCaptcha = captcha;
        }

        #endregion Captcha

        #region Return
        private async Task ReturnLoginAsync()
        {
            AuthenticationStateContainer?.SelectedComponentChanged("Login");
        }

        #endregion Return

        #region SetKeyName
        /*private string SetKeyName(string key)
        {
            return Translation[key];
        }*/
        #endregion SetKeyName
    }
}