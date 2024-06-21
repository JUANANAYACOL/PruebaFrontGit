using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
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
    public partial class SubSeriesModal
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

        private InputModalComponent codeInput { get; set; } = new();
        private InputModalComponent nameInput { get; set; } = new();
        private NewPaginationComponent<SecurityLevelDtoResponse, SecurityLevelFilterDtoRequest> paginationComponent = new();

        #endregion Components

        #region Modals

        private NotificationsComponentModal notificationModal = new();
        private RetentionsModal modalRetentions = new();
        private RetentionSSDtoRequest retention = new();
        private SecurityLevelFilterDtoRequest securityLevelFilterDtoRequest = new();

        #endregion Modals

        #region Parameters

        [Parameter] public EventCallback<bool> OnStatusUpdate { get; set; }

        #endregion Parameters

        #region Models

        private SubSerieDtoRequest subSerieDtoRequest = new();
        private SubSeriesUpdateDtoRequest subSerieUpdateDtoRequest = new();
        private SubSeriesDtoResponse _selectedRecord = new();
        private GenericSearchModal genericSearchModal { get; set; } = new();

        private PaginationInfo paginationInfo = new();

        #endregion Models

        #region Environments

        #region Environments(Numeric)

        private decimal CharacterCounter = 0;
        private int IdDocumental { get; set; } = 0;
        private int idAdUnit { get; set; } = 0;
        private int IdproOffice { get; set; } = 0;
        private int IdSerie { get; set; } = 0;
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

        private bool IsEditForm { get; set; } = false;
        private bool docTypoExist { get; set; } = false;
        private bool activeState { get; set; } = true;
        private bool IsDisabledCode { get; set; } = false;
        private bool modalStatus { get; set; } = false;
        private bool administrativeUnitEnable { get; set; } = false;
        private bool productionOfficeEnable { get; set; } = false;
        private bool serieEnable { get; set; } = false;
        private bool saveIsDisable { get; set; } = true;
        private bool administrativeUnitEnableSL { get; set; } = false;
        private bool productionOfficeEnableSL { get; set; } = false;
        private bool currentOptionSecurityLevel { get; set; } = true;
        private bool multipleSelectionManager { get; set; } = new();

        private bool dataChargue;
        private bool selectAllManager { get; set; } = false;
        private bool retentionExist { get; set; } = true;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<SeriesDtoResponse> seriesList { get; set; } = new();
        private List<DocumentalVersionDtoResponse> documentalVersionsList { get; set; } = new();
        private List<AdministrativeUnitsDtoResponse> administrativeUnitsList { get; set; } = new();
        private List<ProductionOfficesDtoResponse> productionOfficesList { get; set; } = new();

        private List<DocumentalVersionDtoResponse> documentalVersionsListSL { get; set; } = new();
        private List<AdministrativeUnitsDtoResponse> administrativeUnitsListSL { get; set; } = new();
        private List<ProductionOfficesDtoResponse> productionOfficesListSL { get; set; } = new();
        private List<VSystemParamDtoResponse> seriesClassList = new();
        private List<VSystemParamDtoResponse> securityLevelsList = new();
        private List<SecurityLevelDtoResponse> securityLevelsSeriesList { get; set; } = new();
        private List<SecurityLevelDtoResponse> securityLevelsSeriesNewList { get; set; } = new();
        private List<string> optionsSecurityLevel;
        private List<int> idsUsers;
        private SecurityLevelFilterDtoRequest securityLevelFilter = new();
        private List<VUserDtoResponse> usersInManagerToReturn { get; set; } = new();
        private List<SecurityLevelDtoResponse> selectedUsers = new List<SecurityLevelDtoResponse>();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            optionsSecurityLevel = new List<string>() { Translation["ProductionOffice"], Translation["User"] };
            selectedOptionSecurityLevel = Translation["ProductionOffice"];
            await GetDocumentalVersions();
            await GetSeriesClass();
            await GetSecurityLevels();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        #region FormMethods

        #region HandleValidSubmit

        private async Task HandleValidSubmit()
        {
            if (IsEditForm)
            {
                await HandleFormUpdate();
            }
            else
            {
                if (!RetentionEmpty(subSerieDtoRequest.Retention))
                {
                    await HandleFormCreate();
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Information, Translation["VerifyRetention"], true, Translation["Accept"], "");
                }
            }
            StateHasChanged();
        }

        #endregion HandleValidSubmit

        #region HandleFormCreate

        private async Task HandleFormCreate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                if (codeInput.IsInputValid)
                {
                    subSerieDtoRequest.SeriesId = IdSerie;
                    subSerieDtoRequest.ActiveState = activeState;
                    subSerieDtoRequest.SecurityLevel = securityLevel;
                    subSerieDtoRequest.Class = seriesClass;
                    if (RetentionEmpty(subSerieDtoRequest.Retention))
                    {
                        subSerieDtoRequest.Retention = null;
                    }
                    
                    var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/SubSeries/CreateSubSerie", subSerieDtoRequest);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<SubSeriesDtoResponse>>();
                    if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                    {
                        notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                        await OnStatusUpdate.InvokeAsync(true);
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
                    }
                }
                else { notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]); }
            }
            catch (Exception ex)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleFormCreate

        #region HandleFormUpdate

        public bool RetentionEmpty(RetentionSSDtoRequest retention)
        {
            return retention.TimeFileManagement == 0 &&
                   retention.TimeFileCentral == 0 &&
                   !retention.TotalConservation &&
                   !retention.Elimination &&
                   !retention.Selection &&
                   !retention.TechEnvironment &&
                   string.IsNullOrEmpty(retention.ProcedureRet);
        }

        public bool RetentionResponseEmpty(RetentionSSDtoResponse retention)
        {
            return retention.TimeFileManagement == 0 &&
                   retention.TimeFileCentral == 0 &&
                   !retention.TotalConservation &&
                   !retention.Elimination &&
                   !retention.Selection &&
                   !retention.TechEnvironment &&
                   string.IsNullOrEmpty(retention.ProcedureRet);
        }

        private async Task HandleFormUpdate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                subSerieUpdateDtoRequest.SeriesId = IdSerie;
                subSerieUpdateDtoRequest.Name = subSerieDtoRequest.Name;
                subSerieUpdateDtoRequest.Code = subSerieDtoRequest.Code;
                subSerieUpdateDtoRequest.Description = subSerieDtoRequest.Description;
                subSerieUpdateDtoRequest.ActiveState = activeState;
                subSerieUpdateDtoRequest.SecurityLevel = securityLevel;
                subSerieUpdateDtoRequest.Class = seriesClass;

                if (currentOptionSecurityLevel)
                {
                    subSerieUpdateDtoRequest.ProductionOfficeId = IdproOfficeSL;
                    subSerieUpdateDtoRequest.UsersId = new();
                }
                else
                {
                    subSerieUpdateDtoRequest.ProductionOfficeId = 0;
                    subSerieUpdateDtoRequest.UsersId = idsUsers;
                }
                if (_selectedRecord.Retention == null || RetentionResponseEmpty(_selectedRecord.Retention))
                {
                    subSerieUpdateDtoRequest.Retention = null;
                }
                else
                {
                    subSerieUpdateDtoRequest.Retention = FillRetentionUpdate(_selectedRecord.Retention);
                }

                var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/SubSeries/UpdateSubSerie", subSerieUpdateDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<SubSeriesDtoResponse>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                    await OnStatusUpdate.InvokeAsync(true);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleFormUpdate

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                await UpdateModalStatus(args.ModalStatus);
                IsEditForm = false;
                docTypoExist = false;
                retentionExist = true;
                await ResetFormAsync();
            }
            if (notificationModal.Type == ModalType.Warning && args.IsAccepted && args.ModalOrigin != null && args.ModalOrigin.Equals("DeleteSecurityLevel"))
            {
                var selectedUserIds = selectedUsers.Select(user => user.UserId).ToList();

                securityLevelsSeriesNewList = securityLevelsSeriesNewList.Where(user => !selectedUserIds.Contains(user.UserId)).ToList();
                idsUsers = idsUsers.Except(selectedUserIds).ToList();

                selectedUsers.Clear();
            }
        }

        #endregion HandleModalNotiClose

        #region ResetFormAsync

        private async Task ResetFormAsync()
        {
            if (!IsEditForm)
            {
                subSerieDtoRequest = new();
            }
            else
            {
                subSerieUpdateDtoRequest.SubSeriesId = _selectedRecord.SubSeriesId;
                await GetDocumentaryTypologies(_selectedRecord.SubSeriesId);
                var reten = _selectedRecord.Retention != null ? new RetentionSSDtoResponse() : null;
                _selectedRecord = new();
                _selectedRecord.Retention = reten;
            }

            subSerieDtoRequest.Name = string.Empty;
            subSerieDtoRequest.Code = string.Empty;
            subSerieDtoRequest.Description = string.Empty;
            activeState = true;
            CharacterCounter = 0;

            #region DropDownLists

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

            #endregion DropDownLists

            #region SecurityLevel&Class

            seriesClass = string.Empty;
            securityLevel = string.Empty;
            seriesClassList = new();
            securityLevelsList = new();
            idsUsers = new();

            #endregion SecurityLevel&Class

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

            //_selectedRecord = new();
            await GetDocumentalVersions();
            await GetSeriesClass();
            await GetSecurityLevels();
            //await GetSecurityLevelsList();
            currentOptionSecurityLevel = true;
            selectedOptionSecurityLevel = Translation["ProductionOffice"];
            StateHasChanged();
        }

        #endregion ResetFormAsync

        #endregion FormMethods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #endregion HandleMethods

        #region OthersMethods

        #region CountCharacters

        private void CountCharacters(ChangeEventArgs e)
        {
            string value = e.Value.ToString() ?? string.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                CharacterCounter = value.Length;
            }
            else
            {
                CharacterCounter = 0;
            }
        }

        #endregion CountCharacters

        #region ModalMethods

        #region UpdateSelectedRecordAsync

        public async Task UpdateSelectedRecordAsync(SubSeriesDtoResponse response)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            await GetDocumentaryTypologies(response.SubSeriesId);
            IsEditForm = true;
            _selectedRecord = response;

            await GetDocumentalVersions();
            IdDocumental = _selectedRecord.DocumentalVersionId;
            await GetAdministrativeUnits(IdDocumental);
            idAdUnit = _selectedRecord.AdministrativeUnitId;
            await GetProductionOffices(idAdUnit);
            IdproOffice = _selectedRecord.ProductionOfficeId;
            await GetSeries(IdproOffice);
            IdSerie = _selectedRecord.SeriesId;

            await GetSeriesClass();
            securityLevel = _selectedRecord.Class;
            await GetSecurityLevels();
            securityLevel = _selectedRecord.SecurityLevel;

            await GetSecurityLevelsList();

            #region SecurityLevelOptions

            IdDocumentalSL = 0;
            idAdUnitSL = 0;
            IdproOfficeSL = 0;

            administrativeUnitEnableSL = false;
            productionOfficeEnableSL = false;
            administrativeUnitsListSL = new();
            productionOfficesListSL = new();

            #endregion SecurityLevelOptions

            subSerieUpdateDtoRequest.SubSeriesId = _selectedRecord.SubSeriesId;
            retentionExist = _selectedRecord.Retention != null;
            idsUsers = new();
            activeState = _selectedRecord.ActiveState;
            subSerieDtoRequest.ActiveState = _selectedRecord.ActiveState;
            subSerieDtoRequest.SeriesId = _selectedRecord.SeriesId;
            subSerieDtoRequest.Code = _selectedRecord.Code!;
            subSerieDtoRequest.Name = _selectedRecord.Name;
            securityLevel = _selectedRecord.SecurityLevel;
            seriesClass = _selectedRecord.Class;
            subSerieDtoRequest.Description = string.IsNullOrEmpty(_selectedRecord.Description) ? "" : _selectedRecord.Description;
            CharacterCounter = string.IsNullOrEmpty(_selectedRecord.Description) ? 0 : _selectedRecord.Description.Length;

            currentOptionSecurityLevel = true;
            selectedOptionSecurityLevel = Translation["ProductionOffice"];

            EnableSaveButton();

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion UpdateSelectedRecordAsync

        #region UpdateModalStatus

        public async Task UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            //IsEditForm = false;
            //await ResetFormAsync();
            StateHasChanged();
        }

        #endregion UpdateModalStatus

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

        #region HandleModalClosedAsync

        private async Task HandleModalClosedAsync(bool status)
        {
            modalStatus = status;
            CharacterCounter = 0;
            subSerieDtoRequest = new();
            IsEditForm = false;
            await ResetFormAsync();
            StateHasChanged();
        }

        #endregion HandleModalClosedAsync

        #endregion ModalMethods

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
                serieEnable = false;
                administrativeUnitsList = new();
                productionOfficesList = new();
                seriesList = new();

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
                        seriesList = new();
                        administrativeUnitEnable = false;
                        productionOfficeEnable = false;
                        serieEnable = false;
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

        #region GetProductionOffices

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

                    EnableSaveButton();
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
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

        #endregion GetProductionOffices

        #region GetSeries

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

                    EnableSaveButton();
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetSeries

        #region GetSeriesClass

        private async Task GetSeriesClass()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("ParamCode");
                HttpClient?.DefaultRequestHeaders.Add("ParamCode", "SCLAS");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("ParamCode");
                if (deserializeResponse!.Succeeded)
                {
                    seriesClassList = deserializeResponse.Data!;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Translation["LoadErrorMessage"]}: {ex.Message}");
            }
        }

        #endregion GetSeriesClass

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
            catch (Exception ex)
            {
                Console.WriteLine($"{Translation["LoadErrorMessage"]}: {ex.Message}");
            }
        }

        private async Task GetSecurityLevelsList()
        {
            try
            {
                securityLevelFilter = new SecurityLevelFilterDtoRequest()
                {
                    SubSeriesId = _selectedRecord.SubSeriesId
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
            catch (Exception ex)
            {
                Console.WriteLine($"{Translation["LoadErrorMessage"]}: {ex.Message}");
            }
        }

        #endregion GetSecurityLevels

        #region GetDocumentaryTypologies

        private async Task GetDocumentaryTypologies(int idSubSeries)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            var request = new DocumentaryTypologiesFilterDtoRequest()
            {
                SubSeriesId = idSubSeries
            };
            var responseApi = await HttpClient!.PostAsJsonAsync("documentarytypologies/DocumentaryTypologies/ByFilter", request);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<DocumentaryTypologiesDtoResponse>>>();
            if (deserializeResponse!.Succeeded && deserializeResponse.Data != null && deserializeResponse!.Data.Data.Count > 0)
            {
                docTypoExist = true;
            }
            else
            {
                docTypoExist = false;
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetDocumentaryTypologies

        #endregion GetMethods

        #region EnableSaveButton

        public void EnableSaveButton()
        {
            var optionSL = false;
            if (IsEditForm && currentOptionSecurityLevel && docTypoExist && !securityLevel.Contains("PUBL"))
            {
                optionSL = IdDocumentalSL <= 0 || idAdUnitSL <= 0 || IdproOfficeSL <= 0;
            }
            else if (IsEditForm && !currentOptionSecurityLevel && docTypoExist && !securityLevel.Contains("PUBL"))
            {
                optionSL = securityLevelsSeriesNewList.Count <= 0;
            }

            var retentionPresent = true;
            retentionPresent = retentionExist;

            if (IsEditForm && retentionExist)
            {
                retentionPresent = !(_selectedRecord.Retention != null && !RetentionResponseEmpty(_selectedRecord.Retention!));
            }
            else if (!IsEditForm && retentionExist)
            {
                retentionPresent = !(subSerieDtoRequest.Retention != null && !RetentionEmpty(subSerieDtoRequest.Retention!));
            }

            if (optionSL || retentionPresent || IdDocumental <= 0 || idAdUnit <= 0 || IdproOffice <= 0 || IdSerie <= 0 || string.IsNullOrEmpty(subSerieDtoRequest.Code) || string.IsNullOrEmpty(subSerieDtoRequest.Name) || string.IsNullOrEmpty(seriesClass) || string.IsNullOrEmpty(securityLevel))
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

        #region Retention

        public void UpdateModalStatusForRetention(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        private async Task ShowModalCreate(bool newvalue)
        {
            UpdateModalStatusForRetention(false);
            await modalRetentions.UpdateModalStatus(true);
            _selectedRecord.Retention = _selectedRecord.Retention == null ? new RetentionSSDtoResponse() : _selectedRecord.Retention;
            if (IsEditForm)
            {
                modalRetentions.UpdateSelectedRecord(_selectedRecord.Retention);
            }
            else
            {
                if (subSerieDtoRequest.Retention != null)
                {
                    modalRetentions.UpdateSelectedRecord(FillRetentionRequestNew(subSerieDtoRequest.Retention));
                }
            }
        }

        private RetentionSSDtoRequest FillRetentionUpdate(RetentionSSDtoResponse trd)
        {
            var response = new RetentionSSDtoRequest();

            response.TimeFileCentral = trd.TimeFileCentral;
            response.TimeFileManagement = trd.TimeFileManagement;
            response.TotalConservation = trd.TotalConservation;
            response.Elimination = trd.Elimination;
            response.Selection = trd.Selection;
            response.TechEnvironment = trd.TechEnvironment;
            response.ProcedureRet = trd.ProcedureRet;
            return response;
        }

        private RetentionSSDtoResponse FillRetentionRequestNew(RetentionSSDtoRequest trd)
        {
            var response = new RetentionSSDtoResponse();

            response.TimeFileCentral = trd.TimeFileCentral;
            response.TimeFileManagement = trd.TimeFileManagement;
            response.TotalConservation = trd.TotalConservation;
            response.Elimination = trd.Elimination;
            response.Selection = trd.Selection;
            response.TechEnvironment = trd.TechEnvironment;
            response.ProcedureRet = trd.ProcedureRet;
            return response;
        }

        private void FillRetentionEdit(RetentionSSDtoRequest trd)
        {
            _selectedRecord.Retention.TimeFileCentral = trd.TimeFileCentral;
            _selectedRecord.Retention.TimeFileManagement = trd.TimeFileManagement;
            _selectedRecord.Retention.TotalConservation = trd.TotalConservation;
            _selectedRecord.Retention.Elimination = trd.Elimination;
            _selectedRecord.Retention.Selection = trd.Selection;
            _selectedRecord.Retention.TechEnvironment = trd.TechEnvironment;
            _selectedRecord.Retention.ProcedureRet = trd.ProcedureRet;

            StateHasChanged();
        }

        private void FillRetentionRequest(RetentionSSDtoResponse trd)
        {
            subSerieDtoRequest.Retention.TimeFileCentral = trd.TimeFileCentral;
            subSerieDtoRequest.Retention.TimeFileManagement = trd.TimeFileManagement;
            subSerieDtoRequest.Retention.TotalConservation = trd.TotalConservation;
            subSerieDtoRequest.Retention.Elimination = trd.Elimination;
            subSerieDtoRequest.Retention.Selection = trd.Selection;
            subSerieDtoRequest.Retention.TechEnvironment = trd.TechEnvironment;
            subSerieDtoRequest.Retention.ProcedureRet = trd.ProcedureRet;

            StateHasChanged();
        }

        private void FillRetention(RetentionSSDtoRequest trd)
        {
            subSerieDtoRequest.Retention.TimeFileCentral = trd.TimeFileCentral;
            subSerieDtoRequest.Retention.TimeFileManagement = trd.TimeFileManagement;
            subSerieDtoRequest.Retention.TotalConservation = trd.TotalConservation;
            subSerieDtoRequest.Retention.Elimination = trd.Elimination;
            subSerieDtoRequest.Retention.Selection = trd.Selection;
            subSerieDtoRequest.Retention.TechEnvironment = trd.TechEnvironment;
            subSerieDtoRequest.Retention.ProcedureRet = trd.ProcedureRet;

            StateHasChanged();
        }

        private async Task HandleTRDSelectedChanged(MyEventArgs<RetentionSSDtoRequest> trd)
        {
            UpdateModalStatusForRetention(!trd.ModalStatus);

            retention = trd.Data;

            if (retention != null)
            {
                if (IsEditForm)
                {
                    FillRetentionEdit(retention);
                }
                else
                {
                    FillRetention(retention);
                }
            }
            modalRetentions.ResetFormAsync();
            await modalRetentions.UpdateModalStatus(trd.ModalStatus);
        }

        #endregion Retention

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

        #region OpenLeaderManager

        private async Task OpenLeaderManager(bool value, bool multipleUsers = false)
        {
            multipleSelectionManager = multipleUsers;
            multipleSelectionManager = value;
            genericSearchModal.UpdateModalStatus(true);
            await UpdateModalStatusSL(false, false);
        }

        #endregion OpenLeaderManager

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
                    if (idsUsers != null && !idsUsers.Contains(secLevel.UserId))
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

            /*securityLevelsSeriesList = (List<VUserDtoResponse>)user.Data![0];*/
            genericSearchModal.UpdateModalStatus(false);

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