using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Pages.Administration.AdministracionTRD
{
    public partial class DocumentalVersionPage
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

        #endregion Inject

        #region Modals

        private DocumentalVersionModal modalDocumentalVersion = new();
        private AdministrativeActModal modalAdministrativeAct = new();
        private NotificationsComponentModal notificationModal = new();

        #endregion Modals

        #region Models

        private PaginationInfo paginationInfo = new();
        private DocumentalVersionFilterDtoRequest? filterDtoRequest { get; set; } = new();
        private NewPaginationComponent<DocumentalVersionDtoResponse, DocumentalVersionFilterDtoRequest> paginationComponent = new();
        private DocumentalVersionDtoResponse versionToDelete = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string UriFilterDocumentalVersion = "paramstrd/DocumentalVersions/ByFilter";
        private string versionTypeFilter { get; set; } = "";
        private string codeFilter { get; set; } = "";
        private string NameFilter { get; set; } = "";
        private string ResponseRecordClosed = "";

        #endregion Environments(String)

        #region Environments(DateTime)

        private DateTime? startDateFilter { get; set; }
        private DateTime? endDatefilter { get; set; }
        private DateTime minValueTo { get; set; } = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime maxValueTo { get; set; } = DateTime.Now;

        #endregion Environments(DateTime)

        #region Environments(Bool)

        private bool currentVersionFilter { get; set; } = false;
        public bool CancelExport { get; set; }
        private bool ExportAllPages { get; set; }

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        public List<DocumentalVersionDtoResponse> documentalVersionsList { get; set; } = new();
        public List<DocumentalVersionsFilterExcelDtoResponse> documentalVersionsExcelList { get; set; } = new();

        private string[] documentalType = { "TVD", "TRD" };
        private List<string> optionsRecordClosed;

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetDocumentalVersion();
            optionsRecordClosed = new List<string>() { Translation["Yes"], Translation["No"] };
            ResponseRecordClosed = Translation["No"];
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleMethods

        #region OthersMethods

        #region Modal

        private void ShowModal()
        {
            modalDocumentalVersion.UpdateModalStatus(true);
        }

        private void ShowModalEdit(DocumentalVersionDtoResponse record)
        {
            modalDocumentalVersion.ReceiveDocumentalVersion(record);
            modalDocumentalVersion.UpdateModalStatus(true);
        }

        private async Task ShowModalAdminstrativeAct(DocumentalVersionDtoResponse record)
        {
            modalAdministrativeAct.UpdateModalStatus(true);
            await modalAdministrativeAct.ReceiveDocumentalVersionAsync(record.DocumentalVersionId);
        }

        #endregion Modal

        #region DeleteDocumentalVersion

        private void HandleRecordToDelete(DocumentalVersionDtoResponse record)
        {
            versionToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, Translation["Yes"], Translation["No"], modalOrigin: "DeleteModal");
        }

        private async Task HandleModalNotiCloseAsync(ModalClosedEventArgs args)
        {
            try
            {
                if (args.IsAccepted && args.ModalOrigin!.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new() { Id = versionToDelete.DocumentalVersionId, User = "user" };

                    var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/DocumentalVersions/DeleteDocumentalVersion", request);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse!.Succeeded)
                    {
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            await HandleRefreshGridData(true);
                            notificationModal.UpdateModal(ModalType.Success, Translation["DeleteSuccessfulMessage"], true, Translation["Accept"]);
                        }
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true, Translation["Accept"]);
                    }
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true);
            }
        }

        #endregion DeleteDocumentalVersion

        #region GetDocumentalVersion

        private async Task GetDocumentalVersion()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                //filterDtoRequest = new();
                filterDtoRequest = new() { OrderBy = "startDate", OrderAsc = false };
                var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterDocumentalVersion, filterDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<DocumentalVersionDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                {
                    documentalVersionsList = deserializeResponse.Data.Data ?? new();
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                }
                else
                {
                    documentalVersionsList = new();
                    notificationModal.UpdateModal(ModalType.Error, Translation["LoadDocumentalVersionErrorMessage"], true);
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadDocumentalVersionErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetDocumentalVersion

        #region RefreshGrid

        private async Task HandleRefreshGridData(bool refresh)
        {
            await GetDocumentalVersion();
        }

        #endregion RefreshGrid

        #region HandlePagination

        private void HandlePaginationGrid(List<DocumentalVersionDtoResponse> newDataList)
        {
            documentalVersionsList = newDataList;
        }

        #endregion HandlePagination

        #region HandleFile

        #region GetFile

        private async Task<FileDtoResponse?> GetFile(int? id)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", $"{id}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                if (deserializeResponse!.Succeeded)
                {
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                    return deserializeResponse.Data!;
                }
                else
                {
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                    return null;
                }
            }
            catch
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                return null;
            }
        }

        #endregion GetFile

        #region DownloadFile

        private async Task DownloadFile(int? fileId)
        {
            if (fileId != 0 && fileId != null)
            {
                FileDtoResponse objFile = await GetFile(fileId);
                string nameFile = "";
                if (!objFile!.FileName.Contains($".{objFile.FileExt}"))
                {
                    nameFile = $"{objFile!.FileName}.{objFile.FileExt}";
                }
                else
                {
                    nameFile = objFile!.FileName;
                }

                var Data = Convert.ToBase64String(objFile.DataFile);

                bool download = await Js.InvokeAsync<bool>("DescargarArchivoBase64", nameFile, Data);
                if (download)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["DownloadOrganizationChartSuccessfullyMessage"], true, Translation["Accept"], title: Translation["DownloadSuccessfullyMessage"]);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["DownloadOrganizationChartErrorMessage"], true, Translation["Accept"], "", "", "");
                }
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["DownloadOrganizationChartErrorMessage"], true, Translation["Accept"], "", "", "");
            }
        }

        #endregion DownloadFile

        #endregion HandleFile

        #region Filters

        #region ApplyFilters

        private async Task ApplyFiltersAsync()
        {//&& ResponseRecordClosed == Translation["No"]
            if (string.IsNullOrEmpty(versionTypeFilter) && string.IsNullOrEmpty(codeFilter) && string.IsNullOrEmpty(NameFilter) && !currentVersionFilter && startDateFilter == null && endDatefilter == null && (ResponseRecordClosed == Translation["No"] && documentalVersionsList != null && documentalVersionsList.Count > 1))
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["SelectOneFilter"], true, Translation["Accept"]);
                await GetDocumentalVersion();
            }
            else
            {
                try
                {
                    DocumentalVersionFilterDtoRequest request = new() { VersionType = versionTypeFilter, Code = codeFilter, Name = NameFilter, CurrentVersion = currentVersionFilter, StartDate = (startDateFilter != null ? startDateFilter.Value.Date : startDateFilter), EndDate = (endDatefilter != null ? endDatefilter.Value.Date : endDatefilter), OrderBy = "startDate", OrderAsc = false };
                    filterDtoRequest = request;
                    var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterDocumentalVersion, filterDtoRequest);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<DocumentalVersionDtoResponse>>>();

                    if (deserializeResponse!.Succeeded && deserializeResponse.Data != null && deserializeResponse.Data.TotalCount > 0)
                    {
                        documentalVersionsList = deserializeResponse.Data.Data ?? new();
                        paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                        paginationComponent.ResetPagination(paginationInfo);
                    }
                    else
                    {
                        versionTypeFilter = "";
                        codeFilter = "";
                        NameFilter = "";
                        ResponseRecordClosed = Translation["No"];
                        currentVersionFilter = false;
                        startDateFilter = null;
                        endDatefilter = null;
                        minValueTo = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        maxValueTo = DateTime.Now;
                        notificationModal.UpdateModal(ModalType.Error, Translation["NoCoincidences"], true, Translation["Accept"]);
                        await GetDocumentalVersion();
                    }
                }
                catch
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["ErrorFiltering"], true, Translation["Accept"]);
                }
            }
            StateHasChanged();
        }

        #endregion ApplyFilters

        #region ResetFilter

        public async Task ResetFiltersAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            if (!string.IsNullOrEmpty(versionTypeFilter) || !string.IsNullOrEmpty(codeFilter) || !string.IsNullOrEmpty(NameFilter) || currentVersionFilter || startDateFilter != null || endDatefilter != null)
            {
                versionTypeFilter = "";
                codeFilter = "";
                NameFilter = "";
                ResponseRecordClosed = Translation["No"];
                currentVersionFilter = false;
                startDateFilter = null;
                endDatefilter = null;
                minValueTo = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                maxValueTo = DateTime.Now;
                await GetDocumentalVersion();
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["AtLeastOne4Clean"], true, Translation["Accept"]);
            }

            ExportAllPages = false;
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion ResetFilter

        #endregion Filters

        #region HandleDatePicker

        #region updateMinValue

        public void updateMinValue()
        {
            if (startDateFilter != null)
            {
                minValueTo = (DateTime)startDateFilter;
            }
            StateHasChanged();
        }

        #endregion updateMinValue

        #region updateMaxValue

        public void updateMaxValue()
        {
            if (endDatefilter != null)
            {
                maxValueTo = (DateTime)endDatefilter;
            }
            StateHasChanged();
        }

        #endregion updateMaxValue

        #endregion HandleDatePicker

        #region OnBeforeExcelExport

        public async Task GetDataExcel()
        {
            var request = new DocumentalVersionFilterDtoRequest() { VersionType = versionTypeFilter, Code = codeFilter, Name = NameFilter, CurrentVersion = currentVersionFilter, StartDate = (startDateFilter != null ? startDateFilter.Value.Date : startDateFilter), EndDate = (endDatefilter != null ? endDatefilter.Value.Date : endDatefilter), OrderBy = "startDate", OrderAsc = false };

            var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/DocumentalVersions/ByFilterExcel", request);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<DocumentalVersionsFilterExcelDtoResponse>>>();

            if (deserializeResponse!.Succeeded)
            {
                documentalVersionsExcelList = deserializeResponse.Data!;
            }
            else
            {
                documentalVersionsExcelList = new();
            }
        }

        public async Task OnBeforeExcelExport(GridBeforeExcelExportEventArgs args)
        {
            if (ExportAllPages)
            {
                await GetDataExcel();

                if (documentalVersionsExcelList.Any())
                {
                    args.Data = documentalVersionsExcelList;
                }
            }
            else { args.Data = documentalVersionsList; }

            args.IsCancelled = CancelExport;
        }

        #endregion OnBeforeExcelExport

        #endregion OthersMethods

        #region OnValueChangedRecordClosed

        private void OnValueChangedRecordClosed(string selectedValue)
        {
            ResponseRecordClosed = selectedValue;
            if (ResponseRecordClosed == Translation["Yes"])
            {
                currentVersionFilter = true;
            }
            else if (ResponseRecordClosed == Translation["No"])
            {
                currentVersionFilter = false;
            }
        }

        #endregion OnValueChangedRecordClosed

        #endregion Methods
    }
}