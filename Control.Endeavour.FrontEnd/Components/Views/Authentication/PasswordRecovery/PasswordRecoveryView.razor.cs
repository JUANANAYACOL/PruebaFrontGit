using Control.Endeavour.FrontEnd.Components.Components.DropDownList;
using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
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
using System.Reflection;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Components.Views.Authentication.PasswordRecovery
{
    public partial class PasswordRecoveryView
    {

		#region Variables
		#region Inject 
		/*[Inject]
		private EventAggregatorService? EventAggregator { get; set; }*/
        [Inject]
        private ProfileStateContainer? AuthenticationStateContainer { get; set; }
        [Inject]
		private HttpClient? HttpClient { get; set; }
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion

        #region Components

        private ChangePasswordDtoRequest formModel = new ();
        private InputComponent? emailInput;
        #endregion

        #region Modals
        private NotificationsComponentModal? NotificationModal { get; set; }


        #endregion

        #region Parameters
        private bool formSubmitted = true;
        private bool formSubEmail = false;


        #endregion

        #region Models
        private PasswordCodeRecoveryDtoRequest recoveryPasswordRequest { get; set; } = new();
        #endregion

        #region Environments

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
			////EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
		}


		#endregion

		#region Methods

		#region HandleLanguageChanged

		private async Task HandleLanguageChanged()
		{
			StateHasChanged();
		}
        #endregion

        private void validateEmail()
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
            await HandlePasswordRecoverySubmit();
            ResetForm();
        }
        private void ResetForm()
        {
            emailInput?.Reset();
            formSubmitted = false;
        }

        private async Task HandlePasswordRecoverySubmit()
        {
            
            Guid myuuid = Guid.NewGuid();

            recoveryPasswordRequest.Email = emailInput!.InputValue ?? string.Empty;
            recoveryPasswordRequest.UUID = myuuid.ToString();

            try
            {
                var responseApi = await HttpClient!.PostAsJsonAsync("security/Session/CreateRecoveryPassword", recoveryPasswordRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();
                if (deserializeResponse!.Succeeded)
                {
                    AuthenticationStateContainer?.Parametros(recoveryPasswordRequest.Email, recoveryPasswordRequest.UUID, "");
                    AuthenticationStateContainer?.SelectedComponentChanged("ChangePassword");
                }
                else
                {
                    NotificationModal?.UpdateModal(ModalType.Error, Translation["ProblemsSendingSecurityCode"], true, Translation["Accept"]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al intentar enviar el código de seguridad: {ex.Message}");
            }
        }

        

        private void CambiarComponente()
        {
            AuthenticationStateContainer?.SelectedComponentChanged("Login");
        }

       /* private string setKeyName(string key)
        {
            return Translation[key];
        }*/
        #endregion

    }
}
