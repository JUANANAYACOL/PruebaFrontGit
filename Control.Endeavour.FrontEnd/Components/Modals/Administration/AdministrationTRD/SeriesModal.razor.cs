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
    public partial class SeriesModal
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
        private RetentionsModal modalRetentions { get; set; } = new();
        private RetentionSSDtoRequest retention = new();
        private SecurityLevelFilterDtoRequest securityLevelFilterDtoRequest = new();

        #endregion Modals

        #region Parameters

        [Parameter] public EventCallback<bool> OnStatusUpdate { get; set; }

        #endregion Parameters

        #region Models

        private SeriesDtoRequest serieDtoRequest = new();
        private SeriesUpdateDtoRequest serieUpdateDtoRequest = new();
        private SeriesDtoResponse _selectedRecord = new();
        private GenericSearchModal genericSearchModal { get; set; } = new();

        private PaginationInfo paginationInfo = new();

        #endregion Models

        #region Environments

        #region Environments(Numeric)

        private decimal CharacterCounter = 0;
        private int IdDocumental { get; set; } = 0;
        private int idAdUnit { get; set; } = 0;
        private int IdproOffice { get; set; } = 0;
        private int IdDocumentalSL { get; set; } = 0;
        private int idAdUnitSL { get; set; } = 0;
        private int IdproOfficeSL { get; set; } = 0;

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
        private bool modalStatus { get; set; } = false;
        private bool IsDisabledCode { get; set; } = false;
        private bool administrativeUnitEnable { get; set; } = false;
        private bool productionOfficeEnable { get; set; } = false;
        private bool administrativeUnitEnableSL { get; set; } = false;
        private bool productionOfficeEnableSL { get; set; } = false;
        private bool saveIsDisable { get; set; } = true;
        private bool currentOptionSecurityLevel { get; set; } = true;
        private bool multipleSelectionManager { get; set; } = new();

        private bool dataChargue;
        private bool selectAllManager { get; set; } = false;
        private bool retentionExist { get; set; } = true;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

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
        private List<int> idsUsers { get; set; }
        private SecurityLevelFilterDtoRequest securityLevelFilter = new();
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
                if (!RetentionEmpty(serieDtoRequest.Retention))
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
                    serieDtoRequest.ProductionOfficeId = IdproOffice;
                    serieDtoRequest.ActiveState = activeState;
                    serieDtoRequest.SecurityLevel = securityLevel;
                    serieDtoRequest.Class = seriesClass;

                    var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/Series/CreateSeries", serieDtoRequest);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<SeriesDtoResponse>>();

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
                serieUpdateDtoRequest.ProductionOfficeId = IdproOffice;
                serieUpdateDtoRequest.Name = serieDtoRequest.Name;
                serieUpdateDtoRequest.Code = serieDtoRequest.Code;
                serieUpdateDtoRequest.Description = serieDtoRequest.Description;
                serieUpdateDtoRequest.ActiveState = activeState;
                serieUpdateDtoRequest.Retention = serieDtoRequest.Retention;
                serieUpdateDtoRequest.SecurityLevel = securityLevel;
                serieUpdateDtoRequest.Class = seriesClass;

                if (docTypoExist)
                {
                    if (currentOptionSecurityLevel)
                    {
                        serieUpdateDtoRequest.ProductionOfficeIdSL = IdproOfficeSL;
                        serieUpdateDtoRequest.UsersId = new();
                    }
                    else
                    {
                        serieUpdateDtoRequest.ProductionOfficeIdSL = 0;
                        serieUpdateDtoRequest.UsersId = idsUsers;
                    }
                }

                if (_selectedRecord.Retention == null || RetentionResponseEmpty(_selectedRecord.Retention))
                {
                    serieUpdateDtoRequest.Retention = null;
                }
                else
                {
                    serieUpdateDtoRequest.Retention = FillRetentionUpdate(_selectedRecord.Retention);
                }

                var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/Series/UpdateSeries", serieUpdateDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<SeriesDtoResponse>>();

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

        #region ResetFormAsync

        private async Task ResetFormAsync()
        {
            if (!IsEditForm)
            {
                serieDtoRequest = new();
            }
            else
            {
                serieUpdateDtoRequest.SeriesId = _selectedRecord.SeriesId;
                await GetDocumentaryTypologies(_selectedRecord.SeriesId);
                var reten = _selectedRecord.Retention != null ? new RetentionSSDtoResponse() : null;
                _selectedRecord = new();
                _selectedRecord.Retention = reten;
            }

            serieDtoRequest.Name = string.Empty;
            serieDtoRequest.Code = string.Empty;
            serieDtoRequest.Description = string.Empty;
            activeState = true;
            CharacterCounter = 0;

            #region DropDownLists

            IdDocumental = 0;
            idAdUnit = 0;
            IdproOffice = 0;
            administrativeUnitEnable = false;
            productionOfficeEnable = false;
            administrativeUnitsList = new();
            productionOfficesList = new();

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

            await GetDocumentalVersions();
            await GetSeriesClass();
            await GetSecurityLevels();

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

        #region UpdateSelectedRecord

        public async Task UpdateSelectedRecord(SeriesDtoResponse response)
        {
            //SpinnerLoaderService.ShowSpinnerLoader(Js);
            await GetDocumentaryTypologies(response.SeriesId);
            IsEditForm = true;
            _selectedRecord = response;

            await GetDocumentalVersions();
            IdDocumental = _selectedRecord.DocumentalVersionId;
            await GetAdministrativeUnits(IdDocumental);
            idAdUnit = _selectedRecord.AdministrativeUnitId;
            await GetProductionOffices(idAdUnit);
            IdproOffice = _selectedRecord.ProductionOfficeId;

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
            
            serieUpdateDtoRequest.SeriesId = _selectedRecord.SeriesId;
            retentionExist = _selectedRecord.Retention != null;
            idsUsers = new();
            activeState = _selectedRecord.ActiveState;
            serieDtoRequest.Code = _selectedRecord.Code;
            serieDtoRequest.Name = _selectedRecord.Name;
            securityLevel = _selectedRecord.SecurityLevel;
            seriesClass = _selectedRecord.Class;
            serieDtoRequest.ProductionOfficeId = _selectedRecord.ProductionOfficeId;
            serieDtoRequest.Description = string.IsNullOrEmpty(_selectedRecord.Description) ? "" : _selectedRecord.Description;
            CharacterCounter = string.IsNullOrEmpty(_selectedRecord.Description) ? 0 : serieDtoRequest.Description.Length;
            serieDtoRequest.ActiveState = _selectedRecord.ActiveState;
            currentOptionSecurityLevel = true;
            selectedOptionSecurityLevel = Translation["ProductionOffice"];
            EnableSaveButton();

            StateHasChanged();
            //SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion UpdateSelectedRecord

        #region UpdateModalStatus

        public async Task UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            //IsEditForm = optional;
            //await ResetFormAsync();
            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region HandleModalClosed

        private async Task HandleModalClosed(bool status)
        {
            modalStatus = status;
            CharacterCounter = 0;
            serieDtoRequest = new();
            IsEditForm = false;
            await ResetFormAsync();
            StateHasChanged();
        }

        #endregion HandleModalClosed

        #region HandleModalNotiCloseAsync

        private async Task HandleModalNotiCloseAsync(ModalClosedEventArgs args)
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

        #endregion HandleModalNotiCloseAsync

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
                    SeriesId = _selectedRecord.SeriesId
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

        private async Task GetDocumentaryTypologies(int idSerie)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            var request = new DocumentaryTypologiesFilterDtoRequest()
            {
                SeriesId = idSerie
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
                retentionPresent = !(serieDtoRequest.Retention != null && !RetentionEmpty(serieDtoRequest.Retention!));
            }

            if (optionSL || retentionPresent || IdDocumental <= 0 || idAdUnit <= 0 || IdproOffice <= 0 || string.IsNullOrEmpty(serieDtoRequest.Code) || string.IsNullOrEmpty(serieDtoRequest.Name) || string.IsNullOrEmpty(seriesClass) || string.IsNullOrEmpty(securityLevel))
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
                if (serieDtoRequest.Retention != null)
                {
                    modalRetentions.UpdateSelectedRecord(FillRetentionRequest(serieDtoRequest.Retention));
                }
            }
        }

        private RetentionSSDtoResponse FillRetentionRequest(RetentionSSDtoRequest trd)
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

        private void FillRetention(RetentionSSDtoRequest trd)
        {
            serieDtoRequest.Retention.TimeFileCentral = trd.TimeFileCentral;
            serieDtoRequest.Retention.TimeFileManagement = trd.TimeFileManagement;
            serieDtoRequest.Retention.TotalConservation = trd.TotalConservation;
            serieDtoRequest.Retention.Elimination = trd.Elimination;
            serieDtoRequest.Retention.Selection = trd.Selection;
            serieDtoRequest.Retention.TechEnvironment = trd.TechEnvironment;
            serieDtoRequest.Retention.ProcedureRet = trd.ProcedureRet;

            StateHasChanged();
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