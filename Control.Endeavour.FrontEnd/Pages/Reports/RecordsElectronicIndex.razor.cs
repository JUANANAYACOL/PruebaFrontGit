using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Models.Models.Records.Request;
using Control.Endeavour.FrontEnd.Models.Models.Records.Response;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Globalization;
using Telerik.Blazor.Components;
using Telerik.ReportViewer.BlazorNative;
using Telerik.SvgIcons;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.MetaData;
using Control.Endeavour.FrontEnd.Components.Modals.Records;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using System.Xml;

namespace Control.Endeavour.FrontEnd.Pages.Reports
{

    public partial class RecordsElectronicIndex
    {
        #region Environments(String)
        private string PanelConsultRecordClass = "";
        private string PanelReportsClass = "d-none";
        private string PanelViewRecordClass = "d-none";
        private string DocumentFilterClass = "d-none";
        private string ResponseRecordClosed = "No";
        private string UriFilterRecords = "records/Records/Filter";
        private string ButtonCreateRecordClass = "d-none";
        #endregion
        #region Environments(Numeric)

        private int AdministrativeUnitId = 0;
        private int ProductionOfficeId = 0;
        private int idDocVersion = 0;
        private int currentDocumentalVersionID = 0;
        private int recordVolumenId = 0;
        private int ActiveTabIndex = 0;

        #endregion
        #region Environments(Bool)

        private bool isEnableProOffice = false;
        private bool isEnableSerie = false;
        private bool isEnableSubSerie = false;
        private bool isEnableAdminUnit = false;
        #endregion
        #region Environments(List & Dictionary)

        #region ConsultPage
        private List<AdministrativeUnitsDtoResponse> adminUnitList = new();
        private List<ProductionOfficesDtoResponse> proOfficesList = new();
        private List<SubSeriesDtoResponse> subSeriesList = new();
        private List<SeriesDtoResponse> seriesList = new();
        private List<VSystemParamDtoResponse> recordTypeList = new();
        private List<VSystemParamDtoResponse> recordFileTypeList = new();
        private List<VRecordsDtoResponse> recordsInfoList = new();
        private List<string> optionsRecordClosed = new List<string>() { "Sí", "No" };
        private List<DocumentalVersionDtoResponse> docVersionList { get; set; } = new();
        #endregion

        public byte[]? FileData { get; set; }
        private List<RecordHistoryDtoResponse> recordHistoryList { get; set; } = new();

        #endregion
        #region Modals

        private NotificationsComponentModal notificationModal = new();
        private IndexDocumentaryTypologyModal indexDocumentaryTypologyModal = new();

        #endregion
        #region Models

        private RecordFilterDtoRequest recordFilter = new();
        private PaginationInfo paginationInfo = new();
        private MetaDataFilterDtoRequest filterRequest = new();

        #endregion
        #region Inject
        [Inject]
        private HttpClient? HttpClient { get; set; }
        [Inject]
        private IJSRuntime Js { get; set; }
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion
        #region Components
        NewPaginationComponent<VRecordsDtoResponse, RecordFilterDtoRequest> paginationComponent = new();
        #endregion
        #region report
        public ScaleMode ScaleMode { get; set; } = ScaleMode.Specific;

        public ViewMode ViewMode { get; set; } = ViewMode.Interactive;

        public bool ParametersAreaVisible { get; set; }
        public bool DocumentMapVisible { get; set; }

        public double Scale { get; set; } = 1.0;

        public ReportSourceOptions ReportSource { get; set; }
        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetDocumentalVersions();
            await GetRecordFileType();
            await GetRecordType();
            await GetRecords();
            optionsRecordClosed = new List<string>() { Translation["Yes"], Translation["No"] };
            ResponseRecordClosed = Translation["No"];
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }


        #endregion

