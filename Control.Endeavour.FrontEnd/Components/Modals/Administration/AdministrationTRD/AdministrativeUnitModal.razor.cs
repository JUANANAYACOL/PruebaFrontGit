using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffice.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD
{
    public partial class AdministrativeUnitModal
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

        #region Modals

        private NotificationsComponentModal notificationModal = new();

        #endregion Modals

        #region Parameters

        [Parameter] public EventCallback<bool> OnStatusUpdate { get; set; }

        #endregion Parameters

        #region Models

        private InputModalComponent codeinput = new();
        private InputModalComponent nameinput = new();
        private VUserDtoResponse BossSelected = new();
        private AdministrativeUnitDtoRequest adminUnitDtoRequest { get; set; } = new();
        private AdministrativeUnitUpdateDtoRequest adminUnitUpdateDtoRequest { get; set; } = new();
        private AdministrativeUnitsDtoResponse adminUnitDtoResponse { get; set; } = new();
        private AdministrativeUnitsDtoResponse _selectedRecord { get; set; } = new();

        #endregion Models

        #region Environments

        #region Environments(Numeric)

        private decimal CharacterCounter = 0;
        private int IdDocumental { get; set; } = 0;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool IsEditForm { get; set; } = false;
        private bool activeState { get; set; } = true;
        private bool modalStatus { get; set; } = false;
        private bool IsDisabledCode { get; set; } = false;
        private bool saveIsDisable { get; set; } = true;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<DocumentalVersionDtoResponse> docVersionList { get; set; } = new();

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

        #region Forms

        #region HandleValidSubmit

        private async Task HandleValidSubmit()
        {
            try
            {
                if (IsEditForm)
                {
                    await HandleFormUpdate();
                }
                else
                {
                    await HandleFormCreate();
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true, Translation["Accept"]);
            }

            StateHasChanged();
        }

        #endregion HandleValidSubmit

        #region HandleFormCreate

        private async Task HandleFormCreate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            if (codeinput.IsInputValid) //revisar IsInputValue de Name
            {
                adminUnitDtoRequest.DocumentalVersionId = IdDocumental;
                adminUnitDtoRequest.Name = nameinput.InputValue;
                adminUnitDtoRequest.Code = codeinput.InputValue;
                adminUnitDtoRequest.Description = adminUnitDtoResponse.Description;
                adminUnitDtoRequest.ActiveState = activeState;

                var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/AdministrativeUnit/CreateAdministrativeUnit", adminUnitDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<AdministrativeUnitsDtoResponse>>();

                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                    await OnStatusUpdate.InvokeAsync(true);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
                }
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleFormCreate

        #region HandleFormUpdate

        private async Task HandleFormUpdate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            adminUnitUpdateDtoRequest.DocumentalVersionId = IdDocumental;
            adminUnitUpdateDtoRequest.Name = nameinput.InputValue;
            adminUnitUpdateDtoRequest.Code = codeinput.InputValue;
            adminUnitUpdateDtoRequest.Description = adminUnitDtoResponse.Description;
            adminUnitUpdateDtoRequest.ActiveState = activeState;

            var response = await HttpClient!.PostAsJsonAsync("paramstrd/AdministrativeUnit/UpdateAdministrativeUnit", adminUnitUpdateDtoRequest);

            var deserializeResponse = await response.Content.ReadFromJsonAsync<HttpResponseWrapperModel<BranchOfficesDtoResponse>>();

            if (deserializeResponse!.Succeeded)
            {
                notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                await OnStatusUpdate.InvokeAsync(true);
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleFormUpdate

        #endregion Forms

        #region Modal

        #region Notifications

        private async Task HandleModalNotiCloseAsync(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                await UpdateModalStatusAsync(args.ModalStatus);
                IsEditForm = false;
                await ResetFormAsync();
            }
            StateHasChanged();
        }

        #endregion Notifications

        #region HandleModalClosedAsync

        private async Task HandleModalClosedAsync(bool status)
        {
            modalStatus = status;
            IsEditForm = false;
            await ResetFormAsync();
            //await OnStatusUpdate.InvokeAsync(true);
            StateHasChanged();
        }

        #endregion HandleModalClosedAsync

        #endregion Modal

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #endregion HandleMethods

        #region OthersMethods

        #region ModalPrepareData


        #region UpdateModalStatusAsync

        public async Task UpdateModalStatusAsync(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        #endregion UpdateModalStatusAsync

        #region PreparedModal

        public async Task PreparedModal()
        {
            StateHasChanged();
            adminUnitDtoRequest = new();
            await GetDocumentalVersions();
        }

        #endregion PreparedModal

        #region UpdateSelectedRecord

        public async Task UpdateSelectedRecord(AdministrativeUnitsDtoResponse response)
        {
            _selectedRecord = response;

            await GetDocumentalVersions();

            IsEditForm = true;
            adminUnitUpdateDtoRequest.AdministrativeUnitId = _selectedRecord.AdministrativeUnitId;
            IdDocumental = _selectedRecord.DocumentalVersionId;
            activeState = _selectedRecord.ActiveState;
            adminUnitDtoResponse.Code = _selectedRecord.Code;
            adminUnitDtoResponse.Name = _selectedRecord.Name;
            adminUnitDtoResponse.Description = string.IsNullOrEmpty(_selectedRecord.Description) ? "" : _selectedRecord.Description;
            CharacterCounter = string.IsNullOrEmpty(adminUnitDtoResponse.Description) ? 0 : adminUnitDtoResponse.Description.Length;

            EnableSaveButton();
            StateHasChanged();
        }

        #endregion UpdateSelectedRecord

        #endregion ModalPrepareData

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
                    docVersionList = deserializeResponse.Data;
                    docVersionList.ForEach(x =>
                    {
                        x.Code = $"{((x.EndDate == null || x.EndDate.Value > DateTime.Now) ? $"{x.StartDate!.Value.Year} {toCurrent} - {documentaryVersion} {x.Code}" : $"{x.StartDate!.Value.Year} a {x.EndDate.Value.Year} - {documentaryVersion} {x.Code} {x.VersionType}")}";
                    });
                    docVersionList = docVersionList.OrderBy(x => x.StartDate).Reverse().ToList();
                }
                else
                {
                    docVersionList = new();
                    notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetDataMethods

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

        #region ResetFormAsync

        public async Task ResetFormAsync()
        {
            if (!IsEditForm)
            {
                adminUnitDtoResponse = new();
            }
            else
            {
                adminUnitDtoResponse.AdministrativeUnitId = _selectedRecord.AdministrativeUnitId;
            }

            IdDocumental = 0;
            adminUnitDtoResponse.Name = string.Empty;
            adminUnitDtoResponse.Code = string.Empty;
            adminUnitDtoResponse.Description = string.Empty;
            activeState = true;
            CharacterCounter = 0;

            _selectedRecord = new();

            await GetDocumentalVersions();
            EnableSaveButton();
            StateHasChanged();
        }

        #endregion ResetFormAsync

        #region EnableSaveButton

        public void EnableSaveButton()
        {
            if (IdDocumental <= 0 || string.IsNullOrEmpty(adminUnitDtoResponse.Code) || string.IsNullOrEmpty(adminUnitDtoResponse.Name))
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

        #endregion OthersMethods

        #endregion Methods
    }
}