using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VBehaviorTypology.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VBehaviorTypology.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VDocumentaryTypology.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VDocumentaryTypology.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Generic.TypologySearch
{
    public partial class GenericDocTypologySearchModal
    {
        #region Variables

        #region Inject

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Components
        private NewPaginationComponent<VDocumentaryTypologyDtoResponse, VDocumentaryTypologyDtoRequest> paginationComponent = new();
        private NotificationsComponentModal notificationModal { get; set; } = new();
        public InputModalComponent typologyNameInput { get; set; } = new();

        #endregion Components

        #region Parameters

        [Parameter] public EventCallback<MyEventArgs<VDocumentaryTypologyDtoResponse>> OnStatusChanged { get; set; }
        [Parameter] public string title { get; set; } = "";

        #endregion Parameters

        #region Models
        private PaginationInfo paginationInfo = new();
        public VDocumentaryTypologyDtoResponse vDocumentary { get; set; } = new();
        public VDocumentaryTypologyDtoRequest filterVDocTypology { get; set; } = new() { Radication=true};

        private VBehaviorTypologyDtoRequest fitlerBehaviourTypologyRequest { get; set; } = new();

        private List<VDocumentaryTypologyDtoResponse> docTypologyList { get; set; } = new();
        private List<VDocumentaryTypologyDtoResponse> docTypologiesList { get; set; } = new();
        private List<DocumentalVersionDtoResponse> docVersionList { get; set; } = new();
        private List<AdministrativeUnitsDtoResponse> adminUnitList { get; set; } = new();
        private List<ProductionOfficesDtoResponse> proOfficesList { get; set; } = new();
        private List<SubSeriesDtoResponse> subSeriesList { get; set; } = new();
        private List<SeriesDtoResponse> seriesList { get; set; } = new();
        private MetaModel meta { get; set; } = new() { PageSize = 10 };

        #endregion Models

        #region Environments

        private bool modalStatus { get; set; } = false;
        private int idDocVersion { get; set; } = new();
        private int idAdminUnit { get; set; } = new();
        private int idProOffice { get; set; } = new();
        private int idSerie { get; set; } = new();
        private int idSubSerie { get; set; } = new();
        private int idDocTypologies { get; set; } = new();
        private bool isEnableAdminUnit = false;
        private bool isEnableProOffice = false;
        private bool isEnableSerie = false;
        private bool isEnableSubSerie = false;
        private bool isEnableDocTypology = false;
        private bool isVisibleTypologyNameInput = false;
        private bool isEnableButton = true;
        private string UriFilterTypology = "generalviews/VDocumentaryTypology/ByFilter";
        private string ClassCodeVar = string.Empty;

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        private void HandlePaginationGrid(List<VDocumentaryTypologyDtoResponse> newDataList)
        {
            docTypologyList = newDataList;
        }

        #region UpdateModalStatus

        public async void UpdateModalStatus(bool newValue, string classCode = "")
        {
            modalStatus = newValue;
            ClassCodeVar = classCode;
            await GetDocumentalVersions();
            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region HandleModalClosed

        private async Task HandleModalClosed(bool status)
        {
            modalStatus = status;
            await resetModal();
        }

        #endregion HandleModalClosed

        #region GetDocumentalVersions

        public async Task GetDocumentalVersions()
        {
            try
            {
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<DocumentalVersionDtoResponse>>>("paramstrd/DocumentalVersions/ByDocumentalVersions");
                if (deserializeResponse!.Succeeded)
                {
                    docVersionList = deserializeResponse.Data!;
                    docVersionList = docVersionList.Where(x => x.EndDate > DateTime.Now || x.EndDate == null).ToList();
                    docVersionList.ForEach(x =>
                    {
                        x.Code = $"{x.StartDate!.Value.Year} a Actual - Version Documental {x.Code}";
                    });
                    meta = deserializeResponse.Meta ?? new() { PageSize = 10 };
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["ErrorGettingRecords"]}", true, Translation["Accept"]);
            }
        }

        #endregion GetDocumentalVersions

        #region GetAdministrativeUnits

        public async Task GetAdministrativeUnits(int id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                idDocVersion = id;
                adminUnitList = new();
                idAdminUnit = 0;

                await GetDocTypologies();
                HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");
                HttpClient?.DefaultRequestHeaders.Add("documentalVersionsId", $"{idDocVersion}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeUnitsDtoResponse>>>("paramstrd/AdministrativeUnit/ByAdministrativeUnits");
                HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");

                if (deserializeResponse!.Succeeded && ( deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null ))
                {
                    adminUnitList = deserializeResponse.Data!;
                    EnableField(true, false, false, false, false, false, true);
                    meta = deserializeResponse.Meta!;
                }
                else
                {
                    EnableField(false, false, false, false, false, false, true);
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["ErrorGettingRecords"]}", true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetAdministrativeUnits

        #region GetProducOffice

        public async Task GetProducOffice(int id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                idAdminUnit = id;
                proOfficesList = new();
                idProOffice = 0;

                await GetDocTypologies();

                HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                HttpClient?.DefaultRequestHeaders.Add("AdministrativeUnitId", $"{idAdminUnit}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<ProductionOfficesDtoResponse>>>("paramstrd/ProductionOffice/ByProductionOffices");
                HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");

                if (deserializeResponse!.Succeeded && ( deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null ))
                {
                    proOfficesList = deserializeResponse.Data!;
                    EnableField(true, true, false, false, false, false, true);
                    meta = deserializeResponse.Meta!;
                }
                else
                {
                    EnableField(true, false, false, false, false, false, true);
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["ErrorGettingRecords"]}", true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetProducOffice

        #region GetSeries

        public async Task GetSeries(int id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                idProOffice = id;
                seriesList = new();
                idSerie = 0;

                await GetDocTypologies();

                HttpClient?.DefaultRequestHeaders.Remove("productionOfficeId");
                HttpClient?.DefaultRequestHeaders.Add("productionOfficeId", $"{idProOffice}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<SeriesDtoResponse>>>("paramstrd/Series/BySeries");
                HttpClient?.DefaultRequestHeaders.Remove("productionOfficeId");

                if (deserializeResponse!.Succeeded && ( deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null ))
                {
                    seriesList = deserializeResponse.Data!;
                    EnableField(true, true, true, false, false, true, false);
                    meta = deserializeResponse.Meta!;
                }
                else
                {
                    EnableField(true, true, false, false, false, false, false);
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["ErrorGettingRecords"]}", true, Translation["Accept"]);
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

                await GetDocTypologies();

                HttpClient?.DefaultRequestHeaders.Remove("seriesId");
                HttpClient?.DefaultRequestHeaders.Add("seriesId", $"{idSerie}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<SubSeriesDtoResponse>>>("paramstrd/SubSeries/BySubSeries");
                HttpClient?.DefaultRequestHeaders.Remove("seriesId");

                if (deserializeResponse!.Succeeded && ( deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null ))
                {
                    subSeriesList = deserializeResponse.Data!;
                    EnableField(true, true, true, true, false, true, false);
                    meta = deserializeResponse.Meta!;
                }
                else
                {
                    EnableField(true, true, true, false, false, true, false);
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["ErrorGettingRecords"]}", true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetSubSeries

        #region GetDocTypologiesBySubSerieId

        public async Task GetDocTypologiesBySubSerieId(int id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                idSubSerie = id;
                docTypologiesList = new();
                idDocTypologies = 0;

                filterVDocTypology = new()
                {
                    DocumentalVersionId = idDocVersion,
                    AdministrativeUnitId = idAdminUnit,
                    ProductionOfficeId = idProOffice,
                    SeriesId = idSerie,
                    SubSeriesId = idSubSerie,
                    Radication = true
                };
                if (!string.IsNullOrEmpty(ClassCodeVar))
                {
                    filterVDocTypology.ClassCode = ClassCodeVar;
                }

                var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterTypology, filterVDocTypology);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<VDocumentaryTypologyDtoResponse>>>();
                if (deserializeResponse!.Succeeded)
                {
                    docTypologiesList = deserializeResponse.Data.Data!;
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                    EnableField(true, true, true, true, true, true, false);
                    //meta = deserializeResponse.Meta!;
                }
                else
                {
                    EnableField(true, true, true, true, false, true, false);
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["ErrorGettingRecords"]}", true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetDocTypologiesBySubSerieId

        #region OnClickButton

        public async Task OnClickButton()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                string tipologyToSearch = (string)filterVDocTypology.TypologyName;

                filterVDocTypology = new()
                {
                    DocumentalVersionId = idDocVersion,
                    AdministrativeUnitId = idAdminUnit,
                    ProductionOfficeId = idProOffice,
                    SeriesId = idSerie,
                    SubSeriesId = idSubSerie,
                    DocumentaryTypologyId = idDocTypologies,
                    TypologyName = tipologyToSearch,
                    Radication=true
                };
                if (!string.IsNullOrEmpty(ClassCodeVar))
                {
                    filterVDocTypology.ClassCode = ClassCodeVar;
                }

                var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterTypology, filterVDocTypology);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<VDocumentaryTypologyDtoResponse>>>();
                if (deserializeResponse!.Succeeded && ( deserializeResponse.Data.Data?.Count != 0 || deserializeResponse.Data != null ))
                {
                    docTypologyList = deserializeResponse.Data.Data!;
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                    //meta = deserializeResponse.Meta!;
                }
                else
                {
                    docTypologyList = new();
                    //meta = new() { PageSize = 10 };
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["ErrorGettingRecords"]}", true, Translation["Accept"]);
            }
        }

        #endregion OnClickButton

        #region SelectDocTypology

        private void SelectDocTypology(VDocumentaryTypologyDtoResponse docTypology)
        {
            vDocumentary = docTypology;
            var gestor = vDocumentary.LmfullName;
            var tipologia = vDocumentary.TypologyName;

            if (vDocumentary != null)
            {
                notificationModal.UpdateModal(ModalType.Information, $"{Translation["LeadingManager"]}: " + gestor + $"\n {Translation["DocumentaryTypology"]}: " + tipologia, true, "", "", $"{Translation["DRTSelected"]}");
            }
        }

        #endregion SelectDocTypology

        #region

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type != ModalType.Error && args.IsAccepted)
            {
                fitlerBehaviourTypologyRequest = new()
                {
                    DocumentaryTypologyId = vDocumentary.DocumentaryTypologyId ?? new()
                };

                var responseApi = await HttpClient!.PostAsJsonAsync("generalviews/VBehaviorTypology/ByFilter", fitlerBehaviourTypologyRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<VBehaviorTypologyDtoResponse>>>();
                if (deserializeResponse!.Succeeded && ( deserializeResponse.Data != null || deserializeResponse.Data?.Count != 0 ))
                {
                    vDocumentary.VBehaviors = deserializeResponse.Data ?? new();
                }
                else
                {
                    vDocumentary.VBehaviors = new();
                }

                var eventArgs = new MyEventArgs<VDocumentaryTypologyDtoResponse>
                {
                    Data = vDocumentary,
                    ModalStatus = false
                };
                await OnStatusChanged.InvokeAsync(eventArgs);
            }
            else
            {
                docTypologyList.Where(x => x.DocumentaryTypologyId.Equals(vDocumentary.DocumentaryTypologyId)).FirstOrDefault().Selected = false;
            }
        }

        #endregion Methods

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
                    Radication = true
                };
                if (!string.IsNullOrEmpty(ClassCodeVar))
                {
                    filterVDocTypology.ClassCode = ClassCodeVar;
                }

                var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterTypology, filterVDocTypology);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<VDocumentaryTypologyDtoResponse>>>();
                if (deserializeResponse!.Succeeded && ( deserializeResponse.Data != null || deserializeResponse.Data.Data?.Count != 0 ))
                {
                    docTypologiesList = deserializeResponse.Data.Data!;
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                    //meta = deserializeResponse.Meta!;
                }
                else
                {
                    docTypologiesList = new();
                    //meta = new() { PageSize = 10 };
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["ErrorGettingRecords"]}", true, Translation["Accept"]);
            }
        }

        #endregion GetDocTypologies

        #region resetModal

        public async Task resetModal()
        {
            docTypologyList = new();
            docTypologiesList = new();
            docVersionList = new();
            adminUnitList = new();
            proOfficesList = new();
            seriesList = new();
            subSeriesList = new();

            isEnableAdminUnit = false;
            isEnableProOffice = false;
            isEnableSerie = false;
            isEnableSubSerie = false;
            isEnableDocTypology = false;
            isVisibleTypologyNameInput = false;
            isEnableButton = true;

            idDocVersion = 0;
            idAdminUnit = 0;
            idProOffice = 0;
            idSerie = 0;
            idSubSerie = 0;
            idDocTypologies = 0;

            filterVDocTypology = new() { Radication=true};

            await GetDocumentalVersions();
        }

        #endregion resetModal

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
            idDocTypologies = e ? idDocTypologies : 0;
        }

        #endregion EnableField

        #endregion Methods
    }
}