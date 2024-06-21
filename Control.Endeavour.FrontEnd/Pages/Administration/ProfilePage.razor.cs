using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.Profile;
using Control.Endeavour.FrontEnd.Components.Modals.Administration.Replacement;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Profile.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Profile.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Replacement.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VReplacement.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;
using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using System.Text.RegularExpressions;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using System.Runtime.CompilerServices;

namespace Control.Endeavour.FrontEnd.Pages.Administration
{
    public partial class ProfilePage
    {
        #region Variables

        #region Inject

        [Inject] private NavigationManager NavigationManager { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Components

        private InputModalComponent ProfileNameInput { get; set; } = new();

        private InputModalComponent ProfileCodeInput { get; set; } = new();

        private NotificationsComponentModal notificationModal { get; set; } = new();
        private NotificationsComponentModal notificationModalSucces { get; set; } = new();
        private NewPaginationComponent<ProfileDtoResponse, ProfileFilterDtoRequest> paginationComponent { get; set; } = new();

        #endregion Components

        #region Modals

        private ProfileModal profileModal { get; set; } = new();

        #endregion Modals

        #region Models

        public FunctionalityToJson? Permissions { get; set; } = new();
        private ProfileDtoResponse profileToUpdate { get; set; } = new();
        private DeleteGeneralDtoRequest deleteRequest { get; set; } = new();
        private PaginationInfo paginationInfo = new();
        private ProfileFilterDtoRequest profileFilterDtoRequest { get; set; } = new();

        #endregion Models

        #region Environments

        #region Environments(Bool)

        private bool ExportAllPages { get; set; }
        public bool CancelExport { get; set; }
        private bool dataChargue { get; set; } = false;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<ProfileDtoResponse> allRecords { get; set; }
        private List<ProfileDtoResponse> profileList { get; set; } = new();

        #endregion Environments(List & Dictionary)



        #region Environments(String)

        private string patternNumeric { get; set; } = @"\d";
        private string ProfileName { get; set; } = string.Empty;
        private string ProfileCode { get; set; } = string.Empty;
        private string UriFilterProfile { get; set; } = "security/Profile/ByFilterPagination";

        #endregion Environments(String)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            await GetPermissions();
            await GetProfiles();
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #region HandlePaginationGrid

        private void HandlePaginationGrid(List<ProfileDtoResponse> newDataList)
        {
            profileList = newDataList;
        }

        #endregion HandlePaginationGrid

        #endregion HandleMethods

        #region OthersMethods

        #region ShowModal

        private void ShowModal()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            profileModal.UpdateModalStatus(true);
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private void ShowModalEdit(ProfileDtoResponse profile)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            profileModal.UpdateModalStatus(true);
            profileModal.UpdateSelectedProfile(profile);
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task HandleProfileModalStatusChangedAsync(bool status)
        {
            profileModal.UpdateModalStatus(status);
            await GetProfiles();

            StateHasChanged();
        }

        #endregion ShowModal

        private async Task GetProfiles()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                profileFilterDtoRequest = new()
                {
                    Profile1 = ProfileName,
                    ProfileCode = ProfileCode
                };

                var responseApi = await HttpClient!.PostAsJsonAsync(UriFilterProfile, profileFilterDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<ProfileDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data!.Data != null)
                {
                    profileList = deserializeResponse.Data.Data ?? new();

                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                    dataChargue = true;
                }
                else
                {
                    profileList = new();
                    paginationInfo = new();
                    paginationComponent.ResetPagination(paginationInfo);
                    dataChargue = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los perfiles de usuario: {ex.Message}");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private void HandleRecordToDelete(ProfileDtoResponse args)
        {
            profileToUpdate = args;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, Translation["Yes"], Translation["No"]);
        }

        private async Task HandleModalNotiCloseAsync(ModalClosedEventArgs args)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            if (args.IsAccepted && notificationModal.Type == ModalType.Warning)
            {
                deleteRequest.Id = profileToUpdate.ProfileId;

                var responseApi = await HttpClient!.PostAsJsonAsync("security/Profile/DeleteProfile", deleteRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                if (deserializeResponse!.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["DeleteSuccessfulMessage"], true, Translation["Accept"]);
                    await GetProfiles();
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true, Translation["Accept"]);
                }
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true, Translation["Accept"]);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task OnClickReset()
        {
            ProfileName = string.Empty;
            ProfileCode = string.Empty;
            await GetProfiles();
            StateHasChanged();
        }

        public void replaceNumbers()
        {
            ProfileName = string.IsNullOrEmpty(ProfileName) ? string.Empty : Regex.Replace(ProfileName, patternNumeric, string.Empty);

            StateHasChanged();
        }

        #endregion OthersMethods

        #region GetPermissions

        private async Task GetPermissions()
        {
            try
            {
                string nameView = NavigationManager.Uri.Remove(0, NavigationManager.BaseUri.Length);

                if (!nameView.Equals("Home") && !nameView.Equals("UserProfile"))
                {
                    HttpClient?.DefaultRequestHeaders.Remove("viewName");
                    HttpClient?.DefaultRequestHeaders.Add("viewName", nameView);
                    var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<FunctionalityToJson>>("access/ViewsFuncionality/FuncionalityPerViewName");
                    HttpClient?.DefaultRequestHeaders.Remove("viewName");

                    if (deserializeResponse!.Succeeded)
                    {
                        Permissions = deserializeResponse.Data ?? new();
                    }
                }
            }
            catch
            {
                Permissions = new();
            }

            StateHasChanged();
        }

        #endregion GetPermissions

        public async Task GetAllRecords()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                profileFilterDtoRequest = new()
                {
                    Profile1 = ProfileName,
                    ProfileCode = ProfileCode
                };

                var responseApi = await HttpClient!.PostAsJsonAsync("security/Profile/ByFilterList", profileFilterDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<ProfileDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                {
                    allRecords = deserializeResponse.Data;
                }
                else
                {
                    allRecords = default;
                }
            }
            catch (Exception ex)
            {
            }
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
            else { args.Data = profileList; }

            args.IsCancelled = CancelExport;
        }

        #endregion Methods
    }
}