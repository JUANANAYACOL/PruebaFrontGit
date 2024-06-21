using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Profile.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
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
    public partial class ProductionOfficePage
    {
        #region Variables

        #region Inject

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private NavigationManager navigationManager { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Cascade

        [CascadingParameter]
        public FunctionalityToJson PermissionsList { get; set; } = new();

        #endregion Cascade

        #region Modals

        private DeleteGeneralDtoRequest deleteRequest { get; set; } = new();
        private ProductionOfficeFilterDtoRequest productionByFilter { get; set; } = new();
        private ProductionOfficeModal productionOfficeModal { get; set; } = new();

        private NotificationsComponentModal modalNotification { get; set; } = new();

        private GenericSearchModal genericSearchModal { get; set; } = new();

        #endregion Modals

        #region Models

        private MetaModel meta { get; set; } = new() { PageSize = 10 };
        private PaginationInfo paginationInfo = new();

        #endregion Models

        #region Componnet

        private NewPaginationComponent<ProductionOfficesDtoResponse, ProductionOfficeFilterDtoRequest> paginationComponent { get; set; } = new();

        #endregion Componnet

        #region Enviroments

        #region Environments(String)

        private string UriFilterProductionOffice { get; set; } = "paramstrd/ProductionOffice/ByFilter";
        private string codeFilter { get; set; } = "";
        private string NameFilter { get; set; } = "";

        #endregion Environments(String)

        #region Environments(Numeric)

        private int userIdToDelete { get; set; } = new();
        private int idAdUnit { get; set; } = 0;
        private int IdDocumental { get; set; } = 0;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool isEnable { get; set; } = true;
        private bool dataChargue { get; set; } = false;
        private bool administrativeUnitEnable { get; set; } = false;
        private bool saveIsDisable { get; set; } = true;
        public bool CancelExport { get; set; }
        private bool ExportAllPages { get; set; }

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        /*    private FunctionalityToJson PermissionsList { get; set; } = new();*/

        private List<DocumentalVersionDtoResponse> documentalVersionsList = new();

        private List<AdministrativeUnitsDtoResponse> administrativeUnitsList { get; set; } = new();
        private List<ProductionOfficesDtoResponse> productionOfficesList { get; set; } = new();
        private List<ProductionOfficesDtoResponse> productionOfficesExcelList { get; set; } = new();

        #endregion Environments(List & Dictionary)

        #endregion Enviroments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //await FillAdministrativeUnitDdl();
            await GetDocumentalVersions();
            StateHasChanged();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #region Modals

        #region ShowModal

        private async Task ShowModal()
        {
            await productionOfficeModal.UpdateModalStatus(true);
        }

        #endregion ShowModal

        #region ShowModalEdit

        private async Task ShowModalEdit(ProductionOfficesDtoResponse value)
        {
            await productionOfficeModal.UpdateModalStatus(true);
            await productionOfficeModal.UpdateSelectedRecord(value);
        }

        #endregion ShowModalEdit

        #region HandleStatusChanged

        private async Task HandleStatusChanged(bool status)
        {
            genericSearchModal.UpdateModalStatus(status);
        }

        #endregion HandleStatusChanged

        #region HandleStatusChangedUpdated

        private async Task HandleStatusChangedUpdated(bool status)
        {
            await productionOfficeModal.UpdateModalStatus(status);

            if (productionOfficesList.Count > 0)
            {
                await GetProductionOffices();
            }
        }

        #endregion HandleStatusChangedUpdated

        #region HandleUserSelectedChanged

        private void HandleUserSelectedChanged(MyEventArgs<VUserDtoResponse> user)
        {
            productionOfficeModal.updateBossSelection(user.Data);
            genericSearchModal.UpdateModalStatus(user.ModalStatus);
        }

        #endregion HandleUserSelectedChanged

        #endregion Modals

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted && args.ModalOrigin.Equals("DeleteModal"))
            {
                await DeleteProductionOffice(args);
            }
        }

        #endregion HandleModalNotiClose

        #region DeleteProductionOffice

        private void ShowModalDelete(ProductionOfficesDtoResponse value)
        {
            deleteRequest.Id = value.ProductionOfficeId;
            modalNotification.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, Translation["Yes"], Translation[Translation["No"]], modalOrigin: "DeleteModal");
        }

        public async Task DeleteProductionOffice(ModalClosedEventArgs args)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/ProductionOffice/DeleteProductionOffice", deleteRequest);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
            if (deserializeResponse!.Succeeded)
            {
                await GetProductionOffices();
                modalNotification.UpdateModal(ModalType.Success, Translation["DeleteSuccessfulMessage"], true, Translation["Accept"]);
            }
            else
            {
                modalNotification.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true, Translation["Accept"]);
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion DeleteProductionOffice

        #region HandlePaginationGrid

        private void HandlePaginationGrid(List<ProductionOfficesDtoResponse> newDataList)
        {
            productionOfficesList = newDataList;
        }

        #endregion HandlePaginationGrid

        #region Permissions

        private async Task GetPermissions()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                string nameView = navigationManager.Uri.Remove(0, navigationManager.BaseUri.Length);
                HttpClient?.DefaultRequestHeaders.Add("viewName", nameView);
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FunctionalityToJson>>("access/ViewsFuncionality/FuncionalityPerViewName");
                HttpClient?.DefaultRequestHeaders.Remove("viewName");

                if (deserializeResponse!.Succeeded)
                {
                    PermissionsList = deserializeResponse.Data ?? new();
                }
                else
                {
                    PermissionsList = new();

                    NavigationManager?.NavigateTo("404");
                }

                StateHasChanged();
            }
            catch
            {
                NavigationManager?.NavigateTo("404");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion Permissions

        #region GetMethods

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
                    modalNotification.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
                }
            }
            catch
            {
                modalNotification.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
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
                administrativeUnitsList = new();

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
                        administrativeUnitEnable = false;
                    }
                }
            }
            catch
            {
                modalNotification.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetAdministrativeUnits

        #region GetProductionOffices

        private async Task GetProductionOffices()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            productionByFilter = new();
            productionByFilter.AdministrativeUnitId = idAdUnit;
            var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/ProductionOffice/ByFilter", productionByFilter);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<ProductionOfficesDtoResponse>>>();
            if (deserializeResponse!.Succeeded && (deserializeResponse.Data != null && deserializeResponse.Data.Data != null))
            {
                productionOfficesList = deserializeResponse.Data.Data ?? new();
                paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                paginationComponent.ResetPagination(paginationInfo);
                dataChargue = true;
            }
            else
            {
                productionOfficesList = new();
                paginationInfo = new();
                paginationComponent.ResetPagination(paginationInfo);
                dataChargue = false;
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetProductionOffices

        #region FillAdministrativeUnitDdl

        private async Task FillAdministrativeUnitDdl()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");
            HttpClient?.DefaultRequestHeaders.Add("documentalVersionsId", "0");
            var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeUnitsDtoResponse>>>("paramstrd/AdministrativeUnit/ByAdministrativeUnits");
            HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");

            if (deserializeResponse!.Succeeded)
            {
                administrativeUnitsList = deserializeResponse.Data ?? new();
                dataChargue = false;
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion FillAdministrativeUnitDdl

        #endregion GetMethods

        #region Filters

        #region ApplyFilters

        private async Task ApplyFiltersAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            if (IdDocumental == 0 && idAdUnit == 0 && string.IsNullOrEmpty(codeFilter) && string.IsNullOrEmpty(NameFilter))
            {
                await GetProductionOffices();
            }
            else
            {
                try
                {
                    ProductionOfficeFilterDtoRequest request = new() { DocumentalVersionId = IdDocumental, AdministrativeUnitId = idAdUnit, Code = codeFilter, Name = NameFilter };
                    productionByFilter = request;
                    var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterProductionOffice, productionByFilter);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<ProductionOfficesDtoResponse>>>();

                    if (deserializeResponse!.Succeeded && deserializeResponse.Data != null && deserializeResponse.Data.TotalCount > 0)
                    {
                        productionOfficesList = deserializeResponse.Data.Data;
                        paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                        paginationComponent.ResetPagination(paginationInfo);
                        dataChargue = true;
                    }
                    else if (deserializeResponse!.Succeeded && deserializeResponse.Data == null)
                    {
                        productionOfficesList = new();
                        paginationInfo = new();
                        paginationComponent = new();
                        dataChargue = false;
                    }
                    else
                    {
                        IdDocumental = 0;
                        idAdUnit = 0;
                        administrativeUnitEnable = false;
                        administrativeUnitsList = new();
                        codeFilter = "";
                        NameFilter = "";
                        await GetProductionOffices();
                        modalNotification.UpdateModal(ModalType.Error, Translation["NoCoincidences"], true, Translation["Accept"]);
                    }
                }
                catch
                {
                    modalNotification.UpdateModal(ModalType.Error, Translation["ErrorFiltering"], true, Translation["Accept"]);
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
            if (IdDocumental != 0 || idAdUnit != 0 || !string.IsNullOrEmpty(codeFilter) || !string.IsNullOrEmpty(NameFilter))
            {
                IdDocumental = 0;
                idAdUnit = 0;
                administrativeUnitEnable = false;
                administrativeUnitsList = new();
                codeFilter = "";
                NameFilter = "";
                productionOfficesList = new();
                dataChargue = false;
                //await GetProductionOffices();
            }
            else
            {
                modalNotification.UpdateModal(ModalType.Error, Translation["AtLeastOne4Clean"], true, Translation["Accept"]);
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
            if (IdDocumental <= 0 || idAdUnit <= 0)
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
            var request = new ProductionOfficeFilterDtoRequest() { DocumentalVersionId = IdDocumental, AdministrativeUnitId = idAdUnit, Code = codeFilter, Name = NameFilter };
            var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/ProductionOffice/ByFilterExcel", request);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<ProductionOfficesDtoResponse>>>();

            if (deserializeResponse!.Succeeded)
            {
                productionOfficesExcelList = deserializeResponse.Data!;
                productionOfficesExcelList.ForEach(x => { x.BossName = string.IsNullOrEmpty(x.BossName) ? Translation["NoAssignedBoss"] : x.BossName; });
            }
            else
            {
                productionOfficesExcelList = new();
            }
        }

        public async Task OnBeforeExcelExport(GridBeforeExcelExportEventArgs args)
        {
            if (ExportAllPages)
            {
                await GetDataExcel();

                if (productionOfficesExcelList.Any())
                {
                    args.Data = productionOfficesExcelList;
                }
            }
            else
            {
                productionOfficesList.ForEach(x => { x.BossName = string.IsNullOrEmpty(x.BossName) ? Translation["NoAssignedBoss"] : x.BossName; });
                args.Data = productionOfficesList;
            }

            args.IsCancelled = CancelExport;
        }

        #endregion OnBeforeExcelExport

        #endregion Methods
    }
}