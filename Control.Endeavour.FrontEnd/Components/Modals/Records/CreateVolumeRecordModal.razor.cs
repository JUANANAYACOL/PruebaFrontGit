using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Records.Request;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Components.Modals.Records
{
    public partial class CreateVolumeRecordModal
    {

        #region Variables

        #region Inject 
        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }
        [Inject] private IJSRuntime? Js { get; set; }
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion

        #region Components

        public TelerikPdfViewer PdfViewerRef { get; set; } = new();
        #endregion

        #region Modals

        private NotificationsComponentModal notificationModal = new();

        #endregion

        #region Parameters

        [Parameter] public EventCallback<bool> OnRefreshData { get; set; }

        #endregion

        #region Models

        private VolumeDtoRequest createVolumeRequest = new();

        #endregion

        #region Environments

        #region Environments(String)
        private string recordNumber = string.Empty;
        private string uploadPDF = "";
        private string AlertMessage = "";
        private string ViewPDF = "d-none";
        #endregion

        #region Environments(Numeric)
        private int FileSize = 50;
        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool BtnDisablebStatus = true;

        #endregion

        #region Environments(List & Dictionary)
        private string[] AllowedExtensions { get; set; } = { ".pdf" };
        public byte[]? FileData { get; set; }
        public FileInfoData FileDataFinal = new();
        #endregion

        #endregion
        MarkupString formatMessage;
        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            formatMessage = new MarkupString(Translation["ChoosePDFFileCreateMessage"]);
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }
        private async Task HandleModalClosedAsync(bool status)
        {
            modalStatus = status;
            StateHasChanged();
        }
        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {

        }


        #endregion

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #region HandleFormMethods
        private void ResetForm()
        {
            List<FileInfoData> data = new();
            HandleFile(data);
        }

        private async Task HandleCreateNewVolume()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);


                createVolumeRequest.Archive = Convert.ToBase64String(FileDataFinal.Base64Data);
                createVolumeRequest.User = "JohanC"; //To-do cambiar por usuario de la sesión

                var responseApi = await HttpClient!.PostAsJsonAsync("records/Volume/CreateVolume", createVolumeRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<object>>();
                if (deserializeResponse!.Succeeded)
                {

                    notificationModal.UpdateModal(ModalType.Success, string.Format(Translation["NewVolumeCreatedMessage"], recordNumber), true, Translation["Accept"]);
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                    await OnRefreshData.InvokeAsync(true);
                }
                else
                {

                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            catch (Exception ex)
            {

                notificationModal.UpdateModal(ModalType.Error, Translation["AssigningImageError"], true);
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                throw;
            }
        }

        #endregion

        #region HanldeUploadPDF
        private void HandleFile(List<FileInfoData> data)
        {
            if (data != null && data.Count > 0)
            {
                FileData = data[0].Base64Data;
                FileDataFinal = data[0];
                ViewPDF = "";
                BtnDisablebStatus = false;
            }
            else
            {
                FileData = null;
                ViewPDF = "d-none";
                FileDataFinal = new();
                BtnDisablebStatus = true;
            }
        }


        #endregion
        #endregion

        #region OthersMethods

        #region ModalMethods
        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }
        public void GetDataForCreateVolumen(int recordId, int recordNumberData, int nextTomeNumber)
        {
            createVolumeRequest.RecordId = recordId;
            createVolumeRequest.TomeNumber = nextTomeNumber;
            recordNumber = recordNumberData.ToString();
        }
        #endregion


        #endregion

        #endregion

    }
}
