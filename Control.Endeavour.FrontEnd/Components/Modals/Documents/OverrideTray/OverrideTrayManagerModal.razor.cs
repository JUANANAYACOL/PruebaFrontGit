using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.User.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.User.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals.Address.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using DevExpress.Pdf.Native.BouncyCastle.Asn1.Ocsp;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Drawing;
using System.Net.Http.Json;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.OverrideTray
{
    public partial class OverrideTrayManagerModal : ComponentBase
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

        private NotificationsComponentModal notificationModal;
        private InputModalComponent InputNombre;

        #endregion Components

        #region Modals

        private GenericSearchModal genericSearchModal = new();
        private GenericUserSearchModal genericUserSearch = new();

        #endregion Modals

        #region Models

        private OverrideTrayManagerDtoResponse _selectedRecord = new();
        private RetunUserSearch vUserSelected = new();
        private RetunUserSearch vUserTemporal = new();
        private CancelationRequestQueryFilter RequestFilter = new();

        #endregion Models

        #region Parameters

        [Parameter] public bool modalStatus { get; set; } = false;
        [Parameter] public EventCallback<bool> OnChangeData { get; set; }

        #endregion Parameters

        #region Environments

        #region Environments(String)

        private string TypeCode = "";
        private string DFCode = "";
        private string IsRequired = "visible";

        #endregion Environments(String)

        #region Environments(Bool)

        private bool Habilitar = false;
        private bool activeState = true;
        private bool isEnableReceiverButton = false;
        private bool changeUser = false;
        private bool putOrPost = false;
        private bool enableButton = true;
        private bool UserModalStatus = false;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<VSystemParamDtoResponse> TypeCodeList = new();
        private List<OverrideTrayRequestDtoResponse> RequestList = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            DFCode = Translation["SelectAnOption"];
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

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            changeUser = false;
            StateHasChanged();
        }

        #endregion UpdateModalStatus

        private void OpenNewModalUser()
        {
            UserModalStatus = true;
            //genericSearchModal.UpdateModalStatus(true);
        }

        public void GetReceiverUserData(RetunUserSearch user)
        {
            vUserSelected = user;
        }

        private async void HandleUserChanged(List<RetunUserSearch> user)
        {
            if(putOrPost)
            {
                changeUser = true;
                await GetRequest(vUserSelected.UserId);

                if (RequestList.Any())
                {
                    notificationModal.UpdateModal(ModalType.Information, Translation["TransferredRequests"], true, modalOrigin: "changeUser");

                    if (notificationModal.ModalOrigin.Equals("changeUser"))
                    {
                        vUserTemporal = user[0];
                        //genericSearchModal.UpdateModalStatus(user.ModalStatus);
                    }
                }
                else
                {
                    GetReceiverUserData(user[0]);
                    //genericSearchModal.UpdateModalStatus(user.ModalStatus);
                }
            }
            else
            {
                GetReceiverUserData(user[0]);
                //genericSearchModal.UpdateModalStatus(user.ModalStatus);
            }
            RequestList.Clear();
        }

        #region GetTypeCode

        private async Task GetTypeCode()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "TCA");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse.Succeeded)
                {
                    Habilitar = true;
                    TypeCodeList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<VSystemParamDtoResponse>();
                }

                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las funcionalidades: {ex.Message}");
            }
        }

        #endregion GetTypeCode

        #region HandleModalClosed

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
            reset();
            StateHasChanged();
        }

        #endregion HandleModalClosed

        #region HandleModalNotiClose

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
                //genericUserSearch.RemoveUserSelectedById(vUserSelected.UserId);
                reset();
            }
            else if (notificationModal.Type == ModalType.Error)
            {
                UpdateModalStatus(args.ModalStatus);
            }
            else if (args.IsAccepted && notificationModal.ModalOrigin.Equals("changeUser"))
            {
                GetReceiverUserData(vUserTemporal);
            }
            else if (args.IsAccepted && notificationModal.ModalOrigin.Equals("userUndefine"))
            {
                UpdateModalStatus(true);
            }
            else if (notificationModal.Type == ModalType.Information)
            {
                UpdateModalStatus(args.ModalStatus);
            }

        }

        #endregion HandleModalNotiClose

        #region UpdateSelectedRecord

        public void UpdateSelectedRecord(OverrideTrayManagerDtoResponse? record)
        {
            putOrPost = true;
            IsRequired = "hidden";
            _selectedRecord = record;
            Habilitar = false;
            vUserSelected.UserName = _selectedRecord.NameUser;
            vUserSelected.UserId = _selectedRecord.UserId;
            TypeCode = _selectedRecord.TypeCode;
            activeState = _selectedRecord.ActiveState;
        }

        #endregion UpdateSelectedRecord

        #region Reset

        private void reset()
        {
            TypeCode = "";
            DFCode = Translation["SelectAnOption"];
            vUserSelected = new();
            activeState = true;
            Habilitar = true;
            IsRequired = "visible";
        }

        #endregion Reset

        #region PostManager

        private async Task PostManager()
        {
            OverrideTrayManagerDtoRequest Manager = new();
            Manager.UserId = vUserSelected.UserId;
            Manager.TypeCode = TypeCode;
            Manager.ActiveState = activeState;

            if(Manager.UserId > 0)
            {
                var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationManager/CreateCancelationManager", Manager);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<OverrideTrayManagerDtoRequest>>();
                if (deserializeResponse.Succeeded)
                {
                    //Logica Exitosa
                    notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                    await OnChangeData.InvokeAsync(true);
                    //reset();
                }
                else
                {
                    //Logica no Exitosa
                    notificationModal.UpdateModal(ModalType.Information, Translation["ExistManager"], true, Translation["Accept"], buttonTextCancel: "");
                    //reset();
                }
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["NecessaryUserToContinue"], true, buttonTextCancel: "", modalOrigin: "userUndefine");
            }

            putOrPost = false;
        }

        #endregion PostManager

        #region PutManager

        private async Task PutManager()
        {
            OverrideTrayManagerEditDtoRequest ManagerEdit = new();
            ManagerEdit.CancelationManagerId = _selectedRecord.CancelationManagerId;
            ManagerEdit.TypeCode = TypeCode;
            ManagerEdit.UserId = vUserSelected.UserId;

            if (!changeUser)
            {
                await GetRequest(ManagerEdit.UserId);
            }

            if (RequestList.Any())
            {
                activeState = true;
                notificationModal.UpdateModal(ModalType.Information, Translation["ManagerPendingRequests"], true, buttonTextCancel: "");
            }
            else 
            {
                ManagerEdit.ActiveState = activeState;
                var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationManager/UpdateCancelationManager", ManagerEdit);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<OverrideTrayManagerEditDtoRequest>>();
                if (deserializeResponse.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                    await OnChangeData.InvokeAsync(true);
                    //reset();
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Information, Translation["UpdateErrorMessage"], true, Translation["Accept"], buttonTextCancel: "");
                    //reset();
                }
            }

            RequestList.Clear();
            putOrPost = false;
            changeUser = false;
        }

        #endregion PutManager

        #region Save

        private async Task Save()
        {
            try
            {
                if (Habilitar)
                {
                    await PostManager();
                }
                else
                {
                    await PutManager();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar un administrador {ex.Message}");
            }
        }

        #endregion Save

        #region GetRequest

        private async Task GetRequest(int id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                if (vUserSelected.UserId > 0)
                {
                    RequestFilter.UserId = id;
                    RequestFilter.CancelationState = "TEA,PE";
                    RequestFilter.TypeCode = TypeCode;
                    RequestFilter.UserManagerId = true;
                }

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

        #region BindTypeCode

        public void BindTypeCode(string value)
        {
            TypeCode = value;
            ValidateEnableButton();
        }

        #endregion BindTypeCode

        #region CloseModalSearchUser
        private void HandleModalUserClosed(bool status)
        {
            UserModalStatus = status;
            genericUserSearch.RemoveUserSelectedById(vUserSelected.UserId);
        }

        #endregion

        #region ValidationMethods

        private void ValidateEnableButton()
        {
            enableButton = string.IsNullOrWhiteSpace(vUserSelected.UserName) ||
                   string.IsNullOrWhiteSpace(TypeCode);
        }

        #endregion

        #endregion Methods
    }
}