using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Spinners;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.City.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Country.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Replacement.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.State.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using DevExpress.DirectX.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.Address
{
    public partial class AddressModal : ComponentBase
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

        private InputModalComponent stNumber { get; set; } = new();
        private InputModalComponent stLetter { get; set; } = new();
        private InputModalComponent stComplement { get; set; } = new();
        private InputModalComponent crNumber { get; set; } = new();
        private InputModalComponent crLetter { get; set; } = new();
        private InputModalComponent crComplement { get; set; } = new();
        private InputModalComponent houseNumber { get; set; } = new();
        private MetaModel meta { get; set; } = new() { PageSize = 10 };

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
        public EventCallback<MyEventArgs<List<(string, AddressDtoRequest)>>> OnStatusChangedMultipleSelection { get; set; }

        #endregion Parameters

        #region Models

        //Request and Response
        private AddressDtoRequest addressDtoRequest = new();

        private AddressDtoResponse _selectedRecord = new();
        private AddressDtoResponse addressDtoResponse = new();

        //Listas de DropDownLists
        private List<VSystemParamDtoResponse> cardinalityList = new();

        private List<VSystemParamDtoResponse> houseClassList = new();
        private List<VSystemParamDtoResponse> houseTypeList = new();
        private List<VSystemParamDtoResponse> scTypeList = new();
        private List<CountryDtoResponse> countryList = new();
        private List<StateDtoResponse> stateList = new();
        private List<CityDtoResponse> cityList = new();
        private List<(string, AddressDtoRequest)> createResult = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string address = "";
        private string stType = "";
        private string stCardinality = "";
        private string crCardinality = "";
        private string crType = "";

        private string houseType = "";
        private string houseClass = "";

        #endregion Environments(String)

        #region Environments(Numeric)

        private int addressId;

        #endregion Environments(Numeric)

        #region Environments(DateTime)

        private int country = 0;
        private int state = 0;
        private int city = 0;

        #endregion Environments(DateTime)

        #region Environments(Bool)

        private bool EnabledDepartamento { get; set; } = false;
        private bool EnabledMunicipio { get; set; } = false;

        private bool saveIsDisable { get; set; } = true;

        private bool? stBis = false;

        private bool? crBis = false;

        #endregion Environments(Bool)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator!.LanguageChangedEvent += HandleLanguageChanged;
            ModalStatus = false;
            await GetCardinality();
            await GetHouseClass();
            await GetHouseType();
            await GetSCType();
            await GetCountry();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #region GetAddress

        private async Task GetAddressAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("IdAddress");
                HttpClient?.DefaultRequestHeaders.Add("IdAddress", $"{addressId}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<AddressDtoResponse>>("administration/Address/ByFilterId");
                HttpClient?.DefaultRequestHeaders.Remove("IdAddress");

                if (deserializeResponse!.Succeeded)
                {
                    addressDtoResponse = deserializeResponse.Data!;
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessageS"], true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetAddress

        #region GetsLocation

        #region GetCountry

        private async Task GetCountry()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<CountryDtoResponse>>>("location/Country/ByFilter");
                countryList = deserializeResponse!.Data != null ? deserializeResponse.Data : new List<CountryDtoResponse>();

                if (countryList.Count > 0)
                {
                    meta = deserializeResponse.Meta!;
                    EnabledDepartamento = false;
                    EnabledMunicipio = false;
                }
                else
                {
                    EnabledDepartamento = false;
                    EnabledMunicipio = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Translation["LoadErrorMessage"]}: {ex.Message}");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetCountry

        #region GetState

        private async Task GetState()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            if (country > 0)
            {
                try
                {
                    EnabledDepartamento = true;
                    EnabledMunicipio = false;

                    HttpClient?.DefaultRequestHeaders.Remove("countryId");
                    HttpClient?.DefaultRequestHeaders.Add("countryId", country.ToString());
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<StateDtoResponse>>>("location/State/ByFilter");
                    HttpClient?.DefaultRequestHeaders.Remove("countryId");

                    stateList = deserializeResponse!.Data != null ? deserializeResponse.Data : new List<StateDtoResponse>();

                    if (stateList.Count > 0)
                    {
                        meta = deserializeResponse.Meta!;
                        state = 0;
                    }
                    else
                    {
                        EnabledDepartamento = false;
                        EnabledMunicipio = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{Translation["LoadErrorMessage"]}: {ex.Message}");
                }
            }
            else
            {
                state = 0;
                city = 0;
                EnabledDepartamento = false;
                EnabledMunicipio = false;
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
            EnableSaveButton();
            StateHasChanged();
        }

        #endregion GetState

        #region GetCity

        private async Task GetCity()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            if (state > 0)
            {
                try
                {
                    EnabledMunicipio = true;

                    HttpClient?.DefaultRequestHeaders.Remove("stateId");
                    HttpClient?.DefaultRequestHeaders.Add("stateId", state.ToString());
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<CityDtoResponse>>>("location/City/ByFilter");
                    HttpClient?.DefaultRequestHeaders.Remove("stateId");

                    cityList = deserializeResponse!.Data != null ? deserializeResponse.Data : new List<CityDtoResponse>();

                    if (cityList.Count > 0)
                    {
                        meta = deserializeResponse.Meta!;
                    }
                    else
                    {
                        EnabledMunicipio = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{Translation["LoadErrorMessage"]}: {ex.Message}");
                }
            }
            else
            {
                city = 0;
                EnabledMunicipio = false;
            }

            EnableSaveButton();
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetCity

        #endregion GetsLocation

        #region GetsParams

        #region GetCardinality

        private async Task GetCardinality()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("ParamCode");
                HttpClient?.DefaultRequestHeaders.Add("ParamCode", "CARD");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("ParamCode");
                if (deserializeResponse!.Succeeded)
                {
                    cardinalityList = deserializeResponse.Data!;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Translation["LoadErrorMessage"]}: {ex.Message}");
            }
        }

        #endregion GetCardinality

        #region GetHouseClass

        private async Task GetHouseClass()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("ParamCode");
                HttpClient?.DefaultRequestHeaders.Add("ParamCode", "HC");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("ParamCode");
                if (deserializeResponse!.Succeeded)
                {
                    houseClassList = deserializeResponse.Data!;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Translation["LoadErrorMessage"]}: {ex.Message}");
            }
        }

        #endregion GetHouseClass

        #region GetHouseType

        private async Task GetHouseType()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("ParamCode");
                HttpClient?.DefaultRequestHeaders.Add("ParamCode", "HT");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("ParamCode");
                if (deserializeResponse!.Succeeded)
                {
                    houseTypeList = deserializeResponse.Data!;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Translation["LoadErrorMessage"]}: {ex.Message}");
            }
        }

        #endregion GetHouseType

        #region GetSCType

        private async Task GetSCType()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("ParamCode");
                HttpClient?.DefaultRequestHeaders.Add("ParamCode", "SCT");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("ParamCode");
                if (deserializeResponse!.Succeeded)
                {
                    scTypeList = deserializeResponse.Data!;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Translation["LoadErrorMessage"]}: {ex.Message}");
            }
        }

        #endregion GetSCType

        #endregion GetsParams

        #region GetRecord

        #region GetRecordSelected

        public async Task UpdateModalIdAsync(int id)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            addressId = id;
            await GetAddressAsync();
            await ReceiveRecordsAsync(addressDtoResponse!);
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        public async Task UpdateModalNewAddressAsync(AddressDtoResponse newAddress)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            addressDtoResponse = newAddress;
            await ReceiveRecordsAsync(addressDtoResponse!);
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        public void ResetForm()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            ResetFormAsync();
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetRecordSelected

        #region ReceiveRecordsAsync

        public async Task ReceiveRecordsAsync(AddressDtoResponse response)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            _selectedRecord = response;

            addressDtoRequest.StNumber = _selectedRecord.StNumber!;
            addressDtoRequest.StType = _selectedRecord.StType!;
            addressDtoRequest.StLetter = _selectedRecord.StLetter!;
            addressDtoRequest.StBis = _selectedRecord.StBis != null ? (bool)_selectedRecord.StBis : false;
            addressDtoRequest.StComplement = _selectedRecord.StComplement!;
            addressDtoRequest.CrNumber = _selectedRecord.CrNumber!;
            addressDtoRequest.CrType = _selectedRecord.CrType!;
            addressDtoRequest.CrLetter = _selectedRecord.CrLetter!;
            addressDtoRequest.CrBis = _selectedRecord.CrBis != null ? (bool)_selectedRecord.CrBis : false;
            addressDtoRequest.CrComplement = _selectedRecord.CrComplement!;
            addressDtoRequest.HouseType = _selectedRecord.HouseType!;
            addressDtoRequest.HouseClass = _selectedRecord.HouseClass!;
            addressDtoRequest.HouseNumber = _selectedRecord.HouseNumber!;

            stBis = addressDtoRequest.StBis;
            crBis = addressDtoRequest.CrBis;

            // dropdown StCardinality
            stCardinality = _selectedRecord.StCardinality!;

            // dropdown CrCardinality
            crCardinality = _selectedRecord.CrCardinality!;

            //dropdown StType
            stType = _selectedRecord.StType!;

            //dropdown CrType
            crType = _selectedRecord.CrType!;

            //dropdown HouseType
            houseType = _selectedRecord.HouseType!;

            //dropdown HouseCLass
            houseClass = _selectedRecord.HouseClass!;

            //dropdown Country
            addressDtoRequest.CountryId = _selectedRecord.CountryId!;
            country = _selectedRecord.CountryId;

            EnabledDepartamento = true;
            EnabledMunicipio = true;
            await GetState();

            //dropdown State
            state = _selectedRecord.StateId;
            addressDtoRequest.StateId = _selectedRecord.StateId;

            await GetCity();

            //dropdown City
            city = _selectedRecord.CityId;
            addressDtoRequest.CityId = _selectedRecord.CityId;

            ActualizarDireccion();
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion ReceiveRecordsAsync

        #endregion GetRecord

        #region GetValuesParams

        //Metodo para tomar el Value de StType, CrType, HouseType y HouseClass
        private string GetValueFromList(List<VSystemParamDtoResponse> itemList, string selectedValue)
        {
            // Verificar si selectedValue es nulo o vacío
            if (string.IsNullOrEmpty(selectedValue))
            {
                return "";
            }

            var selectedDataItem = itemList.Find(item => item.Code == selectedValue);
            return selectedDataItem?.Value ?? "";
        }

        //Uso del Metodo de obtener valor, para cada Param
        private string valueCardinality(string selectedValue)
        {
            return GetValueFromList(cardinalityList, selectedValue);
        }

        private string valueStType(string selectedValue)
        {
            return GetValueFromList(scTypeList, selectedValue);
        }

        private string valueCrType(string selectedValue)
        {
            return GetValueFromList(scTypeList, selectedValue);
        }

        private string valueHouseType(string selectedValue)
        {
            return GetValueFromList(houseTypeList, selectedValue);
        }

        private string valueHouseClass(string selectedValue)
        {
            return GetValueFromList(houseClassList, selectedValue);
        }

        #endregion GetValuesParams

        #region RefreshAddress

        private void ActualizarDireccion()
        {
            var stnumber = addressDtoRequest.StNumber;
            var stletter = addressDtoRequest.StLetter;
            var stcomplement = addressDtoRequest.StComplement;
            var crnumber = addressDtoRequest.CrNumber;
            var crletter = addressDtoRequest.CrLetter;
            var crcomplement = addressDtoRequest.CrComplement;
            var housenumbert = addressDtoRequest.HouseNumber;

            var stcardinality = valueCardinality(stCardinality);
            var crcardinality = valueCardinality(crCardinality);
            var sct = valueStType(stType);
            var crt = valueCrType(crType);
            var hst = valueHouseType(houseType);
            var hsc = valueHouseClass(houseClass);

            address = $"{sct} {( string.IsNullOrEmpty(stnumber) ? string.Empty : $"#{stnumber}" )} {stletter} {( ( stBis != null && stBis != false ) ? "Bis" : string.Empty )} {stcomplement} {stcardinality} {crt} {( string.IsNullOrEmpty(crnumber) ? string.Empty : $"#{crnumber}" )} {crletter} {( ( crBis != null && crBis != false ) ? "Bis" : string.Empty )} {crcomplement} {crcardinality} {hst} {hsc} {housenumbert}";
            EnableSaveButton();
            StateHasChanged();
        }

        #endregion RefreshAddress

        #region ResetForm

        public void ResetFormAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            addressDtoResponse = new();
            addressDtoRequest = new AddressDtoRequest();
            address = "";
            stBis = false;
            crBis = false;
            stCardinality = "";
            crCardinality = "";
            stType = "";
            crType = "";
            houseType = "";
            houseClass = "";
            country = 0;
            state = 0;
            city = 0;
            stNumber = new();
            stLetter = new();
            stComplement = new();
            crNumber = new();
            crLetter = new();
            crComplement = new();
            houseNumber = new();
            EnabledDepartamento = false;
            EnabledMunicipio = false;
            createResult = new();
            EnableSaveButton();
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion ResetForm

        #region FormMethods

        private List<(string, AddressDtoRequest)> Create()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            var AddressDtoRequest = new AddressDtoRequest
            {
                CountryId = country,
                StateId = state,
                CityId = city,
                StType = stType,
                StNumber = stNumber!.InputValue,
                StLetter = stLetter!.InputValue,
                StBis = stBis,
                StComplement = stComplement!.InputValue,
                StCardinality = stCardinality,
                CrType = crType,
                CrNumber = crNumber!.InputValue,
                CrLetter = crLetter!.InputValue,
                CrBis = crBis,
                CrComplement = crComplement!.InputValue,
                CrCardinality = crCardinality,
                HouseType = houseType,
                HouseClass = houseClass,
                HouseNumber = houseNumber!.InputValue
            };

            List<(string, AddressDtoRequest)> result = new List<(string, AddressDtoRequest)>
            {
                (address, AddressDtoRequest),
            };
            SpinnerLoaderService.HideSpinnerLoader(Js);
            return result;
        }

        private async Task HandleModalClosed(bool status)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            if (createResult.Count > 0)
            {
                var eventArgs = new MyEventArgs<List<(string, AddressDtoRequest)>>
                {
                    Data = createResult,
                    ModalStatus = status
                };
                await OnStatusChangedMultipleSelection.InvokeAsync(eventArgs);
            }
            else
            {
                ModalStatus = status;
            }

            ResetFormAsync();
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task HandleValidSubmit()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            createResult = Create();
            StateHasChanged();
            var eventArgs = new MyEventArgs<List<(string, AddressDtoRequest)>>
            {
                Data = createResult,
                ModalStatus = false
            };
            await OnStatusChangedMultipleSelection.InvokeAsync(eventArgs);
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion FormMethods

        #region ModalNotification

        public void UpdateModalStatus(bool newValue)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            ModalStatus = newValue;
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion ModalNotification

        #region EnableSaveButton

        public void EnableSaveButton()
        {
            int outNumber = 0;
            addressDtoRequest.StNumber = ( !string.IsNullOrEmpty(addressDtoRequest.StNumber) && int.TryParse(addressDtoRequest.StNumber, out outNumber) ) ? outNumber.ToString() : string.Empty;
            addressDtoRequest.CrNumber = ( !string.IsNullOrEmpty(addressDtoRequest.CrNumber) && int.TryParse(addressDtoRequest.CrNumber, out outNumber) ) ? outNumber.ToString() : string.Empty;

            if (country <= 0 || state <= 0 || city <= 0 || string.IsNullOrEmpty(stType) || string.IsNullOrEmpty(addressDtoRequest.StNumber) || string.IsNullOrEmpty(addressDtoRequest.CrNumber))
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

        #endregion Methods
    }
}