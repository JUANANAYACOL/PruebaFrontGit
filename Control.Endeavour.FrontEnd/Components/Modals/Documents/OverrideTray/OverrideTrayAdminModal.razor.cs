using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Filing;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.OverrideTray
{
    public partial class OverrideTrayAdminModal
    {


        #region Variables
        #region Inject 
        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private FilingStateContainer? FilingSC { get; set; }
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion

        #region Components


        #endregion

        #region Modals

        private OverrideTrayModal _ModalOverrideTray = new();

        #endregion

        #region Parameters
        [Parameter] public EventCallback<bool> OnChangeData { get; set; }
        [Parameter] public EventCallback<bool> OnChangeDataValidation { get; set; }
        [Parameter] public EventCallback<int> ControlId { get; set; }
        #endregion

        #region Models

        #endregion

        #region Environments


        public bool modalStatus = false;
        private string txtComentario;


        private string typecode;
        private string reasonCode;
        private string TypeRequestCode;
        private string CancelationState = "TEA,PE";
        private string cancelationReasonId;
        private int Reason;
        private string controlId;
        private int contadorcarac = 0;


        private NotificationsComponentModal notificationModal;

        private List<FileInfoData> fileInfoDatas = new List<FileInfoData>();

        private string email;
        private int ManagerId;
        private string ShowTable = "d-none";
        private OverrideTrayManagerDtoRequest Request;
        private OverrideTrayRequestDtoResponse _selectedRecord;
        private List<OverrideTrayManagerDtoResponse> UserManager;


        private List<VUserDtoResponse> userList;

        private List<VSystemParamDtoResponse> TypeCodeList;
        private List<OverrideTrayRequestDtoResponse> RequestList = new();

        private List<FileCompanyDtoRequest> File = new();
        private FileCompanyDtoRequest _FileData = new();
        private List<int> ListId = new();

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }


        #endregion

        #region Methods

        #region HandleLanguageChanged

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        #endregion

        #region HandleFilesList

        private async Task HandleFilesList(List<FileInfoData> newList)
        {
            fileInfoDatas = newList;
            foreach (var item in fileInfoDatas)
            {
                _FileData.DataFile = item.Base64Data;
                _FileData.FileExt = item.Extension;
                _FileData.FileName = item.Name;
                File.Add(_FileData);

            }
        }
        #endregion

        #region UpdateModalStatus
        public void UpdateModalStatusAdmin(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }
        #endregion

        #region UpdateSelectedRecord
        public async void UpdateSelectedRecord(OverrideTrayRequestDtoResponse? record)
        {
            try
            {
                _selectedRecord = record;
                if (RequestList.Count > 0) { RequestList.Clear(); }

                RequestList.Add(_selectedRecord);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las razones: {ex.Message}");
            }
        }
        #endregion

        #region HandleModalClosed
        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
            StateHasChanged();
        }
        #endregion

        #region HandleModalNotiClose
        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatusAdmin(args.ModalStatus);
            }
            if (notificationModal.Type == ModalType.Error)
            {
                UpdateModalStatusAdmin(args.ModalStatus);
            }

        }
        #endregion

        #region Getlist
        public void Getlist(List<OverrideTrayRequestDtoResponse> newList)
        {

            try
            {
                if (newList.Any())
                {
                    RequestList = newList;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las razones: {ex.Message}");
            }
        }
        #endregion

        #region ShowModalOverrideTrayAdmin

        private async void ShowModalOverrideTrayAdmin(OverrideTrayRequestDtoResponse args)
        {
            await _ModalOverrideTray.PrepareModal(args, true);
            _ModalOverrideTray.UpdateModalStatus(true);
        }

        #endregion ShowModalOverrideTrayAdmin

        #region reset
        public void reset()
        {  
            txtComentario = "";
            contadorcarac = 0;
            RequestList.Clear();
            StateHasChanged();
        }
        #endregion

        #region NewModalValidation
        public async void NewModalValidation()
        {
            if (Convert.ToInt32(controlId) > 0)
            {
                FilingSC.DocumentId = controlId;
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Warning, Translation["DocumentIdRequiredMessage"], true, Translation["Accept"], Translation["Cancel"]);

            }
        }
        #endregion

        #region CountChar

        private void ContarCaracteres(ChangeEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value.ToString()))
            {
                contadorcarac = e.Value.ToString().Length;
            }
            else
            {
                contadorcarac = 0;
            }
        }

        #endregion CountChar

        #region GetAdmin
        private async Task GetAdmin()
        {
            OverrideTrayManagerDtoRequest manager = new();
            manager.TypeCode = typecode;
            try
            {
                var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationManager/ByFilter", manager);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<OverrideTrayManagerDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {
                    UserManager = deserializeResponse.Data;
                    ManagerId = UserManager[0].UserId;
                    await GetInfoUser();
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al obtener el usuario administrador" + ex.Message.ToString());
            }

        }
        #endregion

        private async Task GetInfoUser()
        {
            VUserDtoRequest user = new();
            user.UserId = ManagerId;
            try
            {
                var responseApi = await HttpClient.PostAsJsonAsync("generalviews/VUser/ByFilter", user);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<VUserDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {
                    userList = deserializeResponse.Data;
                    email = userList[0].Email;
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al obtener el usuario administrador" + ex.Message.ToString());
            }

        }
        #endregion

        #region UpdateRequest

        private async Task UpdateRequest()
        {
            try
            {
                List<OverrideTrayRequestUpdateDtoRequest> listRecords = new();

                if (!string.IsNullOrEmpty(txtComentario))
                {
                    foreach (var item in RequestList)
                    {
                        if (item.TypeRequestCode.Equals("RCA,ANU"))
                        {
                            OverrideTrayRequestUpdateDtoRequest record = new()
                            {
                                CancelationRequestId = item.CancelationRequestId.Value,
                                TypeRequestCode = item.TypeRequestCode,
                                UserRequestId = item.UserRequestId.Value,
                                RejectionComment = txtComentario,
                                CancelationState = "TEA,AN",
                            };

                            listRecords.Add(record);
                        }
                        else
                        {
                            OverrideTrayRequestUpdateDtoRequest record = new()
                            {
                                CancelationRequestId = item.CancelationRequestId.Value,
                                TypeRequestCode = item.TypeRequestCode,
                                UserRequestId = item.UserRequestId.Value,
                                RejectionComment = txtComentario,
                                CancelationState = "TEA,DESAN",
                            };

                            listRecords.Add(record);
                        }

                    }

                    var responseApiMasivo = await HttpClient.PostAsJsonAsync("overridetray/CancelationRequest/UpdateCancelationRequests", listRecords);
                    var deserializeResponseMasivo = await responseApiMasivo.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<OverrideTrayRequestDtoResponse>>>();
                    if (deserializeResponseMasivo.Succeeded)
                    {
                        //Logica Exitosa
                        notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                        await OnChangeData.InvokeAsync(true);
                        reset();
                    }
                    else
                    {
                        //Logica no Exitosa
                        notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
                        reset();
                    }
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["NecessaryRequest"], true, Translation["Accept"]);
                }

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al guardar cancelationRequest: {ex.Message}");

            }
        }
        #endregion
    }
}
