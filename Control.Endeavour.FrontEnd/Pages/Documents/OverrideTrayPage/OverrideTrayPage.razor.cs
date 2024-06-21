using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.OverrideTray;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Xml.Linq;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;


namespace Control.Endeavour.FrontEnd.Pages.Documents.OverrideTrayPage
{
    public partial class OverrideTrayPage
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
        private NewPaginationComponent<OverrideTrayRequestDtoResponse, CancelationRequestQueryFilter> paginationComponetPost = new();

        #endregion Components

        #region Modals

        private OverrideTrayModal _ModalOverrideTray = new();
        private OverrideTrayAdminModal _ModalOverrideTrayAdmin = new();
        private GenericSearchModal genericSearchModal = new();

        #endregion Modals

        #region Models

        private VUserDtoResponse vUserSelected = new();
        private PaginationInfo paginationInfo = new();
        private CancelationRequestQueryFilter Request = new();
        private DonnaDtoResponse Dona = new();
        private OverrideTrayRequestDtoResponse recordToDelete;
        private List<OverrideTrayRequestDtoResponse> RequestList = new();
        private List<VSystemParamDtoResponse> TypeRequestCodeList = new();
        private List<OverrideTrayReasonDtoResponse> overrideTrayReasons = new();
        private List<DataCardDtoRequest> Data = new();
        private List<OverrideTrayRequestDtoResponse> CanselationRequest { get; set; } = new();
        private List<OverrideTrayRequestDtoResponse>? CancelationRequestToReturn { get; set; } = new();

        #endregion Models

        #region Environments

        private string TypeRequestLabel = "";
        private string Target = "TEA,PE";
        private string UriFilterRecords = "overridetray/CancelationRequest/ByFilterCancelation";
        private string TypeRequestCode = "";
        private string CardPendiente;
        private string CardAnulados;
        private string CardDesanulados;
        private string CardRechazados;
        private string CardP;
        private string CardA;
        private string CardD;
        private string CardR;
        private string CardPP;
        private string CardPA;
        private string CardPD;
        private string CardPR;
        private string typeCode = "";
        public int controlId { get; set; }

        private int Reason = 0;
        private bool isAdmin = false;
        private bool isButton = false;
        private bool isReason = false;
        private bool isSelected = false;
        private bool isAction = true;
        private bool isTypeRequestCode = true;

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            TypeRequestLabel = @Translation["MassCancellation"];

            await ValidUser();
            Request.CancelationState = Target;
            await GetRequest();
            await GetDonna();

            if (isAdmin)
            {
                await GetTypeRequestCode();
                await GetReason();
            }

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

        #region ShowModal

        private void ShowModalOverrideTray()
        {
            _ModalOverrideTray.UpdateModalStatus(true);
        }

        private async void ShowModalOverrideTrayAdmin(OverrideTrayRequestDtoResponse args)
        {
            await _ModalOverrideTray.PrepareModal(args, isAdmin);
            _ModalOverrideTray.UpdateModalStatus(true);
        }

        private void ShowModalAdmin()
        {
            if (CancelationRequestToReturn.Any())
            {
                _ModalOverrideTrayAdmin.Getlist(CanselationRequest);
                _ModalOverrideTrayAdmin.UpdateModalStatusAdmin(true);
            }
            else 
            { 
                notificationModal.UpdateModal(ModalType.Information, "Es necesario seleccionar registros para continuar con el proceso", true, buttonTextCancel: ""); 
            }
        }

        private void ShowUsersModal(bool value)
        {
            genericSearchModal.UpdateModalStatus(true);
        }

        #endregion ShowModal

        #region ShowModalDelete

