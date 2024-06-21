using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VDocumentaryTypology.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD
{
    public partial class DocumentaryTypologyModal
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

        #region Modals

        private NotificationsComponentModal notificationModal = new();
        private SecurityLevelFilterDtoRequest securityLevelFilterDtoRequest = new();

        #endregion Modals

        #region Parameteres

        [Parameter] public EventCallback<bool> OnStatusChanged { get; set; }
        [Parameter] public string Title { get; set; } = string.Empty;

        #endregion Parameteres

        #region Components

        private NewPaginationComponent<SecurityLevelDtoResponse, SecurityLevelFilterDtoRequest> paginationComponent = new();

        #endregion Components

        #region Models

        private DocumentaryTypologiesDtoRequest DTCreateRequest { get; set; } = new();
        public DocumentaryTypologiesFilterNameDtoRequest filterVDocTypology { get; set; } = new();
        private DocumentaryTypologiesUpdateDtoRequest DTUpdateDtoRequest { get; set; } = new();
        private GenericSearchModal genericSearchModal { get; set; } = new();

        private PaginationInfo paginationInfo = new();

        #endregion Models

        #region Environments

        #region Environments(Numeric)

        private int idDocVersion { get; set; } = new();
        private int idAdminUnit { get; set; } = new();
        private int idProOffice { get; set; } = new();
        private int idSerie { get; set; } = new();
        private int idSubSerie { get; set; } = new();
        private int IdDocumentalSL { get; set; } = 0;
        private int idAdUnitSL { get; set; } = 0;
        private int IdproOfficeSL { get; set; } = 0;
        private int securityLevelToDelete { get; set; } = new();

        #endregion Environments(Numeric)

        #region Environments(String)

        private string securityLevel = "";
        private string seriesClass = "";
        private string selectedOptionSecurityLevel = "";
        private string uriFilterSecurityLevels = "documentarytypologies/SecurityLevel/ByFilter";

        #endregion Environments(String)

        #region Environments(Bool)

        private bool SubSerieSpan { get; set; } = false;
        private bool isEditForm { get; set; } = false;

        private bool activeState { get; set; } = true;
        private bool modalStatus { get; set; } = false;

        private bool saveIsDisable { get; set; } = true;

        private bool administrativeUnitEnableSL { get; set; } = false;
        private bool productionOfficeEnableSL { get; set; } = false;
        private bool isEnableAdminUnit { get; set; } = false;
        private bool isEnableProOffice { get; set; } = false;
        private bool isEnableSerie { get; set; } = false;
        private bool isEnableSubSerie { get; set; } = false;
        private bool isEnableDocTypology { get; set; } = false;
        private int idDocTypologiesBag { get; set; } = new();
        private bool currentOptionSecurityLevel { get; set; } = true;
        private bool multipleSelectionManager { get; set; } = new();

        private bool dataChargue;
        private bool selectAllManager { get; set; } = false;

        #endregion Environments(Bool)

        #region Enviroments(List && Dictionary)

        private List<VDocumentaryTypologyDtoResponse> docTypologyList { get; set; } = new();
        private List<DocumentaryTypologiesBagDtoResponse> docTypologiesBagList { get; set; } = new();
        private List<DocumentalVersionDtoResponse> documentalVersionsList { get; set; } = new();
        private List<AdministrativeUnitsDtoResponse> adminUnitList { get; set; } = new();
        private List<ProductionOfficesDtoResponse> proOfficesList { get; set; } = new();
        private List<SubSeriesDtoResponse> subSeriesList { get; set; } = new();
        private List<SeriesDtoResponse> seriesList { get; set; } = new();
        private List<AdministrativeUnitsDtoResponse> administrativeUnitsListSL { get; set; } = new();
        private List<ProductionOfficesDtoResponse> productionOfficesListSL { get; set; } = new();
        private List<VSystemParamDtoResponse> securityLevelsList = new();
        private List<SecurityLevelDtoResponse> securityLevelsSeriesList { get; set; } = new();
        private List<SecurityLevelDtoResponse> securityLevelsSeriesNewList { get; set; } = new();
        private List<string> optionsSecurityLevel;
        private List<int> idsUsers { get; set; } = new();
        private SecurityLevelFilterDtoRequest securityLevelFilter = new();
        private List<SecurityLevelDtoResponse> selectedUsers = new List<SecurityLevelDtoResponse>();

        #endregion Enviroments(List && Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            optionsSecurityLevel = new List<string>() { Translation["ProductionOffice"], Translation["User"] };
            selectedOptionSecurityLevel = Translation["ProductionOffice"];
            await GetDocumentalVersions();
            await GetSecurityLevels();
        }

        private async Task HandleModalClosed(bool status)
        {
            modalStatus = status;
            await ResetFormAsync();
            StateHasChanged();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private async Task HandleValidSubmit()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            if (isEditForm)
            {
                await HandleFormUpdate();
            }
            else
            {
                await HandleFormCreate();
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #region HandleFormCreate

        private async Task HandleFormCreate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                DTCreateRequest.ActiveState = activeState;
                DTCreateRequest.SeriesId = idSerie;
                DTCreateRequest.SubSeriesId = (idSubSerie == 0) ? null : idSubSerie;
                DTCreateRequest.DocumentaryTypologyBagId = idDocTypologiesBag;
                DTCreateRequest.SecurityLevel = securityLevel;
                if (currentOptionSecurityLevel)
                {
                    DTCreateRequest.ProductionOfficeId = IdproOfficeSL;
                    DTCreateRequest.UsersId = new();
                }
                else
                {
                    DTCreateRequest.ProductionOfficeId = 0;
                    DTCreateRequest.UsersId = idsUsers;
                }

                var responseApi = await HttpClient!.PostAsJsonAsync("documentarytypologies/DocumentaryTypologies/CreateDocumentaryTypologies", DTCreateRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<DocumentaryTypologiesDtoResponse>>();

                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                    await OnStatusChanged.InvokeAsync(true);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Information, Translation["CreateErrorMessage"], true, Translation["Accept"], "");
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["CreateErrorMessage"], true, Translation["Accept"], "");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleFormCreate

        private async Task HandleFormUpdate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                DTUpdateDtoRequest.ActiveState = activeState;
                DTUpdateDtoRequest.SeriesId = idSerie;
                DTUpdateDtoRequest.SubSeriesId = (idSubSerie == 0) ? null : idSubSerie;
                DTUpdateDtoRequest.DocumentaryTypologyBagId = idDocTypologiesBag;
                DTUpdateDtoRequest.SecurityLevel = securityLevel;
                if (currentOptionSecurityLevel)
                {
                    DTUpdateDtoRequest.ProductionOfficeId = IdproOfficeSL;
                    DTUpdateDtoRequest.UsersId = new();
                }
                else
                {
                    DTUpdateDtoRequest.ProductionOfficeId = 0;
                    DTUpdateDtoRequest.UsersId = idsUsers;
                }

                var responseApi = await HttpClient!.PostAsJsonAsync("documentarytypologies/DocumentaryTypologies/UpdateDocumentaryTypologies", DTUpdateDtoRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<DocumentaryTypologiesDtoResponse>>();

                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                    await OnStatusChanged.InvokeAsync(true);
                    
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Information, Translation["UpdateErrorMessage"], true, Translation["Accept"], "");
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["UpdateErrorMessage"], true, Translation["Accept"], "");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleMethods

        #region OthersMethods

        #region EnableField

        public void EnableField(bool admUnit, bool proOffic, bool ser, bool subs, bool docTyp)
        {
            isEnableAdminUnit = admUnit;
            isEnableProOffice = proOffic;
            isEnableSerie = ser;
            isEnableSubSerie = subs;
            isEnableDocTypology = docTyp;

            idAdminUnit = admUnit ? idAdminUnit : 0;
            idProOffice = proOffic ? idProOffice : 0;
            idSerie = ser ? idSerie : 0;
            idSubSerie = subs ? idSubSerie : 0;
            idDocTypologiesBag = docTyp ? idDocTypologiesBag : 0;
        }

        #endregion EnableField

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success && args.IsAccepted)
            {
                await UpdateModalStatus(args.ModalStatus);
                await ResetFormAsync();

            }
            if (notificationModal.Type == ModalType.Warning && args.IsAccepted && args.ModalOrigin != null && args.ModalOrigin.Equals("DeleteSecurityLevel"))
            {
                var selectedUserIds = selectedUsers.Select(user => user.UserId).ToList();

                securityLevelsSeriesNewList = securityLevelsSeriesNewList.Where(user => !selectedUserIds.Contains(user.UserId)).ToList();
                idsUsers = idsUsers.Except(selectedUserIds).ToList();

                selectedUsers.Clear();
                if (!securityLevelsSeriesNewList.Any())
                {
                    selectAllManager = false;
                }

                EnableSaveButton();
            }

            StateHasChanged();
        }

        #endregion HandleModalNotiClose

        #region GetMethods

        #region GetDocumentalVersions

        public async Task GetDocumentalVersions()
        {
            try
            {
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<DocumentalVersionDtoResponse>>>("paramstrd/DocumentalVersions/ByDocumentalVersions");
                if (deserializeResponse!.Succeeded)
                {
                    documentalVersionsList = deserializeResponse.Data!;
                    string toCurrent = Translation["ToCurrent"];
                    string documentaryVersion = Translation["DocumentaryVersion"];
                    documentalVersionsList.ForEach(x =>
                    {
                        x.Code = $"{((x.EndDate == null || x.EndDate.Value > DateTime.Now) ? $"{x.StartDate!.Value.Year} {toCurrent} - {documentaryVersion} {x.Code}" : $"{x.StartDate!.Value.Year} a {x.EndDate.Value.Year} - {documentaryVersion} {x.Code} {x.VersionType}")}";
                    });
                    documentalVersionsList = documentalVersionsList.OrderBy(x => x.StartDate).Reverse().ToList();
                    EnableField(false, false, false, false, false);
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["LoadDocumentalVersionErrorMessage"]}", true, Translation["Accept"]);
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

                if (idDocVersion != 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");
                    HttpClient?.DefaultRequestHeaders.Add("documentalVersionsId", $"{idDocVersion}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeUnitsDtoResponse>>>("paramstrd/AdministrativeUnit/ByAdministrativeUnits");
                    HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");

                    if (deserializeResponse!.Succeeded && (deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null))
                    {
                        adminUnitList = deserializeResponse.Data!;
                        EnableField(true, false, false, false, false);
                    }
                    else
                    {
                        EnableField(false, false, false, false, false);
                    }

                    EnableSaveButton();
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["ErrorGettingRecords"]}", true, Translation["Accept"]);
            }
        }

        private async Task GetAdministrativeUnitsSL(int value)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                IdDocumentalSL = value;
                idAdUnitSL = 0;
                administrativeUnitEnableSL = false;
                productionOfficeEnableSL = false;
                administrativeUnitsListSL = new();
                productionOfficesListSL = new();

                if (IdDocumentalSL != 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");
                    HttpClient?.DefaultRequestHeaders.Add("documentalVersionsId", $"{IdDocumentalSL}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeUnitsDtoResponse>>>("paramstrd/AdministrativeUnit/ByAdministrativeUnits");
                    HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");

                    if (deserializeResponse!.Succeeded && deserializeResponse.Data?.Count != 0)
                    {
                        administrativeUnitsListSL = deserializeResponse.Data ?? new();
                        administrativeUnitEnableSL = true;
                    }
                    else
                    {
                        administrativeUnitsListSL = new();
                        productionOfficesListSL = new();
                        administrativeUnitEnableSL = false;
                        productionOfficeEnableSL = false;
                    }

                    EnableSaveButton();
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
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

                if (idAdminUnit != 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                    HttpClient?.DefaultRequestHeaders.Add("AdministrativeUnitId", $"{idAdminUnit}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<ProductionOfficesDtoResponse>>>("paramstrd/ProductionOffice/ByProductionOffices");
                    HttpClient?.DefaultRequestHeaders.Remove("key");

                    if (deserializeResponse!.Succeeded && (deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null))
                    {
                        proOfficesList = deserializeResponse.Data!;
                        EnableField(true, true, false, false, false);
                    }
                    else
                    {
                        EnableField(true, false, false, false, false);
                    }

                    EnableSaveButton();
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["ErrorGettingRecords"]}", true, Translation["Accept"]);
            }
        }

        private async Task GetProductionOfficesSL(int value)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                idAdUnitSL = value;
                IdproOfficeSL = 0;
                productionOfficeEnableSL = false;
                productionOfficesListSL = new();

                if (idAdUnitSL != 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                    HttpClient?.DefaultRequestHeaders.Add("AdministrativeUnitId", $"{idAdUnitSL}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<ProductionOfficesDtoResponse>>>("paramstrd/ProductionOffice/ByProductionOffices");
                    HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");

                    if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                    {
                        productionOfficesListSL = deserializeResponse.Data ?? new();
                        productionOfficeEnableSL = true;
                    }
                    else
                    {
                        productionOfficesListSL = new();
                        productionOfficeEnableSL = false;
                    }

                    EnableSaveButton();
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
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

                if (idProOffice != 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("productionOfficeId");
                    HttpClient?.DefaultRequestHeaders.Add("productionOfficeId", $"{idProOffice}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<SeriesDtoResponse>>>("paramstrd/Series/BySeries");
                    HttpClient?.DefaultRequestHeaders.Remove("productionOfficeId");

                    if (deserializeResponse!.Succeeded && (deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null))
                    {
                        seriesList = deserializeResponse.Data!;

                        EnableField(true, true, true, false, false);
                    }
                    else
                    {
                        EnableField(true, true, false, false, false);
                    }

                    EnableSaveButton();
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["ErrorGettingRecords"]}", true, Translation["Accept"]);
            }
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

                if (idSerie != 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("seriesId");
                    HttpClient?.DefaultRequestHeaders.Add("seriesId", $"{idSerie}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<SubSeriesDtoResponse>>>("paramstrd/SubSeries/BySubSeries");
                    HttpClient?.DefaultRequestHeaders.Remove("seriesId");

                    if (deserializeResponse!.Succeeded && (deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null))
                    {
                        subSeriesList = deserializeResponse.Data!;
                        if (subSeriesList.Any())
                        {
                            SubSerieSpan = true;
                            EnableField(true, true, true, true, false);
                        }
                        else
                        {
                            SubSerieSpan = false;
                            EnableField(true, true, true, false, false);
                            await GetDocTypologiesBySubSerieId(0);
                        }
                    }
                    else
                    {
                        SubSerieSpan = false;
                        EnableField(true, true, true, false, false);
                        await GetDocTypologiesBySubSerieId(0);
                    }

                    EnableSaveButton();
                }
                await GetDocTypologiesBySubSerieId(0);
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["ErrorGettingRecords"]}", true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetSubSeries

        #region GetSecurityLevels

        private async Task GetSecurityLevels()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("ParamCode");
                HttpClient?.DefaultRequestHeaders.Add("ParamCode", "SECL");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("ParamCode");
                if (deserializeResponse!.Succeeded)
                {
                    securityLevelsList = deserializeResponse.Data!;
                }
            }
            catch
            {
                Console.WriteLine($"{Translation["LoadErrorMessage"]}");
            }
        }

        private async Task GetSecurityLevelsList()
        {
            try
            {
                securityLevelFilter = new SecurityLevelFilterDtoRequest()
                {
                    DocumentaryTypologyId = DTUpdateDtoRequest.DocumentaryTypologyId
                };

                var responseApi = await HttpClient!.PostAsJsonAsync(uriFilterSecurityLevels, securityLevelFilter);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<SecurityLevelDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null && deserializeResponse.Data.Data.Count > 0)
                {
                    securityLevelsSeriesList = deserializeResponse.Data.Data ?? new();
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                    dataChargue = true;
                }
                else
                {
                    securityLevelsSeriesList = new();
                    paginationInfo = new();
                    paginationComponent.ResetPagination(paginationInfo);
                    dataChargue = false;
                }
            }
            catch
            {
                Console.WriteLine($"{Translation["LoadErrorMessage"]}");
            }
        }

        #endregion GetSecurityLevels

        #region GetDocTypologiesBySubSerieId

        public async Task GetDocTypologiesBySubSerieId(int id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                if (id != 0)
                {
                    idSubSerie = id;
                }

                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<DocumentaryTypologiesBagDtoResponse>>>("documentarytypologies/DocumentaryTypologiesBag/ByDocumentaryTypologiesBag");
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                {
                    docTypologiesBagList = deserializeResponse.Data;
                    if (subSeriesList.Any())
                    {
                        EnableField(true, true, true, true, true);
                    }
                    else
                    {
                        EnableField(true, true, true, false, true);
                    }
                }
                else
                {
                    if (subSeriesList.Any())
                    {
                        EnableField(true, true, true, true, false);
                    }
                    else
                    {
                        EnableField(true, true, true, false, false);
                    }
                }

                EnableSaveButton();
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["ErrorGettingRecords"]}", true, Translation["Accept"]);
            }
        }

        #endregion GetDocTypologiesBySubSerieId

        #endregion GetMethods

        #region UpdateModalStatus

        public async Task UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            //await GetDocumentalVersions();
            EnableSaveButton();
            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region ResetFormAsync

        private async Task ResetFormAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            #region DropDownLists

            documentalVersionsList = new();
            idDocVersion = 0;
            activeState = true;
            adminUnitList = new();
            idAdminUnit = 0;
            proOfficesList = new();
            idProOffice = 0;
            seriesList = new();
            idSerie = 0;
            subSeriesList = new();
            idSubSerie = 0;
            docTypologiesBagList = new();
            idDocTypologiesBag = 0;
            docTypologyList = new();

            #endregion DropDownLists

            #region SecurityLevel

            securityLevel = string.Empty;
            securityLevelsList = new();
            idsUsers = new();

            #endregion SecurityLevel

            #region SecurityLevelOptions

            IdDocumentalSL = 0;
            idAdUnitSL = 0;
            IdproOfficeSL = 0;

            administrativeUnitEnableSL = false;
            productionOfficeEnableSL = false;
            administrativeUnitsListSL = new();
            productionOfficesListSL = new();
            //securityLevelsSeriesList = new();
            securityLevelsSeriesNewList = new();

            #endregion SecurityLevelOptions

            activeState = true;
            await GetDocumentalVersions();
            await GetSecurityLevels();
            selectAllManager = false;
            currentOptionSecurityLevel = true;
            selectedOptionSecurityLevel = Translation["ProductionOffice"];
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion ResetFormAsync

        #region updateDocumentaryTypology

        public async Task updateDocumentaryTypology(DocumentaryTypologiesNameDtoResponse record)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            DTUpdateDtoRequest.DocumentaryTypologyId = record.DocumentaryTypologiesId;

            isEditForm = true;

            await GetDocumentalVersions();
            if (documentalVersionsList.Select(item => item.DocumentalVersionId).Contains(record.DocumentalVersionId))
            {
                await GetAdministrativeUnits(record.DocumentalVersionId);

                if (adminUnitList.Select(item => item.AdministrativeUnitId).Contains(record.AdministrativeUnitId))
                {
                    await GetProducOffice(record.AdministrativeUnitId);

                    if (proOfficesList.Select(item => item.ProductionOfficeId).Contains(record.ProductionOfficeId))
                    {
                        await GetSeries(record.ProductionOfficeId);

                        if (seriesList.Select(item => item.SeriesId).Contains(record.SeriesId))
                        {
                            await GetSubSeries(record.SeriesId);
                            if (record.SubseriesId != null && subSeriesList.Select(item => item.SubSeriesId).Contains((int)record.SubseriesId))
                            {
                                await GetDocTypologiesBySubSerieId((int)record.SubseriesId);
                            }
                        }
                    }
                }
            }
            await GetSecurityLevels();
            securityLevel = record.SecurityLevel;

            await GetSecurityLevelsList();

            activeState = record.ActiveState;
            idDocTypologiesBag = record.DocumentaryTypologyBagId;
            EnableSaveButton();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion updateDocumentaryTypology

        #region EnableSaveButton

        public void EnableSaveButton()
        {
            var optionSL = false;
            if (currentOptionSecurityLevel && !securityLevel.Contains("PUBL"))
            {
                optionSL = IdDocumentalSL <= 0 || idAdUnitSL <= 0 || IdproOfficeSL <= 0;
            }
            else if (!currentOptionSecurityLevel && !securityLevel.Contains("PUBL"))
            {
                optionSL = securityLevelsSeriesNewList.Count <= 0;
            }

            if (optionSL || idAdminUnit == 0 || idProOffice == 0 || idSerie == 0 || idDocTypologiesBag == 0 || string.IsNullOrEmpty(securityLevel))
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

        #region OnValueChangedRecordClosed

        private void OnValueChangedRecordClosed(string selectedValue)
        {
            selectedOptionSecurityLevel = selectedValue;
            if (selectedOptionSecurityLevel == Translation["ProductionOffice"])
            {
                currentOptionSecurityLevel = true;
            }
            else if (selectedOptionSecurityLevel == Translation["User"])
            {
                currentOptionSecurityLevel = false;
            }
        }

        #endregion OnValueChangedRecordClosed

        #region HandleRecordToDelete

        private void HandleRecordToDelete()
        {
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteUsersQuestionMessage"], true, Translation["Yes"], Translation["No"], modalOrigin: "DeleteSecurityLevel");
        }

        #endregion HandleRecordToDelete

        #region ChangeAllStateManager

        private void ChangeAllStateManager()
        {
            foreach (var user in securityLevelsSeriesNewList)
            {
                user.Selected = selectAllManager;
                if (selectAllManager)
                {
                    if (!selectedUsers.Contains(user))
                    {
                        selectedUsers.Add(user);
                    }
                }
                else
                {
                    selectedUsers.Remove(user);
                }
            }
        }

        private void ChangeStateManager(SecurityLevelDtoResponse user)
        {
            user.Selected = !user.Selected;
            if (user.Selected && !selectedUsers.Contains(user))
            {
                selectedUsers.Add(user);
            }
            else if (!user.Selected && selectedUsers.Contains(user))
            {
                selectedUsers.Remove(user);
            }
        }

        #endregion ChangeAllStateManager

        #region UpdateModalStatus

        public async Task UpdateModalStatusSL(bool newValue, bool search = true)
        {
            modalStatus = newValue;
            if (search)
            {
                await GetSecurityLevelsList();
            }
            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region OpenLeaderManager

        private async Task OpenLeaderManager(bool value, bool multipleUsers = false)
        {
            multipleSelectionManager = multipleUsers;
            multipleSelectionManager = value;
            genericSearchModal.UpdateModalStatus(true);
            await UpdateModalStatusSL(false, false);
            EnableSaveButton();
        }

        #endregion OpenLeaderManager

        #region HandleGenericSearchStatusChanged

        private async Task HandleGenericSearchStatusChanged(MyEventArgs<VUserDtoResponse> user)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            await UpdateModalStatusSL(true, false);
            genericSearchModal.UpdateModalStatus(false);

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleGenericSearchStatusChanged

        #region HandleGenericMultipleSearchStatusChanged

        private async Task HandleGenericMultipleSearchStatusChanged(MyEventArgs<List<Object>> user)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            await UpdateModalStatusSL(true, false);

            var list = (List<VUserDtoResponse>)user.Data![0];
            if (list.Any())
            {
                foreach (var secLevel in list)
                {
                    if (!idsUsers.Contains(secLevel.UserId))
                    {
                        if (idsUsers.Count <= 49 && securityLevelsSeriesNewList.Count <= 49)
                        {
                            idsUsers.Add(secLevel.UserId);
                            securityLevelsSeriesNewList.Add(new()
                            {
                                UserId = secLevel.UserId,
                                FullName = secLevel.FullName,
                                ProductionOfficeName = secLevel.ProductionOfficeName!
                            });
                        }
                        else
                        {
                            notificationModal.UpdateModal(ModalType.Information, Translation["MaxUsersSecurityLevelMessage"], true, Translation["Accept"]);
                        }
                    }
                }
            }

            genericSearchModal.UpdateModalStatus(false);
            EnableSaveButton();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleGenericMultipleSearchStatusChanged

        #region HandlePaginationGrid

        private void HandlePaginationGrid(List<SecurityLevelDtoResponse> newDataList)
        {
            securityLevelsSeriesList = newDataList;
        }

        #endregion HandlePaginationGrid

        #region HandleClosed

        private void HandleClosed(bool state)
        {
            modalStatus = (!state);
            StateHasChanged();
        }

        #endregion HandleClosed

        #endregion OthersMethods

        #endregion Methods
    }
}