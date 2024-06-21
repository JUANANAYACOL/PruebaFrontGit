using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.OverrideTray;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.City.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components;
using Control.Endeavour.FrontEnd.Resources;
using Microsoft.Extensions.Localization;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos.Attachments.Request;
using System.Diagnostics;
using System.Xml.Linq;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Pages.Documents.OverrideTrayPage
{
    public partial class OverrideTrayReasonPage
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

        private NotificationsComponentModal notificationModalSucces = new();
        private NotificationsComponentModal notificationModal = new();
        private NewPaginationComponent<OverrideTrayReasonDtoResponse, OverrideTrayReasonDtoRequest> paginationComponetPost = new();

        #endregion Components

        #region Modals

        private OverrideTrayReasonModal modalReason = new();

        #endregion Modals

        #region Models

        private OverrideTrayReasonDtoResponse recordToDelete = new();
        private PaginationInfo paginationInfo = new();
        private OverrideTrayReasonDtoRequest reason = new();

        #endregion Models

        #region Environments

        #region Environments(bool)

        private bool Habilitar;

        #endregion Environments(bool)

        #region Environments(String)

        private string UriFilterReasons = "overridetray/CancelationReason/ByFilterReason";
        private string ReasonCode = "";
        private string TypeCode = "";
        private string DTReason = "";
        private string DTTypeCode = "";
        private string NameReason = "";

        #endregion Environments(String)

        #region Environments(List & Dictionary)

        private List<OverrideTrayReasonDtoResponse> ReasonList = new();
        private List<VSystemParamDtoResponse> TypeCodeList = new();
        private List<VSystemParamDtoResponse> ReasonCodeList = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            DTReason = Translation["SelectAnOption"];
            DTTypeCode = Translation["SelectAnOption"];
            await GetReason();
            await GetReasonCode();
            await GetTypeCode();
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #region ShowModalEdit

        private async Task ShowModalEdit(OverrideTrayReasonDtoResponse args)
        {
            modalReason.UpdateModalStatus(true);
            modalReason.UpdateSelectedRecord(args);
            modalReason.Temp = false;
        }

        #endregion ShowModalEdit

        #region ShowModal

        private async Task ShowModal()
        {
            modalReason.UpdateModalStatus(true);
            modalReason.reset();
            modalReason.Temp = true;
        }

        #endregion ShowModal

        #region ShowModalDelete

        private void ShowModalDelete(OverrideTrayReasonDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, @Translation["Yes"], @Translation["No"], modalOrigin: "DeleteModal");
        }

        #endregion ShowModalDelete

        #region GetTypeCode

        private async Task GetTypeCode()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "TCA");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse.Succeeded)
                {
                    Habilitar = true;

                    TypeCodeList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<VSystemParamDtoResponse>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las funcionalidades: {ex.Message}");
            }
        }

        #endregion GetTypeCode

        #region GetReasonCode

        private async Task GetReasonCode()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "RCA");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse.Succeeded)
                {
                    ReasonCodeList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<VSystemParamDtoResponse>();
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las funcionalidades: {ex.Message}");
            }
        }

        #endregion GetReasonCode

        #region GetReason

        private async Task GetReason()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                reason = new();
                reason.NameReason = string.IsNullOrEmpty(NameReason) ? "" : NameReason;
                reason.ReasonCode = string.IsNullOrEmpty(ReasonCode) ? "" : ReasonCode;
                reason.TypeCode = string.IsNullOrEmpty(TypeCode) ? "" : TypeCode;

                var responseApi = await HttpClient.PostAsJsonAsync(UriFilterReasons, reason);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<OverrideTrayReasonDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {
                    //Logica Exitosa
                    ReasonList = deserializeResponse.Data != null ? deserializeResponse.Data.Data : new List<OverrideTrayReasonDtoResponse>();
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponetPost.ResetPagination(paginationInfo);
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Translation["LoadErrorMessage"]);
            }
        }

        #endregion GetReason

        #region Reset

        public async Task reset()
        {
            ReasonCode = "";
            TypeCode = "";
            NameReason = "";
            ReasonList = new();
            await GetReason();
        }

        #endregion Reset

        #region HandleRefreshGridDataAsync

        public async Task HandleRefreshGridDataAsync(bool refresh)
        {
            await GetReason();
        }

        #endregion HandleRefreshGridDataAsync

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted && args.ModalOrigin.Equals("DeleteModal"))
            {
                if (recordToDelete != null)
                {
                    DeleteGeneralDtoRequest DeleteManager = new();
                    DeleteManager.Id = recordToDelete.CancelationReasonId;
                    DeleteManager.User = "Admin";

                    var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationReason/DeleteCancelationReason", DeleteManager);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse.Succeeded)
                    {
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            notificationModal.UpdateModal(ModalType.Success, Translation["DeleteSuccessfulMessage"], true);
                        }
                    }
                    else
                    {
                        //Logica no Exitosa
                        notificationModalSucces.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true, Translation["Accept"]);
                    }
                    await HandleRefreshGridDataAsync(true);
                }
            }
            else
            {
                Console.WriteLine(Translation["DeleteErrorMessage"]);
            }
        }

        #endregion HandleModalNotiClose

        #region HandlePaginationGrid

        private void HandlePaginationGrid(List<OverrideTrayReasonDtoResponse> newDataList)
        {
            ReasonList = newDataList;
        }

        #endregion HandlePaginationGrid

        #region ExportToExcel

        public bool CancelExport { get; set; }

        private List<OverrideTrayReasonDtoResponse> allRecords { get; set; }

        bool ExportAllPages { get; set; }

        public async Task GetAllRecords()
        {
            try
            {
                reason = new();
                reason.NameReason = string.IsNullOrEmpty(NameReason) ? "" : NameReason;
                reason.ReasonCode = string.IsNullOrEmpty(ReasonCode) ? "" : ReasonCode;
                reason.TypeCode = string.IsNullOrEmpty(TypeCode) ? "" : TypeCode;

                var responseApi = await HttpClient.PostAsJsonAsync(UriFilterReasons, reason);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<OverrideTrayReasonDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {
                    //Logica Exitosa
                    allRecords = deserializeResponse.Data != null ? deserializeResponse.Data : new List<OverrideTrayReasonDtoResponse>();
                    paginationComponetPost.ResetPagination(paginationInfo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Translation["LoadErrorMessage"]);
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
            else { args.Data = ReasonList; }

            args.IsCancelled = CancelExport;
        }

        #endregion

        #endregion Methods
    }
}