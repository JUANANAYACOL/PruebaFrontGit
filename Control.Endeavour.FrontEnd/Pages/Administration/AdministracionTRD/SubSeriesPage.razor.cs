using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.DocumentaryTypologiesBag;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
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
    public partial class SubSeriesPage
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

        private NewPaginationComponent<SubSeriesDtoResponse, SubSeriesFilterDtoRequest> PaginationComponent = new();

        #endregion Components

        #region Modals

        private SubSeriesModal modalSubseries = new();
        private NotificationsComponentModal notificationModal = new();
        private DocumentaryTypologiesBagMetaDataModal documentaryTypologiesBagMetaDataModal { get; set; } = new();

        #endregion Modals

        #region Models

        private PaginationInfo paginationInfo = new();
        private MetaModel meta = new();
        private MetaModel metasubSeries = new();
        private SubSeriesFilterDtoRequest SubSeriesFilterDtoRequest = new();
        private SubSeriesDtoResponse recordToDelete = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string SubSeriesSelectedModalTitle { get; set; } = string.Empty;
        private string UriFilterSubSeries = "paramstrd/SubSeries/ByFilter";
        private string codeFilter { get; set; } = "";
        private string NameFilter { get; set; } = "";

        #endregion Environments(String)

        #region Environments(Numeric)

        private int IdDocumental { get; set; } = 0;
        private int idAdUnit { get; set; } = 0;
        private int IdproOffice { get; set; } = 0;
        private int IdSerie { get; set; } = 0;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool dataChargue = false;
        private bool administrativeUnitEnable { get; set; } = false;
        private bool productionOfficeEnable { get; set; } = false;
        private bool serieEnable { get; set; } = false;
        private bool saveIsDisable { get; set; } = true;
        public bool CancelExport { get; set; }
        private bool ExportAllPages { get; set; }

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<SubSeriesDtoResponse> subSeriesList { get; set; } = new();
        private List<SubSeriesExcelDtoResponse> subSeriesExcelList { get; set; } = new();
        private List<SeriesDtoResponse> seriesList { get; set; } = new();
        private List<DocumentalVersionDtoResponse> documentalVersionsList { get; set; } = new();
        private List<AdministrativeUnitsDtoResponse> administrativeUnitsList { get; set; } = new();
        private List<ProductionOfficesDtoResponse> productionOfficesList { get; set; } = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetDocumentalVersions();
            //await GetSubSeries();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        #region HandleChangedData

        private async Task HandleChangedData(bool changed)
        {
            if (subSeriesList.Count > 0)
            {
                await GetSubSeries();
            }
        }

        #endregion HandleChangedData

        #region DeleteSubSerie

        private void ShowModalDelete(SubSeriesDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, Translation["Yes"], Translation["No"], modalOrigin: "DeleteModal");
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                if (args.IsAccepted && args.ModalOrigin.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new();
                    request.Id = recordToDelete.SubSeriesId;
                    request.User = "Front"; // Cambiar por la varibale de session del usuario
                    var responseApi = await HttpClient.PostAsJsonAsync("paramstrd/SubSeries/DeleteSubSerie", request);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse.Succeeded)
                    {
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            await GetSubSeries();
                            notificationModal.UpdateModal(ModalType.Success, Translation["DeleteSuccessfulMessage"], true);
                        }
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true);
                    }
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion DeleteSubSerie

        #region HandlePaginationGrid

        private void HandlePaginationGrid(List<SubSeriesDtoResponse> newDataList)
        {
            subSeriesList = newDataList;
        }

        #endregion HandlePaginationGrid

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #endregion HandleMethods

        #region OthersMethods

        #region GetDataMethods

        private async Task GetDocumentalVersions()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                var responseApi = await HttpClient.GetAsync("paramstrd/DocumentalVersions/ByDocumentalVersions");
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<DocumentalVersionDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    string toCurrent = Translation["ToCurrent"];
                    string documentaryVersion = Translation["DocumentaryVersion"];
                    documentalVersionsList = deserializeResponse.Data;
                    documentalVersionsList.ForEach(x =>
                    {
                        x.Code = $"{((x.EndDate == null || x.EndDate.Value > DateTime.Now) ? $"{x.StartDate!.Value.Year} {toCurrent} - {documentaryVersion} {x.Code}" : $"{x.StartDate!.Value.Year} a {x.EndDate.Value.Year} - {documentaryVersion} {x.Code} {x.VersionType}")}";
                    });
                    documentalVersionsList = documentalVersionsList.OrderBy(x => x.StartDate).Reverse().ToList();
                }
                else
                {
                    documentalVersionsList = new();
                    notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task GetAdministrativeUnits(int value)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                IdDocumental = value;
                idAdUnit = 0;
                administrativeUnitEnable = false;
                productionOfficeEnable = false;
                serieEnable = false;
                administrativeUnitsList = new();
                productionOfficesList = new();
                seriesList = new();

                if (IdDocumental != 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");
                    HttpClient?.DefaultRequestHeaders.Add("documentalVersionsId", $"{IdDocumental}");
                    var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeUnitsDtoResponse>>>("paramstrd/AdministrativeUnit/ByAdministrativeUnits");
                    HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");

                    if (deserializeResponse!.Succeeded && deserializeResponse.Data?.Count != 0)
                    {
                        administrativeUnitsList = deserializeResponse.Data ?? new();
                        administrativeUnitEnable = true;
                    }
                    else
                    {
                        administrativeUnitsList = new();
                        productionOfficesList = new();
                        seriesList = new();
                        administrativeUnitEnable = false;
                        productionOfficeEnable = false;
                        serieEnable = false;
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task GetProductionOffices(int value)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                idAdUnit = value;
                IdproOffice = 0;
                productionOfficeEnable = false;
                serieEnable = false;
                productionOfficesList = new();
                seriesList = new();

                if (idAdUnit != 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                    HttpClient?.DefaultRequestHeaders.Add("AdministrativeUnitId", $"{idAdUnit}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<ProductionOfficesDtoResponse>>>("paramstrd/ProductionOffice/ByProductionOffices");
                    HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");

                    if (deserializeResponse!.Succeeded && deserializeResponse.Data != null && deserializeResponse.Data?.Count != 0)
                    {
                        productionOfficesList = deserializeResponse.Data ?? new();
                        productionOfficeEnable = true;
                    }
                    else
                    {
                        productionOfficesList = new();
                        productionOfficeEnable = false;
                        seriesList = new();
                        serieEnable = false;
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task GetSeries(int value)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                IdproOffice = value;
                IdSerie = 0;
                serieEnable = false;
                seriesList = new();

                if (IdproOffice != 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("productionOfficeId");
                    HttpClient?.DefaultRequestHeaders.Add("productionOfficeId", $"{IdproOffice}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<SeriesDtoResponse>>>("paramstrd/Series/BySeries");
                    HttpClient?.DefaultRequestHeaders.Remove("productionOfficeId");
                    if (deserializeResponse!.Succeeded && deserializeResponse.Data != null && deserializeResponse.Data?.Count != 0)
                    {
                        seriesList = deserializeResponse.Data ?? new();
                        serieEnable = true;
                    }
                    else
                    {
                        seriesList = new();
                        serieEnable = false;
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task GetSubSeries()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                SubSeriesFilterDtoRequest = new();
                SubSeriesFilterDtoRequest.SeriesId = IdSerie;

                var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterSubSeries, SubSeriesFilterDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<SubSeriesDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                {
                    subSeriesList = deserializeResponse.Data.Data ?? new();
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    PaginationComponent.ResetPagination(paginationInfo);
                    dataChargue = true;
                }
                else
                {
                    subSeriesList = new();
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetDataMethods

        #region ModalMethods

        private async Task ShowModalCreate()
        {
            await modalSubseries.UpdateModalStatus(true);
        }

        private async Task ShowModalEditAsync(SubSeriesDtoResponse record)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            await modalSubseries.UpdateSelectedRecordAsync(record);
            await modalSubseries.UpdateModalStatus(true);
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion ModalMethods

        #region Filters

        #region ApplyFilters

        private async Task ApplyFiltersAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            if (IdDocumental == 0 && idAdUnit == 0 && IdproOffice == 0 && IdSerie == 0 && string.IsNullOrEmpty(codeFilter) && string.IsNullOrEmpty(NameFilter))
            {
                await GetSubSeries();
            }
            else
            {
                try
                {
                    SubSeriesFilterDtoRequest request = new() { DocumentalVersionId = IdDocumental, AdministrativeUnitId = idAdUnit, ProductionOfficeId = IdproOffice, SeriesId = IdSerie, Code = codeFilter, Name = NameFilter };
                    SubSeriesFilterDtoRequest = request;
                    var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterSubSeries, SubSeriesFilterDtoRequest);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<SubSeriesDtoResponse>>>();

                    if (deserializeResponse!.Succeeded && deserializeResponse.Data != null && deserializeResponse.Data.TotalCount > 0)
                    {
                        subSeriesList = deserializeResponse.Data.Data;
                        paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                        PaginationComponent.ResetPagination(paginationInfo);
                        dataChargue = true;
                    }
                    else if (deserializeResponse!.Succeeded && deserializeResponse.Data == null)
                    {
                        subSeriesList = new();
                        paginationInfo = new();
                        PaginationComponent = new();
                        dataChargue = false;
                    }
                    else
                    {
                        IdDocumental = 0;
                        idAdUnit = 0;
                        IdproOffice = 0;
                        IdSerie = 0;
                        administrativeUnitEnable = false;
                        productionOfficeEnable = false;
                        serieEnable = false;
                        administrativeUnitsList = new();
                        productionOfficesList = new();
                        seriesList = new();
                        codeFilter = "";
                        NameFilter = "";
                        await GetSubSeries();
                        notificationModal.UpdateModal(ModalType.Error, Translation["NoCoincidences"], true, Translation["Accept"]);
                    }
                }
                catch
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["ErrorFiltering"], true, Translation["Accept"]);
                }
            }
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion ApplyFilters

        #region ResetFilter

        public void ResetFiltersAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            if (IdDocumental != 0 || idAdUnit != 0 || IdproOffice != 0 || IdSerie != 0 || !string.IsNullOrEmpty(codeFilter) || !string.IsNullOrEmpty(NameFilter))
            {
                IdDocumental = 0;
                idAdUnit = 0;
                IdproOffice = 0;
                IdSerie = 0;
                administrativeUnitEnable = false;
                productionOfficeEnable = false;
                serieEnable = false;
                administrativeUnitsList = new();
                productionOfficesList = new();
                seriesList = new();
                codeFilter = "";
                NameFilter = "";
                subSeriesList = new();
                dataChargue = false;
                //await GetSubSeries();
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["AtLeastOne4Clean"], true, Translation["Accept"]);
            }
            ExportAllPages = false;
            EnableSaveButton();
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion ResetFilter

        #endregion Filters

        #region EnableSaveButton

        public void EnableSaveButton()
        {
            if (IdDocumental <= 0 || idAdUnit <= 0 || IdproOffice <= 0 || IdSerie <= 0)
            {
                saveIsDisable = true;
            }
            else
            {
                saveIsDisable = false;
            }

            StateHasChanged();
        }

        #endregion EnableSaveButton

        #region ShowModalMetaData

        private async Task ShowModalMetaData(SubSeriesDtoResponse dtb)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            await documentaryTypologiesBagMetaDataModal.UpdateModalStatus(true);
            await documentaryTypologiesBagMetaDataModal.GetmetaFieldsOfBag((dtb ?? new()).SubSeriesId);
            SubSeriesSelectedModalTitle = dtb.Name;

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion ShowModalMetaData

        #region HandleStatusMetaDataChanged

        private async Task HandleStatusMetaDataChanged(bool status)
        {
            documentaryTypologiesBagMetaDataModal.UpdateModalStatus(status);
            await ApplyFiltersAsync();
        }

        #endregion HandleStatusMetaDataChanged

        #region OnBeforeExcelExport

        public async Task GetDataExcel()
        {
            var request = new SubSeriesFilterDtoRequest() { DocumentalVersionId = IdDocumental, AdministrativeUnitId = idAdUnit, ProductionOfficeId = IdproOffice, SeriesId = IdSerie, Code = codeFilter, Name = NameFilter };

            var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/SubSeries/ByFilterExcel", request);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<SubSeriesExcelDtoResponse>>>();

            if (deserializeResponse!.Succeeded)
            {
                subSeriesExcelList = deserializeResponse.Data!;
            }
            else
            {
                subSeriesExcelList = new();
            }
        }

        public async Task OnBeforeExcelExport(GridBeforeExcelExportEventArgs args)
        {
            if (ExportAllPages)
            {
                await GetDataExcel();

                if (subSeriesExcelList.Any())
                {
                    args.Data = subSeriesExcelList;
                }
            }
            else { args.Data = subSeriesList; }

            args.IsCancelled = CancelExport;
        }

        #endregion OnBeforeExcelExport

        #endregion OthersMethods

        #endregion Methods
    }
}