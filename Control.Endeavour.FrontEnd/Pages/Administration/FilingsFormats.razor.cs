using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.SystemConfiguration;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
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
    public partial class FilingsFormats
    {

        #region Variables

        #region Inject 

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }
        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }
        #endregion

        #region Components
        private NewPaginationComponent<DocCodeFormatDtoResponse, DocCodeFormatQueryFilterRequest> paginationComponetPost { get; set; } = new();


        #endregion

        #region Modals
        private NotificationsComponentModal notificationModal = new();
        private FilingsFormatsModal DocFilingsFormatsModal = new();

        #endregion

        #region Parameters


        #endregion

        #region Models

        private DocCodeFormatQueryFilterRequest DocCodeFormatQueryFilterRequest  = new ();
        private PaginationInfo paginationInfo = new();


        #endregion

        #region Environments

        #region Environments(String)

        private string UriFilingsFormats = "documentmanagement/DocCodeFormat/ByFilter";

        #endregion

        #region Environments(Numeric)

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        #endregion

        #region Environments(List & Dictionary)
        private List<DocCodeFormatDtoResponse> FillingsFormatsList = new List<DocCodeFormatDtoResponse>();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            await GetFillingsFormats();

        }


        #endregion

        #region Methods

        #region HandleMethods

        private void HandlePaginationGrid(List<DocCodeFormatDtoResponse> newDataList)
        {
            FillingsFormatsList = newDataList;
        }
        private async Task HandleRefreshGridData(bool refresh)
        {
            await GetFillingsFormats();
        }

        #endregion

        #region OthersMethods

        #region GetData
        private async Task GetFillingsFormats()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {

                var responseApi = await HttpClient.PostAsJsonAsync(UriFilingsFormats, DocCodeFormatQueryFilterRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<DocCodeFormatDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null && deserializeResponse.Data.Data.Any())
                {
                    FillingsFormatsList = deserializeResponse.Data.Data;
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponetPost.ResetPagination(paginationInfo);
                }
                else
                {
                    FillingsFormatsList = new();
                    paginationInfo = new();
                    paginationComponetPost.ResetPagination(paginationInfo);

                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }


        #endregion

        #region ModalMethods

        private void ShowModalCreate()
        {
            DocFilingsFormatsModal.UpdateModalStatus(true);
        }
        private void ShowModalEditFillings(DocCodeFormatDtoResponse record)
        {
            DocFilingsFormatsModal.ReceiveRecord(record);
            DocFilingsFormatsModal.UpdateModalStatus(true);
        }

        #endregion}

        #endregion

        #endregion

    }
}
