using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.ManagementTray;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.Attachments;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.ViewDocuments;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using DevExpress.Blazor.Primitives.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Security.Cryptography;
using DocumentInformation = Control.Endeavour.FrontEnd.Models.Models.Documents.OverrideTray.Response.DocumentInformation;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.OverrideTray
{
    public partial class OverrideTrayModal : ComponentBase
    {
        #region Variables

        #region Inject

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }

        //[Inject]
        //private FilingStateContainer? FilingSC { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Parameters

        [Parameter] public EventCallback<bool> OnChangeData { get; set; }
        [Parameter] public EventCallback<bool> OnChangeDataValidation { get; set; }
        [Parameter] public EventCallback<int> ControlId { get; set; }
        //[Parameter] public bool IsAdmin { get; set; } = true;

        #endregion Parameters

        #region Modals

        private NotificationsComponentModal notificationModal = new();
        private GeneralInformationModal generalInformationModal = new();
        private ViewDocumentModal viewDocumentModal = new();
        private ValidateDocumentGeneralInfoModal validateDocumentInfo = new();
        private AttachmentTrayModal attachmentTrayModal = new();

        #endregion Modals

        #region Models

        private CancelationRequestQueryFilter RequestFilter = new();
        private CancelationManagerDtoResponse cancelationManager = new();
        private CancelationRequestInformationDtoResponse cancelationRequestInformationDtoResponse = new();
        private OverrideTrayRequestDtoResponse overrideTrayRequestDtoResponse = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        public string[] ext { get; set; } = { ".xlsx", ".pdf", ".docx", ".doc", ".xls", ".png", ".jpeg", ".gif" };

        public string descriptionInput { get; set; }
        public string descriptionAdminInput { get; set; }

        private string labelDescription = "";
        private string filingCode { get; set; } = string.Empty;

        private string typecode = "";
        private string TypeRequestCode = "";
        private string TypeRequestAdminCode = "";
        private string filingCodeData = "";
        private string DTTypeCode = "";
        private string DTTReasonCode = "";
        private string DTTReason = "";
        private string txtAInformation = "";
        private string PHInput = "";
        private string systemParamCL = "";

        private string ShowTable = "d-none";

        #endregion Environments(String)

        #region Environments(Numeric)

        private int Reason = 0;
        private int intControlId = 0;
        private int ManagerId = 0;
        private int contadorcarac = 0;
        private int contadorcaracAdmin = 0;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        public bool modalStatus = false;
        private bool BtnVerificarDisabled = true;
        private bool IsDisabledCode = true;
        private bool BtnAddFilingCode = true;
        private bool IsAdmin = false;
        private bool tipyCodeEnabled = true;
        private bool typeRequestCodeEnabled = true;
        private bool reasonEnabled = true;
        private bool systemParamCLEnabled = false;
        private bool textAreaDisabled = false;
        private bool enableButton = true;
        private bool enableButtonAdmin = true;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<VSystemParamDtoResponse> TypeCodeList = new();
        private List<VSystemParamDtoResponse> ReasonCodeList = new();
        private List<VSystemParamDtoResponse> ReasonAdminCodeList = new();
        private List<OverrideTrayReasonDtoResponse> overrideTrayReasons = new();
        private List<OverrideTrayManagerDtoResponse> UserManager = new();
        private List<VUserDtoResponse> userList = new();
        private List<FileCompanyDtoRequest> files = new();
        private List<int> ListId = new();
        private List<string> filigCodeList = new();
        private List<AttachmentInformation> attachmentList = new();
        private List<DocumentInformation> documentsList = new();
        private List<FileInfoData> fileInfoDatas = new List<FileInfoData>();
        private List<VSystemParamDtoResponse> systemFieldsCLList = new();
        private List<OverrideTrayRequestDtoResponse> RequestList = new();
        private List<AppKeysDtoResponse> OverrideTrayParams = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            DTTypeCode = Translation["SelectAnOption"];
            DTTReasonCode = Translation["SelectAnOption"];
            DTTReason = Translation["SelectAnOption"];
            PHInput = Translation["EnterRecordNumber"];
            txtAInformation = Translation["OfficialAssignedToValidateRequest"];
            await GetReasonCode();
            await GetTypeCode();
            await GetClassCom();
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

        #region HandleFilesList

        private void HandleFilesList(List<FileInfoData> newList)
        {
            fileInfoDatas = newList;
            foreach (var item in fileInfoDatas)
            {
                FileCompanyDtoRequest file = new()
                {
                    CompanyId = 17,
                    DataFile = item.Base64Data,
                    FileExt = item.Extension.Trim('.'),
                    FileName = item.Name,
                };

                files.Add(file);
            }
        }

        #endregion HandleFilesList

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        #endregion UpdateModalStatus

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
                reset();
            }
        }

        #endregion HandleModalNotiClose

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
                    txtAInformation = Translation["OfficialAssignedToValidateRequest"] + " \n ID: " + UserManager[0].UserId + "\n "
                        + Translation["Manager"] + ": " + UserManager[0].FullName + "\n " + Translation["Unit"] + ": " + UserManager[0].ChargeName
                        + "\n " + Translation["Office"] + ": " + UserManager[0].ChargeName;
                    await GetInfoUser();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el usuario administrador" + ex.Message.ToString());
            }
        }

        #endregion GetAdmin

        #region GetInfoUser

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
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el usuario administrador" + ex.Message.ToString());
            }
        }

        #endregion GetInfoUser

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
                    TypeCodeList = deserializeResponse.Data != null ? deserializeResponse.Data : new List<VSystemParamDtoResponse>();
                    await GetAdmin();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el tipo: {ex.Message}");
            }
        }

        #endregion GetTypeCode

        #region GetComunicationClass

        public async Task GetClassCom()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "CL");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse.Succeeded)
                {
                    systemFieldsCLList = deserializeResponse.Data ?? new();
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener la clase de comunicacion: {ex.Message}");
            }
        }

        private void validInput(string value)
        {
            systemParamCL = value;

            if (!string.IsNullOrEmpty(systemParamCL))
            {
                IsDisabledCode = false;
            }
            else { IsDisabledCode = true; }
        }

        #endregion GetComunicationClass

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
                Console.WriteLine($"Error al obtener las razones: {ex.Message}");
            }
        }

        private void validLabel(string value)
        {
            TypeRequestCode = value;
            systemParamCLEnabled = true;

            if (TypeRequestCode.Equals("RCA,ANU"))
            {
                labelDescription = Translation["Cancellation"];
            }
            else { labelDescription = Translation["Uncancellation"]; }
            ValidateEnableButton();
        }

        #endregion GetReasonCode

        #region GetReasonAdminCode

        private async Task GetReasonAdminCode()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "TEA");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse.Succeeded)
                {
                    ReasonAdminCodeList = deserializeResponse.Data != null ? deserializeResponse.Data.Where(x => !x.FieldCode.Equals("PE")).ToList() : new List<VSystemParamDtoResponse>();
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las razones: {ex.Message}");
            }
        }

        #endregion GetReasonAdminCode

        #region GetReason

        private async Task GetReason(string value)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                typecode = value;
                await GetManager(typecode);
                OverrideTrayReasonDtoRequest reason = new();
                reason.TypeCode = typecode;
                var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationReason/ByFilter", reason);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<OverrideTrayReasonDtoResponse>>>();
                if (deserializeResponse.Succeeded)
                {
                    overrideTrayReasons = deserializeResponse.Data != null ? deserializeResponse.Data : new List<OverrideTrayReasonDtoResponse>();
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
                ValidateEnableButton();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las razones: {ex.Message}");
            }
        }

        #endregion GetReason

        #region GetManager

        private async Task GetManager(string value)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("typecode");
                HttpClient?.DefaultRequestHeaders.Add("typecode", value);
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<CancelationManagerDtoResponse>>("overridetray/CancelationManager/GetCancelationManager");
                HttpClient?.DefaultRequestHeaders.Remove("typecode");

                if (deserializeResponse.Succeeded)
                {
                    cancelationManager = deserializeResponse.Data != null ? deserializeResponse.Data : new();
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las razones: {ex.Message}");
            }
        }

        #endregion GetManager

        #region reset

        public void reset()
        {
            DTTypeCode = Translation["SelectAnOption"];
            DTTReasonCode = Translation["SelectAnOption"];
            DTTReason = Translation["SelectAnOption"];
            PHInput = Translation["EnterRecordNumber"];
            txtAInformation = Translation["OfficialAssignedToValidateRequest"];
            ShowTable = "d-none";
            TypeRequestCode = "";
            filingCode = "";
            systemParamCL = "";
            typecode = "";
            Reason = 0;
            contadorcarac = 0;
            contadorcaracAdmin = 0;
            descriptionInput = "";
            ListId.Clear();
            fileInfoDatas.Clear();
            fileInfoDatas = new();
            IsDisabledCode = true;
            files = new();
            filigCodeList = new();
            cancelationManager = new();
            TypeRequestAdminCode = "";
            descriptionAdminInput = "";
            BtnAddFilingCode = true;
            IsAdmin = false;
            tipyCodeEnabled = true;
            typeRequestCodeEnabled = true;
            reasonEnabled = true;
            systemParamCLEnabled = false;
            textAreaDisabled = false;
            StateHasChanged();
        }

        #endregion reset

        #region CountChar

        private void ContarCaracteres(ChangeEventArgs e, int value)
        {
            int length = 0;

            if (!string.IsNullOrEmpty(e.Value?.ToString()))
            {
                length = e.Value.ToString().Length;
            }

            if (value == 1)
            {
                contadorcarac = length;
            }
            else if (value == 2)
            {
                contadorcaracAdmin = length;
            }
            ValidateEnableButton();
        }

        #endregion CountChar

        #region CallParams

        public async Task<List<AppKeysDtoResponse>> CallParams()
        {
            AppKeysFilterDtoRequest appKeysFilter = new();
            appKeysFilter.FunctionName = "OverrideTrayParam";
            var responseApi = await HttpClient.PostAsJsonAsync("params/AppKeys/ByFilter", appKeysFilter);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<AppKeysDtoResponse>>>();

            if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
            {
                return deserializeResponse.Data;
            }
            else { return new List<AppKeysDtoResponse>(); }
        }

        #endregion

        #region SendDocumentRelation

        private async Task SelectDocRelationAsync()
        {
            try
            {
                if (filingCode != null)
                {
                    SearchDtoRequest filtro = new SearchDtoRequest();

                    if (TypeRequestCode.Equals("RCA,ANU"))
                    {
                        filtro.FilingCode = filingCode;
                        filtro.ClassCode = systemParamCL;
                        filtro.SearchDocumentCanceled = false;
                        filtro.SearchFilingCode = true;
                    }
                    else
                    {
                        filtro.FilingCode = filingCode;
                        filtro.ClassCode = systemParamCL;
                        filtro.SearchDocumentCanceled = true;
                        filtro.SearchFilingCode = true;
                    }

                    var responseApi = await HttpClient.PostAsJsonAsync("documents/Document/SearchEngineDocument", filtro);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<SearchDtoResponse>>>();

                    if (deserializeResponse.Data.Data.Count > 0 && deserializeResponse.Data != null)
                    {
                        OverrideTrayParams = await CallParams();

                        switch (OverrideTrayParams[0].Value1.Equals("SI"))
                        {
                            case true:
                                #region TimeToCancellation
                                DateTime createFilingCodeDate = deserializeResponse.Data.Data[0].CreateDate.Value;
                                DateTime currentDate = DateTime.Now;

                                int timeToCancellation = int.Parse(OverrideTrayParams[0].Value2);
                                TimeSpan timeDifference = currentDate - createFilingCodeDate;
                                double hoursDifference = timeDifference.TotalHours;

                                if (hoursDifference > timeToCancellation && TypeRequestCode.Equals("RCA,ANU"))
                                {
                                    intControlId = 0;
                                    filingCode = string.Empty;
                                    systemParamCL = string.Empty;
                                    IsDisabledCode = true;
                                    BtnAddFilingCode = true;
                                    notificationModal.UpdateModal(ModalType.Warning, Translation["TimeToCancellation"], true, Translation["Accept"], buttonTextCancel: "");
                                }
                                else
                                {
                                    intControlId = deserializeResponse.Data.Data[0].ControlId;
                                    filingCodeData = deserializeResponse.Data.Data[0].FilingCode;
                                }
                                #endregion
                                break;
                            case false:
                                intControlId = deserializeResponse.Data.Data[0].ControlId;
                                filingCodeData = deserializeResponse.Data.Data[0].FilingCode;
                                break;
                        }
                    }
                    else
                    {
                        intControlId = 0;
                        filingCode = string.Empty;
                        systemParamCL = string.Empty;
                        IsDisabledCode = true;
                        BtnAddFilingCode = true;
                        notificationModal.UpdateModal(ModalType.Information, Translation["NoRecordWithThatComunicationType"], true, buttonTextCancel: "");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        #endregion SendDocumentRelation

        #region NewModalValidation

        public async Task NewModalValidationAsync()
        {
            await SelectDocRelationAsync();

            if (intControlId > 0)
            {
                await generalInformationModal.GeneralInformation(intControlId);
                generalInformationModal.UpdateModalStatus(true);
                BtnAddFilingCode = false;
            }
        }

        #endregion NewModalValidation

        #region CaptureIdControl

        public void CaptureIdControl()
        {
            if (!String.IsNullOrEmpty(filingCode))
            {
                BtnVerificarDisabled = false;
            }
            else
            {
                BtnVerificarDisabled = true;
            }
        }

        #endregion CaptureIdControl

        #region PostRequest

        private async Task PostRequest()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                if (string.IsNullOrEmpty(TypeRequestCode) && Reason < 0 && string.IsNullOrEmpty(typecode))
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
                    //reset();
                }
                else if (ListId.Count > 1)
                {
                    List<OverrideTrayRequestDtoRequest> Requests = new();

                    foreach (var item in ListId)
                    {
                        OverrideTrayRequestDtoRequest RequestMasivo = new()
                        {
                            ControlId = item,
                            CancelationReasonId = Reason,
                            RequestComment = descriptionInput,
                            UserId = cancelationManager.UserId,
                            TypeRequestCode = TypeRequestCode,
                            TypeCode = typecode,
                            FileDtoRequests = files,
                            ActionType = ActionType.USUARIO
                        };
                        Requests.Add(RequestMasivo);
                    }

                    var responseApiMasivo = await HttpClient.PostAsJsonAsync("overridetray/CancelationRequest/CreateCancelationRequests", Requests);
                    var deserializeResponseMasivo = await responseApiMasivo.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<OverrideTrayReasonDtoRequest>>>();
                    if (deserializeResponseMasivo.Succeeded)
                    {
                        //Logica Exitosa
                        notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                        await OnChangeData.InvokeAsync(true);
                        //reset();
                    }
                    else
                    {
                        //Logica no Exitosa
                        notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
                    }
                }
                else
                {
                    OverrideTrayRequestDtoRequest Request = new();
                    Request.ControlId = intControlId;
                    Request.CancelationReasonId = Reason;
                    Request.UserId = cancelationManager.UserId;
                    Request.RequestComment = descriptionInput;
                    Request.TypeRequestCode = TypeRequestCode;
                    Request.TypeCode = typecode;
                    Request.FileDtoRequests = files;
                    Request.ActionType = ActionType.USUARIO;

                    var responseApi = await HttpClient.PostAsJsonAsync("overridetray/CancelationRequest/CreateCancelationRequest", Request);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<OverrideTrayReasonDtoRequest>>();
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
                        notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
                    }
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar cancelationRequest: {ex.Message}");
            }
        }

        #endregion PostRequest

        #region DeleteControlID

        private async Task DeleteControlID(string filingCode)
        {
            if (!string.IsNullOrEmpty(filingCode))
            {
                filigCodeList.Remove(filingCode);
            }
            if (filigCodeList.Count == 0)
            {
                ShowTable = "d-none";
            }
        }

        #endregion DeleteControlID

        #region AddListControlId

        private async Task AddListControlId()
        {
            if (!string.IsNullOrEmpty(filingCode))
            {
                if (filigCodeList.Contains(filingCode))
                {
                    notificationModal.UpdateModal(ModalType.Warning, Translation["DocHasAlreadyAdded"], true, Translation["Accept"]);
                }
                else
                {
                    await ValidIsCancelationRequest(intControlId);

                    switch (RequestList.Any())
                    {
                        case true:
                            if (RequestList.LastOrDefault().CancelationState.Equals("TEA,PE"))
                            {
                                notificationModal.UpdateModal(ModalType.Warning, Translation["DocAssignedToProcess"], true, Translation["Accept"], buttonTextCancel: "");
                            }
                            else
                            {
                                OverrideTrayParams = await CallParams();

                                switch (OverrideTrayParams[1].Value1.Equals("SI"))
                                {
                                    case true:
                                        #region TimeToUncancellation
                                        DateTime RequestTime = RequestList[0].DateManage.Value;
                                        DateTime currentDate = DateTime.Now;

                                        int timeToUncancellation = int.Parse(OverrideTrayParams[1].Value2);
                                        TimeSpan timeDifference = currentDate - RequestTime;
                                        double hoursDifference = timeDifference.TotalHours;

                                        switch (hoursDifference > timeToUncancellation)
                                        {
                                            case true:
                                                notificationModal.UpdateModal(ModalType.Warning, Translation["TimeToUncancellation"], true, Translation["Accept"], buttonTextCancel: "");
                                                break;
                                            case false:
                                                ListId.Add(intControlId);
                                                filigCodeList.Add(filingCodeData);
                                                ShowTable = "";
                                                break;
                                        }
                                        #endregion
                                        break;
                                    case false:
                                        ListId.Add(intControlId);
                                        filigCodeList.Add(filingCodeData);
                                        ShowTable = "";
                                        break;
                                }
                            }
                            break;
                        case false:
                            ListId.Add(intControlId);
                            filigCodeList.Add(filingCodeData);
                            ShowTable = "";
                            break;
                    }
                }
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Warning, Translation["DocumentIdRequiredMessage"], true, Translation["Accept"]);
            }

            StateHasChanged();
            ValidateEnableButton();
            filingCode = string.Empty;
            systemParamCL = string.Empty;
            IsDisabledCode = true;
            BtnAddFilingCode = true;
        }

        #endregion AddListControlId

        #region ViewAttachment

        private async Task HandleViewAttachment(AttachmentInformation item)
        {
            await viewDocumentModal.DownloadFile(item.FileId);
        }

        #endregion ViewAttachment

        #region ViewDocument

        private async Task HandleViewDocument(DocumentInformation item)
        {
            if (item != null)
            {
                await validateDocumentInfo.GeneralInformation(item.ControlId);
                validateDocumentInfo.UpdateModalStatus(true);
            }
        }

        #endregion ViewDocument

        #region ViewAttachmentList

        public async Task ViewAttachmentList(DocumentInformation model)
        {
            await attachmentTrayModal.UpdateAttachmentDocument(model.ControlId);
            attachmentTrayModal.UpdateModalStatus(true);
        }

        #endregion ViewAttachmentList

        #region PrepareModal

        public async Task PrepareModal(OverrideTrayRequestDtoResponse item, bool isAdmin)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                await GetReasonAdminCode();
                IsAdmin = isAdmin;
                overrideTrayRequestDtoResponse = item;
                HttpClient?.DefaultRequestHeaders.Remove("CancelationRequestId");
                HttpClient?.DefaultRequestHeaders.Add("CancelationRequestId", item.CancelationRequestId.ToString());
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<CancelationRequestInformationDtoResponse>>("overridetray/CancelationRequest/GetRequestInformation");
                HttpClient?.DefaultRequestHeaders.Remove("CancelationRequestId");

                if (deserializeResponse.Succeeded)
                {
                    cancelationRequestInformationDtoResponse = deserializeResponse.Data != null ? deserializeResponse.Data : new();

                    if (cancelationRequestInformationDtoResponse != null)
                    {
                        tipyCodeEnabled = false;
                        typeRequestCodeEnabled = false;
                        reasonEnabled = false;
                        textAreaDisabled = true;
                        typecode = cancelationRequestInformationDtoResponse.TypeCode;
                        await GetReason(typecode);
                        TypeRequestCode = cancelationRequestInformationDtoResponse.TypeRequestCode;
                        Reason = cancelationRequestInformationDtoResponse.CancelationReasonId.Value;
                        descriptionInput = cancelationRequestInformationDtoResponse.RequestComment;
                        contadorcarac = descriptionInput.Count();
                        attachmentList = cancelationRequestInformationDtoResponse.Attachments;
                        documentsList = cancelationRequestInformationDtoResponse.Documents;

                        if (TypeRequestCode.Equals("RCA,ANU"))
                        {
                            ReasonAdminCodeList = ReasonAdminCodeList.Where(x => !x.Code.Equals("TEA,DESAN")).ToList();
                        }
                        else { ReasonAdminCodeList = ReasonAdminCodeList.Where(x => !x.Code.Equals("TEA,AN")).ToList(); }
                    }
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar la clasificacion del documento: {ex.Message}");
            }
        }

        #endregion PrepareModal

        #region ValidIsCancelationRequest

        private async Task ValidIsCancelationRequest(int id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                RequestFilter.controlId = id;

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

        #endregion ValidIsCancelationRequest

        #region UpdateRequest

        private async Task UpdateRequest()
        {
            try
            {

                OverrideTrayRequestUpdateDtoRequest record = new()
                {
                    CancelationRequestId = overrideTrayRequestDtoResponse.CancelationRequestId.Value,
                    TypeRequestCode = overrideTrayRequestDtoResponse.TypeRequestCode,
                    UserRequestId = overrideTrayRequestDtoResponse.UserRequestId.Value,
                    RejectionComment = descriptionAdminInput,
                    CancelationState = TypeRequestAdminCode,
                };

                var responseApiMasivo = await HttpClient.PostAsJsonAsync("overridetray/CancelationRequest/UpdateCancelationRequest", record);
                var deserializeResponseMasivo = await responseApiMasivo.Content.ReadFromJsonAsync<HttpResponseWrapperModel<OverrideTrayRequestDtoResponse>>();
                if (deserializeResponseMasivo.Succeeded)
                {
                    //Logica Exitosa
                    notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                    await OnChangeData.InvokeAsync(true);
                    //reset();
                }
                else
                {
                    //Logica no Exitosa
                    notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al guardar cancelationRequest: {ex.Message}");

            }
        }
        #endregion

        #region BindTypeCode

        public void BindReason(int value)
        {
            Reason = value;
            ValidateEnableButton();
        }

        #endregion BindTypeCode

        #region BindTypeCode

        public void BindTypeRequestAdminCode(string value)
        {
            TypeRequestAdminCode = value;
            ValidateEnableButtonAdmin();
        }

        #endregion BindTypeCode

        #region ValidationMethods

        private void ValidateEnableButton()
        {
            enableButton = string.IsNullOrWhiteSpace(typecode) ||
                   string.IsNullOrWhiteSpace(TypeRequestCode) ||
                   string.IsNullOrWhiteSpace(descriptionInput) ||
                   Reason == 0 ||
                   filigCodeList.Count == 0;
        }

        private void ValidateEnableButtonAdmin()
        {
            enableButtonAdmin = string.IsNullOrWhiteSpace(TypeRequestAdminCode);
        }

        #endregion

        #endregion Methods
    }
}