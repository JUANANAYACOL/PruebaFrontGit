using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.SystemConfiguration;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.NoWorkDays;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class NoWorkDaysAdministration
    {

        #region Variables

        #region Inject 


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

        private NotificationsComponentModal NotificationModal = new();
        private NoWorkDaysModals NoWorkDaysModal = new();
        #endregion



        #region Models

        private NoWorkDaysFilterDtoRequest NoWorkDaysFilter = new();
        private PaginationInfo paginationInfo = new();
        private NewPaginationComponent<NoWorkDaysDtoResponse, NoWorkDaysFilterDtoRequest> paginationComponent = new();
        private NoWorkDaysDtoResponse recordToDelete = new();

        #endregion

        #region Environments

        #region Environments(String)

        private string UriNoworkDay = "params/NoWorkDay/ByFilter";

        #endregion

        #region Environments(Numeric)

        #endregion

        #region Environments(DateTime)
        private DateTime Min = new DateTime(2019, 12, 31);
        private DateTime Max = new DateTime(2080, 12, 31);

        #endregion

        #region Environments(Bool)

        #endregion

        #region Environments(List & Dictionary)
        private List<VSystemParamDtoResponse> ReasonNoWorkDays = new();
        private List<NoWorkDaysDtoResponse> NoWorkDaysList = new();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            await GetNoWorkDays();
            await GetNoWorkDaysReasons();
        }


        #endregion

        #region Methods

        #region HandleMethods

        private void HandlePaginationGrid(List<NoWorkDaysDtoResponse> newDataList)
        {
            NoWorkDaysList = newDataList;

        }
        private async Task HandleRefreshData(bool response)
        {
            await GetNoWorkDays();
        }
        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                if (args.IsAccepted && args.ModalOrigin.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new();
                    request.Id = recordToDelete.NoWorkDayId;
                    var responseApi = await HttpClient.PostAsJsonAsync("params/NoWorkDay/DeleteNoWorkDay", request);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse.Succeeded)
                    {
                        await GetNoWorkDays();
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            NotificationModal.UpdateModal(ModalType.Success, Translation["DeleteSuccessfulMessage"], true);
                        }
                    }
                    else
                    {
                        NotificationModal.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true, Translation["Accept"]);
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);

        }

        #endregion

        #region OthersMethods

        #region GetData
        private async Task GetNoWorkDays()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                var responseApi = await HttpClient!.PostAsJsonAsync(UriNoworkDay, NoWorkDaysFilter);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<NoWorkDaysDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data.Data.Count != 0)
                {


                    NoWorkDaysList = deserializeResponse.Data.Data;

                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else
                {
                    NoWorkDaysList = new();
                    NotificationModal.UpdateModal(ModalType.Error, Translation["Grid_NoRecords"], true);
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            catch (Exception ex)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                NotificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);

            }

        }
        private async Task GetNoWorkDaysReasons()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "RNWD");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    ReasonNoWorkDays = deserializeResponse.Data;
                }
                else { ReasonNoWorkDays = new(); }

            }
            catch (Exception ex)
            {

                NotificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }
        #endregion

        #region ActionsMethods

        private void DatesFilterMethod(DateTime? value, string type)
        {
            
            switch (type)
            {
                case "StartDate":
                    
                    NoWorkDaysFilter.StarDate = value != null ? value.Value.Date : (DateTime?)null;

                    break;
                case "EndDate":
                    NoWorkDaysFilter.EndDate = value != null ? value.Value.Date : (DateTime?)null;
                    break;
            }
        }
        #endregion

        #region FilterMethods
        
        private async Task CleanFilter()
        {
            NoWorkDaysFilter = new();
                        
            await GetNoWorkDays();
            StateHasChanged();
        }
        private async Task SearchByFilter()
        {
            if (!IsAnyFilterParameterSet())
            {
                NotificationModal.UpdateModal(ModalType.Warning, Translation["EnterParameterSearch"], true, buttonTextCancel: "");
                return;
            }

            if (!ValidateNoWorkDaysFilter())
            {
                return;
            }

            await GetNoWorkDays();
        }

        private bool IsAnyFilterParameterSet()
        {
            return !string.IsNullOrEmpty(NoWorkDaysFilter.Reason) ||
                   NoWorkDaysFilter.StarDate != default ||
                   NoWorkDaysFilter.EndDate.HasValue;
        }


        private bool ValidateNoWorkDaysFilter()
        {

            if (NoWorkDaysFilter.EndDate.HasValue && NoWorkDaysFilter.StarDate == default)
            {
                NotificationModal.UpdateModal(ModalType.Warning, string.Format(Translation["CharacterSelectionValidation"], Translation["DateTimePicker_Date"]), true, buttonTextCancel: "");
                return false;
            }

            if (NoWorkDaysFilter.EndDate.HasValue && NoWorkDaysFilter.StarDate > NoWorkDaysFilter.EndDate.Value)
            {
                NotificationModal.UpdateModal(ModalType.Warning, Translation["InvalidDateRange"], true, buttonTextCancel: "");
                return false;
            }

            return true;
        }

        #endregion

        #region ModalMethods

        private void ShowModalCreate()
        {
            NoWorkDaysModal.UpdateModalStatus(true);
        }
        private void ShowModalEdit(NoWorkDaysDtoResponse record)
        {
            NoWorkDaysModal.ReceiveRecord(record);
            NoWorkDaysModal.UpdateModalStatus(true);
            
        }
        private void ShowModalDelete(NoWorkDaysDtoResponse record)
        {
            recordToDelete = record;
            NotificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, @Translation["Yes"], @Translation["No"], modalOrigin: "DeleteModal");
        }

        #endregion

        #endregion

        #endregion

    }
}
