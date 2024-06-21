using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdParty.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.User.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.ThirdParty
{
    public partial class ThirdPartyModal : ComponentBase
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

        //Listas
        private List<VSystemParamDtoResponse>? identificationTypeList;

        private List<VSystemParamDtoResponse>? chargeList;
        private List<VSystemParamDtoResponse>? natureList;

        //Inputs
        private InputModalComponent thirdPartyId { get; set; } = new();

        private InputModalComponent identification { get; set; } = new();
        private InputModalComponent names { get; set; } = new();
        private InputModalComponent email1 { get; set; } = new();
        private InputModalComponent email2 { get; set; } = new();
        private InputModalComponent chargue { get; set; } = new();
        private InputModalComponent webpage { get; set; } = new();
        private InputModalComponent lastNames { get; set; } = new();
        private InputModalComponent initials { get; set; } = new();
        private InputModalComponent phone1 { get; set; } = new();
        private InputModalComponent phone2 { get; set; } = new();

        #endregion Components

        #region Modals

        private NotificationsComponentModal notificationModal = new();

        #endregion Modals

        #region Parameters

        [Parameter]
        public bool ModalStatus { get; set; } = false;

        [Parameter]
        public string Id { get; set; } = "";

        [Parameter]
        public bool CrearEditar { get; set; }

        [Parameter]
        public EventCallback<bool> OnStatusChanged { get; set; }

        [Parameter]
        public EventCallback<bool> OnChangeData { get; set; }

        [Parameter]
        public EventCallback<string> OnPersonType { get; set; }

        [Parameter]
        public string PersonType { get; set; } = "";

        [Parameter]
        public EventCallback<AddressDtoResponse> OnDataSaved { get; set; }

        [Parameter]
        public EventCallback OnResetForm { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }

        #endregion Parameters

        #region Models

        private ThirdPartyDtoResponse _selectedRecord = new();
        private ThirdPartyCreateDtoRequest thirdPartyDtoRequest = new();
        private ThirdPartyUpdateDtoRequest thirdPartyUpdateDtoRequest = new();
        private ThirdPartyDtoResponse thirdPartyDtoResponse = new();
        private AddressDtoRequest? addressDtoRequest = null;
        private AddressDtoResponse addressDtoResponse = new();
        private NotificationService NotificationService { get; set; } = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string textAddress = "";
        private string personType { get; set; } = "";
        private string chargeCode = "";
        private string natureCode { get; set; } = "";
        private string identificationTypeCode = "";
        private string pattern { get; set; } = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        private string patternNumeric { get; set; } = @"\d";

        #endregion Environments(String)

        #region Environments(Numeric)

        private int addressId;
        private int IdThirdParty = 0;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool lastNameActive { get; set; }
        private bool activeState = true;
        private bool IsEditForm = false;
        private bool saveIsDisable { get; set; } = true;

        #endregion Environments(Bool)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator!.LanguageChangedEvent += HandleLanguageChanged;
            ModalStatus = false;
            await GetIdentificationType();
            await GetNature();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #region OtherMethods

        #region Gets

        #region GetDropDownLists

        #region GetIdentificationType

        private async Task GetIdentificationType()
        {
            var type = "";
            if (personType == "" || personType == null)
            {
                type = "TDIN";
            }
            else
            {
                type = ( personType == "PN" ) ? "TDIN" : "TDIJ";
            }

            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", $"{type}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                if (deserializeResponse!.Succeeded)
                {
                    identificationTypeList = deserializeResponse.Data;
                }
                else
                {
                    identificationTypeList = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de identificaciones: {ex.Message}");
            }
        }

        #endregion GetIdentificationType

        #region GetNature

        private async Task GetNature()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "NAT");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                if (deserializeResponse!.Succeeded)
                {
                    natureList = deserializeResponse.Data;
                }
                else
                {
                    natureList = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las naturalezas: {ex.Message}");
            }
        }

        #endregion GetNature

        #endregion GetDropDownLists

        #endregion Gets

        #region Address

        #region GetAddress

        private async Task GetAddressAsync(int addressId)
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("IdAddress");
                HttpClient?.DefaultRequestHeaders.Add("IdAddress", addressId.ToString());
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<AddressDtoResponse>>("administration/Address/ByFilterId");
                HttpClient?.DefaultRequestHeaders.Remove("IdAddress");

                if (deserializeResponse!.Succeeded)
                {
                    addressDtoResponse = deserializeResponse.Data!;
                    addressDtoRequest = fillAddress(addressDtoResponse);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessageS"], true);
                }

                EnableSaveButton();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las naturalezas: {ex.Message}");
            }
        }

        #endregion GetAddress

        #region FillAddress

        private AddressDtoRequest fillAddress(AddressDtoResponse address)
        {
            AddressDtoRequest? request = new AddressDtoRequest();
            request.CountryId = address.CountryId;
            request.StateId = address.StateId;
            request.CityId = address.CityId;
            request.StType = address.StType!;
            request.StNumber = address.StNumber!;
            request.StLetter = address.StLetter!;
            request.StBis = (bool)address.StBis!;
            request.StComplement = address.StComplement!;
            request.StCardinality = address.StCardinality!;
            request.CrType = address.CrType!;
            request.CrNumber = address.CrNumber!;
            request.CrLetter = address.CrLetter!;
            request.CrBis = (bool)address.CrBis!;
            request.CrComplement = address.CrComplement!;
            request.CrCardinality = address.CrCardinality!;
            request.HouseType = address.HouseType!;
            request.HouseClass = address.HouseClass!;
            request.HouseNumber = address.HouseNumber!;
            return request;
        }

        #endregion FillAddress

        #region UpdateAddress

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
                (textAddress, addressDtoRequest) = address[0];
            }
            addressDtoResponse = fillAddressUpdate(address[0].Item2);
            EnableSaveButton();
            StateHasChanged();
        }

        #endregion UpdateAddress

        #region EmptyAddress

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

        #endregion EmptyAddress

        #endregion Address

        #region Record

        #region ReceiveThirdParty

        public async Task ReceiveThirdPartyAsync(ThirdPartyDtoResponse response)
        {
            ResetFormAsync();
            _selectedRecord = response;
            IdThirdParty = _selectedRecord.ThirdPartyId;

            textAddress = _selectedRecord.Address!;
            activeState = _selectedRecord.ActiveState;
            thirdPartyDtoResponse.IdentificationNumber = _selectedRecord.IdentificationNumber;
            thirdPartyDtoResponse.Names = _selectedRecord.FullName.Replace(_selectedRecord.LastName, string.Empty);
            thirdPartyDtoResponse.ActiveState = _selectedRecord.ActiveState;
            thirdPartyDtoResponse.Email1 = _selectedRecord.Email1;
            thirdPartyDtoResponse.Email2 = _selectedRecord.Email2;
            thirdPartyDtoResponse.WebPage = _selectedRecord.WebPage;
            thirdPartyDtoResponse.FullName = _selectedRecord.FullName;
            thirdPartyDtoResponse.FirstName = _selectedRecord.FirstName;
            thirdPartyDtoResponse.MiddleName = _selectedRecord.MiddleName;
            thirdPartyDtoResponse.LastName = _selectedRecord.LastName;
            thirdPartyDtoResponse.Initials = _selectedRecord.Initials;
            thirdPartyDtoResponse.Phone1 = _selectedRecord.Phone1;
            thirdPartyDtoResponse.Phone2 = _selectedRecord.Phone2;

            identificationTypeCode = _selectedRecord.IdentificationType;
            thirdPartyDtoResponse.IdentificationType = _selectedRecord.IdentificationType;

            thirdPartyDtoResponse.Charge = _selectedRecord.Charge!;

            natureCode = _selectedRecord.NatureCode!;
            thirdPartyDtoResponse.NatureCode = _selectedRecord.NatureCode;
            IsEditForm = true;
            if (_selectedRecord.AddressId != 0 && _selectedRecord.AddressId != null)
            {
                addressId = _selectedRecord.AddressId;
                await GetAddressAsync(addressId);
            }

            EnableSaveButton();
            StateHasChanged();
        }

        #endregion ReceiveThirdParty

        #region ResetForm

        public void ResetFormAsync()
        {
            thirdPartyDtoRequest = new ThirdPartyCreateDtoRequest();
            thirdPartyUpdateDtoRequest = new ThirdPartyUpdateDtoRequest();
            thirdPartyDtoResponse = new ThirdPartyDtoResponse();
            addressDtoRequest = null;
            addressDtoResponse = new();
            addressId = 0;
            textAddress = "";
            IdThirdParty = 0;
            identificationTypeCode = "";
            chargeCode = "";
            natureCode = "";
            activeState = true;
            StateHasChanged();
        }

        #endregion ResetForm

        #endregion Record

        #region Modal

        #region OpenNewModal

        private async Task OpenNewModal()
        {
            await OnStatusChanged.InvokeAsync(true);
            if (addressDtoResponse != null)
            {
                await OnDataSaved.InvokeAsync(addressDtoResponse);
            }
            EnableSaveButton();
        }

        #endregion OpenNewModal

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)
        {
            ModalStatus = newValue;
            StateHasChanged();
            EnableSaveButton();
        }

        #endregion UpdateModalStatus

        #region HandleModalClosed

        private void HandleModalClosed(bool status)
        {
            ModalStatus = status;
            IsEditForm = false;
            OnResetForm.InvokeAsync();
            ResetFormAsync();
            StateHasChanged();
        }

        #endregion HandleModalClosed

        #region HandleModalNotiClose

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                ResetFormAsync();
                OnResetForm.InvokeAsync();
                UpdateModalStatus(args.ModalStatus);
            }
        }

        #endregion HandleModalNotiClose

        #region PersonTypeSelectedModal

        public async Task PersonTypeSelectedModal(int persontype)
        {
            personType = ( persontype == 0 ) ? "PN" : "PJ";
            lastNameActive = ( persontype == 0 );
            await GetIdentificationType();
            StateHasChanged();
        }

        #endregion PersonTypeSelectedModal

        #endregion Modal

        #region FormMethods

        #region SelectedMethod

        private async Task HandleValidSubmit()
        {
            try
            {
                if (personType.Equals("PN") && Regex.IsMatch(thirdPartyDtoResponse.Names, patternNumeric))
                {
                    notificationModal.UpdateModal(ModalType.Information, string.Format(Translation["NotValidParameter"], $"{Translation["Names"]}", $"{Translation["Number(s)"]}"), true, Translation["Accept"], "");
                }
                else if (!string.IsNullOrEmpty(thirdPartyDtoResponse.LastName) && Regex.IsMatch(thirdPartyDtoResponse.LastName, patternNumeric))
                {
                    notificationModal.UpdateModal(ModalType.Information, string.Format(Translation["NotValidParameter"], $"{Translation["LastName"]}", $"{Translation["Number(s)"]}"), true, Translation["Accept"], "");
                }
                else if (string.IsNullOrEmpty(thirdPartyDtoResponse.Email2) ? !Regex.IsMatch(thirdPartyDtoResponse.Email1, pattern) : !( Regex.IsMatch(thirdPartyDtoResponse.Email1, pattern) && Regex.IsMatch(thirdPartyDtoResponse.Email2, pattern) ))
                {
                    notificationModal.UpdateModal(ModalType.Information, Translation["VerifyEmail"], true, Translation["Accept"], "");
                }
                else
                {
                    if (IsEditForm)
                    {
                        await Update();
                    }
                    else
                    {
                        await Create();
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["BeServerErrorException"], true, Translation["Accept"], "");
            }

            StateHasChanged();
        }

        #endregion SelectedMethod

        #region FullName

        public static (string firstName, string middleName) SplitFirstAndMiddleName(string fullName)
        {
            string[]? nameParts = fullName.Trim().Split(' ');

            string? firstName = nameParts[0];
            string? middleName = nameParts.Length > 1 ? nameParts[1] : string.Empty;

            return (firstName, middleName);
        }

        #endregion FullName

        #region CreateThirdParty

        private async Task Create()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                thirdPartyDtoRequest = new();
                thirdPartyDtoRequest.PersonType = personType!;
                thirdPartyDtoRequest.Address = addressDtoRequest;

                thirdPartyDtoRequest.IdentificationType = identificationTypeCode;
                thirdPartyDtoRequest.IdentificationNumber = thirdPartyDtoResponse.IdentificationNumber;
                thirdPartyDtoRequest.ActiveState = activeState;
                thirdPartyDtoRequest.Names = thirdPartyDtoResponse.Names;

                // ThirData
                if (personType == "PJ")
                {
                    thirdPartyDtoRequest.Names = thirdPartyDtoResponse.Names;
                    thirdPartyDtoRequest.FirstName = null;
                    thirdPartyDtoRequest.MiddleName = null;
                    thirdPartyDtoRequest.LastName = null;
                    thirdPartyDtoRequest.Initials = thirdPartyDtoResponse.Initials;
                    thirdPartyDtoRequest.Charge = thirdPartyDtoResponse.Charge;
                }
                else
                {
                    thirdPartyDtoRequest.FirstName = thirdPartyDtoResponse.Names;

                    thirdPartyDtoRequest.LastName = thirdPartyDtoResponse.LastName;
                    thirdPartyDtoRequest.Names = $"{thirdPartyDtoResponse.Names} {thirdPartyDtoResponse.LastName}";
                    thirdPartyDtoRequest.Initials = null;
                }
                thirdPartyDtoRequest.Email1 = thirdPartyDtoResponse.Email1;
                thirdPartyDtoRequest.Email2 = string.IsNullOrEmpty(thirdPartyDtoResponse.Email2) ? null : thirdPartyDtoResponse.Email2;
                thirdPartyDtoRequest.WebPage = webpage!.InputValue;
                thirdPartyDtoRequest.Charge = string.IsNullOrEmpty(thirdPartyDtoResponse.Charge) ? null : thirdPartyDtoResponse.Charge;
                thirdPartyDtoRequest.NatureCode = string.IsNullOrEmpty(natureCode) ? null : natureCode;
                thirdPartyDtoRequest.Phone1 = thirdPartyDtoResponse.Phone1;
                thirdPartyDtoRequest.Phone2 = string.IsNullOrEmpty(thirdPartyDtoResponse.Phone2) ? null : thirdPartyDtoResponse.Phone2;

                var responseApi = await HttpClient!.PostAsJsonAsync("administration/ThirdParty/CreateThirdParty", thirdPartyDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ThirdPartyDtoResponse>>();
                var NotiResult = NotificationService.NotificationDecrypt(Translation, deserializeResponse);
                notificationModal.UpdateModal(NotiResult.ModalType, NotiResult.NotificationMessage, true, Translation["Accept"], "");
                if (deserializeResponse!.Succeeded)
                {
                    await OnChangeData.InvokeAsync(true);
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"], "");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion CreateThirdParty

        #region UpdateThirdParty

        private async Task Update()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                thirdPartyUpdateDtoRequest = new();
                thirdPartyUpdateDtoRequest.Address = addressDtoRequest;
                thirdPartyUpdateDtoRequest.IdentificationType = identificationTypeCode;
                thirdPartyUpdateDtoRequest.IdentificationNumber = thirdPartyDtoResponse.IdentificationNumber;
                thirdPartyUpdateDtoRequest.Names = thirdPartyDtoResponse.Names;
                thirdPartyUpdateDtoRequest.ActiveState = activeState;
                // ThirData
                if (personType == "PJ")
                {
                    thirdPartyUpdateDtoRequest.Names = thirdPartyDtoResponse.Names;
                    thirdPartyUpdateDtoRequest.Initials = thirdPartyDtoResponse.Initials;
                    thirdPartyUpdateDtoRequest.Email2 = string.IsNullOrEmpty(thirdPartyDtoResponse.Email2) ? null : thirdPartyDtoResponse.Email2;
                    thirdPartyUpdateDtoRequest.NatureCode = string.IsNullOrEmpty(natureCode) ? null : natureCode;
                }
                else
                {
                    thirdPartyUpdateDtoRequest.FirstName = thirdPartyDtoResponse.Names;

                    thirdPartyUpdateDtoRequest.LastName = thirdPartyDtoResponse.LastName;
                    thirdPartyUpdateDtoRequest.Names = $"{thirdPartyDtoResponse.FirstName}";
                    thirdPartyUpdateDtoRequest.Initials = null;
                    thirdPartyUpdateDtoRequest.Email2 = null;
                    thirdPartyUpdateDtoRequest.Charge = string.IsNullOrEmpty(thirdPartyDtoResponse.Charge) ? null : thirdPartyDtoResponse.Charge;
                }
                thirdPartyUpdateDtoRequest.Email1 = thirdPartyDtoResponse.Email1;
                thirdPartyUpdateDtoRequest.WebPage = thirdPartyDtoResponse.WebPage;
                thirdPartyUpdateDtoRequest.Phone1 = thirdPartyDtoResponse.Phone1;
                thirdPartyUpdateDtoRequest.Phone2 = string.IsNullOrEmpty(thirdPartyDtoResponse.Phone2) ? null : thirdPartyDtoResponse.Phone2;
                thirdPartyUpdateDtoRequest.ThirdPartyId = IdThirdParty;

                var responseApi = await HttpClient!.PostAsJsonAsync("administration/ThirdParty/UpdateThirdParty", thirdPartyUpdateDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<ThirdPartyDtoResponse>>();
                var NotiResult = NotificationService.NotificationDecrypt(Translation, deserializeResponse);
                notificationModal.UpdateModal(NotiResult.ModalType, NotiResult.NotificationMessage, true, Translation["Accept"], "");

                if (deserializeResponse!.Succeeded)
                {
                    await OnChangeData.InvokeAsync(true);
                    IsEditForm = false;
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"], "");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion UpdateThirdParty

        #region EnableSaveButton

        private void EnableSaveButton()
        {
            if (personType.Equals("PN"))
            {
                thirdPartyDtoResponse.Names = string.IsNullOrEmpty(thirdPartyDtoResponse.Names) ? string.Empty : Regex.Replace(thirdPartyDtoResponse.Names, patternNumeric, string.Empty);
            }

            thirdPartyDtoResponse.LastName = string.IsNullOrEmpty(thirdPartyDtoResponse.LastName) ? string.Empty : Regex.Replace(thirdPartyDtoResponse.LastName, patternNumeric, string.Empty);

            if (personType.Equals("PN") && checkConditions() && addressDtoRequest != null && !string.IsNullOrEmpty(thirdPartyDtoResponse.LastName))
            {
                saveIsDisable = false;
            }
            else if (personType.Equals("PJ") && checkConditions() && addressDtoRequest != null)
            {
                saveIsDisable = false;
            }
            else
            {
                saveIsDisable = true;
            }

            StateHasChanged();
        }

        private bool checkConditions()
        {
            bool resuilt = ( !string.IsNullOrEmpty(identificationTypeCode) && !string.IsNullOrEmpty(thirdPartyDtoResponse.IdentificationNumber) && !string.IsNullOrEmpty(thirdPartyDtoResponse.Names) && !string.IsNullOrEmpty(thirdPartyDtoResponse.Email1)
                   && !string.IsNullOrEmpty(thirdPartyDtoResponse.Phone1) );

            return resuilt;
        }

        #endregion EnableSaveButton

        #endregion FormMethods

        #endregion OtherMethods

        #endregion Methods
    }
}