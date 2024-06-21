using Control.Endeavour.FrontEnd.Components.Components.DropDownList;
using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Timers;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.CodeRecovery.Request;
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
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Components.Views.Authentication.ChangePassword
{
    public partial class ChangePasswordView
    {
        #region Variables

        #region Inject

        [Inject]
        private ProfileStateContainer? AuthenticationStateContainer { get; set; }

        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        [Inject]
        private IAuthenticationJWT? AuthenticationJWT { get; set; }

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Components

        private CodeInputComponent codeInputComponent { set; get; } = new();
        private ChangePasswordDtoRequest formModel = new();
        private InputComponent? passwordInput;
        private InputComponent? passwordConfirmationInput;

        #endregion Components

        #region Modals

        private NotificationsComponentModal? NotificationModal { get; set; }

        #endregion Modals

        #region Models

        private ChangePasswordDtoRequest recoveryPasswordRequest { get; set; } = new();
        private PasswordCodeRecoveryDtoRequest passwordRecoveryCodeRequestResend { get; set; } = new();

        #endregion Models

        #region Environments

        private bool formSubmitted = true;
        private bool formSubmPass = false;
        private bool formSubmPassConfirmation = false;

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            ////EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            passwordRecoveryCodeRequestResend.Email = AuthenticationStateContainer!.User;
            passwordRecoveryCodeRequestResend.UUID = AuthenticationStateContainer.Uuid;
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        private void validatePassword()
        {
            passwordInput!.ValidateInput();
            passwordConfirmationInput!.ValidateInput();

            // Verifica si alguno de los campos de contraseña es inválido
            bool passwordInvalid = passwordInput.IsInvalid;
            bool confirmationInvalid = passwordConfirmationInput.IsInvalid;

            // Verifica si las contraseñas no coinciden
            bool passwordsMismatch = !passwordInvalid && !confirmationInvalid && passwordInput.InputValue != passwordConfirmationInput.InputValue && passwordConfirmationInput.InputValue != "";

            formSubmPass = passwordInvalid;
            formSubmPassConfirmation = confirmationInvalid || passwordsMismatch;

            if (!formSubmPass && !formSubmPassConfirmation) { formSubmitted = false; }
            else { formSubmitted = true; }
        }

        private async Task HandleValidSubmit()
        {
            await HandleChangePasswordSubmit();
        }

        private void ResetForm()
        {
            codeInputComponent.Reset();
            passwordInput?.Reset();
            passwordConfirmationInput?.Reset();
            formSubmitted = true;
        }

        private async Task HandleChangePasswordSubmit()
        {
            if (!codeInputComponent.IsInvalid)
            {
                recoveryPasswordRequest.Email = AuthenticationStateContainer?.User;
                recoveryPasswordRequest.Code = codeInputComponent?.InputValue.ToUpper() ?? string.Empty;
                recoveryPasswordRequest.NewPassword = passwordInput?.InputValue ?? string.Empty;
                recoveryPasswordRequest.UUID = AuthenticationStateContainer?.Uuid;
                recoveryPasswordRequest.Ip = "1.1.1.1";

                try
                {
                    var answer = await HttpClient!.PostAsJsonAsync("security/Session/UpdatePassword", recoveryPasswordRequest);
                    var CodeRecoveryResponse = await answer.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();

                    if (CodeRecoveryResponse!.Succeeded)
                    {
                        switch (CodeRecoveryResponse.Data)
                        {
                            case "RPA1":
                                NotificationModal?.UpdateModal(ModalType.Success, Translation["PasswordSuccesfullyChanged"], true, Translation["Accept"]);
                                await Task.Delay(4000);
                                AuthenticationStateContainer?.SelectedComponentChanged("Login");
                                ResetForm();
                                break;

                            case "RPA2":
                                NotificationModal?.UpdateModal(ModalType.Information, Translation["RecoveryCodeExpired"], true, Translation["Accept"], "");
                                break;

                            case "RPA3":
                                NotificationModal?.UpdateModal(ModalType.Information, Translation["PasswordInUser"], true, Translation["Accept"], "");
                                break;

                            case "RPA4":
                                NotificationModal?.UpdateModal(ModalType.Information, Translation["CodeNotExisting"], true, Translation["Accept"], "");
                                break;
                        }
                    }
                    else
                    {
                        NotificationModal?.UpdateModal(ModalType.Error, Translation["ChangingPasswordProblems"], true, Translation["Accept"]);
                        ResetForm();
                    }
                }
                catch (Exception ex)
                {
                    NotificationModal?.UpdateModal(ModalType.Error, Translation["ChangingPasswordError"], true, Translation["Accept"]);
                }
            }
        }

        private async Task HandleOperationExcuted(bool request)
        {
            if (request)
            {
                var responseApi = await HttpClient!.PostAsJsonAsync("security/Session/CreateRecoveryPassword", passwordRecoveryCodeRequestResend);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();
                if (!deserializeResponse!.Succeeded)
                {
                    NotificationModal?.UpdateModal(ModalType.Error, Translation["ProblemsSendingSecurityCode"], true, Translation["Accept"]);
                }
                else
                {
                    codeInputComponent.Reset();
                    ResetForm();
                }
                
                
            }
        }
        private void CambiarComponente()
        {
            AuthenticationStateContainer?.SelectedComponentChanged("Login");
        }

        

        #endregion Methods
    }
}