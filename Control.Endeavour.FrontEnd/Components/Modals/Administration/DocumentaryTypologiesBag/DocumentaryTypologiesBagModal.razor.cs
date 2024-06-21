using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.DocumentaryTypologiesBag.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.DocumentaryTypologiesBag.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaValues.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ProfileUsers.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.DocumentaryTypologiesBag
{
    public partial class DocumentaryTypologiesBagModal : ComponentBase
    {
        #region Variables

        #region Inject

        [Inject]
        private IJSRuntime Js { get; set; }

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        #endregion Inject

        #region models

        private DocumentaryTypologiesBagCreateDtoRequest documentaryTypologiesBagCreateDtoRequest { get; set; } = new();
        private DocumentaryTypologiesBagUpdateDtoRequest documentaryTypologiesBagUpdateDtoRequest { get; set; } = new();

        #endregion models

        #region Components

        private NotificationsComponentModal notificationModal { get; set; } = new();
        private InputModalComponent nameInput { get; set; } = new();

        #endregion Components

        #region Parameters

        [Parameter] public string Title { get; set; } = string.Empty;
        [Parameter] public bool modalStatus { get; set; } = new();
        [Parameter] public EventCallback<bool> OnStatusChanged { get; set; } = new();

        #endregion Parameters

        #region Environments

        #region Environments(String)

        public string description { get; set; } = "";
        public string name { get; set; } = "";

        public string profileCode { get; set; } = "";

        #endregion Environments(String)

        #region Environments(Numeric)

        private decimal CharacterCounter { get; set; } = 0;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool activeState { get; set; } = true;
        private bool isEditForm { get; set; } = false;

        private bool saveIsDisable { get; set; } = true;

        #endregion Environments(Bool)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        #region HandleValidSubmit

        private async Task HandleValidSubmit()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            if (string.IsNullOrEmpty(name))
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["EnterName"], true, Translation["Accept"], "");
            }
            else if (string.IsNullOrEmpty(description))
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["EnterDescription"], true, Translation["Accept"], "");
            }
            else if (isEditForm)
            {
                await HandleFormUpdate();
            }
            else
            {
                await HandleFormCreate();
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleValidSubmit

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #region HandleFormCreate

        private async Task HandleFormCreate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                documentaryTypologiesBagCreateDtoRequest.TypologyDescription = description;
                documentaryTypologiesBagCreateDtoRequest.TypologyName = name;
                documentaryTypologiesBagCreateDtoRequest.ActiveState = activeState;

                var responseApi = await HttpClient!.PostAsJsonAsync("documentarytypologies/DocumentaryTypologiesBag/CreateDocumentaryTypologiesBag", documentaryTypologiesBagCreateDtoRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ProfilesDtoResponse>>();

                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleFormCreate

        #region HandleFormUpdate

        private async Task HandleFormUpdate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                documentaryTypologiesBagUpdateDtoRequest.TypologyDescription = description;
                documentaryTypologiesBagUpdateDtoRequest.TypologyName = name;
                documentaryTypologiesBagUpdateDtoRequest.ActiveState = activeState;

                var responseApi = await HttpClient!.PostAsJsonAsync("documentarytypologies/DocumentaryTypologiesBag/UpdateDocumentaryTypologiesBag", documentaryTypologiesBagUpdateDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ProfilesDtoResponse>>();
                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleFormUpdate

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                await OnStatusChanged.InvokeAsync(false);
            }

            StateHasChanged();
        }

        #endregion HandleModalNotiClose

        #region HandleModalClosed

        private void HandleModalClosed(bool status)
        {
            ResetFormAsync();
            modalStatus = status;
            StateHasChanged();
        }

        #endregion HandleModalClosed

        #endregion HandleMethods

        #region OthersMethods

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
        }

        #endregion CountCharacters

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            isEditForm = false;
            ResetFormAsync();
            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region ResetFormAsync

        private void ResetFormAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            name = "";
            description = "";
            activeState = true;
            CharacterCounter = 0;
            StateHasChanged();

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion ResetFormAsync

        #region UpdateSelectedDTB

        public void UpdateSelectedDTB(DocumentaryTypologiesBagDtoResponse dtb)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            this.isEditForm = true;
            documentaryTypologiesBagUpdateDtoRequest = new();
            documentaryTypologiesBagUpdateDtoRequest.DocumentaryTypologyBagId = dtb.DocumentaryTypologyBagId;
            this.name = dtb.TypologyName;
            this.activeState = dtb.ActiveState;
            this.description = dtb.TypologyDescription;
            CharacterCounter = this.description.Length;
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion UpdateSelectedDTB

        public void EnableSaveButton()
        {
            if (string.IsNullOrEmpty(name))
            {
                saveIsDisable = true;
            }
            else
            {
                saveIsDisable = false;
            }
            StateHasChanged();
        }

        #endregion OthersMethods

        #endregion Methods
    }
}