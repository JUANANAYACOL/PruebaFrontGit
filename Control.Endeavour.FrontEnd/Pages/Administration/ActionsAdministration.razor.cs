using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Records;
using Control.Endeavour.FrontEnd.Components.Modals.SystemConfiguration;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using DevExpress.DirectX.StandardInterop.DirectWrite;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.DataSource;
using Telerik.SvgIcons;


namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class ActionsAdministration
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
        private NotificationsComponentModal notificationModal = new();
        private ActionsModal actionsModal = new();

        #endregion

        #region Parameters


        #endregion

        #region Models

        private ActionsDtoResponse recordToDelete = new();
        private ActionsFilterDtoRequest actionsFilterDtoRequest = new();


        #endregion

        #region Environments

        #region Environments(String)

        #endregion

        #region Environments(Numeric)

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        #endregion

        #region Environments(List & Dictionary)
        private List<ActionsDtoResponse> actionsDataList = new();
        private List<VSystemParamDtoResponse> classCodeList = new();
        private List<VSystemParamDtoResponse> flowStateList = new();
        private List<VSystemParamDtoResponse> originList = new();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            ////EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            try
            {
                await GetActions();
                await GetClasses();
                await GetFlowState();
                await GetOrigin();
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }


        }


        #endregion

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        private async Task HandleRefreshGridData(bool refresh)
        {
            await GetActions();
        }
        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                if (args.IsAccepted && args.ModalOrigin.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new();
                    request.Id = (recordToDelete.ActionId.HasValue) ? recordToDelete.ActionId.Value : 0;
                    request.User = "Front"; // Cambiar por la varibale de session del usuario
                    var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/Actions/DeleteActions", request);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse.Succeeded)
                    {
                        await GetActions();
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            notificationModal.UpdateModal(ModalType.Success, Translation["DeleteSuccessfulMessage"], true);
                        }
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true, Translation["Accept"]);
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);

        }
        #endregion

        #region OthersMethods

        #region ModalMethods

        private void ShowModalCreate()
        {
            actionsModal.UpdateModalStatus(true);
        }
        private void ShowModalEdit(ActionsDtoResponse record)
        {
            actionsModal.UpdateModalStatus(true);
            actionsModal.ReceiveRecord(record);
        }
        private void ShowModalDelete(ActionsDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, @Translation["Yes"], @Translation["No"], modalOrigin: "DeleteModal");
        }

        #endregion

        #region GetData

        private async Task GetActions()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                var responseApi = await HttpClient!.PostAsJsonAsync("documentmanagement/Actions/ByFilter", actionsFilterDtoRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<ActionsDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                {
                    actionsDataList = deserializeResponse.Data;
                }
                else
                {
                    actionsDataList = new List<ActionsDtoResponse>();
                    notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
                }
            }
            catch (Exception ex)
            {
                
                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }
        private async Task GetClasses()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "CL");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    classCodeList = deserializeResponse.Data;
                }
                else { classCodeList = new(); }

            }
            catch (Exception ex)
            {

                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }
        private async Task GetOrigin()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "ORI");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    originList = deserializeResponse.Data;
                }
                else { originList = new(); }

            }
            catch (Exception ex)
            {

                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task GetFlowState()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "ES");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    flowStateList = deserializeResponse.Data;
                }
                else { flowStateList = new(); }

            }
            catch (Exception ex)
            {

                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion

        #region FilterMethods

        private async Task CleanFilter()
        {
            actionsFilterDtoRequest = new();
            await GetActions();

        }
        private async Task SearchByFilter()
        {
            if (!string.IsNullOrEmpty(actionsFilterDtoRequest.Name) || !string.IsNullOrEmpty(actionsFilterDtoRequest.ClassCode) ||
                !string.IsNullOrEmpty(actionsFilterDtoRequest.Origin) || !string.IsNullOrEmpty(actionsFilterDtoRequest.FlowStateCode))
            {

                await GetActions();
                
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Warning, Translation["EnterParameterSearch"], true, buttonTextCancel: "");
            }
        }

        #endregion

        #endregion

        #endregion

    }
}
