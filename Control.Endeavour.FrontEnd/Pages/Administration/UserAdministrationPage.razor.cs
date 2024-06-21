using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.UsersAdministration;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffice.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.BranchOffices;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Permission.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Profile.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Replacement.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Signature.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.User.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using DevExpress.Pdf;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class UserAdministrationPage
    {
        #region Variables

        #region Inject

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Components

        private PaginationInfo paginationInfo = new();
        private NewPaginationComponent<VUserDtoResponse, VUserDtoRequest> paginationComponetPost { get; set; } = new();

        #endregion Components

        #region Modals

        private ChangePasswordUserModal changePasswordUserModal { get; set; } = new();
        private DragAndDropImageComponent bannerDADComponent { get; set; } = new();
        private NotificationsComponentModal notificationModal { get; set; } = new();
        private UserProfilesModal modalUserProfiles { get; set; } = new();
        private UserPermissionModal modalUserPermission { get; set; } = new();

        #endregion Modals

        #region Models

        private MetaModel userMeta { get; set; } = new();
        private VUserDtoRequest vUserFilterDtoRequest { get; set; } = new();
        private CreateUsersDtoRequest _createUsersDtoRequest { get; set; } = new();
        private VUserDtoResponse recordToDelete { get; set; } = new();
        private ProfilesDtoResponse recordToDeleteProfile = new();
        private CreatePermissionDtoRequest recordToDeletePermission { get; set; } = new();
        private UsersSignatureDtoRequest recordToUpdate { get; set; } = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string pattern { get; set; } = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        private string patternNumeric { get; set; } = @"\d";
        private string? userNameFilter { get; set; }
        private string? userLastNameFilter { get; set; }
        private string? userLoginFilter { get; set; }
        private string Panel1Class { get; set; } = "";
        private string Panel2Class { get; set; } = "d-none";
        private string ProfileName { get; set; } = "";

        private string idUserSelected { get; set; } = string.Empty;

        private string UriFilterVUser { get; set; } = "generalviews/VUser/ByFilter";

        #region Variables Color Condition

        private string OneCapitalLetterPassword { get; set; } = "red";
        private string OneNumberPassword { get; set; } = "red";
        private string OneSpecialCharacterPassword { get; set; } = "red";
        private string MinimumCharactersPassword { get; set; } = "red";

        private string OneCapitalLetterSecondPassword { get; set; } = "red";
        private string OneNumberSecondPassword { get; set; } = "red";
        private string OneSpecialCharacterSecondPassword { get; set; } = "red";
        private string MinimumCharactersSecondPassword { get; set; } = "red";

        #endregion Variables Color Condition

        #region Variables Tabs Signatures

        private string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png" };
        private string[] DataFormat { get; set; } = { ".jpg, .png, .svg, .jpeg", "10" };
        private string AlertMessage { get; set; } = string.Empty;
        private string SourceMechanicalSignature { get; set; } = string.Empty;
        private string SourceSignatureHeading { get; set; } = string.Empty;
        private string SourceDigitalSignature { get; set; } = string.Empty;

        #endregion Variables Tabs Signatures

        #region Variables Additional Signature Tabs

        private string SourceMechanicalSignatureAdditional1 { get; set; } = string.Empty;
        private string SourceMechanicalSignatureAdditional2 { get; set; } = string.Empty;
        private string SourceMechanicalSignatureAdditional3 { get; set; } = string.Empty;

        #endregion Variables Additional Signature Tabs

        #region Default Text Combos

        private string ContracTypeText { get; set; } = "";
        private string CharguesTypeText { get; set; } = "";
        private string BranchOfficesText { get; set; } = "";
        private string ProductionOfficesText { get; set; } = "";
        private string AdministrativeUnitText { get; set; } = "";
        private string IdentificationText { get; set; } = "";

        #endregion Default Text Combos

        #endregion Environments(String)

        #region Environments(Numeric)

        private int profileIdSelected { get; set; } = new();
        private int FileSize { get; set; } = 10;

        private int administrativeUnitId { get; set; } = 0;
        private int companyId { get; set; } = 0;
        private int documentalVersionId { get; set; } = 0;
        private int userId { get; set; } = 0;

        private int SourceMechanicalSignatureId = 0;
        private int SourceSignatureHeadingId = 0;
        private int SourceDigitalSignatureId = 0;
        private int currentTab { get; set; } = 0;

        #endregion Environments(Numeric)

        #region Variables Additional Signature Tabs

        private int SourceMechanicalSignatureAdditional1Id = 0;
        private int SourceMechanicalSignatureAdditional2Id = 0;
        private int SourceMechanicalSignatureAdditional3Id = 0;

        #endregion Variables Additional Signature Tabs

        #region Enviroments (DateTime)

        private DateTime? birthDate { get; set; }
        private DateTime? starContractDate { get; set; }
        private DateTime? endContractDate { get; set; }
        private DateTime MinDatePicker { get; set; } = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime minValueTo { get; set; } = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime maxValueTo { get; set; } = new DateTime(3900, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion Enviroments (DateTime)

        #region Environments(Bool)

        private bool ExportAllPages { get; set; }
        public bool CancelExport { get; set; }
        private bool saveIsDisable { get; set; } = true;
        private bool isEnableProOfficeDrop { get; set; } = false;
        private bool editFormUser { get; set; } = false;
        private bool createFormUser { get; set; } = true;
        private bool activeState { get; set; } = true;

        private bool stateSignatureOperation { get; set; }

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        #region GetDataList

        private FileCompanyDtoRequest bannerFile { get; set; } = new();
        private List<VUserDtoResponse> userListData { get; set; } = new();
        private List<UsersSignatureDtoRequest> usersSignatureList { get; set; } = new();
        private List<VSystemParamDtoResponse> lstDocumentType { get; set; } = new();
        private List<VSystemParamDtoResponse> lstCharguesTypes { get; set; } = new();
        private List<AdministrativeUnitsDtoResponse> lstAdministrativeUnit { get; set; } = new();
        private List<ProductionOfficesDtoResponse> lstProductionOffices { get; set; } = new();
        private List<BranchOfficesDtoResponse> lstBranchOffices { get; set; } = new();
        private List<VSystemParamDtoResponse> lstContracTypes { get; set; } = new();
        private List<ProfilesDtoResponse> lstProfilesByUserId { get; set; } = new();
        private List<ProfilesDtoResponse> lstProfilesByCompanyID { get; set; } = new();
        private List<CreatePermissionDtoRequest> lstPermissionByUserId { get; set; } = new();
        private List<int> existingFunctionalities { get; set; } = new();
        private List<ProfileDtoResponse> porfileDropDownList { get; set; } = new();
        private List<CreatePermissionDtoRequest> lstUserPermissionSelected { get; set; } = new();
        private List<UsersSignatureDtoRequest> lstSignatures { get; set; } = new();

        private List<FileInfoData> bannerFileInfoData { get; set; } = new();

        #endregion GetDataList

        #region SignaturesList

        private List<FileInfoData> lstSignaturesMC { get; set; } = new();
        private List<FileInfoData> lstSignaturesRB { get; set; } = new();
        private List<FileInfoData> lstSignaturesDP12 { get; set; } = new();
        private List<FileInfoData> lstSignaturesAMC1 { get; set; } = new();
        private List<FileInfoData> lstSignaturesAMC2 { get; set; } = new();
        private List<FileInfoData> lstSignaturesAMC3 { get; set; } = new();

        #endregion SignaturesList

        private List<VUserDtoResponse> allRecords { get; set; }

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                ContracTypeText = Translation["ContractType"];
                CharguesTypeText = Translation["SelectAnOption"];
                BranchOfficesText = Translation["SelectAnOption"];
                ProductionOfficesText = Translation["SelectAnOption"];
                AdministrativeUnitText = Translation["SelectAnOption"];
                IdentificationText = Translation["SelectAnOption"];

                await GetUsersData();
                await GetDocumentPersonType();
                await GetContractType();
                await GetAdministrativeUnits();
                await GetBranchsOffices();
                await getProfileList();
                await GetChargueType();
                await GetDefaultProfilePicture(string.Empty);
                AlertMessage = GetUploadMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al realizar la initialización: {ex.Message}");
            }
            //PageLoadService.OcultarSpinnerReadLoad(Js);
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        #region Form Methods

        private async Task HandleValidSubmit()
        {
            if (Regex.IsMatch(_createUsersDtoRequest.FirstName, patternNumeric))
            {
                notificationModal.UpdateModal(ModalType.Information, string.Format(Translation["NotValidParameter"], $"{Translation["FirstName"]}", $"{Translation["Number(s)"]}"), true, Translation["Accept"], "");
            }
            else if (!string.IsNullOrEmpty(_createUsersDtoRequest.MiddleName) && Regex.IsMatch(_createUsersDtoRequest.MiddleName, patternNumeric))
            {
                notificationModal.UpdateModal(ModalType.Information, string.Format(Translation["NotValidParameter"], $"{Translation["MiddleName"]}", $"{Translation["Number(s)"]}"), true, Translation["Accept"], "");
            }
            else if (Regex.IsMatch(_createUsersDtoRequest.LastName, patternNumeric))
            {
                notificationModal.UpdateModal(ModalType.Information, string.Format(Translation["NotValidParameter"], $"{Translation["Surnames"]}", $"{Translation["Number(s)"]}"), true, Translation["Accept"], "");
            }
            else if (Regex.IsMatch(_createUsersDtoRequest.Email, pattern))
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                if (!editFormUser)
                {
                    await HandleFormCreate();
                }
                else
                {
                    await HandleFromUpdate();
                }
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["VerifyEmail"], true, Translation["Accept"], "");
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task HandleFromUpdate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                UserUpdateDtoRequest request = new()
                {
                    UserId = userId,
                    ProfileId = profileIdSelected,
                    UserName = _createUsersDtoRequest.UserName,
                    ProductionOfficeId = _createUsersDtoRequest.ProductionOfficeId,
                    BranchOfficeId = _createUsersDtoRequest.BranchOfficeId,
                    ActiveState = activeState,
                    FirstName = _createUsersDtoRequest.FirstName,
                    MiddleName = _createUsersDtoRequest.MiddleName,
                    LastName = _createUsersDtoRequest?.LastName,
                    IdentificationType = _createUsersDtoRequest?.IdentificationType,
                    Identification = _createUsersDtoRequest.Identification,
                    BirthDate = (DateTime)birthDate,
                    Address = _createUsersDtoRequest.Address,
                    PhoneNumber = _createUsersDtoRequest.PhoneNumber,
                    CellPhoneNumber = _createUsersDtoRequest.CellPhoneNumber,
                    Email = _createUsersDtoRequest.Email,
                    ChargeCode = _createUsersDtoRequest.ChargeCode,
                    ContractType = _createUsersDtoRequest.ContractType,
                    ContractNumber = _createUsersDtoRequest.ContractNumber,
                    ContractStartDate = starContractDate,
                    ContractFinishDate = endContractDate,
                    OriginAd = _createUsersDtoRequest.OriginAd
                };
                var responseApi = await HttpClient.PostAsJsonAsync("security/User/UpdateUser", request);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<VUserDtoResponse>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"], "");
                    await CloseCreateView();
                    await GetUsersData();
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"], "");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear: {ex.Message}");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task HandleFormCreate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                _createUsersDtoRequest.ProfileId = profileIdSelected;
                _createUsersDtoRequest.Signatures = lstSignatures;
                _createUsersDtoRequest.ContractStartDate = starContractDate;
                _createUsersDtoRequest.ContractFinishDate = endContractDate;
                _createUsersDtoRequest.BirthDate = (DateTime)birthDate;
                _createUsersDtoRequest.ContractType = _createUsersDtoRequest.ContractType;
                _createUsersDtoRequest.ChargeCode = _createUsersDtoRequest.ChargeCode;
                _createUsersDtoRequest.ActiveState = activeState;
                _createUsersDtoRequest.IdentificationType = _createUsersDtoRequest.IdentificationType;
                var responseApi = await HttpClient.PostAsJsonAsync("security/User/CreateUser", _createUsersDtoRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<VUserDtoResponse>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"], "");
                    await CloseCreateView();
                    await GetUsersData();
                }
                else
                {
                    Console.WriteLine(deserializeResponse.Errors.ToString());
                    notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"], "");
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"], "");
                Console.WriteLine($"Error al crear: {ex.Message}");
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion Form Methods

        #region Signatures Methods

        private async Task HandleFileFMC(List<FileInfoData> data)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            UsersSignatureDtoRequest signatureObject = new UsersSignatureDtoRequest();
            if (data != null && data.Count > 0 && !string.IsNullOrEmpty(data[0].PathView))
            {
                stateSignatureOperation = string.IsNullOrEmpty(SourceMechanicalSignature);

                SourceMechanicalSignature = data[0].PathView;

                signatureObject.FileExt = data[0].Extension.Replace(".", "");
                signatureObject.FileName = data[0].Name;
                signatureObject.DataFile = data[0].Base64Data;
                signatureObject.SignatureType = "TYFR,FIME";
                signatureObject.SignatureName = "Firma mecanica";
                lstSignatures.Add(signatureObject);
                if (editFormUser && !stateSignatureOperation)
                {
                    signatureObject.UserSignatureId = SourceMechanicalSignatureId;

                    await UpdateSignature(signatureObject, true, false);
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else if (editFormUser && stateSignatureOperation)
                {
                    await UpdateSignature(signatureObject, true, true);
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            else
            {
                SourceMechanicalSignature = string.Empty;
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task HandleFileFRB(List<FileInfoData> data)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            UsersSignatureDtoRequest signatureObject = new UsersSignatureDtoRequest();
            if (data != null && data.Count > 0 && !string.IsNullOrEmpty(data[0].PathView))
            {
                stateSignatureOperation = string.IsNullOrEmpty(SourceSignatureHeading);

                SourceSignatureHeading = data[0].PathView;
                signatureObject.FileExt = data[0].Extension.Replace(".", "");
                signatureObject.FileName = data[0].Name;
                signatureObject.DataFile = data[0].Base64Data;
                signatureObject.SignatureType = "TYFR,FIRU";
                signatureObject.SignatureName = "Firma rubrica";
                lstSignatures.Add(signatureObject);
                if (editFormUser && !stateSignatureOperation)
                {
                    signatureObject.UserSignatureId = SourceSignatureHeadingId;
                    stateSignatureOperation = false;
                    await UpdateSignature(signatureObject, true, false);
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else if (editFormUser && stateSignatureOperation)
                {
                    stateSignatureOperation = true;
                    await UpdateSignature(signatureObject, true, true);
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            else
            {
                SourceSignatureHeading = string.Empty;
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task HandleFileFDP12(List<FileInfoData> data)
        {
            UsersSignatureDtoRequest signatureObject = new UsersSignatureDtoRequest();
            if (data != null && data.Count > 0 && !string.IsNullOrEmpty(data[0].PathView))
            {
                SourceDigitalSignature = data[0].PathView;
                signatureObject.FileExt = data[0].Extension.Replace(".", "");
                signatureObject.FileName = data[0].Name;
                signatureObject.DataFile = data[0].Base64Data;
                signatureObject.SignatureType = "TYFR,DIGI";
                signatureObject.SignatureName = "Firma digital";
                lstSignatures.Add(signatureObject);
            }
            else
            {
                SourceDigitalSignature = string.Empty;
            }
        }

        #endregion Signatures Methods

        #region Additional Signatures Methods

        private async Task MechanicalSignatureAdditional1(List<FileInfoData> data)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            UsersSignatureDtoRequest signatureObject = new UsersSignatureDtoRequest();
            if (data != null && data.Count > 0 && !string.IsNullOrEmpty(data[0].PathView))
            {
                stateSignatureOperation = string.IsNullOrEmpty(SourceMechanicalSignatureAdditional1);
                SourceMechanicalSignatureAdditional1 = data[0].PathView;

                signatureObject.FileExt = data[0].Extension.Replace(".", "");
                signatureObject.FileName = data[0].Name;
                signatureObject.DataFile = data[0].Base64Data;
                signatureObject.SignatureType = "TYFR,FIME";
                signatureObject.SignatureName = "Firma mecanica Adicional 1";
                lstSignatures.Add(signatureObject);
                if (editFormUser && !stateSignatureOperation)
                {
                    signatureObject.UserSignatureId = SourceMechanicalSignatureAdditional1Id;
                    await UpdateSignature(signatureObject, true, false);
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else if (editFormUser && stateSignatureOperation)
                {
                    signatureObject.UserSignatureId = SourceMechanicalSignatureAdditional1Id;
                    await UpdateSignature(signatureObject, true, true);
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            else
            {
                SourceMechanicalSignatureAdditional1 = string.Empty;
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task MechanicalSignatureAdditional2(List<FileInfoData> data)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            UsersSignatureDtoRequest signatureObject = new UsersSignatureDtoRequest();
            if (data != null && data.Count > 0 && !string.IsNullOrEmpty(data[0].PathView))
            {
                stateSignatureOperation = string.IsNullOrEmpty(SourceMechanicalSignatureAdditional2);

                SourceMechanicalSignatureAdditional2 = data[0].PathView;
                signatureObject.FileExt = data[0].Extension.Replace(".", "");
                signatureObject.FileName = data[0].Name;
                signatureObject.DataFile = data[0].Base64Data;
                signatureObject.SignatureType = "TYFR,FIME";
                signatureObject.SignatureName = "Firma mecanica Adicional 2";
                lstSignatures.Add(signatureObject);
                if (editFormUser && !stateSignatureOperation)
                {
                    signatureObject.UserSignatureId = SourceMechanicalSignatureAdditional2Id;
                    await UpdateSignature(signatureObject, true, false);
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else if (editFormUser && stateSignatureOperation)
                {
                    signatureObject.UserSignatureId = SourceMechanicalSignatureAdditional2Id;
                    await UpdateSignature(signatureObject, true, true);
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            else
            {
                SourceMechanicalSignatureAdditional2 = string.Empty;
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task MechanicalSignatureAdditional3(List<FileInfoData> data)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            UsersSignatureDtoRequest signatureObject = new UsersSignatureDtoRequest();
            if (data != null && data.Count > 0 && !string.IsNullOrEmpty(data[0].PathView))
            {
                stateSignatureOperation = string.IsNullOrEmpty(SourceMechanicalSignatureAdditional3);
                SourceMechanicalSignatureAdditional3 = data[0].PathView;
                signatureObject.FileExt = data[0].Extension.Replace(".", "");
                signatureObject.FileName = data[0].Name;
                signatureObject.DataFile = data[0].Base64Data;
                signatureObject.SignatureType = "TYFR,FIME";
                signatureObject.SignatureName = "Firma mecanica Adicional 3";
                lstSignatures.Add(signatureObject);
                if (editFormUser && !stateSignatureOperation)
                {
                    signatureObject.UserSignatureId = SourceMechanicalSignatureAdditional2Id;
                    await UpdateSignature(signatureObject, true, false);
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else if (editFormUser && stateSignatureOperation)
                {
                    signatureObject.UserSignatureId = SourceMechanicalSignatureAdditional2Id;
                    await UpdateSignature(signatureObject, true, true);
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            else
            {
                SourceMechanicalSignatureAdditional3 = string.Empty;
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task UpdateSignature(UsersSignatureDtoRequest record, bool modal, bool create)
        {
            try
            {
                if (modal)
                {
                    notificationModal.UpdateModal(ModalType.Warning, Translation["UpdateSignatureQuestion"], true, Translation["Accept"], Translation["Cancel"], modalOrigin: "UpdateSignature");
                    recordToUpdate = record;
                }
                else if (!create)
                {
                    var responseApi = await HttpClient.PostAsJsonAsync("security/Signature/UpdateSignature", recordToUpdate);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<UsersSignatureDtoRequest>>();
                    if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                    {
                        notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"], "");
                        await GetSignaturesByUserId(userId);
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"], "");
                    }
                }
                else
                {
                    var siggnatureToCreate = new SignatureCreateDtoRequest()
                    {
                        UserId = userId,
                        NewSignatures = new()
                    };

                    siggnatureToCreate.NewSignatures.Add(
                        new()
                        {
                            DataFile = record.DataFile,
                            FileExt = record.FileExt,
                            FileName = record.FileName,
                            SignatureDescription = record.SignatureDescription,
                            SignatureFunction = record.SignatureFunction,
                            SignatureName = record.SignatureName,
                            SignatureType = record.SignatureType,
                        }

                        );
                    var responseApi = await HttpClient.PostAsJsonAsync("security/Signature/CreateUserSignatures", siggnatureToCreate);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<UsersSignatureDtoRequest>>>();
                    if (deserializeResponse.Succeeded)
                    {
                        notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"], "");
                        await GetSignaturesByUserId(userId);
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"], "");
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"], "");
            }
        }

        #endregion Additional Signatures Methods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleMethods

        #region OthersMethods

        #region Get Data Methods

        private async Task GetUsersData()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                vUserFilterDtoRequest = new()
                {
                    FirstName = userNameFilter,
                    LastName = userLastNameFilter,
                    Username = userLoginFilter
                };
                var responseApi = await HttpClient.PostAsJsonAsync(UriFilterVUser, vUserFilterDtoRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<VUserDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null && deserializeResponse.Data.Data.Any())
                {
                    userListData = deserializeResponse.Data.Data;
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponetPost.ResetPagination(paginationInfo);
                }
                else
                {
                    userListData = new();
                    paginationInfo = new();
                    paginationComponetPost.ResetPagination(paginationInfo);
                    /*                 notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);*/
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
            }

            StateHasChanged();

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task GetContractType()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "TDC");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    lstContracTypes = deserializeResponse.Data;
                }
                else { lstContracTypes = new(); }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
            }
        }

        private async Task GetChargueType()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "CAR");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    lstCharguesTypes = deserializeResponse.Data;
                }
                else { lstCharguesTypes = new(); }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
            }
        }

        private async Task GetDocumentPersonType()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "TDIN");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    lstDocumentType = deserializeResponse.Data;
                }
                else { lstDocumentType = new(); }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
            }
        }

        public async Task GetAdministrativeUnits()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");
                HttpClient?.DefaultRequestHeaders.Add("documentalVersionsId", documentalVersionId.ToString());
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeUnitsDtoResponse>>>("paramstrd/AdministrativeUnit/ByAdministrativeUnits");
                HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    lstAdministrativeUnit = deserializeResponse.Data;
                }
                else
                {
                    lstAdministrativeUnit = new();
                    notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true, Translation["Accept"], "");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener Unidades Administrativas: {ex.Message}");
            }
        }

        public async Task GetProducOffice(int id)
        {
            try
            {
                administrativeUnitId = id;
                HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                HttpClient?.DefaultRequestHeaders.Add("AdministrativeUnitId", id.ToString());
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<ProductionOfficesDtoResponse>>>("paramstrd/ProductionOffice/ByProductionOffices");
                HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    lstProductionOffices = deserializeResponse.Data;
                    isEnableProOfficeDrop = true;
                }
                else
                {
                    isEnableProOfficeDrop = false;
                    lstProductionOffices = new();
                    notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true, Translation["Accept"], "");
                }

                EnableSaveButton();
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true, Translation["Accept"], "");
            }
        }

        private async Task GetBranchsOffices()
        {
            try
            {
                BranchOfficeFilterDtoRequest? FilterDtoRequest = new();
                var responseApi = await HttpClient.PostAsJsonAsync("params/BranchOffice/ByFilterTotal", FilterDtoRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<BranchOfficesDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    lstBranchOffices = deserializeResponse.Data;
                }
                else
                {
                    lstBranchOffices = new List<BranchOfficesDtoResponse>();
                    notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true, Translation["Accept"], "");
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores en caso de fallo al obtener las sucursales.
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true, Translation["Accept"], "");
            }
        }

        private async Task GetProfileByUserId(int userId)
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("ProUserid");
                HttpClient?.DefaultRequestHeaders.Add("ProUserid", userId.ToString());
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<ProfilesDtoResponse>>>("permission/Profile/ByFilterUserId");
                HttpClient?.DefaultRequestHeaders.Remove("ProUserid");
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    lstProfilesByUserId = deserializeResponse.Data;
                }
                else
                {
                    lstProfilesByUserId = new();
                    notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true, Translation["Accept"], "");
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true, Translation["Accept"], "");
            }
        }

        private async Task LoadProfileDatabyUser(int userId, string fileId)
        {
            try
            {
                await GetSignaturesByUserId(userId);
                if (!string.IsNullOrEmpty(fileId))
                {
                    await GetDefaultProfilePicture(fileId);
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true, Translation["Accept"], "");
            }
        }

        private async Task GetDefaultProfilePicture(string fileId)
        {
            try
            {
                fileId = string.IsNullOrEmpty(fileId) ? "6791" : fileId;

                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", fileId);
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    var pictureBanner = deserializeResponse.Data;

                    bannerFileInfoData = new();
                    bannerFileInfoData.Add(new FileInfoData { Name = pictureBanner!.FileName, Extension = pictureBanner.FileExt, Base64Data = pictureBanner.DataFile });
                    StateHasChanged();
                    bannerFile = new() { FileExt = pictureBanner.FileExt.Replace(".", ""), DataFile = pictureBanner.DataFile, FileName = pictureBanner.FileName };
                    await bannerDADComponent.NotifyFileListChanged();

                    /*         string base64String = Convert.ToBase64String(deserializeResponse.Data.DataFile);
                             profilePictureSrc = $"data:image/{deserializeResponse.Data.FileExt.ToLowerInvariant()};base64,{base64String}";
                             defaultProfilePicture = profilePictureSrc;*/
                }
                else
                {
                    var pictureBanner = new FileDtoResponse();
                    bannerFileInfoData = new();
                    bannerFileInfoData.Add(new FileInfoData { Name = pictureBanner!.FileName, Extension = pictureBanner.FileExt, Base64Data = pictureBanner.DataFile });
                    StateHasChanged();
                    bannerFile = new() { FileExt = pictureBanner.FileExt.Replace(".", ""), DataFile = pictureBanner.DataFile, FileName = pictureBanner.FileName };
                    await bannerDADComponent.NotifyFileListChanged();

                    notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true, Translation["Accept"], "");
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true, Translation["Accept"], "");
            }

            StateHasChanged();
        }

        private async Task GetSignaturesByUserId(int userId, bool loadSpiner = false)
        {
            try
            {
                if (loadSpiner)
                {
                    SpinnerLoaderService.ShowSpinnerLoader(Js);
                }
                UserSignatureFilterDtoRequest userSignatureFilter = new();
                userSignatureFilter.UserId = userId;
                var responseApi = await HttpClient.PostAsJsonAsync("security/Signature/ByFilterSignatures", userSignatureFilter);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<UsersSignatureDtoRequest>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    usersSignatureList = deserializeResponse.Data;
                    SourceMechanicalSignature = OrderSignatures(usersSignatureList, "TYFR,FIME", "Firma mecanica", ref SourceMechanicalSignatureId);
                    SourceSignatureHeading = OrderSignatures(usersSignatureList, "TYFR,FIRU", "Firma rubrica", ref SourceSignatureHeadingId);
                    SourceMechanicalSignatureAdditional1 = OrderSignatures(usersSignatureList, "TYFR,FIME", "Firma mecanica Adicional 1", ref SourceMechanicalSignatureAdditional1Id);
                    SourceMechanicalSignatureAdditional2 = OrderSignatures(usersSignatureList, "TYFR,FIME", "Firma mecanica Adicional 2", ref SourceMechanicalSignatureAdditional2Id);
                    SourceMechanicalSignatureAdditional3 = OrderSignatures(usersSignatureList, "TYFR,FIME", "Firma mecanica Adicional 3", ref SourceMechanicalSignatureAdditional3Id);
                }
                else
                {
                    usersSignatureList = new();
                    /*                  notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);*/
                }
                StateHasChanged();
                if (loadSpiner)
                {
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
            }
        }

        private string OrderSignatures(List<UsersSignatureDtoRequest> signatures, string signatureType, string signatureName, ref int userSignatoryId)
        {
            // Validar si la lista de firmas está vacía
            if (signatures == null || signatures.Count == 0)
            {
                return string.Empty;
            }

            // Buscar el objeto en la lista que coincida con el tipo y nombre de firma especificados
            var signature = signatures.FirstOrDefault(s => s.SignatureType.Equals(signatureType) && s.SignatureName.TrimEnd().Equals(signatureName));

            if (signature == null)
            {
                return string.Empty;
            }

            userSignatoryId = (int)signature.UserSignatureId;

            // Comprobar si se encontró un objeto que coincida
            var src = signature != null ?
                $"data:image/{signature.FileExt.ToLowerInvariant()};base64,{Convert.ToBase64String(signature.DataFile)}" :
                string.Empty;

            return src;
        }

        #endregion Get Data Methods

        #region General Methods

        private void EditPermission(CreatePermissionDtoRequest record)
        {
            modalUserPermission.UpdatePermissionData(record);
            modalUserPermission.UpdateModalStatus(true, editFormUser);
        }

        private void DeletePermission(CreatePermissionDtoRequest record)
        {
            switch (editFormUser)
            {
                case true:
                    // Aquí puedes poner el código que se ejecutará cuando editFormUser sea true

                    //lstUserPermissionRequest.Remove()
                    break;

                case false:
                    lstUserPermissionSelected.Remove(record);
                    // Aquí puedes poner el código que se ejecutará cuando editFormUser sea false
                    break;
            }
        }

        public string GetUploadMessage()
        {
            string extensions = string.Join(" " + Translation["Or"] + " ", AllowedExtensions).Replace(".", "").ToUpper();
            return string.Format(Translation["AllowedFormatFiles"], extensions, FileSize);
        }

        private async Task CleanFilter()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            userNameFilter = string.Empty;
            userLoginFilter = string.Empty;
            userLastNameFilter = string.Empty;
            vUserFilterDtoRequest = new();
            await GetUsersData();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task SearchButtom()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                if (!string.IsNullOrEmpty(userNameFilter) || !string.IsNullOrEmpty(userLastNameFilter) || !string.IsNullOrEmpty(userLoginFilter))
                {
                    await GetUsersData();
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Information, Translation["AtLeastOneMessage"], true, Translation["Accept"], "");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task AddNewProfiles()
        {
            if (companyId == 0)
            {
                companyId = 17;
            }
            if (editFormUser)
            {
                await modalUserProfiles.UpdateModalStatusAsync(true);
            }
            else
            {
                await modalUserProfiles.UpdateModalStatusAsync(true);
            }
        }

        private async Task AddNewPermission()
        {
            modalUserPermission.UpdateModalStatus(true, editFormUser, existingFunctionalities);
        }

        private async Task HandleUserProfilePicture(List<FileInfoData> data)
        {
            if (data != null && data.Count > 0 && ( !string.IsNullOrEmpty(data[0].PathView) ))
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                bannerFileInfoData = new();

                _createUsersDtoRequest.FileExt = data[0].Extension.Replace(".", "");
                _createUsersDtoRequest.FileName = data[0].Name;
                _createUsersDtoRequest.DataFile = data[0].Base64Data;

                var pictureBanner = data[0];
                bannerFile = new FileCompanyDtoRequest();
                bannerFile = new() { FileExt = pictureBanner.Extension.Replace(".", ""), DataFile = pictureBanner.Base64Data!, FileName = pictureBanner.Name };
                bannerFileInfoData.Add(new FileInfoData { Name = pictureBanner!.Name, Extension = pictureBanner.Extension, Base64Data = pictureBanner.Base64Data });
                //StateHasChanged();

                if (editFormUser)
                {
                    var newPicture = new UserProfilePictureDtoRequest()
                    {
                        DataFile = _createUsersDtoRequest.DataFile,
                        FileExt = _createUsersDtoRequest.FileExt,
                        FileName = _createUsersDtoRequest.FileName,
                        UserId = userId
                    };

                    var responseApi = await HttpClient.PostAsJsonAsync("security/User/UpdateProfilePicture", newPicture);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<VUserDtoResponse>>();
                    if (deserializeResponse.Succeeded)
                    {
                        notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"], "");
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"], "");
                    }
                }

                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
        }

        private void ResetSignature()
        {
            lstSignaturesAMC1 = new();
            lstSignaturesAMC2 = new();
            lstSignaturesAMC3 = new();
            lstSignaturesMC = new();
            lstSignaturesRB = new();
            lstSignaturesDP12 = new();
            SourceMechanicalSignature = string.Empty;
            SourceSignatureHeading = string.Empty;
            SourceDigitalSignature = string.Empty;
            SourceMechanicalSignatureAdditional1 = string.Empty;
            SourceMechanicalSignatureAdditional2 = string.Empty;
            SourceMechanicalSignatureAdditional3 = string.Empty;
        }

        #endregion General Methods

        #region ModalMethods

        public void checkAge()
        {
            if (birthDate != null)
            {
                int age = DateTime.Today.Year - birthDate.Value.Year;

                var ageToCompare = birthDate.Value.AddYears(age);
                if (DateTime.Today < ageToCompare)
                {
                    age--;
                }

                if (age < 15)
                {
                    birthDate = null;
                    notificationModal.UpdateModal(ModalType.Information, Translation["UnderAgeError"], true, Translation["Accept"], "");
                }
            }
            StateHasChanged();
        }

        public void updateMinValue()
        {
            if (starContractDate != null)
            {
                minValueTo = ( (DateTime)starContractDate ).AddDays(1);
            }

            EnableSaveButton();
            StateHasChanged();
        }

        public void updateMaxValue()
        {
            if (endContractDate != null)
            {
                maxValueTo = (DateTime)endContractDate;
            }

            EnableSaveButton();
            StateHasChanged();
        }

        private async Task TabChangedHandler(int newIndex)
        {
            currentTab = newIndex;
        }

        private async Task CloseCreateView()
        {
            editFormUser = false;
            createFormUser = true;
            Panel1Class = "";
            Panel2Class = "d-none";
            lstPermissionByUserId = new();
            lstProfilesByUserId = new();
            administrativeUnitId = 0;

            lstUserPermissionSelected = new();
            lstProfilesByUserId.Clear();
            companyId = 0;
            starContractDate = null;
            endContractDate = null;
            birthDate = null;
            currentTab = 0;
            await GetUsersData();
            ResetSignature();
            StateHasChanged();
        }

        private async Task OpenCreateView()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            await ResetForm();
            bannerFileInfoData = new();
            editFormUser = false;
            idUserSelected = string.Empty;
            _createUsersDtoRequest = new CreateUsersDtoRequest();
            Panel1Class = "d-none";
            Panel2Class = "";
            createFormUser = true;
            userId = 0;
            profileIdSelected = 0;
            EnableSaveButton();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task OpenEditView(VUserDtoResponse record)
        {
            createFormUser = false;
            await ResetForm();

            SpinnerLoaderService.ShowSpinnerLoader(Js);
            bannerFileInfoData = new();

            editFormUser = true;
            userId = record.UserId;
            idUserSelected = record.UserId.ToString();
            await LoadProfileDatabyUser(record.UserId, record.PictureFileId.ToString());
            profileIdSelected = record.ProfileId;
            _createUsersDtoRequest.IdentificationType = record.IdentificationTypeCode;
            //IdentificationText = lstDocumentType.Where(s => s.code.Equals(record.IdentificationType)).Select(x => x.value).First();
            _createUsersDtoRequest.Identification = record.Identification;
            _createUsersDtoRequest.UserName = record.UserName;
            _createUsersDtoRequest.FirstName = record.FirstName;
            _createUsersDtoRequest.MiddleName = record.MiddleName;
            _createUsersDtoRequest.LastName = record.LastName;
            _createUsersDtoRequest.Email = record.Email;
            _createUsersDtoRequest.PhoneNumber = record.PhoneNumber;
            _createUsersDtoRequest.CellPhoneNumber = record.CellPhoneNumber;
            _createUsersDtoRequest.ProductionOfficeId = record.ProfileId;
            _createUsersDtoRequest.OriginAd = record.OriginAd;
            activeState = record.UserActiveState;
            if (record.AdministrativeUnitId != 0)
            {
                await GetProducOffice(record.AdministrativeUnitId);
            }
            if (record.ProductionOfficeId.HasValue)
            {
                _createUsersDtoRequest.ProductionOfficeId = (int)record.ProductionOfficeId;
            }
            if (record.BranchOfficeId.HasValue)
            {
                _createUsersDtoRequest.BranchOfficeId = (int)record.BranchOfficeId;
            }
            _createUsersDtoRequest.ChargeCode = record.ChargeCode;
            _createUsersDtoRequest.ContractType = record.ContractTypeCode;
            _createUsersDtoRequest.ContractNumber = record.ContractNumber;
            starContractDate = record.ContractStartDate;
            endContractDate = record.ContractFinishDate;

            updateMinValue();
            updateMaxValue();
            birthDate = record.BirthDate;

            Panel1Class = "d-none";
            Panel2Class = "";
            EnableSaveButton();
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #region DeleteMethods

        private void ShowModalDelete(VUserDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, @Translation["Yes"], @Translation["No"], modalOrigin: "DeleteModal");
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            try
            {
                if (!string.IsNullOrEmpty(args.ModalOrigin))
                {
                    switch (args.ModalOrigin)
                    {
                        case "DeleteModal":
                            await HandleDeleteModal(args);
                            break;

                        case "UpdateSignature":
                            if (args.IsCancelled)
                            {
                                await GetSignaturesByUserId(userId, true);
                            }
                            else
                            {
                                await UpdateSignature(recordToUpdate, false, stateSignatureOperation);
                            }

                            break;

                        default:
                            notificationModal.UpdateModal(ModalType.Error, "ModalOrigin no reconocido", true);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
        }

        private async Task HandleDeleteModal(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                DeleteGeneralDtoRequest request = new();
                request.Id = recordToDelete.UserId;
                request.User = "Front"; // Cambiar por la variable de sesión del usuario
                var responseApi = await HttpClient.PostAsJsonAsync("security/User/DeleteUser", request);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                if (deserializeResponse.Succeeded)
                {
                    await GetUsersData();
                    notificationModal.UpdateModal(ModalType.Success, Translation["DeleteSuccessfulMessage"], true, Translation["Accept"], "");
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true, Translation["Accept"], "");
                }
            }
        }

        private async Task HandleDeleteModalProfile(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                DeleteGeneralDtoRequest request = new();
                request.Id = recordToDeleteProfile.ProfileUserId;
                request.User = "Front"; // Cambiar por la variable de sesión del usuario
                var responseApi = await HttpClient.PostAsJsonAsync("permission/Profile/DeleteAssingProfile", request);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                if (deserializeResponse.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["UserProfileRemovedSuccessfullyMessage"], true, Translation["Accept"], "");
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["RemovingProfileErrorMessage"], true, Translation["Accept"], "");
                }
            }
        }

        private async Task HandleDeleteModalPermission(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                DeleteGeneralDtoRequest request = new();
                request.Id = (int)recordToDeletePermission.PermissionId;
                request.User = "Front"; // Cambiar por la variable de sesión del usuario
                var responseApi = await HttpClient.PostAsJsonAsync("permission/Permission/DeletePermission", request);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                if (deserializeResponse.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["UserProfileRemovedSuccessfullyMessage"], true, Translation["Accept"], "");
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["RemovingProfileErrorMessage"], true, Translation["Accept"], "");
                }
            }
        }

        #endregion DeleteMethods

        private void HandlePaginationGrid(List<VUserDtoResponse> newDataList)
        {
            userListData = newDataList;
        }

        private async Task getProfileList()
        {
            var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<ProfileDtoResponse>>>("security/Profile/ByFilter");

            if (deserializeResponse.Succeeded)
            {
                porfileDropDownList = deserializeResponse.Data;
            }
            else
            {
                porfileDropDownList = new();
            }
        }

        private void ShowChangePaswwordModal(bool UseSecondPassword = false)
        {
            changePasswordUserModal.UpdateModalStatus(true);
            changePasswordUserModal.UpdateDataToChange(userId, UseSecondPassword);
        }

        private async Task ResetForm()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            if (!editFormUser)
            {
                userId = 0;
                profileIdSelected = 0;
                _createUsersDtoRequest.Identification = string.Empty;

                bannerFileInfoData = new();

                lstSignaturesMC = new();
                lstSignaturesRB = new();
                lstSignaturesDP12 = new();
                lstSignaturesAMC1 = new();
                lstSignaturesAMC2 = new();
                lstSignaturesAMC3 = new();

                SourceMechanicalSignature = string.Empty;
                SourceSignatureHeading = string.Empty;
                SourceDigitalSignature = string.Empty;

                SourceMechanicalSignatureAdditional1 = string.Empty;
                SourceMechanicalSignatureAdditional2 = string.Empty;
                SourceMechanicalSignatureAdditional3 = string.Empty;

                StateHasChanged();
            }
            _createUsersDtoRequest.Password = string.Empty;
            _createUsersDtoRequest.SeccondPassword = string.Empty;
            _createUsersDtoRequest.UserName = string.Empty;
            _createUsersDtoRequest.FirstName = string.Empty;
            _createUsersDtoRequest.MiddleName = string.Empty;
            _createUsersDtoRequest.LastName = string.Empty;
            birthDate = null;
            _createUsersDtoRequest.Email = string.Empty;
            _createUsersDtoRequest.CellPhoneNumber = string.Empty;
            _createUsersDtoRequest.PhoneNumber = string.Empty;
            _createUsersDtoRequest.ChargeCode = string.Empty;
            _createUsersDtoRequest.ContractType = string.Empty;
            _createUsersDtoRequest.ContractNumber = string.Empty;
            starContractDate = null;
            endContractDate = null;
            _createUsersDtoRequest.BranchOfficeId = 0;
            _createUsersDtoRequest.ProductionOfficeId = 0;
            administrativeUnitId = 0;
            isEnableProOfficeDrop = false;
            activeState = true;
            await GetDocumentPersonType();
            await GetContractType();
            await GetAdministrativeUnits();
            await GetBranchsOffices();
            await getProfileList();
            await GetChargueType();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion ModalMethods

        #region EnableSaveButton

        public void EnableSaveButton()
        {
            var containsCapitalLetterFirst = Regex.IsMatch(_createUsersDtoRequest.Password ?? string.Empty, "(?=.*[A-Z])");
            var containsSpecialCharacterFirst = Regex.IsMatch(_createUsersDtoRequest.Password ?? string.Empty, "(?=.*[\\W_])");
            var ContainsNumberFirst = Regex.IsMatch(_createUsersDtoRequest.Password ?? string.Empty, "(?=.*\\d)");
            var containsAccptableLegnthFirst = Regex.IsMatch(_createUsersDtoRequest.Password ?? string.Empty, "^.{8,20}$");

            OneCapitalLetterPassword = containsCapitalLetterFirst ? "green" : "red";
            OneNumberPassword = ContainsNumberFirst ? "green" : "red";
            OneSpecialCharacterPassword = containsSpecialCharacterFirst ? "green" : "red";
            MinimumCharactersPassword = containsAccptableLegnthFirst ? "green" : "red";

            StateHasChanged();

            var containsCapitalLetterSecond = Regex.IsMatch(_createUsersDtoRequest.SeccondPassword ?? string.Empty, "(?=.*[A-Z])");
            var containsSpecialCharacterSecond = Regex.IsMatch(_createUsersDtoRequest.SeccondPassword ?? string.Empty, "(?=.*[\\W_])");
            var ContainsNumberSecond = Regex.IsMatch(_createUsersDtoRequest.SeccondPassword ?? string.Empty, "(?=.*\\d)");
            var containsAccptableLegnthSecond = Regex.IsMatch(_createUsersDtoRequest.SeccondPassword ?? string.Empty, "^.{8,20}$");

            OneCapitalLetterSecondPassword = containsCapitalLetterSecond ? "green" : "red";
            OneNumberSecondPassword = ContainsNumberSecond ? "green" : "red";
            OneSpecialCharacterSecondPassword = containsSpecialCharacterSecond ? "green" : "red";
            MinimumCharactersSecondPassword = containsAccptableLegnthSecond ? "green" : "red";

            StateHasChanged();
            _createUsersDtoRequest.FirstName = string.IsNullOrEmpty(_createUsersDtoRequest.FirstName) ? string.Empty : Regex.Replace(_createUsersDtoRequest.FirstName, patternNumeric, string.Empty);
            _createUsersDtoRequest.MiddleName = string.IsNullOrEmpty(_createUsersDtoRequest.MiddleName) ? string.Empty : Regex.Replace(_createUsersDtoRequest.MiddleName, patternNumeric, string.Empty);
            _createUsersDtoRequest.LastName = string.IsNullOrEmpty(_createUsersDtoRequest.LastName) ? string.Empty : Regex.Replace(_createUsersDtoRequest.LastName, patternNumeric, string.Empty);
            bool hasRequiredFields = !string.IsNullOrEmpty(_createUsersDtoRequest.IdentificationType) &&
                           !string.IsNullOrEmpty(_createUsersDtoRequest.Identification) &&
                           profileIdSelected > 0 &&
                           !string.IsNullOrEmpty(_createUsersDtoRequest.UserName) &&
                           !string.IsNullOrEmpty(_createUsersDtoRequest.FirstName) &&
                           !string.IsNullOrEmpty(_createUsersDtoRequest.LastName) &&
                           birthDate.HasValue &&
                           _createUsersDtoRequest.BranchOfficeId > 0 &&
                           !string.IsNullOrEmpty(_createUsersDtoRequest.Email) &&
                           !string.IsNullOrEmpty(_createUsersDtoRequest.ChargeCode) &&
                           !string.IsNullOrEmpty(_createUsersDtoRequest.ContractType) &&
                           !string.IsNullOrEmpty(_createUsersDtoRequest.ContractNumber) &&
                           starContractDate.HasValue &&
                           endContractDate.HasValue &&
                           _createUsersDtoRequest.ProductionOfficeId > 0;

            if (!editFormUser)
            {
                hasRequiredFields = hasRequiredFields && containsCapitalLetterSecond && ContainsNumberSecond && containsSpecialCharacterSecond && containsAccptableLegnthSecond;

                if (!_createUsersDtoRequest.OriginAd)
                {
                    hasRequiredFields = hasRequiredFields && containsCapitalLetterFirst && ContainsNumberFirst && containsSpecialCharacterFirst && containsAccptableLegnthFirst;
                }

                hasRequiredFields = hasRequiredFields &&
                                    !string.IsNullOrEmpty(_createUsersDtoRequest.Password) &&
                                    !string.IsNullOrEmpty(_createUsersDtoRequest.SeccondPassword);
            }

            saveIsDisable = !hasRequiredFields;

            StateHasChanged();
        }

        #endregion EnableSaveButton

        public void replaceNumbers()
        {
            userNameFilter = string.IsNullOrEmpty(userNameFilter) ? string.Empty : Regex.Replace(userNameFilter, patternNumeric, string.Empty);
            userLastNameFilter = string.IsNullOrEmpty(userLastNameFilter) ? string.Empty : Regex.Replace(userLastNameFilter, patternNumeric, string.Empty);

            StateHasChanged();
        }

        public async Task AdData()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("AdUsername");
                HttpClient?.DefaultRequestHeaders.Remove("updateData");
                HttpClient?.DefaultRequestHeaders.Add("AdUsername", $"{_createUsersDtoRequest.UserName}");
                HttpClient?.DefaultRequestHeaders.Add("updateData", $"{true}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<CreateUsersDtoRequest>>("security/User/ByFilterFromAD");
                HttpClient?.DefaultRequestHeaders.Remove("AdUsername");
                HttpClient?.DefaultRequestHeaders.Remove("updateData");
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    var record = deserializeResponse.Data;

                    _createUsersDtoRequest.UserName = record.UserName;
                    _createUsersDtoRequest.FirstName = record.FirstName;

                    _createUsersDtoRequest.LastName = record.LastName;
                    _createUsersDtoRequest.Email = record.Email;
                    _createUsersDtoRequest.PhoneNumber = record.PhoneNumber;
                    _createUsersDtoRequest.CellPhoneNumber = record.CellPhoneNumber;

                    //SpinnerLoaderService.ShowSpinnerLoader(Js);
                    //await ResetForm();
                    //bannerFileInfoData = new();
                    //editFormUser = false;
                    //idUserSelected = string.Empty;
                    //_createUsersDtoRequest = new CreateUsersDtoRequest() { OriginAd = true };
                    //Panel1Class = "d-none";
                    //Panel2Class = "";
                    //createFormUser = true;

                    ////await OpenEditView(deserializeResponse.Data);

                    EnableSaveButton();
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Information, Translation[$"{deserializeResponse.Message}"], true, Translation["Accept"], "");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);

            StateHasChanged();
        }

        private async Task GetAllRecords()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                vUserFilterDtoRequest = new()
                {
                    FirstName = userNameFilter,
                    LastName = userLastNameFilter,
                    Username = userLoginFilter
                };
                var responseApi = await HttpClient.PostAsJsonAsync("generalviews/VUser/ByFilterList", vUserFilterDtoRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<VUserDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null && deserializeResponse.Data.Any())
                {
                    allRecords = deserializeResponse.Data;
                }
                else
                {
                    allRecords = new();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
            }

            StateHasChanged();

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        public async Task OnBeforeExcelExport(GridBeforeExcelExportEventArgs args)
        {
            if (ExportAllPages)
            {
                await GetAllRecords(); //aquí lo cambian por su método de consumir todos los registros

                if (allRecords.Any())
                {
                    args.Data = allRecords;
                }
            }
            else { args.Data = userListData; }

            args.IsCancelled = CancelExport;
        }

        #endregion OthersMethods

        #endregion Methods
    }
}