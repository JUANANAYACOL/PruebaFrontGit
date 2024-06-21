using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Filing.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Records.Request;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Filing;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.ServiceModel.Channels;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.Filing
{
    public partial class UploadPDFModal
    {
        #region Variables

        #region Inject 
        //[Inject] private EventAggregatorService? EventAggregator { get; set; }
        [Inject] private HttpClient? HttpClient { get; set; }
        [Inject] private IJSRuntime? Js { get; set; }
        [Inject] private FilingStateContainer? FilingSC { get; set; }
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion

        #region Parameters
        [Parameter] public EventCallback<bool> OnChangeData { get; set; }
        [Parameter] public bool NewFilig { get; set; } = false;

        [Parameter] public string Title { get; set; } = "Subir imagen al repositorio central";
        [Parameter] public bool IsRecord { get; set; } = false;


        #endregion

        #region Modals
        private NotificationsComponentModal NotificationModal = new();
        #endregion

        #region Models
        public TelerikPdfViewer PdfViewerRef { get; set; }
        private LoadImageDocumentDtoRequest LoadImageDocument = new();
        private VolumeDtoRequest UploadVolumeImageDtoRequest = new();
        #endregion

        #region Environments

        #region Environments(String)
        private string AlertMessage = "";
        private string BtnText = "";
        private string BtnLabelText = "";
        private string uploadPDF = "";
        private string ViewPDF = "d-none";
        private string TypeOfUploadPdf = "";

        #endregion

        #region Environments(Numeric)
        private int FileSize = 50;

        #endregion

        #region Environments(Bool)
        private bool ModalStatus = false;
        private bool Disabled = false;
        private bool Verify = false;
        private bool ButtonVisblefiling = true;
        private bool ButtonVisibleRecord = false;
        #endregion

        #region Environments(List & Dictionary)
        private string[] AllowedExtensions { get; set; } = { ".pdf" };
        public byte[]? FileData { get; set; }
        public FileInfoData FileDataFinal = new();
        #endregion

        MarkupString formatMessage;

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            AlertMessage = GetUploadMessage();
            if (IsRecord)
            {
                TypeOfUploadPdf = "volumen del expediente";
                ButtonVisblefiling = !IsRecord;
                ButtonVisibleRecord = IsRecord;
                BtnText = Translation["UploadImage"];
                BtnLabelText = Translation["UploadIt"];
            }
            else
            {
                TypeOfUploadPdf = Translation["Filing"];
                BtnLabelText = Translation["ConfirmIt"];
                BtnText = Translation["ConfirmImageCorrect"];
            }
            TypeOfUploadPdf = string.Format(Translation["ChoosePDFFileMessage"], TypeOfUploadPdf);
            formatMessage = new MarkupString(TypeOfUploadPdf);
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
            ModalStatus = status;
            StateHasChanged();
        }
        private void HandleFile(List<FileInfoData> data)
        {
            if (data != null && data.Count > 0)
            {
                FileData = data[0].Base64Data;
                FileDataFinal = data[0];
                ViewPDF = "";
            }
            else
            {
                FileData = null;
                ViewPDF = "d-none";
            }
        }

        private async Task HandleFormVerify()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                LoadImageDocument.ControlId = Convert.ToDecimal(FilingSC?.DocumentId);
                LoadImageDocument.ArchivoBytes = FileDataFinal.Base64Data;
                LoadImageDocument.Ext = FileDataFinal.Extension?.Replace(".", "");
                LoadImageDocument.CompanyId = 17;

                var responseApi = await HttpClient!.PostAsJsonAsync("documents/Document/LoadImageDocument", LoadImageDocument);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<bool>>();
                if (deserializeResponse!.Succeeded)
                {
                    Disabled = true;
                    BtnText = "¡"+Translation["ConfirmedImage"]+"!";
                    uploadPDF = "d-none";
                    FilingSC.Verify = true;
                    NotificationModal.UpdateModal(ModalType.Success, Translation["UploadSuccessfulMessage"], true, Translation["Accept"], "", Translation["ConfirmedImage"], "");
                    await OnChangeData.InvokeAsync(true);
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else
                {
                    Disabled = false;
                    BtnText = Translation["ConfirmImageCorrect"];
                    uploadPDF = "";
                    FilingSC.Verify = false;
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al asignar la imagen: {ex.Message}");
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                throw;
            }


        }

        private async Task HandleUploadVolumeRecord()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);


                UploadVolumeImageDtoRequest.Archive = Convert.ToBase64String(FileDataFinal.Base64Data);
                UploadVolumeImageDtoRequest.User = "JohanC"; //To-do cambiar por usuario de la sesión

                var responseApi = await HttpClient!.PostAsJsonAsync("records/Volume/UploadImageVolume", UploadVolumeImageDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<bool>>();
                if (deserializeResponse!.Succeeded)
                {
                    Disabled = true;
                    BtnText = "¡" + Translation["ConfirmedImage"] + "!";
                    uploadPDF = "d-none";
                    FilingSC.Verify = true;
                    NotificationModal.UpdateModal(ModalType.Success, Translation["UploadSuccessfulMessage"], true, Translation["Accept"], "", Translation["ConfirmedImage"], "");
                    await OnChangeData.InvokeAsync(true);
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else
                {
                    Disabled = false;
                    BtnText = Translation["ConfirmedImage"];
                    uploadPDF = "";
                    FilingSC.Verify = false;
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
        public void ReceiveRecord(string DocumentId)
        {

        }
        public void UpdateModalStatus(bool newValue)
        {
            if (NewFilig)
            {
                Disabled = false;
                BtnText = Translation["ConfirmImageCorrect"];
                uploadPDF = "";
                FilingSC.Verify = false;
                ViewPDF = "d-none";
                FileData = null;
                FileDataFinal = new();
                ModalStatus = newValue;
                StateHasChanged();
            }
            else
            {
                ModalStatus = newValue;
                StateHasChanged();
            }
        }
        #endregion

        #region OthersMethods
        public string GetUploadMessage()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            string extensions = string.Join(" " + Translation["Or"] +" ", AllowedExtensions).Replace(".", "").ToUpper();
            SpinnerLoaderService.HideSpinnerLoader(Js);
            
            return string.Format(Translation["AllowedFormatFiles"], extensions, FileSize);
        }
        public void GetVolumenId(int volumeId)
        {
            UploadVolumeImageDtoRequest.VolumeId = volumeId;
        }

        #endregion

        #endregion

    }
}
