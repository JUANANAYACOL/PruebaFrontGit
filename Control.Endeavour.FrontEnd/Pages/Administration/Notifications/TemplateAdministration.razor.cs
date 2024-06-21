using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.Notifications;
using Control.Endeavour.FrontEnd.Components.Modals.SystemConfiguration;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Notifications.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Notifications.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Models.Models.Records.Request;
using Control.Endeavour.FrontEnd.Models.Models.Records.Response;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Pages.Administration.Notifications
{
	public partial class TemplateAdministration
	{

        #region Variables

        #region Inject 

        [Inject]
        private IJSRuntime Js { get; set; }
        [Inject]
		private HttpClient? HttpClient { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion

        #region Components
        NewPaginationComponent<TemplateDtoResponse, TemplateQueryFilterDtoRequest> paginationComponent = new();

        #endregion

        #region Modals

        private NotificationsComponentModal notificationModal = new();
        private TemplateNotificationsModal templateNotifications = new();
        #endregion

        #region Parameters


        #endregion

        #region Models

        private PaginationInfo paginationInfo = new();
        private TemplateQueryFilterDtoRequest TemplateQueryFilterDtoRequest = new();

        #endregion

        #region Environments

        #region Environments(String)
        private string UriTemplatenotifications = "notification/Templates/ByFilter";

        #endregion

        #region Environments(Numeric)

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        #endregion

        #region Environments(List & Dictionary)


        private List<TemplateDtoResponse> templateDtoResponsesList = new();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
            await GetRecords();


        }


		#endregion

		#region Methods

		#region HandleMethods

		private async Task HandleLanguageChanged()
		{
			StateHasChanged();
		}
        #endregion

        #region OthersMethods

        #region ModalMethods
        private void ShowModalCreate()
        {
            templateNotifications.UpdateModalStatus(true);
        }
        private async Task HandleRefreshGridData(bool refresh)
        {
            await GetRecords();
        }

        #endregion

        #region GetData
        private async Task GetRecords()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                TemplateQueryFilterDtoRequest.OrderAsc = false;
                var responseApi = await HttpClient.PostAsJsonAsync(UriTemplatenotifications, TemplateQueryFilterDtoRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<TemplateDtoResponse>>>();

                if (deserializeResponse.Succeeded && deserializeResponse.Data.Data != null)
                {
                    templateDtoResponsesList = deserializeResponse.Data.Data;
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                    SpinnerLoaderService.HideSpinnerLoader(Js);

                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }
        #endregion

        #region FilterMethods

        private async Task CleanFilter()
        {
            
        }
        private async Task SearchByFilter()
        {
            
        }

        #endregion

        #endregion

        #endregion

    }
}