        private void OpenReport(VRecordsDtoResponse record)
        {

            PanelConsultRecordClass = "d-none";
            PanelReportsClass = "";
            ButtonCreateRecordClass = "";
            //ReportSource = new ReportSourceOptions("ElectronicIndex.trdp", new Dictionary<string, object> {});
            ReportSource = new ReportSourceOptions("ElectronicIndex.trdp", new Dictionary<string, object> {
                {"RecordId", record.RecordId },
                {"CompanyName", "ACME" },
                { "NIT", "1240642363694"}
            });

            //ReportSource.Parameters["RecordId"] = record.RecordId;
            //ReportSource.Parameters["CompanyName"] = "ACME";
            //ReportSource.Parameters["NIT"] = "1240642363694";
        }
        private void CloseCreateView()
        {
            PanelConsultRecordClass = string.Empty;
            ButtonCreateRecordClass = "d-none";
            PanelReportsClass = "d-none";




        }
        private void OnValueChangedRecordClosed(string newValue)
        {
            ResponseRecordClosed = newValue;
            if (newValue == Translation["Yes"])
            {
                recordFilter.ActiveState = false;
            }
            else if (newValue == Translation["No"])
            {
                recordFilter.ActiveState = true;
            }
        }
        private async Task FetchAdministrativeUnits(int id, string context, bool setFlagToTrue)
        {

            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");
                HttpClient?.DefaultRequestHeaders.Add("documentalVersionsId", $"{id}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeUnitsDtoResponse>>>("paramstrd/AdministrativeUnit/ByAdministrativeUnits");
                HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");

                if (deserializeResponse!.Succeeded && (deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null))
                {
                    if (setFlagToTrue)
                        isEnableAdminUnit = true;
                    adminUnitList = deserializeResponse.Data!;
                }
            }
            catch (Exception ex)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }

        }

        #region Getters
        private async Task GetRecordType()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "TEXP");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    recordTypeList = deserializeResponse.Data;
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else
                {
                    recordTypeList = new();
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }

            }
            catch (Exception)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }
        private async Task GetRecordFileType()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "TAEXP");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    recordFileTypeList = deserializeResponse.Data;
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else
                {
                    recordFileTypeList = new();
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }

            }
            catch (Exception)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }
        public async Task GetDocumentalVersions()
        {
            try
            {
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<DocumentalVersionDtoResponse>>>("paramstrd/DocumentalVersions/ByDocumentalVersions");
                if (deserializeResponse!.Succeeded)
                {
                    docVersionList = deserializeResponse.Data!;
                    string toCurrent = Translation["ToCurrent"];
                    string documentaryVersion = Translation["DocumentaryVersion"];
                    docVersionList.ForEach(x => {
                        x.Code = $"{((x.EndDate == null || x.EndDate.Value > DateTime.Now) ? $"{x.StartDate!.Value.Year} {toCurrent} - {documentaryVersion} {x.Code}" : $"{x.StartDate!.Value.Year} a {x.EndDate.Value.Year} - {documentaryVersion} {x.Code} {x.VersionType}")}";
                    });
                    docVersionList = docVersionList.OrderBy(x => x.DocumentalVersionId).Reverse().ToList();
                    currentDocumentalVersionID = docVersionList.FirstOrDefault(x => x.EndDate == null || x.EndDate.Value > DateTime.Now).DocumentalVersionId;
                    await GetAdministrativeUnits(currentDocumentalVersionID, "create");
                }
            }
            catch (Exception ex)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }
        private async Task GetAdministrativeUnits(int id, string context)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                if (id != 0)
                {
                    switch (context)
                    {
                        case "consult":
                            await FetchAdministrativeUnits(id, "consult", true);
                            idDocVersion = id;
                            break;
                        case "create":
                            await FetchAdministrativeUnits(currentDocumentalVersionID, "create", false);
                            break;
                    }
                }
                else
                {
                    idDocVersion = 0;
                    isEnableAdminUnit = false;
                    isEnableSubSerie = false;
                    isEnableSerie = false;
                    isEnableProOffice = false;
                    proOfficesList = new();
                    adminUnitList = new();
                    seriesList = new();
                    subSeriesList = new();
                }

                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }

        private async Task GetProducOffice(int id, string context)
        {
            try
            {
                if (id != 0)
                {
                    isEnableSubSerie = false;
                    isEnableSerie = false;
                    isEnableProOffice = false;
                    proOfficesList = new();
                    seriesList = new();
                    subSeriesList = new();
                    SpinnerLoaderService.ShowSpinnerLoader(Js);

                    HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                    HttpClient?.DefaultRequestHeaders.Add("AdministrativeUnitId", $"{id}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<ProductionOfficesDtoResponse>>>("paramstrd/ProductionOffice/ByProductionOffices");
                    if (deserializeResponse!.Succeeded && (deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null))
                    {
                        proOfficesList = deserializeResponse.Data!;
                        isEnableProOffice = true;
                        recordFilter.AdministrativeUnitId = id;
                    }
                    HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else
                {
                    isEnableSubSerie = false;
                    isEnableSerie = false;
                    seriesList = new();
                    subSeriesList = new();
                    recordFilter.AdministrativeUnitId = id;
                }

            }
            catch (Exception)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }

        private async Task GetSeries(int id, string context)
        {
            try
            {
                if (id != 0)
                {
                    isEnableSubSerie = false;
                    subSeriesList = new();
                    seriesList = new();
                    SpinnerLoaderService.ShowSpinnerLoader(Js);
                    HttpClient?.DefaultRequestHeaders.Remove("productionOfficeId");
                    HttpClient?.DefaultRequestHeaders.Add("productionOfficeId", $"{id}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<SeriesDtoResponse>>>("paramstrd/Series/BySeries");
                    if (deserializeResponse!.Succeeded && (deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null))
                    {
                        seriesList = deserializeResponse.Data!;
                        isEnableSerie = true;
                        recordFilter.ProductionOfficeId = id;
                    }
                    HttpClient?.DefaultRequestHeaders.Remove("productionOfficeId");

                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }

            }
            catch (Exception)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }

        public async Task GetSubSeries(int id, string context, bool loadMetaData = true)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("seriesId");
                HttpClient?.DefaultRequestHeaders.Add("seriesId", $"{id}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<SubSeriesDtoResponse>>>("paramstrd/SubSeries/BySubSeries");
                HttpClient?.DefaultRequestHeaders.Remove("seriesId");
                if (deserializeResponse!.Succeeded && (deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null))
                {
                    recordFilter.SeriesId = id;
                    subSeriesList = deserializeResponse.Data!;
                    isEnableSubSerie = true;
                    if (subSeriesList.Count() == 0)
                    {
                        notificationModal.UpdateModal(ModalType.Information, Translation["MustSelectSubSeries"], true, buttonTextCancel: "");
                        isEnableSubSerie = true;
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Information, Translation["FilterMetaDataQuestion"], true, modalOrigin: "MetaDataBySeries");

                    }
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }

        private async Task GetRecords()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                recordFilter.OrderAsc = false;
                recordFilter.OrderBy = "RecordId";
                var responseApi = await HttpClient.PostAsJsonAsync(UriFilterRecords, recordFilter);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<VRecordsDtoResponse>>>();

                if (deserializeResponse.Succeeded && deserializeResponse.Data.Data != null)
                {
                    recordsInfoList = deserializeResponse.Data.Data;
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                    SpinnerLoaderService.HideSpinnerLoader(Js);


                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }

        private async Task GetRecordById(string id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("Number");
                HttpClient?.DefaultRequestHeaders.Add("Number", id);
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<VRecordsDtoResponse>>("records/Records/FilterByNumber");
                HttpClient?.DefaultRequestHeaders.Remove("Number");

                if (deserializeResponse!.Succeeded)
                {
                    recordsInfoList = new()
                    {
                        deserializeResponse.Data
                    };
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else
                {
                    recordTypeList = new();
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }

            }
            catch (Exception)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }
        #endregion
        public async Task SelectedSubSeries(int id, string context, bool loadMetaData = true)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                recordFilter.SubSeriesId = id;
                notificationModal.UpdateModal(ModalType.Information, Translation["FilterByMetaDataSubSeries"], true, modalOrigin: "MetaDataBySubSeries");
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }
        #region Handlers
        private void HandlePaginationGrid(List<VRecordsDtoResponse> newDataList)
        {
            recordsInfoList = newDataList;
        }
        private async Task HandleCleanFilter()
        {
            recordFilter = new();
            isEnableProOffice = false;
            isEnableSerie = false;
            isEnableSubSerie = false;
            await GetRecords();

        }

        private async Task HandleSearchFilter()
        {
            if (!int.TryParse(recordFilter.RecordId, out int recordIdValue))
            {
                await GetRecords();
            }
            else if (recordIdValue <= 0)
            {
                notificationModal.UpdateModal(ModalType.Information, "El RecordId ingresado no es válido. Por favor, ingrese un valor positivo mayor que cero.", true);

            }
            else
            {
                await GetRecordById(recordFilter.RecordId);
            }

        }

        #endregion
    }
}