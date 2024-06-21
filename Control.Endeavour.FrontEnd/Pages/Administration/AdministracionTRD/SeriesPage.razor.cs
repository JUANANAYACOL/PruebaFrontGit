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
using Telerik.DataSource.Extensions;

namespace Control.Endeavour.FrontEnd.Pages.Administration.AdministracionTRD
{
    public partial class SeriesPage
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

        private NewPaginationComponent<SeriesDtoResponse, SeriesFilterDtoRequest> paginationComponent = new();

        #endregion Components

        #region Modals

        private DocumentaryTypologiesBagMetaDataModal documentaryTypologiesBagMetaDataModal { get; set; } = new();
        private NotificationsComponentModal notificationModal = new();
        private SeriesModal modalSeries = new();

        #endregion Modals

        #region Models

        private PaginationInfo paginationInfo = new();
        private SeriesDtoResponse recordToDelete = new();
        private SeriesFilterDtoRequest seriesFilterDtoRequest = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string SeriesSelectedModalTitle { get; set; } = string.Empty;
        private string UriFilterSeries = "paramstrd/Series/ByFilter";
        private string codeFilter { get; set; } = "";
        private string NameFilter { get; set; } = "";

        #endregion Environments(String)

        #region Environments(Numeric)

        private int IdDocumental { get; set; } = 0;
        private int idAdUnit { get; set; } = 0;
        private int IdproOffice { get; set; } = 0;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool dataChargue;
        private bool administrativeUnitEnable { get; set; } = false;
        private bool productionOfficeEnable { get; set; } = false;
        private bool saveIsDisable { get; set; } = true;
        public bool CancelExport { get; set; }
        private bool ExportAllPages { get; set; }

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<DocumentalVersionDtoResponse> documentalVersionsList = new();
        private List<AdministrativeUnitsDtoResponse> administrativeUnitsList { get; set; } = new();
        private List<ProductionOfficesDtoResponse> productionOfficesList = new();
        private List<SeriesDtoResponse> seriesList = new();
        private List<SeriesExcelDtoResponse> seriesExcelList = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            try
            {
                //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
                await GetDocumentalVersions();
                //await GetSeries();
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        #region HandleChangedData

        private async Task HandleChangedData(bool changed)
        {
            if (seriesList.Count > 0)
            {
                await GetSeries();
            }
        }

        #endregion HandleChangedData

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                if (args.IsAccepted && args.ModalOrigin.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new();
                    request.Id = recordToDelete.SeriesId;
                    request.User = "Front";
                    var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/Series/DeleteSeries", request);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse!.Succeeded)
                    {
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            await GetSeries();
                            notificationModal.UpdateModal(ModalType.Success, Translation["DeleteSuccessfulMessage"], true);
                        }
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true);
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleModalNotiClose

        #region HandlePaginationGrid

        private void HandlePaginationGrid(List<SeriesDtoResponse> newDataList)
        {
            seriesList = newDataList;
        }

        #endregion HandlePaginationGrid

        #region Language

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion Language

        #endregion HandleMethods

        #region OthersMethods

        #region Modals

        private async Task ShowModalCreate(bool newvalue)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            await modalSeries.UpdateModalStatus(true);
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task ShowModalEditAsync(SeriesDtoResponse record)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            await modalSeries.UpdateSelectedRecord(record);
            await modalSeries.UpdateModalStatus(true);

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private void ShowModalDelete(SeriesDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, Translation["Yes"], Translation["No"], modalOrigin: "DeleteModal");
        }

        #endregion Modals

        #region GetDataMethods

        #region GetDocumentalVersions

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
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetDocumentalVersions

        #region GetAdministrativeUnits

        private async Task GetAdministrativeUnits(int value)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                IdDocumental = value;
                idAdUnit = 0;
                administrativeUnitEnable = false;
                productionOfficeEnable = false;
                administrativeUnitsList = new();
                productionOfficesList = new();

