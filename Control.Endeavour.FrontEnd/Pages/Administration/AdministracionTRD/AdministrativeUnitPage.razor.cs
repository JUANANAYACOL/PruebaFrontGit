using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Request;
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
    public partial class AdministrativeUnitPage
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

        private NewPaginationComponent<AdministrativeUnitsDtoResponse, AdministrativeUnitFilterDtoRequest> paginationComponent = new();

        #endregion Components

        #region Modals

        private GenericSearchModal userSearchModal = new();
        private AdministrativeUnitModal modalAdministrativeUnit { get; set; } = new();
        private NotificationsComponentModal notificationModal = new();

        #endregion Modals

        #region Models

        private PaginationInfo paginationInfo = new();
        private AdministrativeUnitsDtoResponse recordToDelete = new();
        private AdministrativeUnitFilterDtoRequest administrativeUnitFilter = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string UriFilterAdministrativeUnit = "paramstrd/AdministrativeUnit/ByFilter";
        private string codeFilter { get; set; } = "";
        private string NameFilter { get; set; } = "";

        #endregion Environments(String)

        #region Environments(Numeric)

        private int IdDocumental { get; set; } = 0;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool dataChargue { get; set; } = false;
        private bool saveIsDisable { get; set; } = true;
        public bool CancelExport { get; set; }
        bool ExportAllPages { get; set; }

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<AdministrativeUnitsDtoResponse> administrativeUnitList { get; set; } = new();
        private List<AdministrativeUnitsDtoResponse> administrativeUnitExcelList { get; set; } = new();
        private List<DocumentalVersionDtoResponse> documentalVersionsList { get; set; } = new();

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
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadDocumentalVersionErrorMessage"], true);
            }
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        #region Language

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion Language

        #region HandlePaginationGrid

        private void HandlePaginationGrid(List<AdministrativeUnitsDtoResponse> newDataList)
        {
            administrativeUnitList = newDataList;
        }

        #endregion HandlePaginationGrid

        #endregion HandleMethods

        #region OthersMethods

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

        private async Task GetAdministrativeUnits()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                administrativeUnitFilter = new();
                administrativeUnitFilter.DocumentalVersionId = IdDocumental;
                var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterAdministrativeUnit, administrativeUnitFilter);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<AdministrativeUnitsDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                {
                    administrativeUnitList = deserializeResponse.Data.Data;
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                    dataChargue = true;
                }
                else
                {
                    administrativeUnitList = new();
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetAdministrativeUnits

        #endregion GetDataMethods

        #region Modals

        #region ShowModalEdit

        private async Task ShowModalEdit(AdministrativeUnitsDtoResponse record)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            await modalAdministrativeUnit.UpdateSelectedRecord(record);
            await modalAdministrativeUnit.UpdateModalStatusAsync(true);
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion ShowModalEdit

        #region ShowModalCreate

        private async Task ShowModalCreate()
        {
            await modalAdministrativeUnit.PreparedModal();
            await modalAdministrativeUnit.UpdateModalStatusAsync(true);
        }

        #endregion ShowModalCreate

        #region DeleteAdministrativeUnit

        #region ShowModalDelete

        private void ShowModalDelete(AdministrativeUnitsDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, Translation["Yes"], Translation[Translation["No"]], modalOrigin: "DeleteModal");
        }

        #endregion ShowModalDelete

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                if (args.IsAccepted && args.ModalOrigin.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new();
                    request.Id = recordToDelete.AdministrativeUnitId;
                    request.User = "Front"; // Cambiar por la varibale de session del usuario
                    var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/AdministrativeUnit/DeleteAdministrativeUnit", request);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse!.Succeeded)
                    {
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            await GetAdministrativeUnits();
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

        #endregion HandleModalNotiClose

        #endregion DeleteAdministrativeUnit

        #region HandleChangedData

        private async Task HandleChangedData(bool changed)
        {
            //await modalAdministrativeUnit.UpdateModalStatusAsync(changed);

            if (administrativeUnitList.Count > 0)
            {
                await GetAdministrativeUnits();
            }
        }

        #endregion HandleChangedData

        #endregion Modals

        #region Filters

        #region ApplyFilters

        private async Task ApplyFiltersAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            if (IdDocumental == 0 && string.IsNullOrEmpty(codeFilter) && string.IsNullOrEmpty(NameFilter))
            {
                await GetAdministrativeUnits();
            }
            else
            {
                try
                {
                    AdministrativeUnitFilterDtoRequest request = new() { DocumentalVersionId = IdDocumental, Code = codeFilter, Name = NameFilter };
                    administrativeUnitFilter = request;
                    var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterAdministrativeUnit, administrativeUnitFilter);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<AdministrativeUnitsDtoResponse>>>();

                    if (deserializeResponse!.Succeeded && deserializeResponse.Data != null && deserializeResponse.Data.TotalCount > 0)
                    {
                        administrativeUnitList = deserializeResponse.Data.Data;
                        paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                        paginationComponent.ResetPagination(paginationInfo);
                        dataChargue = true;
                    }
                    else if (deserializeResponse!.Succeeded && deserializeResponse.Data == null)
                    {
                        administrativeUnitList = new();
                        paginationInfo = new();
                        paginationComponent = new();
                        dataChargue = false;
                    }
                    else
                    {
                        IdDocumental = 0;
                        codeFilter = "";
                        NameFilter = "";
                        notificationModal.UpdateModal(ModalType.Error, Translation["NoCoincidences"], true, Translation["Accept"]);
                        await GetAdministrativeUnits();
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
            if (IdDocumental != 0 || !string.IsNullOrEmpty(codeFilter) || !string.IsNullOrEmpty(NameFilter))
            {
                IdDocumental = 0;
                codeFilter = "";
                NameFilter = "";
                administrativeUnitList = new();
                dataChargue = false;
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
            if (IdDocumental <= 0)
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

        #region OnBeforeExcelExport

        public async Task GetDataExcel()
        {
            var request = new AdministrativeUnitFilterDtoRequest() { DocumentalVersionId = IdDocumental, Code = codeFilter, Name = NameFilter };
            var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/AdministrativeUnit/ByFilterExcel", request);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeUnitsDtoResponse>>>();

            if (deserializeResponse!.Succeeded)
            {
                administrativeUnitExcelList = deserializeResponse.Data!;
            }
            else
            {
                administrativeUnitExcelList = new();
            }
        }

        public async Task OnBeforeExcelExport(GridBeforeExcelExportEventArgs args)
        {
            if (ExportAllPages)
            {
                await GetDataExcel();

                if (administrativeUnitExcelList.Any())
                {
                    args.Data = administrativeUnitExcelList;
                }
            }
            else { args.Data = administrativeUnitList; }

            args.IsCancelled = CancelExport;
        }

        #endregion OnBeforeExcelExport

        #endregion OthersMethods

        #endregion Methods
    }
}