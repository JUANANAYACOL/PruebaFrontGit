using Control.Endeavour.FrontEnd.Components.Components.ButtonGroup;
using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffice.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffice.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.BranchOffice
{
    public partial class BranchOfficesModal
    {
        #region Variables

        #region Inject

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Components

        private InputModalComponent inputId { get; set; } = new();
        private InputModalComponent inputCode { get; set; } = new();
        private InputModalComponent inputName { get; set; } = new();
        private InputModalComponent inputRegion { get; set; } = new();
        private InputModalComponent inputTerritory { get; set; } = new();
        private ButtonGroupComponent inputAddress { get; set; } = new();

        #endregion Components

        #region Modals

        private NotificationsComponentModal notificationModal { get; set; } = new();

        #endregion Modals

        #region Parameters

        [Parameter] public string IdModalIdentifier { get; set; } = "";

        [Parameter] public EventCallback<bool> OnChangeData { get; set; } = new();

        [Parameter]
        public EventCallback<AddressDtoResponse> OnDataSaved { get; set; }

        [Parameter] public EventCallback<bool> OnAddressStatus { get; set; } = new();

        #endregion Parameters

        #region Models

        private BranchOfficesDtoResponse _selectedRecord { get; set; } = new();
        private BranchOfficeDtoRequest branchOfficeRequest { get; set; } = new();

        private BranchOfficeUpdateDtoRequest requestUpdate { get; set; } = new();
        private AddressDtoRequest? addressRequest { get; set; }
        private AddressDtoResponse addressResponse = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string addressString { get; set; } = "";

        #endregion Environments(String)

        #region Environments(Bool)

        private bool IsActive { get; set; } = true;
        private bool IsEditForm { get; set; } = false;
        private bool modalStatus { get; set; } = false;
        private bool IsDisabledCode { get; set; } = false;
        private bool activeState { get; set; } = true;

        private bool saveIsDisable { get; set; } = true;

        #endregion Environments(Bool)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
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
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true, Translation["Accept"]);
            }

            StateHasChanged();
        }

        #endregion HandleValidSubmit

        #region HandleFormCreate

        private async Task HandleFormCreate()
        {
            try
            {
                branchOfficeRequest.Address = addressRequest;
                branchOfficeRequest.ActiveState = activeState;

                var responseApi = await HttpClient!.PostAsJsonAsync("params/BranchOffice/CreateBranchOffice", branchOfficeRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<BranchOfficesDtoResponse>>();

                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                    await OnChangeData.InvokeAsync(true);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
            }
        }

        #endregion HandleFormCreate

        #region HandleFormUpdate

        private AddressDtoRequest fillAddress(AddressDtoResponse address)
        {
            AddressDtoRequest? request = new AddressDtoRequest();
            request.CountryId = address.CountryId;
            request.StateId = address.StateId;
            request.CityId = address.CityId;
            request.StType = address.StType!;
            request.StNumber = address.StNumber!;
            request.StLetter = address.StLetter!;
            request.StBis = address.StBis != null ? (bool)address.StBis : false;
            request.StComplement = address.StComplement!;
            request.StCardinality = address.StCardinality!;
            request.CrType = address.CrType!;
            request.CrNumber = address.CrNumber!;
            request.CrLetter = address.CrLetter!;
            request.CrBis = address.CrBis != null ? (bool)address.CrBis : false;
            request.CrComplement = address.CrComplement!;
            request.CrCardinality = address.CrCardinality!;
            request.HouseType = address.HouseType!;
            request.HouseClass = address.HouseClass!;
            request.HouseNumber = address.HouseNumber!;
            return request;
        }

        public bool IsEmptyAddress(AddressDtoRequest address)
        {
            return address.CountryId == 0 &&
                   address.StateId == 0 &&
                   address.CityId == 0 &&
                   string.IsNullOrEmpty(address.StType) &&
                   string.IsNullOrEmpty(address.StNumber) &&
                   string.IsNullOrEmpty(address.StLetter) &&
                   ( address.StBis == null ) &&
                   string.IsNullOrEmpty(address.StComplement) &&
                   string.IsNullOrEmpty(address.StCardinality) &&
                   string.IsNullOrEmpty(address.CrType) &&
                   string.IsNullOrEmpty(address.CrNumber) &&
                   string.IsNullOrEmpty(address.CrLetter) &&
                   ( address.CrBis == null ) &&
                   string.IsNullOrEmpty(address.CrComplement) &&
                   string.IsNullOrEmpty(address.CrCardinality) &&
                   string.IsNullOrEmpty(address.HouseType) &&
                   string.IsNullOrEmpty(address.HouseClass) &&
                   string.IsNullOrEmpty(address.HouseNumber);
        }

        private async Task HandleFormUpdate()
        {
            try
            {
                requestUpdate.Address = addressRequest;
                requestUpdate.Code = branchOfficeRequest.Code;
                requestUpdate.NameOffice = branchOfficeRequest.NameOffice;
                requestUpdate.Region = branchOfficeRequest.Region;
                requestUpdate.ActiveState = activeState;
                requestUpdate.Territory = branchOfficeRequest.Territory;

                var response = await HttpClient!.PostAsJsonAsync("params/BranchOffice/UpdateBranchOffice", requestUpdate);

                var deserializeResponse = await response.Content.ReadFromJsonAsync<HttpResponseWrapperModel<BranchOfficesDtoResponse>>();

                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                    await OnChangeData.InvokeAsync(true);
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

            // Aca iria el consumo del micro servicio para actualizar
        }

        #endregion HandleFormUpdate

        #region ResetFormAsync

        // Método para restablecer el formulario.
        public void ResetFormAsync()
        {
          
                branchOfficeRequest.Code = string.Empty;
                requestUpdate = new();
        
            branchOfficeRequest.Region = string.Empty;
            branchOfficeRequest.Territory = string.Empty;
            activeState = true;
            branchOfficeRequest.NameOffice = string.Empty;
            branchOfficeRequest.Address = new();
            addressResponse = new();
            _selectedRecord = new();
            addressRequest = null;
            addressString = "";
            EnableSaveButton();
            StateHasChanged();
        }

        #endregion ResetFormAsync

        #region recieveBranchOffice

        // Método para actualizar el registro seleccionado.
        public async Task recieveBranchOfficeAsync(BranchOfficesDtoResponse response)
        {
            try
            {
                _selectedRecord = response;

                branchOfficeRequest.Code = _selectedRecord.Code;
                branchOfficeRequest.NameOffice = _selectedRecord.NameOffice;
                branchOfficeRequest.Region = _selectedRecord.Region;
                branchOfficeRequest.Territory = _selectedRecord.Territory;
                activeState = response.ActiveState;
                requestUpdate.BranchOfficeId = _selectedRecord.BranchOfficeId;
                addressString = response.AddressString;
                if (response.Address != null)
                {
                    addressRequest = new AddressDtoRequest()
                    {
                        CityId = response.Address.CityId,
                        CountryId = response.Address.CountryId,
                        CrBis = response.Address.CrBis ?? false,
                        CrCardinality = response.Address.CrCardinality,
                        CrComplement = response.Address.CrComplement,
                        CrLetter = response.Address.CrLetter,
                        CrNumber = response.Address.CrNumber,
                        CrType = response.Address.CrType,
                        HouseClass = response.Address.HouseClass,
                        HouseNumber = response.Address.HouseNumber,
                        HouseType = response.Address.HouseType,
                        StateId = response.Address.StateId,
                        StBis = response.Address.StBis ?? false,
                        StCardinality = response.Address.StCardinality,
                        StComplement = response.Address.StComplement,
                        StLetter = response.Address.StLetter,
                        StNumber = response.Address.StNumber,
                        StType = response.Address.StType
                    };
                }
                else
                {
                    addressRequest = null;
                }

                if (_selectedRecord.AddressId != null)
                {
                    await GetAddressAsync();
                }

                IsDisabledCode = true;
                IsEditForm = true;
                StateHasChanged();
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessageS"], true, Translation["Accept"]);
            }
        }

        #endregion recieveBranchOffice

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)
        {
            IsEditForm = false;
            modalStatus = newValue;
            StateHasChanged();
            ResetFormAsync();
          
        }

        #endregion UpdateModalStatus

        public void OpenCreateModal()
        {
            modalStatus = true;
            IsEditForm = false;
            ResetFormAsync();
            StateHasChanged();
        }

        #region HandleModalClosed

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
            branchOfficeRequest = new BranchOfficeDtoRequest();
            IsDisabledCode = false;

            StateHasChanged();
        }

        #endregion HandleModalClosed

        #region HandleModalNotiClose

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
            }
        }

        #endregion HandleModalNotiClose

        private async Task OpenNewModal()
        {
            await OnAddressStatus.InvokeAsync(true);
            if (addressResponse != null)
            {
                await OnDataSaved.InvokeAsync(addressResponse);
            }
        }

        #endregion OthersMethods

        #region GetAddress

        private async Task GetAddressAsync()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("IdAddress");
                HttpClient?.DefaultRequestHeaders.Add("IdAddress", $"{_selectedRecord.AddressId}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<AddressDtoResponse>>("administration/Address/ByFilterId");
                HttpClient?.DefaultRequestHeaders.Remove("IdAddress");

                if (deserializeResponse!.Succeeded)
                {
                    addressResponse = deserializeResponse.Data!;
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessageS"], true, Translation["Accept"]);
            }
        }

        #endregion GetAddress

        #region updateAddressSelection

        private AddressDtoResponse fillAddressUpdate(AddressDtoRequest address)
        {
            AddressDtoResponse? request = new AddressDtoResponse();
            request.CountryId = address.CountryId;
            request.StateId = address.StateId;
            request.CityId = address.CityId;
            request.StType = address.StType!;
            request.StNumber = address.StNumber!;
            request.StLetter = address.StLetter!;
            request.StBis = address.StBis != null ? (bool)address.StBis : false;
            request.StComplement = address.StComplement!;
            request.StCardinality = address.StCardinality!;
            request.CrType = address.CrType!;
            request.CrNumber = address.CrNumber!;
            request.CrLetter = address.CrLetter!;
            request.CrBis = address.CrBis != null ? (bool)address.CrBis : false;
            request.CrComplement = address.CrComplement!;
            request.CrCardinality = address.CrCardinality!;
            request.HouseType = address.HouseType!;
            request.HouseClass = address.HouseClass!;
            request.HouseNumber = address.HouseNumber!;
            return request;
        }

        public void updateAddressSelection(List<(string, AddressDtoRequest)> address)
        {
            if (address != null && address.Count > 0)
            {
                (addressString, addressRequest) = address[0];
            }
            addressResponse = fillAddressUpdate(address[0].Item2);
            StateHasChanged();
        }

        #endregion updateAddressSelection

        public void EnableSaveButton()
        {
            if (string.IsNullOrEmpty(branchOfficeRequest.Code) || string.IsNullOrEmpty(branchOfficeRequest.NameOffice))
            {
                saveIsDisable = true;
            }
            else
            {
                saveIsDisable = false;
            }
            StateHasChanged();
        }

        #endregion Methods
    }
}