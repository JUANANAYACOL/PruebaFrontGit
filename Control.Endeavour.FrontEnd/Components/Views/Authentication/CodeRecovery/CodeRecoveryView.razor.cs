using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.CodeRecovery.Request;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.Login.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Interfaces.Authentication;
using Control.Endeavour.FrontEnd.StateContainer.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Views.Authentication.CodeRecovery   
{
    public partial class CodeRecoveryView
    {
        #region Variables
        #region Inject 


        [Inject]
        private ProfileStateContainer? AuthenticationStateContainer { get; set; }

        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        [Inject]
        private IAuthenticationJWT? AuthenticationJWT { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion

        #region Parameter
        [Parameter]
        public EventCallback<string> BotonClick { get; set; }
        [Parameter]
        public string? Uuid { get; set; }
        [Parameter]
        public string? Ip { get; set; }
        [Parameter]
        public string? User { get; set; }
        #endregion

        #region Components
        private CodeInputComponent CodeInputComponent { set; get; } = new();
        private NotificationsComponentModal? NotificationModal { get; set; }

        #endregion

        #region Models
        private CodeRecoveryDtoRequest CodeRecoveryDtoRequest { set; get; } = new();
        public SendVerificationCodeDtoRequest loginCodeRequestResend { get; set; } = new();

        #endregion

        #region Enviroment
        private object FormModel { set; get; } = new();

        #endregion
        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            loginCodeRequestResend.User = AuthenticationStateContainer?.User;
            loginCodeRequestResend.Uuid = AuthenticationStateContainer?.Uuid;
        }

        #endregion

        #region Valid Submit

        #endregion

        #region Handle Submit
        private async Task HandleValidSubmit()
        {
            if (!CodeInputComponent.IsInvalid)
            {
                CodeRecoveryDtoRequest.UserNameOrEmail = AuthenticationStateContainer?.User;
                CodeRecoveryDtoRequest.Code = CodeInputComponent?.InputValue.ToUpper() ?? string.Empty;
                CodeRecoveryDtoRequest.Uuid = AuthenticationStateContainer?.Uuid;
                CodeRecoveryDtoRequest.Ip = AuthenticationStateContainer?.Ip;
                CodeRecoveryDtoRequest.TypeValidation = 2; // Se valida el User por medio del UserName

                try
                {
                    var answer = await HttpClient.PostAsJsonAsync("security/Session/UpdateRecoveryCode", CodeRecoveryDtoRequest);
                    var CodeRecoveryResponse = await answer.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();

                    if (CodeRecoveryResponse.Succeeded)
                    {
                        await AuthenticationJWT.LoginToken(CodeRecoveryResponse.Data);
                        NavigationManager?.NavigateTo("Home");
                    }
                    else
                    {

                        NotificationModal?.UpdateModal(ModalType.Error, Translation["WrongCode"], true, Translation["Accept"]);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al con el código de verificación: {ex.Message}");
                }
            }

        }
        #endregion
        private async Task HandleOperationExcuted(bool request)
        {
            if (request)
            {
                var answer = await HttpClient!.PostAsJsonAsync("security/Session/SendVerificationCode", loginCodeRequestResend);
                var loginResponse = await answer.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();

                if (!loginResponse!.Succeeded)
                {
                    NotificationModal?.UpdateModal(ModalType.Error, Translation["ProblemsSendingSecurityCode"], true, Translation["Accept"]);
                }
                else
                {
                    CodeInputComponent.Reset();
                }
                
                
            }
        }
        #region Retornar al login
        private void ReturnLogin()
        {
            AuthenticationStateContainer?.SelectedComponentChanged("Login");
        }
        #endregion

    }
}
