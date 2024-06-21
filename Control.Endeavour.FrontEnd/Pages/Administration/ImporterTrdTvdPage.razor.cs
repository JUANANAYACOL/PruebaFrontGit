using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.ImporterTrdTvd;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ImporterTrdTvd.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ImporterTrdTvd.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class ImporterTrdTvdPage
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



        #region Components

        private NewPaginationComponent<ImporterDtoResponse, ImporterDtoRequest> paginationComponent = new();

        #endregion Components

        #region Modals

        private NotificationsComponentModal notificationModal = new();
        private ImporterTrdTvdModal modalImporterTrdTvd = new();
        private ImportResultModal modalImportResult = new();

        #endregion Modals



        #region Models
        private ImporterHistoryDtoRequest? FilterDtoRequest { get; set; } = new();
        private NewPaginationComponent<ImporterHistoryDtoResponse, ImporterHistoryDtoRequest> PaginationComponet = new();
        private PaginationInfo paginationInfo = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string UriFilterImporterHistory = "paramstrd/ImporterHistory/ByFilter";

        #endregion Environments(String)

        #region Environments(Numeric)

        private bool dataChargue { get; set; } = false;

        #endregion Environments(Numeric)



        #region Environments(Bool)
        public bool CancelExport { get; set; }
        private bool ExportAllPages { get; set; }

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<ImporterHistoryDtoResponse> importerHistoryList = new();
        private List<ImporterHistoryDtoResponse> importerHistoryExcelList = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            ////EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetImporterHistory();
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

        #region GetImporterHistory

        private async Task GetImporterHistory()
        {
            try
            {
                var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterImporterHistory, FilterDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<ImporterHistoryDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                {
                    importerHistoryList = deserializeResponse.Data.Data ?? new();
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    dataChargue = true;
                    PaginationComponet.ResetPagination(paginationInfo);
                }
                else
                {
                    importerHistoryList = new();
                    paginationInfo = new();
                    paginationComponent.ResetPagination(paginationInfo);
                    dataChargue = false;
                }
            }
            catch
            {
                importerHistoryList = new();
                dataChargue = false;
                notificationModal.UpdateModal(ModalType.Error, Translation["ErrorGettingRecords"], true);
            }
        }

        #endregion GetImporterHistory

        #region HandlePagination

        private void HandlePaginationGrid(List<ImporterHistoryDtoResponse> newDataList)
        {
            importerHistoryList = newDataList;
        }

        #endregion HandlePagination

        #region HandleStatusChanged

        private async Task HandleStatusChangedAsync(bool status)
        {
            modalImporterTrdTvd.UpdateModalStatus(status);
            if (!status)
            {
                await Task.FromResult(GetImporterHistory);
            }
        }

        #endregion HandleStatusChanged

        #region ShowModal

        private void ShowModal()
        {
            modalImporterTrdTvd.UpdateModalStatus(true);
        }

        #endregion ShowModal

        #region OpenNewModal

        private void OpenNewModal(ImporterDtoResponse response)
        {
            modalImportResult.GetData(response);
            modalImportResult.UpdateModalStatus(true);
        }

        #endregion OpenNewModal

        #region HandleRefreshGridData

        private async Task HandleRefreshGridData(bool refresh)
        {
            await GetImporterHistory();
        }

        #endregion HandleRefreshGridData

        #region HandleModalClosed

        private async Task HandleModalClosedAsync(bool status)
        {
            modalImporterTrdTvd.UpdateModalStatus(status);
            modalImportResult.UpdateModalStatus(status);
            modalImporterTrdTvd.ResetFormAsync();
            if (!status)
            {
                await Task.FromResult(GetImporterHistory);
            }
            StateHasChanged();
        }

        #endregion HandleModalClosed

        #region DownloadFile

        private async Task DownloadFile(ImporterHistoryDtoResponse file)
        {
            var fileId = file.FileId;
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
                    notificationModal.UpdateModal(ModalType.Success, Translation["FileDownloadSuccessfully"], true, Translation["Accept"], title: Translation["DownloadSuccessfullyMessage"]);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["DownloadFileErrorMessage"], true, Translation["Accept"], "", "", "");
                }
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["DownloadFileErrorMessage"], true, Translation["Accept"], "", "", "");
            }
        }

        #endregion DownloadFile

        #region GetFile

        private async Task<FileDtoResponse?> GetFile(int? id)
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", $"{id}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                if (deserializeResponse!.Succeeded)
                {
                    return deserializeResponse.Data!;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        #endregion GetFile

        #region DownloadTemplate

        private async Task DownloadTemplate()
        {
            var fileId = 10333; //Id de la template del importador

            FileDtoResponse objFile = await GetFile(fileId);
            string nameFile = "";
            if (!objFile.FileName.Contains($".{objFile.FileExt}"))
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
                notificationModal.UpdateModal(ModalType.Success, Translation["TemplateDownloadSuccessfully"], true, Translation["Accept"], title: "¡Descarga exitosa!");
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["DownloadTemplateErrorMessage"], true, Translation["Accept"], "", "", "");
            }
        }

        #endregion DownloadTemplate

        #region OnBeforeExcelExport

        public async Task GetDataExcel()
        {
            var request = new ImporterHistoryDtoRequest();

            var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/ImporterHistory/ByFilterExcel", request);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<ImporterHistoryDtoResponse>>>();

            if (deserializeResponse!.Succeeded)
            {
                importerHistoryExcelList = deserializeResponse.Data!;
            }
            else
            {
                importerHistoryExcelList = new();
            }
        }

        public async Task OnBeforeExcelExport(GridBeforeExcelExportEventArgs args)
        {
            if (ExportAllPages)
            {
                await GetDataExcel();

                if (importerHistoryExcelList.Any())
                {
                    UpdateColumnTitles(args);
                    AppendRecordsCreated(importerHistoryExcelList);
                    args.Data = importerHistoryExcelList;
                }
            }
            else
            {
                UpdateColumnTitles(args);
                AppendRecordsCreated(importerHistoryList);
                args.Data = importerHistoryList;
            }

            args.IsCancelled = CancelExport;
        }

        private void UpdateColumnTitles(GridBeforeExcelExportEventArgs args)
        {
            args.Columns.ForEach(x =>
            {
                if (x.Title == nameof(ImporterHistoryDtoResponse.RecordsImported))
                {
                    x.Title = Translation["RecordsCreated"];
                }
            });
        }

        private void AppendRecordsCreated(IEnumerable<ImporterHistoryDtoResponse> historyList)
        {
            foreach (var item in historyList)
            {
                var recordsCreated = new StringBuilder()
                    .AppendLine($"{Translation["CreatedUnits"]}: {item.AdministrativeUnitsCount}")
                    .AppendLine($"{Translation["CreatedOffices"]}: {item.ProductionOfficesCount}")
                    .AppendLine($"{Translation["CreatedSeries"]}: {item.SeriesCount}")
                    .AppendLine($"{Translation["CreatedSubSeries"]}: {item.SubSeriesCount}")
                    .AppendLine($"{Translation["CreatedWithholdings"]}: {item.RetentionsCount}")
                    .AppendLine($"{Translation["CreatedTypologies"]}: {item.DocumentaryTypologiesBagCount}")
                    .AppendLine($"{Translation["CreatedDRT"]}: {item.TRDCount}")
                    .AppendLine($"{Translation["CreatedDRTB"]}: {item.TRDCCount}");

                item.RecordsImported = recordsCreated.ToString();
            }
        }

        #endregion OnBeforeExcelExport

        #endregion OthersMethods

        #endregion Methods
    }
}