        private void ShowModalDelete(OverrideTrayRequestDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, @Translation["Yes"], @Translation["No"], modalOrigin: "DeleteModal");
        }

        #endregion ShowModalDelete

        #region HandleRefreshGridDataAsync

        public async Task HandleRefreshGridData(bool refresh)
        {
            await GetDonna();
            await GetRequest();
        }

        #endregion HandleRefreshGridDataAsync

        #region HandleUserChanged

        private async Task HandleUserChangedAsync(MyEventArgs<VUserDtoResponse> user)
        {
            vUserSelected = user.Data;

            if(vUserSelected != null)
            {
                isSelected = true;
                await GetRequest();
                genericSearchModal.UpdateModalStatus(user.ModalStatus);
            }

        }

        #endregion HandleUserChanged

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted && args.ModalOrigin.Equals("DeleteModal"))
            {
                if (recordToDelete != null)
                {
                    DeleteGeneralDtoRequest DeleteRequest = new();
                    DeleteRequest.Id = recordToDelete.CancelationRequestId.Value;
                    DeleteRequest.User = "Admin";

                    var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationRequest/DeleteCancelationRequest", DeleteRequest);
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
                        notificationModalSucces.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true, Translation["Accept"]);
                    }
                    await HandleRefreshGridData(true);
                }
            }
            else
            {
                Console.WriteLine($"{Translation["DeleteErrorMessage"]}");
            }
        }

        #endregion HandleModalNotiClose

        #region ValidUser

        private async Task ValidUser()
        {
            try
            {
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<ValidateManagerDtoResponse>>("overridetray/CancelationRequest/ValidateUserManage");

                if (deserializeResponse.Succeeded)
                {
                    isAdmin = deserializeResponse.Data.IsAdmin;
                    typeCode = deserializeResponse.Data.TypeCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las funcionalidades: {ex.Message}");
            }
        }

        #endregion ValidUser

        #region GetTypeRequestCode

        private async Task GetTypeRequestCode()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "RCA");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse.Succeeded)
                {
                    TypeRequestCodeList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<VSystemParamDtoResponse>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las funcionalidades: {ex.Message}");
            }
        }

        private async Task ConfirmTypeRequestCode(string value)
        {
            try
            {
                TypeRequestCode = value;

                if (TypeRequestCode != null)
                {
                    isReason = true;
                    isSelected = true;

                    if (TypeRequestCode.Equals("RCA,ANU"))
                    {
                        TypeRequestLabel = @Translation["MassCancellation"];
                        isButton = true;
                    }
                    else if (TypeRequestCode.Equals("RCA,DESA"))
                    {
                        TypeRequestLabel = @Translation["MassUncancellation"];
                        isButton = true;
                    }
                    else { isButton = false; }

                    await GetRequest();
                }
                else 
                { 
                    isButton = false;
                    isReason = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Translation["LoadErrorMessage"]}: {ex.Message}");
            }
        }

        #endregion GetTypeRequestCode

        #region GetReason

        private async Task GetReason()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                OverrideTrayReasonDtoRequest reason = new();
                var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationReason/ByFilter", reason);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<OverrideTrayReasonDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {
                    overrideTrayReasons = deserializeResponse.Data != null ? deserializeResponse.Data : new List<OverrideTrayReasonDtoResponse>();
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las razones: {ex.Message}");
            }
        }

        private async Task ReasonSelected(int value)
        {
            try
            {
                Reason = value;

                if (Reason > 0)
                {
                    isSelected = true;
                    await GetRequest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Translation["LoadErrorMessage"]}: {ex.Message}");
            }
        }

        #endregion GetReason

        #region GetRequest

        private async Task GetRequest()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                if (isAdmin)
                {
                    Request.UserManagerId = true;
                }
                else { Request.UserRequestId = true; }

                if (!string.IsNullOrEmpty(TypeRequestCode))
                {
                    Request.TypeRequestCode = TypeRequestCode;

                    if(Reason > 0)
                    {
                        Request.CancelationReasonId = Reason;
                    }
                }

                if (vUserSelected.UserId > 0)
                {
                    Request.UserId = vUserSelected.UserId;
                    Request.UserManagerId = false;
                }

                var responseApi = await HttpClient.PostAsJsonAsync(UriFilterRecords, Request);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<OverrideTrayRequestDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {
                    RequestList = deserializeResponse.Data != null ? deserializeResponse.Data.Data : new List<OverrideTrayRequestDtoResponse>();
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponetPost.ResetPagination(paginationInfo);
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Translation["LoadErrorMessage"]}: {ex.Message}");
            }
        }

        #endregion GetRequest

        #region ResetFilters

        public async void ResetFilters()
        {
            TypeRequestCode = "";
            Reason = 0;
            vUserSelected = new();
            vUserSelected.UserId = 0;
            await GetRequest();
            StateHasChanged();
        }

        #endregion ResetFilters

        #region GetDonna

        private async Task GetDonna()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                OverrideTrayRequestUserDtoRequest user = new();

                if (isAdmin)
                {
                    user.UserManagerId = true;
                }
                else { user.UserRequestId = true; }

                var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationRequest/ByfilterStatusUserId", user);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<DonnaDtoResponse>>();
                if (deserializeResponse.Succeeded)
                {
                    Dona = deserializeResponse.Data;

                    #region percentage

                    int total = Dona.Anulado + Dona.Desanulado + Dona.Pendiente + Dona.Rechazado;
                    double porcent = Dona.Anulado != 0 ? (Convert.ToDouble(Dona.Anulado) * 100) / total : 0;
                    double porcent2 = Dona.Desanulado != 0 ? (Convert.ToDouble(Dona.Desanulado) * 100) / total : 0;
                    double porcent3 = Dona.Pendiente != 0 ? (Convert.ToDouble(Dona.Pendiente) * 100) / total : 0;
                    double porcent4 = Dona.Rechazado != 0 ? (Convert.ToDouble(Dona.Rechazado) * 100) / total : 0;

                    CardPA = porcent != 0 ? porcent.ToString("N2") + "%" : "0";
                    CardPD = porcent2 != 0 ? porcent2.ToString("N2") + "%" : "0";
                    CardPP = porcent3 != 0 ? porcent3.ToString("N2") + "%" : "0";
                    CardPR = porcent4 != 0 ? porcent4.ToString("N2") + "%" : "0";

                    #endregion percentage

                    CardAnulados = Dona.NombreAnulado;
                    CardA = Dona.Anulado.ToString();
                    CardDesanulados = Dona.NombreDesanulado;
                    CardD = Dona.Desanulado.ToString();
                    CardRechazados = Dona.NombreRechazado;
                    CardR = Dona.Rechazado.ToString();
                    CardPendiente = Dona.NombrePendiente;
                    CardP = Dona.Pendiente.ToString();
                    Data = new();
                    DataCardDtoRequest Anulado = new()
                    {
                        Category = Dona.NombreAnulado,
                        Value = Dona.Anulado,
                        color = "#82A738"
                    };
                    DataCardDtoRequest Desanulado = new()
                    {
                        Category = Dona.NombreDesanulado,
                        Value = Dona.Desanulado,
                        color = "#41BAEA"
                    };
                    DataCardDtoRequest Rechazado = new()
                    {
                        Category = Dona.NombreRechazado,
                        Value = Dona.Rechazado,
                        color = "#EAD519"
                    };
                    DataCardDtoRequest Pendiente = new()
                    {
                        Category = Dona.NombrePendiente,
                        Value = Dona.Pendiente,
                        color = "#AB2222"
                    };
                    Data.Add(Anulado);
                    Data.Add(Desanulado);
                    Data.Add(Rechazado);
                    Data.Add(Pendiente);
                }
                else
                {
                    Console.WriteLine($"{Translation["LoadErrorMessage"]}");
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Translation["LoadErrorMessage"]}: {ex.Message}");
            }
        }

        #endregion GetDonna

        #region reset

        public async void reset()
        {
            Request.TypeRequestCode = "";
            Request.CancelationReasonId = 0;
            Request.UserId = 0;
            TypeRequestCode = "";
            Reason = 0;
            vUserSelected = new();
            vUserSelected.UserId = 0;
            isButton = false;
            isReason = false;
            isSelected = false;
            isAction = true;
            isTypeRequestCode = true;
            await HandleClickCardPendiente();
            StateHasChanged();
        }

        #endregion reset

        #region MetodosCard

        private async Task HandleClickCardPendiente()
        {
            Reason = 0;
            vUserSelected = new();
            TypeRequestCode = "";
            Target = "TEA,PE";
            isSelected = false;
            isButton = false;
            isReason = false;
            isAction = true;
            isTypeRequestCode = true;
            Request.CancelationState = Target;
            await GetRequest();
        }

        private async Task HandleClickCardAnulado()
        {
            Reason = 0;
            vUserSelected = new();
            TypeRequestCode = "";
            Target = "TEA,AN";
            isSelected = false;
            isButton = false;
            isReason = false;
            isAction = false;
            isTypeRequestCode = false;
            Request.CancelationState = Target;
            await GetRequest();
        }

        private async Task HandleClickCardDesanulado()
        {
            Reason = 0;
            vUserSelected = new();
            TypeRequestCode = "";
            Target = "TEA,DESAN";
            isSelected = false;
            isButton = false;
            isReason = false;
            isAction = false;
            isTypeRequestCode = false;
            Request.CancelationState = Target;
            await GetRequest();
        }

        private async Task HandleClickCardRechazado()
        {
            Reason = 0;
            vUserSelected = new();
            TypeRequestCode = "";
            Target = "TEA,RE";
            isSelected = false;
            isButton = false;
            isReason = false;
            isAction = false;
            isTypeRequestCode = false;
            Request.CancelationState = Target;
            await GetRequest();
        }

        #endregion MetodosCard

        #region AddRequest

        public void AddRequest(OverrideTrayRequestDtoResponse model)
        {
            if (!model.selectRecord && CanselationRequest.Contains(model))
            {
                CanselationRequest.Remove(model);
                CancelationRequestToReturn.RemoveAll(x => x == model);
            }
            else if (model.selectRecord && !CanselationRequest.Contains(model))
            {
                CanselationRequest.Add(model);
                CancelationRequestToReturn.Add(model);
            }
        }

        #endregion AddRequest

        #region HandlePaginationGrid

        private void HandlePaginationGrid(List<OverrideTrayRequestDtoResponse> newDataList)
        {
            RequestList = newDataList;
        }

        #endregion HandlePaginationGrid

        #region ExportToExcel

        public bool CancelExport { get; set; }

        private List<TemplateDocumentDtoResponse> allRecords { get; set; }

        bool ExportAllPages { get; set; }

        public async Task GetAllRecords()
        {
            try
            {
                if (isAdmin)
                {
                    Request.UserManagerId = true;
                }
                else { Request.UserRequestId = true; }

                if (!string.IsNullOrEmpty(TypeRequestCode))
                {
                    Request.TypeRequestCode = TypeRequestCode;

                    if (Reason > 0)
                    {
                        Request.CancelationReasonId = Reason;
                    }
                }

                if (vUserSelected.UserId > 0)
                {
                    Request.UserId = vUserSelected.UserId;
                    Request.UserManagerId = false;
                }

                var responseApi = await HttpClient.PostAsJsonAsync(UriFilterRecords, Request);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<OverrideTrayRequestDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {
                    RequestList = deserializeResponse.Data != null ? deserializeResponse.Data.Data : new List<OverrideTrayRequestDtoResponse>();
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponetPost.ResetPagination(paginationInfo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Translation["LoadErrorMessage"]}: {ex.Message}");
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
            else { args.Data = RequestList; }

            args.IsCancelled = CancelExport;
        }

        #endregion

        #endregion Methods
    }
}