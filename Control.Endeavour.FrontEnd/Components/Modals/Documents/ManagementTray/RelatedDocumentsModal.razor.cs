using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.DocumentaryTask;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Pages.Documents.ManagementTray;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.ManagementTray
{
    public partial class RelatedDocumentsModal
    {

        #region Variables

        #region Inject 
       /* [Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion

        #region Components


        #endregion

        #region Modals

        private NotificationsComponentModal notificationModal = new();
        private DocumentRelationModal docRelationModal = new();
        private ValidateDocumentGeneralInfoModal validateDocumentInfo = new();



        #endregion

        #region Parameters


        #endregion

        #region Models

        #endregion

        #region Environments

        #region Environments(String)

        private string FilingCode = string.Empty;

        #endregion

        #region Environments(Numeric)

        private int ControlId = 0;

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool modalStatus = false;

        #endregion

        #region Environments(List & Dictionary)

        private List<DocRelationDtoResponse> docRelatedList = new();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;

        }


        #endregion

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;

        }
        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {

        }
        private async Task HandleValidateDocumentAsync(int controlId)
        {
            await validateDocumentInfo.GeneralInformation(controlId);
            validateDocumentInfo.UpdateModalStatus(true);
        }
        private async Task HandleDocumentAssociated(bool data)
        {
            await GetDocumentRelated();
            docRelationModal.UpdateModalStatus(false);
        }
        #endregion

        #region OthersMethods

        #region GetDataMethods

        private async Task GetDocumentRelated()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("ControlId");
                HttpClient?.DefaultRequestHeaders.Add("ControlId", $"{ControlId}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<DocRelationDtoResponse>>>("documentmanagement/Document/ByFilterId");
                HttpClient?.DefaultRequestHeaders.Remove("ControlId");
                if (deserializeResponse!.Succeeded)
                {
                    docRelatedList = deserializeResponse.Data;
                }
                else
                {
                    docRelatedList = new();
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                Console.WriteLine($"Error al obtener la prioridad: {ex.Message}");
            }
        }
        #endregion

        #region ModalMethods

        public async Task LoadDocumentRelated(int controlId , string fillingCode)
        {
            ControlId = controlId;
            FilingCode = fillingCode;
            await GetDocumentRelated();
        }

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }
        private void ShowDocRelationModal()
        {
            docRelationModal.getDocumentFillCode(FilingCode);
            docRelationModal.UpdateModalStatus(true);
        }

        private async Task ShowModalInformation(DocRelationDtoResponse record)
        {
            await HandleValidateDocumentAsync(record.ControlId);
        }


        #endregion

        #endregion

        #endregion

    }
}
