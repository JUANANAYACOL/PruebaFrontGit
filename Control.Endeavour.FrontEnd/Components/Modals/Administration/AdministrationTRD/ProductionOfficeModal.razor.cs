using Control.Endeavour.FrontEnd.Components.Components.ButtonGroup;
using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD
{
    public partial class ProductionOfficeModal : ComponentBase
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
        private InputModalComponent inputCode { get; set; } = new();
        private InputModalComponent inputName { get; set; } = new();
        private ButtonGroupComponent inputBoss { get; set; } = new();
        private InputModalComponent inputDescription { get; set; } = new();

        #endregion Components

        #region Parameters

        [Parameter]
        public string idModalIdentifier { get; set; } = null!;

        [Parameter]
        public bool modalStatus { get; set; }

        [Parameter]
        public EventCallback<bool> OnStatusChanged { get; set; }

        [Parameter]
        public EventCallback<bool> OnStatusChangedUpdate { get; set; }

        #endregion Parameters

        #region Models

        private VUserDtoResponse BossSelected { get; set; } = new();
        private ProductionOfficeCreateDtoRequest ProductionOfficeRequest { get; set; } = new();
        private ProductionOfficeUpdateDtoRequest ProductionOfficeUpdateRequest { get; set; } = new();
        private List<AdministrativeUnitsDtoResponse> AdministrativeUnitList { get; set; } = new();

        #endregion Models

        #region Enviroments

        #region Environments(String)

        public string bossName { get; set; } = null!;

        #endregion Environments(String)

        #region Environments(Numeric)

        private decimal CharacterCounter { get; set; } = 0;
        private int IdAdUnit { get; set; } = 0;
        private int IdDocumental { get; set; } = 0;
        private int userId { get; set; }

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool isEditForm { get; set; } = false;
        private bool activeState { get; set; } = true;
        private bool administrativeUnitEnable { get; set; } = false;
        private bool saveIsDisable { get; set; } = true;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<DocumentalVersionDtoResponse> documentalVersionsList { get; set; } = new();
        private List<AdministrativeUnitsDtoResponse> administrativeUnitsList { get; set; } = new();

        #endregion Environments(List & Dictionary)

        #endregion Enviroments

        #endregion Variables

        #region HandleMethods

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetDocumentalVersions();
        }

        #endregion OnInitializedAsync

        #region HandleValidSubmit

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

        #endregion HandleValidSubmit

        #region HandleFormCreate

        private async Task HandleFormCreate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                if (inputBoss.IsInputValid)
                {
                    ProductionOfficeRequest.BossId = userId == 0 ? null : userId;
                    ProductionOfficeRequest.Name = inputName.InputValue;
                    ProductionOfficeRequest.Code = inputCode.InputValue;
                    ProductionOfficeRequest.ActiveState = activeState;
                    ProductionOfficeRequest.AdministrativeUnitId = IdAdUnit;

                    var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/ProductionOffice/CreateProductionOffice", ProductionOfficeRequest);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ProductionOfficesDtoResponse>>();
                    if (deserializeResponse!.Succeeded)
                    {
                        notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["CannotCreate"], true, Translation["Accept"]);
                    }
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["MandatoryFieldsMessage"], true, Translation["Accept"]);
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleFormCreate

        #region HandleFormUpdate

        private async Task HandleFormUpdate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                if (inputBoss.IsInputValid)
                {
                    ProductionOfficeUpdateRequest.AdministrativeUnitId = IdAdUnit;
                    ProductionOfficeUpdateRequest.BossId = ProductionOfficeRequest.BossId == 0 ? null : ProductionOfficeRequest.BossId;
                    ProductionOfficeUpdateRequest.Name = inputName.InputValue;
                    ProductionOfficeUpdateRequest.Code = inputCode.InputValue;
                    ProductionOfficeUpdateRequest.Description = ProductionOfficeRequest.Description;
                    ProductionOfficeUpdateRequest.ActiveState = activeState;

                    var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/ProductionOffice/UpdateProductionOffice", ProductionOfficeUpdateRequest);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ProductionOfficesDtoResponse>>();
                    if (deserializeResponse!.Succeeded)
                    {
                        notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["CannotCreate"], true, Translation["Accept"]);
                    }
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["MandatoryFieldsMessage"], true, Translation["Accept"]);
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleFormUpdate

        #region UpdateSelectedRecord

        public async Task UpdateSelectedRecord(ProductionOfficesDtoResponse record)
        {
            await GetDocumentalVersions();
            IdDocumental = record.DocumentalVersionId;
            await GetAdministrativeUnits(IdDocumental);
            IdAdUnit = record.AdministrativeUnitId;

            isEditForm = true;
            activeState = record.ActiveState;

            ProductionOfficeUpdateRequest.ProductionOfficeId = record.ProductionOfficeId;
            ProductionOfficeRequest.Name = string.IsNullOrEmpty(record.Name) ? "" : record.Name;
            ProductionOfficeRequest.Code = record.Code;
            ProductionOfficeRequest.ActiveState = record.ActiveState;
            ProductionOfficeRequest.BossId = record.BossId;
            ProductionOfficeRequest.AdministrativeUnitId = record.AdministrativeUnitId;
            ProductionOfficeRequest.ActiveState = record.ActiveState;
            ProductionOfficeRequest.Description = string.IsNullOrEmpty(record.Description) ? "" : record.Description;
            CharacterCounter = string.IsNullOrEmpty(ProductionOfficeRequest.Description) ? 0 : ProductionOfficeRequest.Description.Length;

            bossName = !record.BossId.HasValue ? "" : record.BossName!;
        }

        #endregion UpdateSelectedRecord

        #endregion HandleMethods

        #region OtherMethods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #region Modal

        #region OpenNewModal

        private async Task OpenNewModal()
        {
            await OnStatusChanged.InvokeAsync(true);
        }

        #endregion OpenNewModal

        #region UpdateModalStatus

        public async Task UpdateModalStatus(bool newValue)
        {
            isEditForm = false;
            modalStatus = newValue;
            await ResetFormAsync();
            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region HandleModalClosed

        private async Task HandleModalClosed(bool status)
        {
            await ResetFormAsync();
            modalStatus = status;
            StateHasChanged();
        }

        #endregion HandleModalClosed

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose()
        {
            if (notificationModal.Type == ModalType.Success)
            {
                await OnStatusChangedUpdate.InvokeAsync(false);
            }

            StateHasChanged();
        }

        #endregion HandleModalNotiClose

        #endregion Modal

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
                IdAdUnit = 0;
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

        #endregion GetMethods

        #region ResetFormAsync

        private async Task ResetFormAsync()
        {
            activeState = true;
            BossSelected = new();
            userId = 0;
            bossName = "";
            ProductionOfficeRequest.Name = "";
            ProductionOfficeRequest.Code = string.Empty;
            ProductionOfficeRequest.AdministrativeUnitId = 0;
            ProductionOfficeRequest.BossId = 0;
            ProductionOfficeRequest.Description = "";
            IdDocumental = 0;
            IdAdUnit = 0;
            administrativeUnitsList = new();
            administrativeUnitEnable = false;
            CharacterCounter = 0;
            await GetDocumentalVersions();
            StateHasChanged();
        }

        #endregion ResetFormAsync

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

        #region updateBossSelection

        public void updateBossSelection(VUserDtoResponse bossToSelect)
        {
            BossSelected = bossToSelect;
            ProductionOfficeRequest.BossId = bossToSelect.UserId;
            bossName = BossSelected.FullName;
            userId = BossSelected.UserId;
            StateHasChanged();
        }

        #endregion updateBossSelection

        #region EnableSaveButton

        public void EnableSaveButton()
        {
            if (IdDocumental <= 0 || IdAdUnit <= 0 || string.IsNullOrEmpty(ProductionOfficeRequest.Code) || string.IsNullOrEmpty(ProductionOfficeRequest.Name))
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

        #endregion OtherMethods
    }
}