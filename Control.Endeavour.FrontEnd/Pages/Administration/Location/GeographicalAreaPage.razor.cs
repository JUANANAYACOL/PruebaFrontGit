using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.City.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.City.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Country.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.State.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Models.Models.Records.Request;
using Control.Endeavour.FrontEnd.Models.Models.Records.Response;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Xml.Linq;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Pages.Administration.Location
{
    public partial class GeographicalAreaPage
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

        private NewPaginationComponent<LocationDtoResponse, LocationDtoRequest> paginationComponetPost = new();

        private InputModalComponent NameLocation { get; set; } = new();

        #endregion Components

        #region Modals

        private NotificationsComponentModal notificationModal = new();
        private NotificationsComponentModal notificationModalSucces = new();

        #endregion Modals

        #region Parameters

        private int state { get; set; }
        private int country { get; set; }
        private int city { get; set; }
        private string Location { get; set; } = "";
        private bool EnabledState { get; set; } = false;
        private bool EnabledCities { get; set; } = false;

        #endregion Parameters

        #region Models

        private PaginationInfo paginationInfo = new();
        private CityDtoResponse recordToDelete = new();
        private LocationDtoRequest locationFilter = new();
        private MetaModel meta = new();
        private AppKeysDtoResponse DefaultLocation = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string UriFilterCities = "location/ConnectionApi/ByFilterLocation";
        private string paraEliminar = "";

        #endregion Environments(String)

        #region Environments(List & Dictionary)

        private List<CountryDtoResponse>? CountriesList = new();
        private List<StateDtoResponse>? StatesList = new();
        private List<CityDtoResponse>? CitiesList = new();
        private List<LocationDtoResponse>? LocationList = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            await GetCountry();
            //await GetDefaultLocation();
            await GetState(country);
            //await GetLocation();
            ////EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #region HandleRefreshGridDataAsync

        private async Task HandleRefreshGridDataAsync(bool refresh)
        {
            await GetCity(state);
        }

        #endregion HandleRefreshGridDataAsync

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            if (args.IsAccepted && args.ModalOrigin.Equals("deleteCity"))
            {
                DeleteGeneralDtoRequest deleteCityDtoRequest = new DeleteGeneralDtoRequest();
                //deleteCityDtoRequest.Id = recordToDelete.CityId;
                deleteCityDtoRequest.User = "admin";

                var responseApi = await HttpClient.PostAsJsonAsync("location/City/DeleteCity", deleteCityDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<bool>>();
                if (deserializeResponse.Succeeded)
                {
                    //Logica Exitosa
                    notificationModalSucces.UpdateModal(ModalType.Success, Translation["DeleteSuccessfulMessage"], true, Translation["Accept"]);
                    await GetCity(state);
                }
                else
                {
                    //Logica no Exitosa
                    notificationModalSucces.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true, Translation["Accept"]);
                }
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion HandleModalNotiClose

        #region HandlePaginationGrid

        private void HandlePaginationGrid(List<LocationDtoResponse> newDataList)
        {
            LocationList = newDataList;
        }

        #endregion HandlePaginationGrid

        #endregion HandleMethods

        #region OthersMethods

        #region GetDefaultLocation

        public async Task GetDefaultLocation()
        {
            AppKeysFilterDtoRequest appKeysFilter = new();
            appKeysFilter.FunctionName = "FilingGeolocation";
            appKeysFilter.KeyName = "HierarchicalLocationID";
            var responseApi = await HttpClient.PostAsJsonAsync("params/AppKeys/ByFilter", appKeysFilter);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<AppKeysDtoResponse>>>();

            if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
            {
                DefaultLocation = deserializeResponse.Data[0];
                country = int.Parse(DefaultLocation.Value1.Trim());
                EnabledState = true;
            }
        }

        #endregion GetDefaultLocation

        #region GetCountry

        private async Task GetCountry()
        {
            try
            {
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<CountryDtoResponse>>>("location/Country/ByFilter");
                CountriesList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<CountryDtoResponse>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el país: {ex.Message}");
            }
        }

        #endregion GetCountry

        #region GetState

        private async Task GetState(int idcountry)
        {
            try
            {
                country = idcountry;

                if (country > 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("countryId");
                    HttpClient?.DefaultRequestHeaders.Add("countryId", country.ToString());
                    var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<StateDtoResponse>>>("location/State/ByFilter");
                    HttpClient?.DefaultRequestHeaders.Remove("countryId");
                    StatesList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<StateDtoResponse>();

                    if (StatesList.Count > 0)
                    {
                        EnabledState = true;
                        state = 0;
                        city = 0;
                    }
                    else
                    {
                        StatesList = new();
                        EnabledState = false;
                    }
                }
                else
                {
                    state = 0;
                    city = 0;
                    EnabledState = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el departamento: {ex.Message}");
            }
        }

        #endregion GetState

        #region GetCity

        private async Task GetCity(int value)
        {
            try
            {
                state = value;

                if(value > 0)
                {
                    HttpClient?.DefaultRequestHeaders.Remove("stateId");
                    HttpClient?.DefaultRequestHeaders.Add("stateId", state.ToString());
                    var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<CityDtoResponse>>>("location/City/ByFilter");
                    HttpClient?.DefaultRequestHeaders.Remove("stateId");
                    CitiesList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<CityDtoResponse>();

                    if (CitiesList.Count > 0)
                    {
                        EnabledCities = true;
                        city = 0;
                    }
                    else
                    {
                        CitiesList = new();
                        EnabledCities = false;
                    }
                }
                else 
                { 
                    EnabledCities = false;
                    city = 0;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el municipio: {ex.Message}");
            }
        }

        #endregion GetCity

        #region GetLocation

        private async Task GetLocation()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                if (country != 0)
                {
                    locationFilter = new()
                    {
                        CountryId = country,
                        StateId = state,
                        CityId = city,
                        Name = Location,
                    };

                    var responseApi = await HttpClient.PostAsJsonAsync(UriFilterCities, locationFilter);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<LocationDtoResponse>>>();

                    if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                    {
                        LocationList = deserializeResponse.Data != null ? deserializeResponse.Data.Data : new List<LocationDtoResponse>();
                        paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                        paginationComponetPost.ResetPagination(paginationInfo);
                    }
                    else
                    {
                        CitiesList = new();
                        paginationInfo = new();
                        paginationComponetPost.ResetPagination(paginationInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el municipio: {ex.Message}");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion

        #region ResetFilters

        public async void ResetFilters()
        {
            country = 0;
            state = 0;
            city = 0;
            EnabledCities = false;
            EnabledState = false;
            NameLocation.InputValue = string.Empty;
            Location = "";
            LocationList = new();
            //await GetDefaultLocation();
            //await GetState(country);
            //await GetLocation();
            StateHasChanged();
        }

        #endregion ResetFilters

        #region ShowModalDelete

        private void ShowModalDelete(CityDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, @Translation["Yes"], @Translation["No"], modalOrigin: "deleteCity");
        }

        #endregion ShowModalDelete

        #region ExportToExcel

        public bool CancelExport { get; set; }

        private List<TemplateDocumentDtoResponse> allRecords { get; set; }

        bool ExportAllPages { get; set; }

        public async Task GetAllRecords()
        {
            try
            {

                locationFilter = new()
                {
                    CountryId = country,
                    StateId = state,
                    CityId = city,
                    Name = Location,
                };

                var responseApi = await HttpClient.PostAsJsonAsync(UriFilterCities, locationFilter);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<LocationDtoResponse>>>();

                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    LocationList = deserializeResponse.Data != null ? deserializeResponse.Data.Data : new List<LocationDtoResponse>();
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponetPost.ResetPagination(paginationInfo);
                }
                else
                {
                    CitiesList = new();
                    paginationInfo = new();
                    paginationComponetPost.ResetPagination(paginationInfo);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el municipio: {ex.Message}");
            }
        }

        public async Task OnBeforeExcelExport(GridBeforeExcelExportEventArgs args)
        {

            if (ExportAllPages)
            {
                await GetAllRecords();

                if (allRecords.Any())
                {
                    args.Data = allRecords;
                }
            }
            else { args.Data = LocationList; }

            args.IsCancelled = CancelExport;
        }

        #endregion

        #endregion OthersMethods

        #endregion Methods
    }
}