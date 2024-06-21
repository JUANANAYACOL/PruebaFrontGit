using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Pages.Administration.AdministracionTRD
{
    public partial class DocumentaryTypologyPage
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

        private NotificationsComponentModal notificationModal { get; set; } = new();

        private DocumentaryTypologyBehaviorModal modalDocumentaryTypologyBehavior = new();
        private DocumentaryTypologyModal documentaryTypologyModal { get; set; } = new();

        #endregion Components

        #region Models

        private PaginationInfo paginationInfo = new();
        private NewPaginationComponent<DocumentaryTypologiesNameDtoResponse, DocumentaryTypologiesFilterNameDtoRequest> paginationComponent = new();
        public DocumentaryTypologiesFilterNameDtoRequest filterVDocTypology { get; set; } = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string UriFilterTypology = "documentarytypologies/DocumentaryTypologies/ByFilterName";
        private string titleDT { get; set; } = string.Empty;

        #endregion Environments(String)

        #region Environments(Numeric)

        private int idDocVersion { get; set; } = new();
        private int idAdminUnit { get; set; } = new();
        private int idProOffice { get; set; } = new();
        private int idSerie { get; set; } = new();
        private int idSubSerie { get; set; } = new();
        private int idDocTypologiesBag { get; set; } = new();

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool SubSerieSpan { get; set; } = false;
        private bool isEnableAdminUnit = false;
        private bool isEnableProOffice = false;
        private bool isEnableSerie = false;
        private bool isEnableSubSerie { get; set; } = false;
        private bool isEnableDocTypology = false;
        private bool isVisibleTypologyNameInput = false;
        private bool isEnableButton = true;
        private bool saveIsDisable { get; set; } = true;

        private bool dataChargue = false;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        public List<DocumentaryTypologiesDtoResponse> documentalVersionsList { get; set; } = new();

        private List<DocumentaryTypologiesNameDtoResponse> docTypologyList { get; set; } = new();

        private List<DocumentaryTypologiesNameDtoResponse> docTypologiesBagList { get; set; } = new();
        private List<DocumentalVersionDtoResponse> docVersionList { get; set; } = new();
        private List<AdministrativeUnitsDtoResponse> adminUnitList { get; set; } = new();
        private List<ProductionOfficesDtoResponse> proOfficesList { get; set; } = new();
        private List<SubSeriesDtoResponse> subSeriesList { get; set; } = new();
        private List<SeriesDtoResponse> seriesList { get; set; } = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetDocumentalVersions();
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

        private async Task ShowModal()
        {
            titleDT = Translation["CreateDocumentTypology"];
            await documentaryTypologyModal.UpdateModalStatus(true);
        }

        private async Task ShowModalEdit(DocumentaryTypologiesNameDtoResponse record)
        {
            titleDT = Translation["EditDocumentaryTypology"];
            await documentaryTypologyModal.updateDocumentaryTypology(record);
            await documentaryTypologyModal.UpdateModalStatus(true);
        }

        private async Task ShowModalDocumetaryTB(DocumentaryTypologiesNameDtoResponse record)
        {
            modalDocumentaryTypologyBehavior.UpdateModalStatus(true);
            await modalDocumentaryTypologyBehavior.UpdateModalIdAsync((int)record.DocumentaryTypologiesId);
        }

        #endregion Modal

        #region HandlePaginationGrid

        private async Task HandlePaginationGridAsync(List<DocumentaryTypologiesNameDtoResponse> newDataList)
        {
            docTypologyList = newDataList;
            foreach (var item in docTypologyList)
            {
                item.CreateUser = item.CreateUser != null ? item.CreateUser : Translation["NotRegistered"];
            }
        }

        #endregion HandlePaginationGrid

        #region HandleModalNotiClose

        private async Task HandleModalNotiCloseAsync(ModalClosedEventArgs args)
        {
            /*try
            {
                if (args.IsAccepted && args.ModalOrigin!.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new() { Id = versionToDelete.DocumentalVersionId, User = "user" };

                    var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/DocumentalVersions/DeleteDocumentalVersion", request);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse!.Succeeded)
                    {
                        await HandleRefreshGridData(true);
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            notificationModal.UpdateModal(ModalType.Success, "¡Se ha eliminado el registro de forma exitosa!", true, Translation["Accept"]);
                        }
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, "¡Se presentó un error a la hora de eliminar el registro, por favor intente de nuevo!", true, Translation["Accept"]);
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }*/
        }

        #endregion HandleModalNotiClose

        #region Gets

        #region EnableField

        public void EnableField(bool a, bool b, bool c, bool d, bool e, bool f, bool g)
        {
            isEnableAdminUnit = a;
            isEnableProOffice = b;
            isEnableSerie = c;
            isEnableSubSerie = d;
            isEnableDocTypology = e;
            isVisibleTypologyNameInput = f;
            isEnableButton = g;

            idAdminUnit = a ? idAdminUnit : 0;
            idProOffice = b ? idProOffice : 0;
            idSerie = c ? idSerie : 0;
            idSubSerie = d ? idSubSerie : 0;
            idDocTypologiesBag = e ? idDocTypologiesBag : 0;
        }

        #endregion EnableField

        #region GetDocumentalVersions

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
                    docVersionList.ForEach(x =>
                    {
                        x.Code = $"{((x.EndDate == null || x.EndDate.Value > DateTime.Now) ? $"{x.StartDate!.Value.Year} {toCurrent} - {documentaryVersion} {x.Code}" : $"{x.StartDate!.Value.Year} a {x.EndDate.Value.Year} - {documentaryVersion} {x.Code} {x.VersionType}")}";
                    });
                    docVersionList = docVersionList.OrderBy(x => x.StartDate).Reverse().ToList();
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ErrorGettingRecords"], true, Translation["Accept"]);
            }
        }

        #endregion GetDocumentalVersions

        #region GetAdministrativeUnits

        public async Task GetAdministrativeUnits(int id)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                idDocVersion = id;
                idAdminUnit = 0;
                adminUnitList = new();
                proOfficesList = new();
                EnableField(false, false, false, false, false, false, true);

                if (idDocVersion != 0)
                {
                    await GetDocTypologies();
                    HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");
                    HttpClient?.DefaultRequestHeaders.Add("documentalVersionsId", $"{idDocVersion}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeUnitsDtoResponse>>>("paramstrd/AdministrativeUnit/ByAdministrativeUnits");
                    HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");

                    if (deserializeResponse!.Succeeded && (deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null))
                    {
                        adminUnitList = deserializeResponse.Data!;
                        EnableField(true, false, false, false, false, false, true);
                    }
                    else
                    {
                        EnableField(false, false, false, false, false, false, true);
                    }
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ErrorGettingRecords"] + " " + Translation["AdministrativeUnits"] , true, Translation["Accept"]);
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetAdministrativeUnits

        #region GetProducOffice

        public async Task GetProducOffice(int id)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                idAdminUnit = id;
                idProOffice = 0;
                proOfficesList = new();
                seriesList = new();

                EnableField(true, false, false, false, false, false, true);

                if (idAdminUnit != 0)
                {
                    await GetDocTypologies();

                    HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                    HttpClient?.DefaultRequestHeaders.Add("AdministrativeUnitId", $"{idAdminUnit}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<ProductionOfficesDtoResponse>>>("paramstrd/ProductionOffice/ByProductionOffices");
                    HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");

                    if (deserializeResponse!.Succeeded && (deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null))
                    {
                        proOfficesList = deserializeResponse.Data!;
                        EnableField(true, true, false, false, false, false, true);
                    }
                    else
                    {
                        EnableField(true, false, false, false, false, false, true);
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ErrorGettingRecords"] + " (" + Translation["ProductionOffices"] + $"): {ex.Message}", true, Translation["Accept"]);
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetProducOffice

        #region GetSeries

        public async Task GetSeries(int id)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                idProOffice = id;
                idSerie = 0;
                seriesList = new();
                subSeriesList = new();
                EnableField(true, true, false, false, false, false, true);

                if (idProOffice != 0)
                {
                    await GetDocTypologies();

                    HttpClient?.DefaultRequestHeaders.Remove("productionOfficeId");
                    HttpClient?.DefaultRequestHeaders.Add("productionOfficeId", $"{idProOffice}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<SeriesDtoResponse>>>("paramstrd/Series/BySeries");
                    HttpClient?.DefaultRequestHeaders.Remove("productionOfficeId");

                    if (deserializeResponse!.Succeeded && (deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null))
                    {
                        seriesList = deserializeResponse.Data!;
                        EnableField(true, true, true, false, false, true, true);
                    }
                    else
                    {
                        EnableField(true, true, false, false, false, false, true);
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ErrorGettingRecords"] + " (" + Translation["Series"] + $"): {ex.Message}", true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetSeries

        #region GetSubSeries

        public async Task GetSubSeries(int id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                idSerie = id;
                subSeriesList = new();
                idSubSerie = 0;

                EnableField(true, true, true, false, false, true, true);
                if (idSerie != 0)
                {
                    await GetDocTypologies();

                    HttpClient?.DefaultRequestHeaders.Remove("seriesId");
                    HttpClient?.DefaultRequestHeaders.Add("seriesId", $"{idSerie}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<SubSeriesDtoResponse>>>("paramstrd/SubSeries/BySubSeries");
                    HttpClient?.DefaultRequestHeaders.Remove("seriesId");

                    if (deserializeResponse!.Succeeded && (deserializeResponse.Data != null))
                    {
                        subSeriesList = deserializeResponse.Data!;

                        if (subSeriesList.Any())
                        {
                            SubSerieSpan = true;
                            EnableField(true, true, true, true, false, true, true);
                        }
                        else
                        {
                            SubSerieSpan = false;
                            EnableField(true, true, true, false, true, true, true);
                            await GetDocTypologiesBySubSerieId(idSubSerie, idSerie);
                        }
                        EnableSaveButton();
                    }
                    else
                    {
                        EnableField(true, true, true, false, false, true, true);
                    }
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ErrorGettingRecords"] + " (" + Translation["SubSeries"] + $"): {ex.Message}", true, Translation["Accept"]);

                SpinnerLoaderService.HideSpinnerLoader(Js);
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetSubSeries

        #region GetDocTypologiesBySubSerieId

        public async Task GetDocTypologiesBySubSerieId(int idSubserie, int idSerie)
        {
            try
            {
                idSubSerie = idSubserie;

                filterVDocTypology = new()
                {
                    DocumentalVersionId = idDocVersion,
                    AdministrativeUnitId = idAdminUnit,
                    ProductionOfficeId = idProOffice,
                    SeriesId = idSerie,
                    SubseriesId = idSubserie,
                };

                var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterTypology, filterVDocTypology);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<DocumentaryTypologiesNameDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                {
                    docTypologiesBagList = deserializeResponse.Data.Data!.DistinctBy(x => x.DocumentaryTypologyBagId).ToList();
                    if (subSeriesList.Any())
                    {
                        EnableField(true, true, true, true, true, true, true);
                    }
                    else
                    {
                        EnableField(true, true, true, false, true, true, true);
                    }
                }
                else
                {
                    if (subSeriesList.Any())
                    {
                        EnableField(true, true, true, true, false, true, true);
                    }
                    else
                    {
                        EnableField(true, true, true, false, false, true, true);
                    }
                }

                EnableSaveButton();
            }
            catch

            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ErrorGettingRecords"] +" " +Translation["DocumentaryTypologies"], true, Translation["Accept"]);
            }
        }

        #endregion GetDocTypologiesBySubSerieId

        #region GetDocTypologies

        public async Task GetDocTypologies()
        {
            try
            {
                filterVDocTypology = new()
                {
                    DocumentalVersionId = idDocVersion,
                    AdministrativeUnitId = idAdminUnit,
                    ProductionOfficeId = idProOffice,
                    SeriesId = idSerie,
                    SubseriesId = idSubSerie,
                };

                var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterTypology, filterVDocTypology);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<DocumentaryTypologiesNameDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                {
                    docTypologiesBagList = deserializeResponse.Data.Data!.DistinctBy(x => x.DocumentaryTypologyBagId).ToList();
                }
                else
                {
                    docTypologiesBagList = new();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener Officinas Productoras: {ex.Message}");
            }
        }

        #endregion GetDocTypologies

        #endregion Gets

        #region Filters

        #region ResetFilter

        public async Task ResetFiltersAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            if (idDocVersion != 0 || idAdminUnit != 0 || idProOffice != 0 || idSerie != 0 || idSubSerie != 0 || idDocTypologiesBag != 0)
            {
                idDocVersion = 0;
                idAdminUnit = 0;
                idProOffice = 0;
                idSerie = 0;
                idSubSerie = 0;
                idDocTypologiesBag = 0;
                /*docVersionList = new();
                adminUnitList = new();
                proOfficesList = new();
                seriesList = new();
                subSeriesList = new();
                docTypologiesBagList = new();*/
                isEnableAdminUnit = false;
                isEnableProOffice = false;
                isEnableSerie = false;
                isEnableSubSerie = false;
                isEnableDocTypology = false;
                docTypologyList = new();
                docTypologiesBagList = new();
                dataChargue = false;
                paginationComponent = new();
                filterVDocTypology = new();

                dataChargue = false;
                //await GetDocumentalVersions();
                //StateHasChanged();
                //await ApplyFiltersAsync();
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["AtLeastOne4Clean"], true, Translation["Accept"]);
            }

            EnableSaveButton();
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #region OnClickButton

        public async Task ApplyFiltersAsync()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                filterVDocTypology = new()
                {
                    DocumentalVersionId = idDocVersion,
                    AdministrativeUnitId = idAdminUnit,
                    ProductionOfficeId = idProOffice,
                    SeriesId = idSerie,
                    SubseriesId = idSubSerie,
                    DocumentaryTypologyBagId = idDocTypologiesBag
                };

                var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterTypology, filterVDocTypology);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<DocumentaryTypologiesNameDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                {
                    docTypologyList = deserializeResponse.Data.Data!;

                    foreach (var item in docTypologyList)
                    {
                        item.CreateUser = item.CreateUser != null ? item.CreateUser : Translation["NotRegistered"];
                    }

                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                    dataChargue = true;
                }
                else
                {
                    docTypologyList = new();
                    dataChargue = false;
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ErrorFiltering"], true, Translation["Accept"]);
            }
        }

        #endregion OnClickButton

        #endregion ResetFilter

        #endregion Filters

        #region HandleStatusChanged

        private async Task HandleStatusChanged(bool status)
        {
            //await documentaryTypologyModal.UpdateModalStatus(status);

            if (idSerie > 0 || idSubSerie > 0)
            {
                await GetDocTypologiesBySubSerieId(idSubSerie, idSerie);
                EnableSaveButton();
            }

            if (docTypologyList.Count > 0)
            {
                await ApplyFiltersAsync();
            }

        }

        #endregion HandleStatusChanged

        #region EnableSaveButton

        public void EnableSaveButton()
        {
            saveIsDisable = idDocVersion <= 0 || idAdminUnit <= 0 || idProOffice <= 0 || (subSeriesList.Any() && idSerie <= 0) || idDocTypologiesBag <= 0 || (subSeriesList.Any() && idSubSerie <= 0);

            StateHasChanged();
        }

        #endregion EnableSaveButton

        #endregion OthersMethods

        #endregion Methods
    }
}