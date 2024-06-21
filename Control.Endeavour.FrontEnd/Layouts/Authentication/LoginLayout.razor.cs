using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using OpenAI_API.Models;
using System.Globalization;
using System.Net.Http.Json;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Layouts.Authentication
{
    public partial class LoginLayout
    {
        #region Modals
        private NotificationsComponentModal notificationModal = new();
        #endregion

        #region Models

        //Pictures
        private List<FileInfoData> logoFile = new();
        private List<FileInfoData> bannerFile = new();
        private List<FileInfoData> mobileBannerFile = new();

        //Company
        private CompanyDtoResponse company = new();

        #endregion Models

        #region Variables

        #region Variables(String)

        private string companyIcon = "";
        private string navbarCollapseCss => isNavbarCollapsed ? "collapse navbar-collapse" : "navbar-collapse";
        private string DropdownMenuCss => isDropdownOpen ? "dropdown-menu show" : "dropdown-menu";

        private string version { get; set; } = string.Empty;
        private string logoSrc { get; set; } = string.Empty;
        private string bannerSrc { get; set; } = string.Empty;
        private string mobileBannerSrc { get; set; } = string.Empty;

        #endregion

        #region Variables(Bool)

        private bool isNavbarCollapsed = true;
        private bool isDropdownOpen = false;
        private bool darkMode = false;

        #endregion

        #region Variables(List)

        private List<AppKeysDtoResponse> LstAppKeys = new();

        #endregion

        #region Inject 

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private IConfiguration? Configuration { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }
        [Inject]
        private HttpClient? HttpClient { get; set; }


        #endregion

        #endregion

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            var ThemeDark = await Js.InvokeAsync<bool>("checkTheme"); // Verificar Theme almacenado en el localStorage      
            version =  Configuration["version"].ToString();
            await GetCompanies();
        }

        #endregion

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        #endregion

        #region TogglesMethods
        void ToggleNavbar()
        {
            isNavbarCollapsed = !isNavbarCollapsed;
        }
        async Task ToggleDropdownAsync()
        {
            isDropdownOpen = !isDropdownOpen;
        }
        void ClosedDropdown()
        {
            isDropdownOpen = false;
        }
        async Task ToggleTheme()
        {
            darkMode = await Js.InvokeAsync<bool>("toggleTheme");
            changeImage(darkMode);
        }
        #endregion

        #region OthersMethods
        
        private void changeImage(bool darkMode)
        {
            if (darkMode)
            {
                #region Icons

                //companyIcon = "../img/menu/bpmWhite.svg";

                #endregion
            }
            else
            {
                #region Icons

                //companyIcon = "../img/menu/bpm.svg";

                #endregion
            }
        }


        #region GetPictures

        private async Task<FileDtoResponse?> GetPictures(int? id)
        {

            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", $"{id}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                if (deserializeResponse!.Succeeded)
                {

                    SpinnerLoaderService.HideSpinnerLoader(Js);
                    return deserializeResponse.Data!;
                }
                else
                {
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                    return null;
                }

            }
            catch
            {
                return new FileDtoResponse();
            }

        }

        #endregion GetPictures

        #region HandlePictures

        private void HandleMobileBannerPicture()
        {
            if (mobileBannerFile != null && mobileBannerFile.Count > 0)
            {
                var picture = mobileBannerFile[0];
                mobileBannerSrc = $"data:image/{picture.Extension!.Split('.')[0]};base64,{Convert.ToBase64String(picture.Base64Data!)}";
            }
        }

        private void HandleBannerPicture()
        {
            if (bannerFile != null && bannerFile.Count > 0)
            {
                var picture = bannerFile[0];
                bannerSrc = $"data:image/{picture.Extension!.Split('.')[0]};base64,{Convert.ToBase64String(picture.Base64Data!)}";
            }
        }

        private void HandleLogoPicture()
        {
            if (logoFile != null && logoFile.Count > 0)
            {
                var picture = logoFile[0];
                logoSrc = $"data:image/{picture.Extension!.Split('.')[0]};base64,{Convert.ToBase64String(picture.Base64Data!)}";
            }
        }

        #endregion HandlePictures

        #region GetCompany

        private async Task GetCompanies()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {


                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<CompanyDtoResponse>>("companies/Company/ByCompany");
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                {
                    company = deserializeResponse.Data;
                    
                    var logo = await GetPictures(company.LogoFileId);
                    logoFile.Add(new FileInfoData { Name = logo!.FileName, Extension = logo.FileExt, Base64Data = logo.DataFile });

                    var banner = await GetPictures(company.BannerFileId);
                    bannerFile.Add(new FileInfoData { Name = banner!.FileName, Extension = banner.FileExt, Base64Data = banner.DataFile });

                    var mobileBanner = await GetPictures(company.MobileBannerFileId);
                    mobileBannerFile.Add(new FileInfoData { Name = mobileBanner!.FileName, Extension = mobileBanner.FileExt, Base64Data = mobileBanner.DataFile });

                    HandleMobileBannerPicture();
                    HandleBannerPicture();
                    HandleLogoPicture();
                    StateHasChanged();
                }
                else
                {
                    company = new();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Translation["LoadErrorMessage"]}: {ex.Message}");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion GetCompany


        #endregion

        #region Terms&ConditionsActions

        private async Task<FileDtoResponse?> GetFile(string id)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", $"{id}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                if (deserializeResponse!.Succeeded)
                {
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                    return deserializeResponse.Data!;
                }
                else
                {
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                    return null;
                }
            }
            catch
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                return null;
            }
        }
        private async Task GetAppKeys(string keyName)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                AppKeysFilterDtoRequest appKeysFilter = new();
                appKeysFilter.FunctionName = "LayoutFunction";
                appKeysFilter.KeyName = keyName;
                var responseApi = await HttpClient.PostAsJsonAsync("params/AppKeys/ByFilter", appKeysFilter);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<AppKeysDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data.Any())
                {
                    LstAppKeys = deserializeResponse.Data;
                }
                else
                {
                    LstAppKeys = new();
                    notificationModal.UpdateModal(ModalType.Error, string.Format(Translation["KeyHasNoValues"], "LayoutFunction"), true);
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);

        }

        private async Task OpenPdfTermsCondition(string codeKeyLog)
        {
            string filename = string.Empty;
            LstAppKeys = new();
            switch (codeKeyLog)
            {
                case "Terms&Conditions":
                    await GetAppKeys("Terms&Conditions");
                    filename = "Terms&Conditions.pdf";
                    break;
                case "PrivacyPolicy":
                    await GetAppKeys("PrivacyPolicy");
                    filename = "PrivacyPolicy";
                    break;
                default:
                    notificationModal.UpdateModal(ModalType.Error, "ModalOrigin no reconocido", true);
                    break;
            }

            if(LstAppKeys != null && LstAppKeys.Any())
            {
                FileDtoResponse objFile = await GetFile(LstAppKeys[0].Value1);
                await Js.InvokeVoidAsync("openPdfFromBytes", objFile.DataFile);
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
                
            }
        }
        #endregion

        #endregion



    }
}
