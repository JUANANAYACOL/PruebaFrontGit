using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Documents;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.IO;
using System.Net.Http.Json;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Components.Modals.Generic.ViewDocuments
{
    public partial class ViewDocumentModal
    {

		#region Variables

		#region Inject

		/*[Inject]
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

        #endregion

        #region Parameters

        [Parameter]
        public EventCallback<MyEventArgs<int>> OnStatusChanged { get; set; }

        #endregion

        #region Models

        public TelerikPdfViewer? PdfViewerRef { get; set; }
        private FileDtoResponse fileInfo = new();

        #endregion

        #region Environments

        #region Environments(String)

        #endregion

        #region Environments(Numeric)

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool modalStatus = false;

        #endregion

        #region Environments(List & Dictionary)

        public byte[] FileData = Array.Empty<byte>();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            ////EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
		{
			StateHasChanged();
        }

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
        }

        #endregion

        #region OthersMethods

        #region ShowPdf

        private async Task ShowDocument(int id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", $"{id}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                fileInfo = deserializeResponse.Data;

                if (fileInfo != null)
                {
                    FileData = fileInfo.DataFile;
                }
                else
                {

                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener archivo: {ex.Message}");
            }
        }

        #endregion

        #region DownloadFile
        public async Task DownloadFile(int id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", $"{id}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                
                if (deserializeResponse.Succeeded)
                {
                    fileInfo = deserializeResponse.Data;
                    string nombreArchivo = fileInfo.FileName + "." + fileInfo.FileExt;

                    bool download = await Js.InvokeAsync<bool>("DescargarArchivoBase64", nombreArchivo, Convert.ToBase64String(fileInfo.DataFile));
                    if (download)
                    {
                        notificationModal.UpdateModal(ModalType.Success, Translation["FileDownloadSuccessfully"], true, Translation["Accept"], title: Translation["DownloadSuccessfullyMessage"]);
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["DownloadFileErrorMessage"], true, Translation["Accept"], "", "", "");
                    }
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener archivo: {ex.Message}");
            }

        }
        #endregion

        #region DataContainer

        public void DataContainer(string data)
        {
            if(data != null)
            {
                FileData = Convert.FromBase64String(data);
            }
        }

        #endregion

        #endregion

        #endregion

    }
}
