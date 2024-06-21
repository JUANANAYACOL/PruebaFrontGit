using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.User.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.UsersAdministration
{
    public partial class ChangePasswordUserModal
    {
        #region Variables

        #region Inject

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Components

        private InputModalComponent OldPasswordInput { get; set; } = new();

        private InputModalComponent NewPasswordInput { get; set; } = new();
        private InputModalComponent NewPasswordCheckInput { get; set; } = new();
        private NotificationsComponentModal notificationModal { get; set; } = new();

        #endregion Components

        #region Model

        private PasswordUpdateDtoRequest newPasswordRequest { get; set; } = new();

        #endregion Model

        #region Parameters

        [Parameter] public string Title { get; set; } = "";

        #endregion Parameters

        #region Environments

        #region Enviroments (String)

        private string OldPassword { get; set; } = string.Empty;

        private string NewPassword { get; set; } = string.Empty;
        private string NewPasswordCheck { get; set; } = string.Empty;
        private string OneCapitalLetter { get; set; } = "red";
        private string OneNumber { get; set; } = "red";
        private string OneSpecialCharacter { get; set; } = "red";
        private string MinimumCharacters { get; set; } = "red";
        private string MatchEquals { get; set; } = "red";
        private string Uri { get; set; } = string.Empty;

        #endregion Enviroments (String)

        #region Environments(Bool)

        private bool saveIsDisable { get; set; } = true;
        private bool IsSecondPassword { get; set; } = false;

        private bool modalStatus { get; set; } = new();

        #endregion Environments(Bool)

        #region Enviroments(Numeric)

        public int UserId { get; set; } = 0;

        #endregion Enviroments(Numeric)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private void HandleModalClosed(bool status)
        {
            ResetForm();
            modalStatus = status;
            StateHasChanged();
        }

        public void UpdateDataToChange(int UserToChange, bool UseSecondPassword = false)
        {
            UserId = UserToChange;
            IsSecondPassword = UseSecondPassword;
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success && args.IsAccepted)
            {
                modalStatus = false;
            }

            StateHasChanged();
        }

        private async Task HandleValidSubmit()
        {
            if (IsSecondPassword)
            {
                Uri = "security/User/UpdateSecondPassword";
            }
            else
            {
                Uri = "security/User/UpdatePassword";
            }

            newPasswordRequest = new()
            {
                UserId = UserId,
                NewPassword = NewPassword,
                OldPassword = OldPassword
            };

            var responseApi = await HttpClient.PostAsJsonAsync(Uri, newPasswordRequest);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();
            if (deserializeResponse.Succeeded)
            {
                notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"], "");
                ResetForm();
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["CannotUpdate"], true, Translation["Accept"], "");
            }
        }

        #endregion HandleMethods

        #region OthersMethods

        public void ResetForm()
        {
            OldPassword = string.Empty;
            NewPassword = string.Empty;
            NewPasswordCheck = string.Empty;
            EnableSaveButton();
        }

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        public void EnableSaveButton()
        {
            var containsCapitalLetter = Regex.IsMatch(NewPassword, "(?=.*[A-Z])");
            var containsSpecialCharacter = Regex.IsMatch(NewPassword, "(?=.*[\\W_])");
            var ContainsNumber = Regex.IsMatch(NewPassword, "(?=.*\\d)");
            var EqualsCheck = NewPassword.Equals(NewPasswordCheck);

            var containsAccptableLegnth = Regex.IsMatch(NewPassword, "^.{8,20}$");

            OneCapitalLetter = containsCapitalLetter ? "green" : "red";
            OneNumber = ContainsNumber ? "green" : "red";
            OneSpecialCharacter = containsSpecialCharacter ? "green" : "red";
            MinimumCharacters = containsAccptableLegnth ? "green" : "red";
            MatchEquals = EqualsCheck ? "green" : "red";

            if (containsCapitalLetter && containsSpecialCharacter && ContainsNumber && containsAccptableLegnth && !string.IsNullOrEmpty(OldPassword) && !string.IsNullOrEmpty(NewPassword) && Regex.IsMatch(OldPassword, "^.{8,20}$") && EqualsCheck)
            {
                saveIsDisable = false;
            }
            else
            {
                saveIsDisable = true;
            }
            StateHasChanged();
        }

        #endregion OthersMethods

        #endregion Methods
    }
}