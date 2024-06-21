using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Records.Request;
using Control.Endeavour.FrontEnd.Models.Models.Records.Response;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.StateContainer.Filing;
using DevExpress.Blazor.Scheduler.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Components.Modals.Records
{
    public partial class UploadPDFAutomaticIndexingModal
    {
        #region Variables

        #region Inject 
        [Inject] private HttpClient? HttpClient { get; set; }
        [Inject] private IJSRuntime? Js { get; set; }
        [Inject] private IStringLocalizer<Translation>? Translation { get; set; }
        #endregion

        #region Modals
        private NotificationsComponentModal NotificationModal = new();
        #endregion

        #region Parameters
        [Parameter] public EventCallback<MyEventArgs<DocumentaryTypologyTDMDtoRequest>> OnChangeData { get; set; } = new();
        [Parameter] public string Title { get; set; } = "";
        #endregion

        #region Models
        public TelerikPdfViewer PdfViewerRef { get; set; }
        FileDocumentaryTypologyDtoRequest fileDocumentaryTypologyDtoRequest = new();
        DocumentaryTypologyTDMDtoRequest documentaryTypologyTDMDtoRequest = new();
        #endregion

        #region Environments

        #region Environments(String)
        private string UploadPDF = "";
        private string AlertMessage = "";
        private string ViewPDF = "d-none";
        private string ViewDate = "d-none";
        private string TypeOfUploadPdf = "";
        private string BtnText = "";
        private string LabelText = "";
        private string FormatMessage = "";
        #endregion

        #region Environments(Numeric)
        private int FileSize = 50;
        #endregion

        #region Environments(DateTime)
        private DateTime FechaDoc;
        #endregion

        #region Environments(Bool)
        private bool ModalStatus = false;
        private bool Disabled = false;
        private bool ButtonVisblefiling = false;
        #endregion

        #region Environments(List & Dictionary)
        private string[] AllowedExtensions { get; set; } = { ".pdf" };
        public byte[]? FileData { get; set; }
        public FileInfoData FileDataFinal = new();
        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
            Title = Translation["GenerateAutIndexing"];
            FormatMessage = Translation["MessageUploadPDF"];
            BtnText = Translation["IndexPDF"];
            LabelText = Translation["DocumentDate"];
            FechaDoc = DateTime.Now;
        }
        #endregion

        #region Methods

        #region HandleMethods
        private async Task HandleModalClosed(bool status)
        {
            ModalStatus = status;

            if(documentaryTypologyTDMDtoRequest.DocumentId != 0)
            {
                var result = new MyEventArgs<DocumentaryTypologyTDMDtoRequest>()
                {
                    Data = documentaryTypologyTDMDtoRequest,
                    ModalStatus = status
                };

                CleanView();
                await OnChangeData.InvokeAsync(result);

            }

            StateHasChanged();
        }
        private void HandleFile(List<FileInfoData> data)
        {
            if (data != null && data.Count > 0)
            {
                FileData = data[0].Base64Data;
                FileDataFinal = data[0];
                ViewPDF = "";
                ViewDate = "";
            }
            else
            {
                FileData = null;
                ViewPDF = "d-none";
                ViewDate = "d-none";
            }
        }
        private async Task HandleAutomaticIndexing()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                fileDocumentaryTypologyDtoRequest.DataFile = FileData;
                fileDocumentaryTypologyDtoRequest.DocDate = FechaDoc;

                var responseApi = await HttpClient!.PostAsJsonAsync("records/ControlBoard/FileDocumentaryTypology", fileDocumentaryTypologyDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<FileDocumentaryTypologyDtoResponse>>();
                if (deserializeResponse!.Succeeded)
                {
                    Disabled = true;
                    FileData = null;
                    BtnText = "¡" + Translation["ConfirmedIndexing"] + "!";
                    documentaryTypologyTDMDtoRequest.VolumeId = fileDocumentaryTypologyDtoRequest.VolumeId;
                    documentaryTypologyTDMDtoRequest.DocumentId = deserializeResponse.Data.DocumentId;
                    UploadPDF = "d-none";
                    NotificationModal.UpdateModal(ModalType.Success, Translation["UploadSuccessfullAIMessage"], true, Translation["Accept"], "", Translation["ConfirmedIndexing"], "");
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else
                {
                    Disabled = false;
                    BtnText = Translation["ErrorIndexing"];
                    NotificationModal.UpdateModal(ModalType.Error, Translation["ErrorIndexing"], true, Translation["Accept"]);
                    UploadPDF = "";
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            catch (Exception ex)
            {

                NotificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessageS"], true);
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                throw;
            }
        }

        #endregion

        #region ModalMethods
        public async Task UpdateModalStatusAsync(bool newValue)
        {
            ModalStatus = newValue;
            StateHasChanged();
        }
        public async Task GetValue(FileDocumentaryTypologyDtoRequest model)
        {
            try
            {
                fileDocumentaryTypologyDtoRequest = model;
            }
            catch (Exception ex)
            {
                NotificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }

        #endregion

        #region OthersMethods

        public void DisabledAutomaticIndexing()
        {
            if(FechaDoc != null)
            {
                ButtonVisblefiling = true;
            }
            else
            {
                ButtonVisblefiling = false;
            }

            StateHasChanged();
        }

        public void CleanView()
        {
            UploadPDF = "";
            ViewPDF = "d-none";
            ViewDate = "d-none";
            BtnText = Translation["IndexPDF"];
        }

        #endregion

        #endregion

    }
}
