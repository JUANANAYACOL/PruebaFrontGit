using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.OverrideTray;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
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
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using System.Diagnostics;
using System.Xml.Linq;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Pages.Documents.OverrideTrayPage
{
    public partial class OverrideTrayManagerPage
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
        private NewPaginationComponent<OverrideTrayManagerDtoResponse, OverrideTrayManagerDtoRequest> paginationComponetPost = new();

        #endregion Components

        #region Modals

        private OverrideTrayManagerModal ModalManager;

        #endregion Modals

        #region Models

        private OverrideTrayManagerDtoRequest filtro = new();
        private List<OverrideTrayManagerDtoResponse> ManagerList = new();
        private OverrideTrayManagerDtoResponse recordToDelete = new();
        private PaginationInfo paginationInfo = new();
        private CancelationRequestQueryFilter RequestFilter = new();

        #endregion Models

        #region Environments

        private bool crear = false;
        private string UriFilterRecords = "overridetray/CancelationManager/ByFilter";
        private List<OverrideTrayRequestDtoResponse> RequestList = new();

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            await GetManager();
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion OnInitializedAsync

        #region Methods

        #region ShowModalEdit

        private void ShowModalEdit(OverrideTrayManagerDtoResponse args)
        {
            ModalManager.UpdateModalStatus(true);
            ModalManager.UpdateSelectedRecord(args);
        }

        #endregion ShowModalEdit

        #region ShowModal

        private async Task ShowModal()
        {
            if (!crear)
            {
                ModalManager.UpdateModalStatus(true);
            }
        }

        #endregion ShowModal

        #region ShowModalDelete

        private async void ShowModalDelete(OverrideTrayManagerDtoResponse record)
        {
            await GetRequest(record.UserId, record.TypeCode);

            if (RequestList.Any())
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["ManagerPendingRequests"], true, buttonTextCancel: "");
            }
            else
            {
                recordToDelete = record;
                notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, @Translation["Yes"], @Translation["No"], modalOrigin: "DeleteModal");
            }

            RequestList = new();
        }

        #endregion ShowModalDelete

        #region GetRequest

        private async Task GetRequest(int id, string typeCode)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);


                    RequestFilter.UserId = id;
                    RequestFilter.CancelationState = "TEA,PE";
                    RequestFilter.TypeCode = typeCode;
                    RequestFilter.UserManagerId = true;


                var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationRequest/GetCancelationRequestFilter", RequestFilter);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<OverrideTrayRequestDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {
                    RequestList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<OverrideTrayRequestDtoResponse>();
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Translation["LoadErrorMessage"]}: {ex.Message}");
            }
        }

        #endregion GetRequest

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleLanguageChanged

        #region GetManager

        private async Task GetManager()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                var responseApi = await HttpClient.PostAsJsonAsync(UriFilterRecords, filtro);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<OverrideTrayManagerDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {
                    ManagerList = deserializeResponse.Data != null ? deserializeResponse.Data.Data : new List<OverrideTrayManagerDtoResponse>();
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponetPost.ResetPagination(paginationInfo);
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las administradores: {ex.Message}");
            }
        }

        #endregion GetManager

        #region HandleRefreshGridDataAsync

        public async Task HandleRefreshGridDataAsync(bool refresh)
        {
            await GetManager();
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
                    DeleteManager.Id = recordToDelete.CancelationManagerId;
                    DeleteManager.User = "Admin";

                    var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationManager/DeleteCancelationManager", DeleteManager);
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
                Console.WriteLine("Registro No eliminado");
            }
        }

        #endregion HandleModalNotiClose

        #region HandlePaginationGrid

        private void HandlePaginationGrid(List<OverrideTrayManagerDtoResponse> newDataList)
        {
            ManagerList = newDataList;
        }

        #endregion HandlePaginationGrid

        #region ExportToExcel

        public bool CancelExport { get; set; }

        private List<OverrideTrayManagerDtoResponse> allRecords { get; set; }

        bool ExportAllPages { get; set; }

        public async Task GetAllRecords()
        {
            try
            {

                var responseApi = await HttpClient.PostAsJsonAsync(UriFilterRecords, filtro);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<OverrideTrayManagerDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {
                    allRecords = deserializeResponse.Data != null ? deserializeResponse.Data : new List<OverrideTrayManagerDtoResponse>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las administradores: {ex.Message}");
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
            else { args.Data = ManagerList; }

            args.IsCancelled = CancelExport;
        }

        #endregion

        #endregion Methods
    }
}