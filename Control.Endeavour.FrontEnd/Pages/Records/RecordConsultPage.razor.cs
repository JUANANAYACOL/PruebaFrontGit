using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.MetaData;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.Attachments;
using Control.Endeavour.FrontEnd.Components.Modals.Records;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaData.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Models.Models.Records.Request;
using Control.Endeavour.FrontEnd.Models.Models.Records.Response;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using DevExpress.Blazor.Primitives.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using OpenAI_API.Models;
using System.Net.Http.Json;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Pages.Records
{
    public partial class RecordConsultPage
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

        NewPaginationComponent<VRecordsDtoResponse, RecordFilterDtoRequest> paginationComponent = new();
        private int? RecordVolumens { get; set; }

        #endregion

        #region Modals

        private NotificationsComponentModal notificationModal = new();
        private MetaDataRecordsFilter metaDataFilterModal = new();
        private MetaDataValueModal metaDataValueModal = new();
        private GetRecordsVolumesModal getRecordsVolumes = new();
        private IndexDocumentaryTypologyModal indexDocumentaryTypologyModal = new();
        private UploadPDFAutomaticIndexingModal uploadPDFAutomaticIndexingModal = new();
        private AttachmentTrayModal attachmentTrayModal = new();

        #endregion

        #region Parameters


        #endregion

        #region Models

        public TelerikPdfViewer? PdfViewerRef { get; set; }
        private RecordFilterDtoRequest recordFilter = new();
        private CreateRecordDtoRequest createRequest = new();
        private PaginationInfo paginationInfo = new();
        private MetaDataFilterDtoRequest filterRequest = new();
        private VRecordsDtoResponse  createdRecord = new();
        private MetaDataTypologyDtoRequest MetaDataTypology = new();

        #endregion

        #region Environments

        #region Environments(String)

        private string ResponseFilter = "Expedientes";
        private string ResponseRecordClosed = "No";
        private string DocumentFilterClass = "d-none";
        private string UriFilterRecords = "records/Records/Filter";
        private string RecordTitlePage = "Capture";
        private string ButtonCreateRecordClass = "d-none";
        private string PanelConsultRecordClass = string.Empty;
        private string PanelHistoryRecord = "d-none";
        private string PanelHistoryRecordClass = "d-none";
        private string PanelCreateRecordClass = "d-none";
        private string PanelViewRecordClass = "d-none";
        private string RecordVolume = string.Empty;
        private string UriMetaDataFilter = "records/MetaData/MetaDataByFilter";
        private string UriCreateMetaData = "records/Records/CreateRecord";
        private string PanelTableTypologyTDM = "d-none";
        private string ColorText = "";
        private string tableText = "Mostrar Flujo de Trabajo";
        private string ShowPDFIcon = "fa-solid fa-eye";
        private string ShowDashboardIcon = "fa-solid fa-eye";
        private string DisplayPDF = "col-lg-6";
        private string DisplayDashboard = "col-lg-6";
        private string TextPDF = "";
        private string TextDashboard = "";
        private string TitleUploadFileImage = "";
        private string TextMassive = "";
        private string DisplayMetadataTypology = "d-none";
        private string TitleMetadataTypology = "d-none";
        private string ColorMetadataTypology = "";

        #endregion

        #region Environments(Numeric)

        private int AdministrativeUnitId = 0;
        private int ProductionOfficeId = 0;
        private int idDocVersion  = 0;
        private int idDocVersionCreate = 0; 

        private int recordVolumenId = 0;
        private int ActiveTabIndex = 0;
        private decimal _TotalFolios = 0;

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        #region ConsultPage
        private bool isEnableProOffice = false;
        private bool isEnableSerie = false;
        private bool isEnableSubSerie = false;
        private bool isEnableAdminUnit = false;
        #endregion

        #region CreatePage
        private bool visibleBottons = true;
        private bool isEnableProOfficeCreate = false;
        private bool isEnableSerieCreate = false;
        private bool isEnableSubSerieCreate = false;
        #endregion

        private bool ValidDisplayPDFViewer = false;
        private bool ValidDisplayDashboard = false;
        private bool DisabledUploadFileImage = true;
        private bool ValidateAutomaticIndexing = true;

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
        private List<string> optionsFilter = new List<string>() { "Expedientes", "Documentos" };
        private List<string> optionsRecordClosed = new List<string>() { "Sí", "No" };
        private List<DocumentalVersionDtoResponse> docVersionList { get; set; } = new();
        #endregion

        #region CreatePage
        private List<AdministrativeUnitsDtoResponse> adminUnitListCreate = new();
        private List<ProductionOfficesDtoResponse> proOfficesListCreate = new();
        private List<SubSeriesDtoResponse> subSeriesListCreate = new();
        private List<SeriesDtoResponse> seriesListCreate = new();
        private List<VSystemParamDtoResponse> recordTypeListCreate = new();
        private List<VSystemParamDtoResponse> recordFileTypeListCreate = new();
        private List<VMetaDataDtoResponse> metaDataList = new();
        private List<MetaDataTypologyDtoResponse> MetaDataTypologyList = new();

        #endregion

        private List<VolumeDtoResponse> lstRecordVolumens = new();
        private List<DocumentaryTypologyTDMDtoRequest> DocumentarTypologyTDM = new();
        public byte[]? FileData { get; set; }
        private List<RecordHistoryDtoResponse> recordHistoryList { get; set; } = new();

        #endregion

        #endregion

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
            optionsFilter = new List<string>() { Translation["Records"], Translation["Documents"] };
            ResponseFilter = Translation["Records"];
            ResponseRecordClosed = Translation["No"];
            TextPDF = Translation["ExpandPDFViewer"];
            TextDashboard = Translation["ExpandDashboard"];
            TitleUploadFileImage = Translation["UploadFileImage"];
            TextMassive = Translation["Massive"];
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }


        #endregion

        #region Methods

        #region HandleMethods

        #region HandleCreateRecordForm

        private async Task HandleCreateForm()
        {
            try
            {
                createRequest.DocumentalVersionId = idDocVersionCreate;
                createRequest.Volume = Int32.Parse(RecordVolume);
                if (ValidateCreateRecordDtoRequest(createRequest))
                {
                    var responseApi = await HttpClient.PostAsJsonAsync(UriCreateMetaData, createRequest);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<VRecordsDtoResponse>>();
                    if (!deserializeResponse.Succeeded)
                    {
                        notificationModal.UpdateModal(ModalType.Warning, $"¡No se creo el expediente!", true, Translation["Accept"], buttonTextCancel: "");
                    }
                    else
                    {
                        createdRecord = deserializeResponse.Data;
                        notificationModal.UpdateModal(ModalType.Success, $"¡Se creo el expediente!", true);
                        await GetRecords();
                        await OpenViewRecord(createdRecord);
                        await OpenVolumesModal(createdRecord);


                    }
                }

            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
            

        }

        #endregion

        #region HandleSearchForm
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
        private async Task HandleMetaDataFilter(List<MetaDataFilter> request)
        {
            recordFilter.MetaData = request;
            await HandleSearchFilter();
        }
        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            try
            {
                if (!string.IsNullOrEmpty(args.ModalOrigin))
                {
                    switch (args.ModalOrigin)
                    {
                        case "MetaDataBySeries":
                            await HandleMetaDatosBySeries(args);
                            break;
                        case "MetaDataBySubSeries":
                            await HandleMetaDatosBySubseries(args);
                            break;
                        default:
                            notificationModal.UpdateModal(ModalType.Error, "ModalOrigin no reconocido", true);
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
        }
        private async Task HandleMetaDatosBySeries(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                await metaDataFilterModal.LoadMetaDataInfo(true, recordFilter.SeriesId.Value);
                await metaDataFilterModal.UpdateModalStatusAsync(true);
            }
        }

        private async Task HandleMetaDatosBySubseries(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                await metaDataFilterModal.LoadMetaDataInfo(false, recordFilter.SubSeriesId.Value);
                await metaDataFilterModal.UpdateModalStatusAsync(true);
            }
        }


        private async Task HandleCleanFilter()
        {
            recordFilter = new();
            isEnableProOffice = false;
            isEnableSerie = false;
            isEnableSubSerie = false;
            await GetRecords();

        }
        #endregion
              
        #region HandleMetaDatos
        private async Task HandleMetaDataRelationSelected(MyEventArgs<MetaDataRelationDtoRequest> data)
        {
            
            metaDataValueModal.UpdateModalStatus(data.ModalStatus);
            var objetoAModificar = metaDataList.FirstOrDefault(x => x.MetaFieldId == data.Data.MetaFieldId);

           
            if (objetoAModificar != null)
            {
                objetoAModificar.Color = data.Data.ColorData;
                objetoAModificar.MetaValue = data.Data.DataText;
                objetoAModificar.IsUpdated = true;
            }
            MetaDataDtoRequest requestMeta = new()
            {
                MetaFieldId = data.Data.MetaFieldId,
                ColorData = data.Data.ColorData,
                DataText = data.Data.DataText,
            };
            createRequest.MetaData.Add(requestMeta);

            if(MetaDataTypology.DocumentId != 0)
            {
                await LoadMetadataTypology();
            }


        }
        #endregion

        private async Task HandleIndexDocumentaryTypologyModal(MyEventArgs<DocumentaryTypologyTDMDtoRequest> data)
        {
            await GetDocumentaryTypologyByVolumenId(data.Data.VolumeId, data.Data.DocumentId, "DOCINDEX");
        }
        private async Task HandleAutomaticIndexing(MyEventArgs<DocumentaryTypologyTDMDtoRequest> data)
        {
            await GetDocumentaryTypologyByVolumenId(data.Data.VolumeId, data.Data.DocumentId, "DOCINDEX");
        }

        private void HandlePaginationGrid(List<VRecordsDtoResponse> newDataList)
        {
            recordsInfoList = newDataList;
        }
        #endregion

        #region OthersMethods

        #region GetDataMethods

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
                else { recordTypeList = new();
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }

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

                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    recordsInfoList = deserializeResponse.Data.Data;
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                    SpinnerLoaderService.HideSpinnerLoader(Js);


                }
                else
                {
                    recordsInfoList = new();
                    paginationInfo = new(); 
                    paginationComponent.ResetPagination(paginationInfo);
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }

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
                    recordTypeListCreate = deserializeResponse.Data;
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else { recordTypeList = new();
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
                    recordFileTypeListCreate = deserializeResponse.Data;
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else { recordFileTypeList = new();
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
                    
                    
                }
            }
            catch (Exception ex)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }
        private async Task FetchAdministrativeUnits(int id,  string context , bool setFlagToTrue)
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
                    switch (context)
                    {
                        case "consult":
                            adminUnitList = deserializeResponse.Data!;
                            break;
                        case "create":
                            adminUnitListCreate = deserializeResponse.Data!;
                            break;
                    }

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
                            idDocVersionCreate = id;
                            await FetchAdministrativeUnits(idDocVersionCreate, "create", false);
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
                    switch (context)
                    {
                        case "consult":
                            isEnableSubSerie = false;
                            isEnableSerie = false;
                            isEnableProOffice = false;
                            proOfficesList = new();
                            seriesList = new();
                            subSeriesList = new();
                            break;
                        case "create":
                            isEnableSubSerieCreate = false;
                            isEnableSerieCreate = false;
                            isEnableProOfficeCreate = false;
                            proOfficesListCreate = new();
                            seriesListCreate = new();
                            subSeriesListCreate = new();
                            break;
                    }
                    SpinnerLoaderService.ShowSpinnerLoader(Js);

                    HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                    HttpClient?.DefaultRequestHeaders.Add("AdministrativeUnitId", $"{id}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<ProductionOfficesDtoResponse>>>("paramstrd/ProductionOffice/ByProductionOffices");
                    if (deserializeResponse!.Succeeded && (deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null))
                    {
                        switch (context)
                        {
                            case "consult":
                                proOfficesList = deserializeResponse.Data!;
                                isEnableProOffice = true;
                                recordFilter.AdministrativeUnitId = id;
                                break;
                            case "create":
                                proOfficesListCreate = deserializeResponse.Data!;
                                isEnableProOfficeCreate = true;
                                AdministrativeUnitId = id;
                                break;
                        }
                    }
                    HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else
                {
                    switch (context)
                    {
                        case "consult":
                            isEnableSubSerie = false;
                            isEnableSerie = false;
                            seriesList = new();
                            subSeriesList = new();
                            recordFilter.AdministrativeUnitId = id;
                            break;
                        case "create":
                            isEnableSubSerieCreate = false;
                            isEnableSerieCreate = false;
                            seriesListCreate = new();
                            subSeriesListCreate = new();
                            AdministrativeUnitId = id;
                            break;
                    }
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
                    switch (context)
                    {
                        case "consult":
                            isEnableSubSerie = false;
                            subSeriesList = new();
                            seriesList = new();
                            break;
                        case "create":
                            isEnableSubSerieCreate = false;
                            subSeriesListCreate = new();
                            seriesListCreate = new();
                            break;
                    }
                    SpinnerLoaderService.ShowSpinnerLoader(Js);
                    HttpClient?.DefaultRequestHeaders.Remove("productionOfficeId");
                    HttpClient?.DefaultRequestHeaders.Add("productionOfficeId", $"{id}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<SeriesDtoResponse>>>("paramstrd/Series/BySeries");
                    if (deserializeResponse!.Succeeded && (deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null))
                    {
                        switch (context)
                        {
                            case "consult":
                                seriesList = deserializeResponse.Data!;
                                isEnableSerie = true;
                                recordFilter.ProductionOfficeId = id;
                                break;
                            case "create":
                                seriesListCreate = deserializeResponse.Data!;
                                isEnableSerieCreate = true;
                                ProductionOfficeId = id;
                                break;
                        }
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

        public async Task GetSubSeries(int id, string context,bool loadMetaData = true)
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
                    switch (context)
                    {
                        case "consult":
                            recordFilter.SeriesId = id;
                            subSeriesList = deserializeResponse.Data!;
                            isEnableSubSerie = true;
                            if(subSeriesList.Count() == 0)
                            {
                                notificationModal.UpdateModal(ModalType.Information, Translation["FilterByMetaDataSeries"], true, modalOrigin: "MetaDataBySeries");
                                isEnableSubSerie = false;

                            }
                            else
                            {
                                isEnableSubSerie = true;
                            }
                            break;
                        case "create":
                            subSeriesListCreate = deserializeResponse.Data!;
                            isEnableSubSerieCreate = true;
                            createRequest.SeriesId = id;
                            if(loadMetaData)
                            {
                                
                                if (subSeriesListCreate.Count() == 0)
                                {
                                    await LoadMetaDataInfo("Serie", id);
                                    isEnableSubSerieCreate = false;
                                    
                                }
                                else
                                {
                                    isEnableSubSerieCreate = true;
                                    notificationModal.UpdateModal(ModalType.Information, Translation["MustSelectSubSeries"], true, buttonTextCancel: "");
                                }
                            }
                            break;
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
        public async Task GetVolumes()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("recordId");
                HttpClient?.DefaultRequestHeaders.Add("recordId", $"{createdRecord.RecordId}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VolumeDtoResponse>>>("records/Volume/VolumeByRecordId");
                HttpClient?.DefaultRequestHeaders.Remove("recordId");

                if (deserializeResponse!.Succeeded)
                {
                    lstRecordVolumens = deserializeResponse.Data;
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, $"Error al obtener los volumenes: {ex.Message}", true, Translation["Accept"]);
            }
        }

        private async Task GetImageVolume()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("volumeId");
                HttpClient?.DefaultRequestHeaders.Add("volumeId", RecordVolumens.ToString());
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<ImageVolumeDtoResponse>>("records/Volume/ImageVolume");
                HttpClient?.DefaultRequestHeaders.Remove("volumeId");

                if (deserializeResponse!.Succeeded)
                {
                    FileData = deserializeResponse.Data?.DataFile;
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["GetImagenErrorMessage"], true, Translation["Accept"]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Translation["GetImagenErrorMessage"] + $": {ex.Message}");
            }
        }

        private async Task GetUploadFileImage()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("volumeId");
                HttpClient?.DefaultRequestHeaders.Add("volumeId", RecordVolumens.ToString());
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<ImageVolumeDtoResponse>>("records/Volume/ImageVolume");
                HttpClient?.DefaultRequestHeaders.Remove("volumeId");

                if (deserializeResponse!.Succeeded)
                {
                    FileData = deserializeResponse.Data?.DataFile;
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["GetImagenErrorMessage"], true, Translation["Accept"]);
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Translation["GetImagenErrorMessage"] + $": {ex.Message}");
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
        }


        private async Task GetViewIndexedPDF(int DocumentId)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("documentId");
                HttpClient?.DefaultRequestHeaders.Add("documentId", DocumentId.ToString());
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<ViewIndexedPDFDtoResponse>>("records/ControlBoard/ImageDocument");
                HttpClient?.DefaultRequestHeaders.Remove("documentId");

                if (deserializeResponse!.Succeeded)
                {
                    FileData = deserializeResponse.Data?.DataFile;

                    if(DisplayDashboard == "col-lg-12")
                    {
                        ShowDashboard();
                    }

                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["GetImagenIndexedErrorMessage"], true, Translation["Accept"]);
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Translation["GetImagenIndexedErrorMessage"] + $": {ex.Message}");
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
        }

        public async Task GetDocumentaryTypologyByVolumenId(int volumenId, int documentId = 0, string typeViewPDF = "")
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                RecordVolumens = volumenId;

                if(typeViewPDF == "DOCINDEX")
                {
                    await GetViewIndexedPDF(documentId);
                }
                else
                {
                    await GetImageVolume();
                }

                HttpClient?.DefaultRequestHeaders.Remove("volumenId");
                HttpClient?.DefaultRequestHeaders.Add("volumenId", $"{volumenId}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<DocumentaryTypologyTDMDtoRequest>>>("records/ControlBoard/DocumentaryTypologyByVolumenId");
                HttpClient?.DefaultRequestHeaders.Remove("volumenId");

                if (deserializeResponse!.Succeeded)
                {
                    DocumentarTypologyTDM = deserializeResponse.Data;
                    PanelTableTypologyTDM = DocumentarTypologyTDM.Count > 0 ? string.Empty : "d-none";

                    _TotalFolios = (decimal)lstRecordVolumens.Where(y => y.VolumeId == RecordVolumens).First().TotalFolios;
                    List<DocumentaryTypologyTDMDtoRequest> listM = DocumentarTypologyTDM.Where(x => x.IndexingType == "M").ToList();
                    ValidateAutomaticIndexing = listM.Any(x => x.EndFolio == _TotalFolios);
                    DisabledUploadFileImage = false;
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else
                {
                    PanelTableTypologyTDM = "d-none";
                    DisabledUploadFileImage = true;
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, $"Error al obtener los volumenes: {ex.Message}", true, Translation["Accept"]);
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
        }

        public async Task SelectedSubSeries(int id, string context, bool loadMetaData = true)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                switch (context)
                {
                    case "consult":
                        recordFilter.SubSeriesId = id;

                        notificationModal.UpdateModal(ModalType.Information, Translation["FilterByMetaDataSubSeries"], true, modalOrigin: "MetaDataBySubSeries");
                        break;
                    case "create":

                        createRequest.SubSeriesId = id;

                        if (loadMetaData)
                        {
                            await LoadMetaDataInfo("SubSerie", id);
                        }
                        break;
                }

                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }
        public async Task LoadMetaDataInfo(string typeOfConsult, int id,int? seriesid = null, int? subSeriesId = null)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            filterRequest = new();
            try
            {
                string nameObject = string.Empty;
                switch (typeOfConsult)
                {
                    case "Serie": //MetaDatos a partir de la serie
                        filterRequest.SeriesId = id;
                        nameObject = Translation["NotFoundMetaDataSeriesMessage"];
                        break;
                    case "SubSerie": //Metadatos a partir de subseries
                        filterRequest.SubSeriesId = id;
                        nameObject = Translation["NotFoundMetaDataSubSeriesMessage"];
                        break;
                    case "Record":
                        filterRequest.RecordsId = id;
                        filterRequest.SeriesId = seriesid;
                        filterRequest.SubSeriesId = subSeriesId;
                        nameObject = Translation["NotFoundMetaDataRecordsMessage"];
                        break;

                }
                var responseApi = await HttpClient.PostAsJsonAsync(UriMetaDataFilter, filterRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<VMetaDataDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data.Count == 0)
                {
                    notificationModal.UpdateModal(ModalType.Information, nameObject, true, Translation["Accept"], buttonTextCancel: "");
                    metaDataList = new();
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else
                {
                    metaDataList = deserializeResponse.Data;
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            catch (Exception ex)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }
        public async Task LoadMetadataTypology()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                var responseApi = await HttpClient.PostAsJsonAsync("records/ControlBoard/MetaDataTypologyByFilter", MetaDataTypology);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<MetaDataTypologyDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data.Count == 0)
                {
                    notificationModal.UpdateModal(ModalType.Information, Translation["NoMetadata"], true, Translation["Accept"], buttonTextCancel: "");
                    ActiveTabIndex = 1;
                    MetaDataTypologyList = new();
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else
                {
                    ActiveTabIndex = 2;
                    MetaDataTypologyList = deserializeResponse.Data;
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            catch (Exception ex)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }

        #endregion

        #region OnValueChangeMethods

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
        private void OnValueChangedFilter(string newValue)
        {
            ResponseFilter = newValue;
            if (newValue == Translation["Records"]) 
            {
                DocumentFilterClass = "d-none";
            }
            else if (newValue == Translation["Documents"]) 
            {
                DocumentFilterClass = string.Empty;
            }
        }

        private void MetaDataSelected(VMetaDataDtoResponse record)
        {
            metaDataValueModal.MetaFieldSelectedRecords(record);
            metaDataValueModal.UpdateModalStatus(true);
        }

        private void MetaDataTypologySelected(MetaDataTypologyDtoResponse record)
        {
            VMetaDataDtoResponse model = new VMetaDataDtoResponse()
            {
                MetaFieldId = record.MetaFieldId,
                NameMetaField = record.NameMetaField,
                FieldTypeCode = record.FieldTypeCode,
                FieldTypeName = record.FieldTypeName,
                MetaDataId = record.MetaDataId,
                Color = record.Color,
                MetaValue = record.MetaValue,
                Active = record.Active,
                Mandatory = (bool)record.Mandatory
            };

            record.DocumentId = MetaDataTypology.DocumentId;

            metaDataValueModal.MetaFieldSelectedRecords(model, record);
            metaDataValueModal.UpdateModalStatus(true);
        }

        private async Task TabChangedHandler(int newIndex)
        {
            ActiveTabIndex = newIndex;
            if(ActiveTabIndex == 1)
            {
                DisplayMetadataTypology = "d-none";
            }
        }

        #endregion

        #region ValidateMethods

        private bool ValidateCreateRecordDtoRequest(CreateRecordDtoRequest request)
        {
            
            if (!ValidateMetaData(metaDataList))
            {
                return false;
            }

            var errors = new List<string>();

            if (request.SeriesId <= 0 || request.SeriesId == null)
            {
                errors.Add(Translation["DocumentarySeries"]);
            }

            if (string.IsNullOrEmpty(request.RecordFileType))
            {
                errors.Add(@Translation["FileRecordType"]);
            }

            if (string.IsNullOrEmpty(request.RecordType))
            {
                errors.Add(Translation["RecordType"]);
            }

            if (request.Volume <= 0)
            {
                errors.Add(Translation["VolumeCount"]);
            }
            if(request.DocumentalVersionId <= 0)
            {
                errors.Add(Translation["DocumentaryVersion"]);
            }
            if(request.RecordName == null || string.IsNullOrEmpty(request.RecordName))
            {
                errors.Add(Translation["RecordName"]);
            }

            if (errors.Any())
            {
                var limitedErrors = errors.Take(2).ToList();
                var message = string.Join("\n", limitedErrors);
                notificationModal.UpdateModal(ModalType.Error, $"Los siguientes campos son obligatorios y no deben estar vacíos: {message}", true);
                return false;
            }

            return true;
        }

        private bool ValidateMetaData(List<VMetaDataDtoResponse> metaDataList)
        {
            var mandatoryButNotUpdatedMetaFields = metaDataList
                .Where(metaData => metaData.Mandatory && !metaData.IsUpdated)
                .Select(metaData => metaData.NameMetaField)
                .ToList();

            if (mandatoryButNotUpdatedMetaFields.Any())
            {
                var message = string.Join(", ", mandatoryButNotUpdatedMetaFields);
                notificationModal.UpdateModal(ModalType.Error, $"Los siguientes metadatos son obligatorios y requieren ser actualizados: {message}", true);
                return false;
            }

            return true;
        }



        #endregion

        #region OnActionsPanels
        private void CloseCreateView()
        {
            PanelConsultRecordClass = string.Empty;
            PanelCreateRecordClass = "d-none";
            PanelHistoryRecord = "d-none";
            PanelViewRecordClass = "d-none";
            visibleBottons = true;
            ButtonCreateRecordClass = "d-none";
            CleanCreateForm();  
            //proOfficesListCreate = new();
            //subSeriesListCreate = new();
            //seriesListCreate = new();
            //isEnableProOfficeCreate = false;
            //isEnableSerieCreate = false;
            //isEnableSubSerieCreate = false;
            //metaDataList = new();
            PanelTableTypologyTDM = "d-none";
            RecordVolumens = null;
            DisabledUploadFileImage = true;
            if (FileData != null)
            {
                Array.Fill(FileData, (byte)0);
            }
            ActiveTabIndex = 0;
            DisplayMetadataTypology = "d-none";
        }

        private void OpenCreateView()
        {
            createRequest = new();
            PanelViewRecordClass = "d-none";
            PanelConsultRecordClass = "d-none";
            PanelCreateRecordClass = string.Empty;
            visibleBottons = false;
            ButtonCreateRecordClass = string.Empty;
            RecordTitlePage = "Creation";
            metaDataList = new();
            RecordVolume = null;
        }

        private void CleanCreateForm()
        {
            metaDataList = new();   
            createRequest = new();
            isEnableSubSerieCreate = false;
            subSeriesListCreate = new();
            isEnableSerieCreate = false;
            seriesListCreate = new();
            isEnableProOfficeCreate = false;
            ProductionOfficeId = 0;
            proOfficesListCreate = new();
            AdministrativeUnitId = 0;
            adminUnitListCreate = new();
            idDocVersionCreate = 0;
            RecordVolume = string.Empty;
        }

        private async Task OpenViewRecord(VRecordsDtoResponse record)
        {
            createdRecord = record;
            visibleBottons = false;
            ButtonCreateRecordClass = string.Empty;
            await LoadMetaDataInfo("Record", record.RecordId, record.SeriesId, record.SubSeriesId);
            createdRecord = record;
            await GetVolumes();
            AdministrativeUnitId = record.AdministrativeUnitId;
            await GetProducOffice(AdministrativeUnitId, "create");
            ProductionOfficeId = record.ProductionOfficeId;
            await GetSeries(ProductionOfficeId, "create");
            await GetSubSeries(record.SeriesId.Value, "create",false);
            RecordVolume = record.Volumes != null ? record.Volumes.ToString() : "0";
            PanelConsultRecordClass = "d-none";
            RecordTitlePage = "Capture";
            PanelCreateRecordClass = "d-none";
            PanelViewRecordClass = string.Empty;
        }

        private async Task OpenVolumesModal(VRecordsDtoResponse record)
        {
            await getRecordsVolumes.GetRecordsVolumens(record.RecordId, record.RecordNumber);
            getRecordsVolumes.UpdateModalStatus(true);
        }

        #region MethodsBoardRecords
        private void noDisponible()
        {
            notificationModal.UpdateModal(ModalType.Information, "¡Esta funcionalidad se encuentra en desarrollo, pronto estará disponible!", true);
        }
        private async Task ViewIndexedPDF(DocumentaryTypologyTDMDtoRequest record)
        {
            await GetViewIndexedPDF(record.DocumentId);
        }
        private async Task OpenIndexDocumentaryTypologyModal(DocumentaryTypologyTDMDtoRequest record)
        {
            var volumenId = record.VolumeId == 0 ? RecordVolumens : record.VolumeId;

            IndexDocumentaryTypologyDtoRequest model = new()
            {
                VolumeId = (int)volumenId,
                DocumentaryTypologyId = record.DocumentaryTypologyId,
                StartFolio = record.StartFolio,
                EndFolio = record.EndFolio,
                Observation = record.DocDescription,
                Color = record.Color,
                Origin = record.OriginCode,
                Options = record.Opcion,
                OptionValue = record.OpcionValor
            };

            await indexDocumentaryTypologyModal.GetValue(model, record.DocumentId, record.IndexingType);
            indexDocumentaryTypologyModal?.UpdateModalStatusAsync(true);
        }
        public async Task ShowTabMetadata(DocumentaryTypologyTDMDtoRequest record)
        {
            DisplayMetadataTypology = "";
            TitleMetadataTypology = record.TypologyName;
            MetaDataTypology.DocumentId = record.DocumentId;
            MetaDataTypology.DocumentaryTypologyId = record.DocumentaryTypologyId;
            await LoadMetadataTypology();
        }
        public async Task ShowAttachmentsModal(DocumentaryTypologyTDMDtoRequest record)
        {
            await attachmentTrayModal.UpdateAttachmentControlPanel(record.DocumentId);
            attachmentTrayModal.UpdateModalStatus(true);
        }
        private async Task AutomaticIndexing(DocumentaryTypologyTDMDtoRequest record)
        {
            if (ValidateAutomaticIndexing)
            {
                var volumenId = record.VolumeId == 0 ? RecordVolumens : record.VolumeId;

                FileDocumentaryTypologyDtoRequest model = new()
                {
                    VolumeId = (int)volumenId,
                    DocumentaryTypologyId = record.DocumentaryTypologyId
                };

                await uploadPDFAutomaticIndexingModal.GetValue(model);
                uploadPDFAutomaticIndexingModal?.UpdateModalStatusAsync(true);
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["AutomaticIndexingErrorMessage"], true);
            }
        }
        #endregion

        #endregion

        #region Display Table or Document

        private void ShowPDFViewer()
        {
            ValidDisplayDashboard = !ValidDisplayDashboard;

            if (ValidDisplayDashboard && DisplayDashboard.Equals("col-lg-6") || (!ValidDisplayDashboard && DisplayDashboard.Equals("col-lg-12") || ValidDisplayDashboard && DisplayDashboard.Equals("col-lg-12")) || !ValidDisplayDashboard && DisplayDashboard.Equals("col-lg-6"))
            {
                DisplayPDF = "col-lg-12";
                ShowPDFIcon = "fa-solid fa-eye";
                TextPDF = Translation["ReducePDF"];

                DisplayDashboard = "d-none";
                ShowDashboardIcon = "fa-solid fa-eye-slash";
                TextDashboard = Translation["ExpandDashboard"];

            }
            else
            {
                DisplayPDF = "col-lg-6";
                DisplayDashboard = "col-lg-6";
                ShowDashboardIcon = "fa-solid fa-eye";
                TextPDF = Translation["ExpandPDFViewer"];
                TextDashboard = Translation["ExpandDashboard"];
            }
        }

        private void ShowDashboard()
        {
            ValidDisplayPDFViewer = !ValidDisplayPDFViewer;

            if (ValidDisplayPDFViewer && DisplayPDF.Equals("col-lg-6") || (!ValidDisplayPDFViewer && DisplayPDF.Equals("col-lg-12") || ValidDisplayPDFViewer && DisplayPDF.Equals("col-lg-12")))
            {
                DisplayDashboard = "col-lg-12";
                ShowDashboardIcon = "fa-solid fa-eye";
                TextDashboard = Translation["ReduceDashboard"];

                DisplayPDF = "d-none";
                ShowPDFIcon = "fa-solid fa-eye-slash";
                TextPDF = Translation["ExpandPDFViewer"];
            }
            else if (!ValidDisplayDashboard && DisplayPDF.Equals("col-lg-6"))
            {
                DisplayDashboard = "col-lg-12";
                ShowDashboardIcon = "fa-solid fa-eye";
                TextDashboard = Translation["ReduceDashboard"];

                DisplayPDF = "d-none";
                ShowPDFIcon = "fa-solid fa-eye-slash";
                TextPDF = Translation["ExpandPDFViewer"];
            }
            else
            {
                DisplayPDF = "col-lg-6";
                DisplayDashboard = "col-lg-6";
                ShowPDFIcon = "fa-solid fa-eye";
                TextPDF = Translation["ExpandPDFViewer"];
                TextDashboard = Translation["ExpandDashboard"];
            }



        }

        #endregion Display Table or Document


        #region GetRecordHistory

        private async Task GetRecordHistory(VRecordsDtoResponse record)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            visibleBottons = false;
            ButtonCreateRecordClass = string.Empty;
            PanelConsultRecordClass = "d-none";
            RecordTitlePage = "Capture";
            PanelCreateRecordClass = "d-none";
            PanelViewRecordClass = "d-none";
            PanelHistoryRecord = string.Empty;

            try
            {

                if (record.RecordId != 0)
                {

                    HttpClient?.DefaultRequestHeaders.Remove("IdRecord");
                    HttpClient?.DefaultRequestHeaders.Add("IdRecord", $"{record.RecordId}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<RecordHistoryDtoResponse>>>("records/RecordHistory/FilterById");
                    HttpClient?.DefaultRequestHeaders.Remove("IdRecord");

                    if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                    {

                        SpinnerLoaderService.HideSpinnerLoader(Js);
                        recordHistoryList = deserializeResponse.Data;
                        StateHasChanged();
                    }
                    else
                    {

                        SpinnerLoaderService.HideSpinnerLoader(Js);
                        notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessageS"], true);
                    }

                }
            }
            catch
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessageS"], true);
            }
        }

        #endregion GetRecordHistory


        #endregion

        #endregion

    }
}
