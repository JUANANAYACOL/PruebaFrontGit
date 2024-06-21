using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.SystemConfiguration;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Request;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class ParametersAdministrationPage
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

        #endregion

        #region Components


        #endregion

        #region Modals

        private SystemFieldsModal modalsystemFields = new();
        private NotificationsComponentModal notificationModal = new();


        #endregion

        #region Parameters


        #endregion

        #region Models

        private SystemFieldsDtoResponse recordToDelete = new();
        private SystemParamsFilterDtoRequest systemParamsFilter = new();
        private SystemFieldsFilterDtoRequest SystemFieldsFilter = new();

        #endregion

        #region Environments

        #region Environments(String)

        private string Panel1Class = "col-md-12";
        private string Panel2Class = "d-none";
        private string paramCode = string.Empty;
        private string systemParamName = string.Empty;

        #endregion

        #region Environments(Numeric)

        private int paramId;
        private int pageSizeSystemParams;
        private int pageSizeSystemFields;


        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        #endregion

        #region Environments(List & Dictionary)

        private List<SystemParamsDtoResponse> systemParamsList = new List<SystemParamsDtoResponse>();
        private List<SystemFieldsDtoResponse> systemFieldsList = new List<SystemFieldsDtoResponse>();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {

            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
                await GetSystemParams();
                SpinnerLoaderService.HideSpinnerLoader(Js);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al realizar la initialización: {ex.Message}");
            }
            //PageLoadService.OcultarSpinnerReadLoad(Js);

        }


        #endregion

        #region Methods

        #region HandleMethods
        private async Task HandleRefreshGridDatasystemFieldsAsync(bool refresh)
        {
            await GetSystemFields(paramCode);
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            try
            {
                if (args.IsAccepted && args.ModalOrigin.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new();
                    request.Id = recordToDelete.SystemFieldId;
                    request.User = "Front"; // Cambiar por la varibale de session del usuario
                    var responseApi = await HttpClient.PostAsJsonAsync("params/SystemFields/DeleteSystemField", request);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse.Succeeded)
                    {
                        await GetSystemFields(paramCode);
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            notificationModal.UpdateModal(ModalType.Success, Translation["DeleteSuccessfulMessage"], true);
                        }
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true);
                    }

                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }

        }
        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        #endregion

        #region OthersMethods

        #region Get Data Methods

        private async Task GetSystemParams()
        {
            try
            {

                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<SystemParamsDtoResponse>>>("params/SystemParams/ByFilter");
                if (deserializeResponse.Succeeded && deserializeResponse.Data.Any())
                {
                    systemParamsList = deserializeResponse.Data;
                    pageSizeSystemParams = systemParamsList.Count();
                }
                else
                {
                    systemParamsList = new();
                    notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
        }

        private async Task GetSystemFields(string systemParamCode)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                SystemFieldsFilterDtoRequest systemFieldsFilter = new();
                systemFieldsFilter.ParamCode = systemParamCode;
                var responseApi = await HttpClient.PostAsJsonAsync("params/SystemFields/ByFilter", systemFieldsFilter);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<SystemFieldsDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data.Any())
                {
                    systemFieldsList = deserializeResponse.Data;
                    pageSizeSystemFields = systemFieldsList.Count();
                }
                else
                {
                    systemFieldsList = new();
                    notificationModal.UpdateModal(ModalType.Error, Translation["ParameterHasNoValuesMessage"], true);
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion

        #region Action Methods

        private async Task ShowSystemFields(SystemParamsDtoResponse record)
        {
            paramCode = record.ParamCode;
            await GetSystemFields(paramCode);
            systemParamName = record.ParamName;
            modalsystemFields.UpdateParamId(record.SystemParamId);
            Panel1Class = "col-md-6";
            Panel2Class = "";

        }
        private async Task ShowModal()
        {
            modalsystemFields.UpdateModalStatus(true);

        }
        private async Task ShowModalSystemFields(SystemFieldsDtoResponse record)
        {
            modalsystemFields.ReceiveRecord(record);
            modalsystemFields.UpdateModalStatus(true);

        }
        private void ShowModalDeleteSystemFields(SystemFieldsDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, @Translation["Yes"], @Translation["No"], modalOrigin: "DeleteModal");
        }

        #endregion

        #region Modal Methods

        #endregion

        #region FilterMethods

        private async Task CleanFilter()
        {
            systemParamsFilter = new();
            await GetSystemParams();
            
        }
        private async Task SearchByFilter()
        {
            if(!string.IsNullOrEmpty(systemParamsFilter.ParamCode) || !string.IsNullOrEmpty(systemParamsFilter.ParamName) ) {

                var responseApi = await HttpClient.PostAsJsonAsync("params/SystemParams/ByFilterSearch", systemParamsFilter);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<SystemParamsDtoResponse>>>();

                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    systemParamsList = deserializeResponse.Data;
                    pageSizeSystemParams = systemParamsList.Count();

                }
                else
                {
                    systemParamsList = new();
                    notificationModal.UpdateModal(ModalType.Information, Translation["ParameterHasNoValuesMessage"], true, buttonTextCancel: "");
                }

            }
            else
            {
                notificationModal.UpdateModal(ModalType.Warning, Translation["EnterParameterSearch"], true,buttonTextCancel:"");
            }
        }
        private async Task CleanFieldsFilter()
        {
            SystemFieldsFilter = new();
            await GetSystemFields(paramCode);

        }
        private async Task SearchFieldsByFilter()
        {
            if (!string.IsNullOrEmpty(SystemFieldsFilter.Code) || !string.IsNullOrEmpty(SystemFieldsFilter.Value))
            {
                SystemFieldsFilter.ParamCode = paramCode;
                var responseApi = await HttpClient.PostAsJsonAsync("params/SystemFields/ByFilterSearch", SystemFieldsFilter);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<SystemFieldsDtoResponse>>>();

                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    systemFieldsList = deserializeResponse.Data;
                    pageSizeSystemFields = systemFieldsList.Count();
                }
                else
                {
                    systemFieldsList = new();
                    notificationModal.UpdateModal(ModalType.Information, Translation["ParameterHasNoValuesMessage"], true,buttonTextCancel:"");
                }

            }
            else
            {
                notificationModal.UpdateModal(ModalType.Warning, Translation["EnterParameterSearch"], true, buttonTextCancel:"");
            }
        }

        #endregion

        #endregion

        #endregion


    }
}
