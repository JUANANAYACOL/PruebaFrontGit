using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Diagnostics;
using System.IO;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Pages.Security
{
    public partial class UserTraceabilityPage
    {

		#region Variables

		#region Inject 
		[Inject]
		private EventAggregatorService? EventAggregator { get; set; }

		[Inject]
		private HttpClient? HttpClient { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }

        #endregion

        #region Components

        private NewPaginationComponent<DocumentDtoResponse, DocumentUserQueryFilter> paginationComponent = new();

        #endregion

        #region Modals

        private GenericSearchModal genericSearchModal = new();
        private NotificationsComponentModal notificationModal = new();
        private GenericUserSearchModal genericUserSearch = new();

        #endregion

        #region Parameters


        #endregion

        #region Models

        private RetunUserSearch vUserSelected = new();
        private DocumentUserQueryFilter filtro { get; set; } = new();
        private PaginationInfo paginationInfo = new();
        private FileDtoResponse fileInfo = new();

        #endregion

        #region Environments

        #region Environments(String)

        private string UriFilterDocs = "documentmanagement/Document/GetInformationDocumentUser";
        private string ResponseRecordClosed = "No";

        #endregion

        #region Environments(Numeric)

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool viewUser = false;
        private bool viewList = false;
        private bool processDocumentValid = false;
        private bool UserModalStatus = false;

        #endregion

        #region Environments(List & Dictionary)

        private List<DocumentDtoResponse> documentList = new();
        private List<string> optionsRecordClosed = new List<string>() { "Sí", "No" };

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

        private void ShowUsersModal(bool value)
        {
            //genericSearchModal.UpdateModalStatus(true);
            UserModalStatus = true;
            viewList = false;
        }

        private void HandleUserChanged(List<RetunUserSearch> user)
        {
            vUserSelected = user[0];
            viewUser = true;
            documentList = new();
            ResponseRecordClosed = "No";
            processDocumentValid = false;
        }

        private void HandlePaginationGrid(List<DocumentDtoResponse> newDataList)
        {
            documentList = newDataList;
        }

        #endregion

        #region OthersMethods

        #region GetInformationDocumentUser

        private async Task GetInformationDocumentUser()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                if(vUserSelected.UserId > 0)
                {
                    filtro.UserId = vUserSelected.UserId;
                    filtro.ProcessDocument = processDocumentValid;
                }
                else { notificationModal.UpdateModal(ModalType.Error, Translation["UserRequired"], true); }

                var responseApi = await HttpClient.PostAsJsonAsync(UriFilterDocs, filtro);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<DocumentDtoResponse>>>();

                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    viewList = true;
                    documentList = deserializeResponse.Data != null ? deserializeResponse.Data.Data.ToList() : new List<DocumentDtoResponse>();
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                }
                else
                {
                    viewList = false;
                    documentList = new();
                    notificationModal.UpdateModal(ModalType.Error, Translation["CannotFindRecords"], true);
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                Console.WriteLine($"Error al obtener tareas documentales: {ex.Message}");
            }
        }

        #endregion GetInformationDocumentUser

        #region DownloadFile
        public async Task DownloadFile(DocumentDtoResponse item)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                if(item.FileId > 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("FileId");
                    HttpClient?.DefaultRequestHeaders.Add("FileId", $"{item.FileId}");
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
                }
                else { notificationModal.UpdateModal(ModalType.Information, Translation["NoImage4Document"], true, Translation["Accept"], buttonTextCancel: ""); }

                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener archivo: {ex.Message}");
            }

        }
        #endregion DownloadFile

        #region ExportExel

        public async Task ExportExel()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                DocumentUserQueryFilter documentFilter = new() 
                {
                    UserId = vUserSelected.UserId,
                    ProcessDocument = processDocumentValid,
                };

                var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/Document/ExcelDocumentUser", documentFilter);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();

                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    string base64Data = deserializeResponse.Data;
                    string nombreArchivo = "Reporte.xlsx";

                    bool download = await Js.InvokeAsync<bool>("DescargarArchivoBase64", nombreArchivo, base64Data);
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

        #endregion ExportExel

        #region ExportDocumentsZip

        public async Task ExportDocumentsZip()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                DocumentUserQueryFilter documentFilter = new()
                {
                    UserId = vUserSelected.UserId,
                    ProcessDocument = processDocumentValid,
                };

                var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/Document/CreateZipArchiveDocumentsAsync", documentFilter);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<byte[]>>();

                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    string base64Data = Convert.ToBase64String(deserializeResponse.Data);
                    string nombreArchivo = "Reporte.zip";

                    bool download = await Js.InvokeAsync<bool>("DescargarArchivoBase64", nombreArchivo, base64Data);
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

        #endregion ExportDocumentsZip

        #region OnValueChangedRecordClosed

        private void OnValueChangedprocessDocument(string newValue)
        {
            ResponseRecordClosed = newValue;
            if (newValue == "Sí")
            {
                processDocumentValid = true;
            }
            else if (newValue == Translation["No"])
            {
                processDocumentValid = false;
            }
        }

        #endregion

        #region resetModal

        public void Reset()
        {
            vUserSelected = new();
            documentList = new();
            viewList = false;
            viewUser = false;
            processDocumentValid = false;
            ResponseRecordClosed = "No";
        }

        #endregion resetModal

        #region CloseModalSearchUser
        private void HandleModalUserClosed(bool status)
        {
            UserModalStatus = status;
            genericUserSearch.RemoveUserSelectedById(vUserSelected.UserId);
        }

        #endregion

        #region ExportToExcel

        public bool CancelExport { get; set; }

        private List<DocumentDtoResponse> allRecords { get; set; }

        bool ExportAllPages { get; set; }

        public async Task GetAllRecords()
        {
            try
            {
                if (vUserSelected.UserId > 0)
                {
                    filtro.UserId = vUserSelected.UserId;
                    filtro.ProcessDocument = processDocumentValid;
                }
                else { notificationModal.UpdateModal(ModalType.Error, Translation["UserRequired"], true); }

                var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/Document/GetInformationExcelDocumentUser", filtro);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<DocumentDtoResponse>>>();

                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    allRecords = deserializeResponse.Data != null ? deserializeResponse.Data.ToList() : new List<DocumentDtoResponse>();
                }
                else
                {
                    allRecords = new();
                    notificationModal.UpdateModal(ModalType.Error, Translation["CannotFindRecords"], true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener tareas documentales: {ex.Message}");
            }
        }

        public async Task OnBeforeExcelExport(GridBeforeExcelExportEventArgs args)
        {

            if (ExportAllPages)
            {
                await GetAllRecords();

                if (allRecords.Any())
                {
                    args.Data = allRecords;
                }
            }
            else { args.Data = documentList; }

            args.IsCancelled = CancelExport;
        }

        #endregion

        #endregion

        #endregion

    }
}
