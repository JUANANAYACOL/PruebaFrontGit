using Control.Endeavour.FrontEnd.Components.Components.ButtonGroup;
using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Replacement.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Replacement.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VReplacement.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.Replacement
{
    public partial class ReplacementModal : ComponentBase
    {
        #region Variables

        #region Inject

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Components

        private NotificationsComponentModal notificationModal { get; set; } = new();
        private ButtonGroupComponent inputUserFullname { get; set; } = new();
        private ButtonGroupComponent inputReplacementFullname { get; set; } = new();

        private InputModalComponent inputReason { get; set; } = new();

        #endregion Components

        #region Parameters

        [Parameter] public string idModalIdentifier { get; set; } = null!;
        [Parameter] public bool modalStatus { get; set; }
        [Parameter] public EventCallback<bool> OnStatusChanged { get; set; }
        [Parameter] public EventCallback<bool> OnStatusChangedUpdate { get; set; }

        #endregion Parameters

        #region Models

        private NotificationService NotificationService { get; set; } = new();
        private ReplacementCreateDtoRequest replacemenetCreateDtoRequest { get; set; } = new();
        private ReplacementUpdateDtoRequest replacemenetUpdateDtoRequest { get; set; } = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        public string userFullname { get; set; } = "";

        public string replacementFullname { get; set; } = "";

        #endregion Environments(String)

        #region Environments(Numeric)

        private decimal CharacterCounter { get; set; } = 0;

        #endregion Environments(Numeric)

        #region Environments(DateTime)

        private DateTime? from { get; set; } = DateTime.Today;
        private DateTime? to { get; set; }

        private DateTime minValueTo { get; set; } = DateTime.Today;
        private DateTime maxValueTo { get; set; } = new DateTime(3900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion Environments(DateTime)

        #region Environments(Bool)

        public bool userSearchModalStatus { get; set; } = new();

        private bool userToFill { get; set; } = false;
        private bool isEditForm { get; set; }

        private bool saveIsDisable { get; set; } = true;

        #endregion Environments(Bool)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            from = DateTime.Today;
            isEditForm = false;
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #region updateMinValue

        public void updateMinValue()
        {
            if (from != null)
            {
                minValueTo = (DateTime)from;
            }
            EnableSaveButton();
            StateHasChanged();
        }

        #endregion updateMinValue

        #region updateMaxValue

        public void updateMaxValue()
        {
            if (to != null)
            {
                maxValueTo = (DateTime)to;
            }
            EnableSaveButton();
            StateHasChanged();
        }

        #endregion updateMaxValue

        #region updateUserSelection

        public void updateUserSelection(VUserDtoResponse userSelected)
        {
            if (!userToFill)
            {
                if (userSelected.UserId == replacemenetCreateDtoRequest.UserReplacementId && replacemenetCreateDtoRequest.UserReplacementId != 0)
                {
                    notificationModal.UpdateModal(ModalType.Information, Translation["SameRecordForReplacement"], true, Translation["Accept"], "");
                    userFullname = string.Empty;
                    replacemenetCreateDtoRequest.UserId = 0;
                }
                else
                {
                    userFullname = userSelected.FullName;
                    replacemenetCreateDtoRequest.UserId = userSelected.UserId;
                }
            }
            else
            {
                if (userSelected.UserId == replacemenetCreateDtoRequest.UserId && replacemenetCreateDtoRequest.UserId != 0)
                {
                    notificationModal.UpdateModal(ModalType.Information, Translation["SameRecordForReplacement"], true, Translation["Accept"], "");
                    replacementFullname = string.Empty;
                    replacemenetCreateDtoRequest.UserReplacementId = 0;
                }
                else
                {
                    replacementFullname = userSelected.FullName;
                    replacemenetCreateDtoRequest.UserReplacementId = userSelected.UserId;
                }
            }
            EnableSaveButton();

            StateHasChanged();
        }

        #endregion updateUserSelection

        #region OpenNewModal

        private async Task OpenNewModal(bool typeOfSelection)
        {
            userToFill = typeOfSelection;
            await OnStatusChanged.InvokeAsync(true);
        }

        #endregion OpenNewModal

        #region ResetFormAsync

        /*
                private void ResetFormAsync()
                {
                    CharacterCounter = 0;
                    from = null;
                    to = null;
                    userFullname = "";
                    replacementFullname = "";
                    replacemenetCreateDtoRequest = new();
                    StateHasChanged();
                }
        */

        #endregion ResetFormAsync

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)
        {
            if (newValue)
            {
                isEditForm = false;
            }

            modalStatus = newValue;

            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region HandleModalClosed

        private void HandleModalClosed(bool status)
        {
            ResetForm();
            modalStatus = status;
            StateHasChanged();
        }

        #endregion HandleModalClosed

        #region HandleValidSubmit

        private async Task HandleValidSubmit()
        {
            if (isEditForm)
            {
                await HandleFormUpdate();
            }
            else
            {
                await HandleFormCreate();
            }
        }

        #endregion HandleValidSubmit

        #region ResetForm

        public void ResetForm()
        {
            replacemenetCreateDtoRequest = new();
            from = DateTime.Today;
            to = null;
            userFullname = "";
            replacementFullname = "";
            CharacterCounter = 0;
            userFullname = string.Empty;
            replacementFullname = string.Empty;
            minValueTo = DateTime.Today;
            maxValueTo = new DateTime(3900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            EnableSaveButton();
            StateHasChanged();
        }

        #endregion ResetForm

        #region HandleFormCreate

        private async Task HandleFormCreate()
        {
            try
            {
                replacemenetCreateDtoRequest.StartDate = (DateTime)from;
                replacemenetCreateDtoRequest.EndDate = (DateTime)to;

                var responseApi = await HttpClient!.PostAsJsonAsync("administration/Replacement/CreateReplacement", replacemenetCreateDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<VReplacementDtoResponse>>();
                var NotiResult = NotificationService.NotificationDecrypt(Translation, deserializeResponse);
                notificationModal.UpdateModal(NotiResult.ModalType, NotiResult.NotificationMessage, true, Translation["Accept"], "");

                //if (deserializeResponse!.Succeeded)

                //{
                //    notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"], "");
                //}
                //else
                //{
                //    notificationModal.UpdateModal(ModalType.Information, Translation["CannotCreate"], true, Translation["Accept"], "");
                //}
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"], "");
            }
        }

        #endregion HandleFormCreate

        #region HandleFormUpdate

        private async Task HandleFormUpdate()
        {
            try
            {
                if (inputReplacementFullname.IsInputValid && inputUserFullname.IsInputValid)
                {
                    replacemenetUpdateDtoRequest.StartDate = (DateTime)from;
                    replacemenetUpdateDtoRequest.EndDate = (DateTime)to;

                    replacemenetUpdateDtoRequest.Reason = replacemenetCreateDtoRequest.Reason;
                    replacemenetUpdateDtoRequest.UserId = replacemenetCreateDtoRequest.UserId;
                    replacemenetUpdateDtoRequest.UserReplacementId = replacemenetCreateDtoRequest.UserReplacementId;

                    var responseApi = await HttpClient!.PostAsJsonAsync("administration/Replacement/UpdateReplacement", replacemenetUpdateDtoRequest);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<VReplacementDtoResponse>>();

                    var NotiResult = NotificationService.NotificationDecrypt(Translation, deserializeResponse);
                    notificationModal.UpdateModal(NotiResult.ModalType, NotiResult.NotificationMessage, true, Translation["Accept"], "");

                    //if (deserializeResponse!.Succeeded)
                    //{
                    //    notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"], "");
                    //}
                    //else
                    //{
                    //    notificationModal.UpdateModal(ModalType.Error, Translation["CannotUpdate"], true, Translation["Accept"], "");
                    //}
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["CannotUpdateByValidation"], true, Translation["Accept"], "");
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
            }
        }

        #endregion HandleFormUpdate

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success && args.IsAccepted)
            {
                await OnStatusChangedUpdate.InvokeAsync(false);
            }

            StateHasChanged();
        }

        #endregion HandleModalNotiClose

        #region UpdateSelectedRemplacement

        public void UpdateSelectedRemplacement(VReplacementDtoResponse replacement)
        {
            isEditForm = true;
            replacemenetCreateDtoRequest.UserReplacementId = replacement.UserReplacementId;

            replacemenetCreateDtoRequest.UserId = replacement.UserId;
            to = replacement.EndDate;
            from = replacement.StartDate;
            replacemenetCreateDtoRequest.Reason = replacement.Reason;
            userFullname = replacement.UserFullName;
            replacementFullname = replacement.ReplacementFullName;
            replacemenetCreateDtoRequest.Reason = replacement.Reason ?? "";
            replacemenetUpdateDtoRequest.ReplacementId = replacement.ReplacementId;
            CharacterCounter = replacemenetCreateDtoRequest.Reason.Length;

            updateMaxValue();
            updateMinValue();
            EnableSaveButton();
            StateHasChanged();
        }

        #endregion UpdateSelectedRemplacement

        #region EnableSaveButton

        public void EnableSaveButton()
        {
            if (CharacterCounter <= 0 || to == null || replacemenetCreateDtoRequest.UserId <= 0 || replacemenetCreateDtoRequest.UserReplacementId <= 0)
            {
                saveIsDisable = true;
            }
            else
            {
                saveIsDisable = false;
            }
            StateHasChanged();
        }

        #endregion EnableSaveButton

        #region CountCharacters

        private void CountCharacters(ChangeEventArgs e)
        {
            string value = e.Value.ToString() ?? string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                CharacterCounter = value.Length;
            }
            else
            {
                CharacterCounter = 0;
            }
            EnableSaveButton();
            StateHasChanged();
        }

        #endregion CountCharacters

        #endregion Methods
    }
}