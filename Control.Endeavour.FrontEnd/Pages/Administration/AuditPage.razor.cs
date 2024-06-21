using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Audit.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Audit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.City.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.City.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VReplacement.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Reflection.PortableExecutable;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class AuditPage
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

        #region Models

        private List<VWLogsAuditDtoBugResponse> vWLogsAuditDtoBugList { get; set; } = new();
        private MetaModel meta { get; set; } = new() { PageSize = 10 };
        private LogByFilterDtoRequest auditFilterDtoRequest { get; set; } = new();

        private PaginationInfo paginationInfo = new();

        #endregion Models

        #region Components

        private NewPaginationComponent<VWLogsAuditDtoBugResponse, LogByFilterDtoRequest> paginationComponent = new();
        private InputModalComponent detailInput { get; set; } = new();

        #endregion Components

        #region Enviroment

        #region Enviroment (Bool)

        private bool dataChargue { get; set; } = false;

        #endregion Enviroment (Bool)

        #region Enviroment (DateTime)

        private DateTime? from { get; set; } = null;

        private DateTime? to { get; set; } = null;

        #endregion Enviroment (DateTime)

        #region Enviroment (String)

        private string UriFilterLogs = "audit/Log/ByFilter";
        public string detail { get; set; } = string.Empty;

        #endregion Enviroment (String)

        #endregion Enviroment

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            await GetAudit();
            StateHasChanged();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleLanguageChanged

       /* private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }*/

        #endregion HandleLanguageChanged

        #region GetAudit

        private async Task GetAudit()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                auditFilterDtoRequest.From = from;
                auditFilterDtoRequest.To = to;
                auditFilterDtoRequest.Detail = detail;

                var responseApi = await HttpClient.PostAsJsonAsync(UriFilterLogs, auditFilterDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<VWLogsAuditDtoBugResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                {
                    vWLogsAuditDtoBugList = deserializeResponse.Data.Data ?? new();
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                    dataChargue = true;
                }
                else
                {
                    vWLogsAuditDtoBugList = new();
                }
            }
            catch
            {
                vWLogsAuditDtoBugList = new();
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetAudit

        #region HandlePaginationGrid

        private void HandlePaginationGrid(List<VWLogsAuditDtoBugResponse> newDataList)
        {
            vWLogsAuditDtoBugList = newDataList;
        }

        #endregion HandlePaginationGrid

        #region OnClickSearch

        private async Task OnClickSearch()
        {
            await GetAudit();
        }

        #endregion OnClickSearch

        #region OnClickReset

        private async Task OnClickReset()
        {
            auditFilterDtoRequest = new();

            detailInput = new();
            detail = string.Empty;
            to = null;
            from = null;
            await GetAudit();
            StateHasChanged();
        }

        #endregion OnClickReset

        #endregion Methods
    }
}