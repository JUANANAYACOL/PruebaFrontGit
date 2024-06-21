using Control.Endeavour.FrontEnd.Components.Modals.Generic.ViewDocuments;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.IO;
using System.Net.Http.Json;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.DocumentaryTask
{
    public partial class DocTaskWorkFlowModal
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

        private ViewDocumentModal viewDocumentModal = new();

        #endregion

        #region Parameters


        #endregion

        #region Models

        private FileDtoResponse fileInfo = new();

        #endregion

        #region Environments

        #region Environments(String)

        private bool modalStatus = false;

        #endregion

        #region Environments(Numeric)

        private int taskId = 0;

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        #endregion

        #region Environments(List & Dictionary)

        private List<WorkFlowDocumentaryTaskDtoResponse> DocworkFlowList = new();
        private byte[] FileData = Array.Empty<byte>();

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

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        #endregion

        #region OthersMethods

        #region GetWorkFlow
        public async Task GetWorkFlowDocumentaryTask(int id)
        {
            try
            {
                taskId = id;
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("TaskId");
                HttpClient?.DefaultRequestHeaders.Add("TaskId", $"{id}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<WorkFlowDocumentaryTaskDtoResponse>>>("documentarytasks/DocumentaryTask/GetDocumentWorkFlow");
                HttpClient?.DefaultRequestHeaders.Remove("TaskId");

                if (deserializeResponse.Data != null)
                {
                    DocworkFlowList = deserializeResponse.Data;
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las acciones: {ex.Message}");
            }
        }

        #endregion

        #region GetFile
        private async Task GetFile(int id)
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
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener archivo: {ex.Message}");
            }
        }

        #endregion

        #region ViewAttachment

        private async Task HandleViewAttachment(WorkFlowDocumentaryTaskDtoResponse data)
        {
            await GetFile(data.FilePdfId.Value);
            viewDocumentModal.DataContainer(Convert.ToBase64String(FileData));
            viewDocumentModal.UpdateModalStatus(true);
        }

        #endregion

        #endregion

        #endregion

    }
}