                if (IdDocumental != 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");
                    HttpClient?.DefaultRequestHeaders.Add("documentalVersionsId", $"{IdDocumental}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeUnitsDtoResponse>>>("paramstrd/AdministrativeUnit/ByAdministrativeUnits");
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
                        administrativeUnitEnable = false;
                        productionOfficeEnable = false;
                    }
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetAdministrativeUnits

        #region GetProductionOffices

        private async Task GetProductionOffices(int value)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                idAdUnit = value;
                IdproOffice = 0;
                productionOfficeEnable = false;
                productionOfficesList = new();

                if (idAdUnit != 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                    HttpClient?.DefaultRequestHeaders.Add("AdministrativeUnitId", $"{idAdUnit}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<ProductionOfficesDtoResponse>>>("paramstrd/ProductionOffice/ByProductionOffices");
                    HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");

                    if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                    {
                        productionOfficesList = deserializeResponse.Data ?? new();
                        productionOfficeEnable = true;
                    }
                    else
                    {
                        productionOfficesList = new();
                        productionOfficeEnable = false;
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetProductionOffices

        #region GetSeries

        private async Task GetSeries()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            seriesFilterDtoRequest = new();
            seriesFilterDtoRequest.ProductionOfficeId = IdproOffice;
            var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterSeries, seriesFilterDtoRequest);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<SeriesDtoResponse>>>();
            if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
            {
                seriesList = deserializeResponse.Data.Data ?? new();
                paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                paginationComponent.ResetPagination(paginationInfo);
                dataChargue = true;
            }
            else
            {
                seriesList = new();
                paginationInfo = new();
                paginationComponent.ResetPagination(paginationInfo);
                dataChargue = false;
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetSeries

        #endregion GetDataMethods

        #region Filters

        #region ApplyFilters

        private async Task ApplyFiltersAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            if (IdDocumental == 0 && idAdUnit == 0 && IdproOffice == 0 && string.IsNullOrEmpty(codeFilter) && string.IsNullOrEmpty(NameFilter))
            {
                await GetSeries();
            }
            else
            {
                try
                {
                    SeriesFilterDtoRequest request = new() { DocumentalVersionId = IdDocumental, AdministrativeUnitId = idAdUnit, ProductionOfficeId = IdproOffice, Code = codeFilter, Name = NameFilter };
                    seriesFilterDtoRequest = request;
                    var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterSeries, seriesFilterDtoRequest);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<SeriesDtoResponse>>>();

                    if (deserializeResponse!.Succeeded && deserializeResponse.Data != null && deserializeResponse.Data.TotalCount > 0)
                    {
                        seriesList = deserializeResponse.Data.Data;
                        paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                        paginationComponent.ResetPagination(paginationInfo);
                        dataChargue = true;
                    }
                    else if (deserializeResponse!.Succeeded && deserializeResponse.Data == null)
                    {
                        seriesList = new();
                        paginationInfo = new();
                        paginationComponent = new();
                        dataChargue = false;
                    }
                    else
                    {
                        IdDocumental = 0;
                        idAdUnit = 0;
                        IdproOffice = 0;
                        administrativeUnitEnable = false;
                        productionOfficeEnable = false;
                        administrativeUnitsList = new();
                        productionOfficesList = new();
                        codeFilter = "";
                        NameFilter = "";
                        await GetSeries();
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
            if (IdDocumental != 0 || idAdUnit != 0 || IdproOffice != 0 || !string.IsNullOrEmpty(codeFilter) || !string.IsNullOrEmpty(NameFilter))
            {
                IdDocumental = 0;
                idAdUnit = 0;
                IdproOffice = 0;
                administrativeUnitEnable = false;
                productionOfficeEnable = false;
                administrativeUnitsList = new();
                productionOfficesList = new();
                codeFilter = "";
                NameFilter = "";
                seriesList = new();
                dataChargue = false;
                //await GetSeries();
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
            if (IdDocumental <= 0 || idAdUnit <= 0 || IdproOffice <= 0)
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

        private async Task ShowModalMetaData(SeriesDtoResponse dtb)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            HttpClient?.DefaultRequestHeaders.Remove("seriesId");
            HttpClient?.DefaultRequestHeaders.Add("seriesId", $"{dtb.SeriesId}");
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<SubSeriesDtoResponse>>>("paramstrd/SubSeries/BySubSeries");
            HttpClient?.DefaultRequestHeaders.Remove("seriesId");

            if (deserializeResponse!.Succeeded && (deserializeResponse.Data != null) && deserializeResponse.Data.Count != 0)
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["ExistingSubSeriesMetaData"], true, Translation["Accept"], "");
            }
            else
            {
                await documentaryTypologiesBagMetaDataModal.UpdateModalStatus(true);
                await documentaryTypologiesBagMetaDataModal.GetmetaFieldsOfBag((dtb ?? new()).SeriesId);
                SeriesSelectedModalTitle = dtb.Name;
            }
            StateHasChanged();

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
            var request = new SeriesFilterDtoRequest() { DocumentalVersionId = IdDocumental, AdministrativeUnitId = idAdUnit, ProductionOfficeId = IdproOffice, Code = codeFilter, Name = NameFilter };

            var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/Series/ByFilterExcel", request);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<SeriesExcelDtoResponse>>>();

            if (deserializeResponse!.Succeeded)
            {
                seriesExcelList = deserializeResponse.Data!;
            }
            else
            {
                seriesExcelList = new();
            }
        }

        public async Task OnBeforeExcelExport(GridBeforeExcelExportEventArgs args)
        {
            if (ExportAllPages)
            {
                await GetDataExcel();

                if (seriesExcelList.Any())
                {
                    args.Data = seriesExcelList;
                }
            }
            else { args.Data = seriesList; }

            args.IsCancelled = CancelExport;
        }

        #endregion OnBeforeExcelExport

        #endregion OthersMethods

        #endregion Methods
    }
}