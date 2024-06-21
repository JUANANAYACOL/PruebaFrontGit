using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Components.Components.User;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.UsersAdministration;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Profile.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Profile.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.User.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.User.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class UserProfilePage
    {
        #region Variables

        #region Inject

        [Inject]
        private EventAggregatorService? EventAggregator { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }

        #endregion Inject

        #region Components

        private DragAndDropImageComponent userProfilePicture { get; set; } = new();

        #endregion Components

        #region Modals

        private NotificationsComponentModal notificationModal { get; set; } = new();
        private ChangePasswordUserModal changePasswordUserModal { get; set; } = new();

        #endregion Modals

        #region Parameters

        [CascadingParameter]
        public UserDtoResponse UserDataToShow { get; set; }

        #endregion Parameters

        #region Models

        private FileCompanyDtoRequest bannerFile { get; set; } = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png" };

        private string SourceMechanicalSignature { get; set; } = string.Empty;

        private string SourceSignatureHeading { get; set; } = string.Empty;
        private string SourceMechanicalSignatureAdditional1 { get; set; } = string.Empty;
        private string SourceMechanicalSignatureAdditional2 { get; set; } = string.Empty;
        private string SourceMechanicalSignatureAdditional3 { get; set; } = string.Empty;

        private string fullName;
        private string charge;
        private string profile;
        private string administrativeUnit;
        private string productionOffice;
        private string name;
        private string lastName;
        private string email;
        private string celphone;

        #endregion Environments(String)

        #region Environments(Numeric)

        private int FileSize { get; set; } = 10;
        private int SourceMechanicalSignatureId = 0;
        private int SourceSignatureHeadingId = 0;
        private int SourceMechanicalSignatureAdditional1Id = 0;
        private int SourceMechanicalSignatureAdditional2Id = 0;
        private int SourceMechanicalSignatureAdditional3Id = 0;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool stateSignatureOperation { get; set; }
        private bool changePicture = false;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<FileInfoData> bannerFileInfoData { get; set; } = new();
        private List<UsersSignatureDtoRequest> lstSignatures { get; set; } = new();

        private List<UsersSignatureDtoRequest> usersSignatureList { get; set; } = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            await GetUserInfo();
            fullName = UserDataToShow.FullName;
            name = $"{UserDataToShow.FirstName} {UserDataToShow.MiddleName}";
            lastName = UserDataToShow.LastName;
            email = UserDataToShow.Email;
            celphone = UserDataToShow.CellPhoneNumber;
            charge = Translation["Position"] + ": " + UserDataToShow.ChargeName;
            await GetProfilePicture(UserDataToShow.PictureFileId.ToString());
            await GetProfile(UserDataToShow.ProfileId);
            await GetTRD(UserDataToShow.ProductionOfficeId);
            await GetSignaturesByUserId(UserDataToShow.UserId);
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private void ShowChangePaswwordModal(bool UseSecondPassword = false)
        {
            changePasswordUserModal.UpdateModalStatus(true);
            changePasswordUserModal.UpdateDataToChange(UserDataToShow.UserId, UseSecondPassword);
        }

        #endregion HandleMethods

        #region OthersMethods

        #region Signatures Methods

        private async Task GetSignaturesByUserId(int userId)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
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
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
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

        #endregion Signatures Methods

        private async Task GetUserInfo()
        {
            var response = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<UserDtoResponse>>("security/User/ByFilterToken");

            if (response.Succeeded)
            {
                UserDataToShow = response.Data;
                await GetProfilePicture(UserDataToShow.PictureFileId.ToString());
            }
            else
            {
                Console.WriteLine("No hay menus disponibles");
            }
        }

        #region HandleUserProfilePicture

        private async Task HandleUserProfilePicture(List<FileInfoData> data)
        {
            if (data != null && data.Count > 0 && (!string.IsNullOrEmpty(data[0].PathView)))
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                bannerFileInfoData = new();

                var pictureBanner = data[0];
                bannerFile = new FileCompanyDtoRequest();
                bannerFile = new() { FileExt = pictureBanner.Extension.Replace(".", ""), DataFile = pictureBanner.Base64Data!, FileName = pictureBanner.Name };
                bannerFileInfoData.Add(new FileInfoData { Name = pictureBanner!.Name, Extension = pictureBanner.Extension, Base64Data = pictureBanner.Base64Data });
                var newPicture = new UserProfilePictureDtoRequest()
                {
                    DataFile = data[0].Base64Data,
                    FileExt = data[0].Extension.Replace(".", ""),
                    FileName = data[0].Name,
                    UserId = UserDataToShow.UserId
                };

                var responseApi = await HttpClient.PostAsJsonAsync("security/User/UpdateProfilePicture", newPicture);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<VUserDtoResponse>>();
                if (deserializeResponse.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"], "");
                    changePicture = true;
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"], "");
                    changePicture = false;
                }

                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
        }

        #endregion HandleUserProfilePicture

        #region GetProfilePicture

        private async Task GetProfilePicture(string fileId)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                fileId = string.IsNullOrEmpty(fileId) ? "6791" : fileId;

                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", fileId);
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    var pictureBanner = deserializeResponse.Data;
                    bannerFileInfoData.Add(new FileInfoData { Name = pictureBanner!.FileName, Extension = pictureBanner.FileExt, Base64Data = pictureBanner.DataFile });
                    bannerFile = new() { FileExt = pictureBanner.FileExt.Replace(".", ""), DataFile = pictureBanner.DataFile, FileName = pictureBanner.FileName };
                    await userProfilePicture.NotifyFileListChanged();
                }
                else
                {
                    var pictureBanner = new FileDtoResponse();
                    bannerFileInfoData.Add(new FileInfoData { Name = pictureBanner!.FileName, Extension = pictureBanner.FileExt, Base64Data = pictureBanner.DataFile });
                    bannerFile = new() { FileExt = pictureBanner.FileExt.Replace(".", ""), DataFile = pictureBanner.DataFile, FileName = pictureBanner.FileName };
                    await userProfilePicture.NotifyFileListChanged();

                    notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true, Translation["Accept"], "");
                }
            }
            catch
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true, Translation["Accept"], "");
            }
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetProfilePicture

        #region GetProfile

        private async Task GetProfile(int id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("request");
                HttpClient?.DefaultRequestHeaders.Add("request", $"{id}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<ProfileDtoResponse>>("security/Profile/ByFilterId");
                HttpClient?.DefaultRequestHeaders.Remove("request");
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    profile = Translation["Profile"] + ": " + deserializeResponse.Data.Profile1;
                }
                else
                {
                    profile = "";
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true, Translation["Accept"], "");
            }

            StateHasChanged();
        }

        #endregion GetProfile

        #region GetTRD

        public async Task GetTRD(int id)
        {
            try
            {
                if (id > 0)
                {
                    SpinnerLoaderService.ShowSpinnerLoader(Js);

                    HttpClient?.DefaultRequestHeaders.Remove("productionOfficeId");
                    HttpClient?.DefaultRequestHeaders.Add("productionOfficeId", $"{id}");
                    var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<ProductionOfficesDtoResponse>>("paramstrd/ProductionOffice/ByFilterId");
                    HttpClient?.DefaultRequestHeaders.Remove("productionOfficeId");

                    if (deserializeResponse.Succeeded)
                    {
                        administrativeUnit = Translation["AdministrativeUnit"] + ": " + deserializeResponse.Data.AdministrativeUnitName;
                        productionOffice = Translation["ProductionOffice"] + ": " + deserializeResponse.Data.Name;
                    }
                    else
                    {
                        administrativeUnit = "";
                        productionOffice = "";
                    }
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["ErrorGettingRecords"]}", true, Translation["Accept"]);
            }
        }

        #endregion GetTRD

        #endregion OthersMethods

        #endregion Methods
    }
}