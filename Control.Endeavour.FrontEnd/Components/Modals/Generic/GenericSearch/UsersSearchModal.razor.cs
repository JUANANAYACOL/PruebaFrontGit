using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministrativeUnit.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch
{
    public partial class UsersSearchModal
    {

        #region Variables

        #region Inject 
        [Inject]
        private HttpClient? HttpClient { get; set; }
        [Inject]
        private IJSRuntime Js { get; set; }
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }
        #endregion

        #region Components

        private NewPaginationComponent<VUserDtoResponse, VUserDtoRequest> paginationComponetPost = new();

        #endregion

        #region Modals
        private NotificationsComponentModal NotificationModal = new();

        #endregion

        #region Parameters
        [Parameter]
        public bool AllowCopiesSelection { get; set; } = false;

        [Parameter]
        public bool AllowMultipleSelection { get; set; } = false;
        [Parameter]
        public List<VUserDtoResponse> UsersChecked { get; set; } = new();
        [Parameter]
        public EventCallback<List<VUserDtoResponse>> OnDataChanged { get; set; }
        [Parameter] public bool IsValidateReplacements { get; set; } = false;

        [Parameter] public bool AllowToCheckReplament { get; set; } = true;



        #endregion

        #region Models
        private PaginationInfo paginationInfo = new();
        private VUserDtoRequest UserFilterDtoRequest = new();
        #endregion

        #region Environments

        #region Environments(String)

        private string UriFilterVUser = "generalviews/VUser/ByFilter";

        #endregion

        #region Environments(Numeric)
        private int currentDocumentalVersionID = 0;
        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)
        private bool isEnableProductionOffice = false;
        #endregion

        #region Environments(List & Dictionary)

        private List<VUserDtoResponse> UserListData = new();
        private List<VSystemParamDtoResponse> lstCharguesTypes = new();
        private List<DocumentalVersionDtoResponse> docVersionList = new();
        private List<AdministrativeUnitsDtoResponse> administrativeUnitList = new();
        private List<ProductionOfficesDtoResponse>? productionOfficeList { get; set; } = new();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {

            await GetDocumentalVersions();
            await GetChargueType();
            if (UsersChecked.Any())
            {
                UserListData = UsersChecked;
            }
        }

        #endregion

        #region Methods

        #region HandleMethods

        #region PaginationMethods
        private void HandlePaginationGrid(List<VUserDtoResponse> newDataList)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            UserListData = UpdateListUserData(newDataList);
            SpinnerLoaderService.HideSpinnerLoader(Js);

        }

        private List<VUserDtoResponse> UpdateListUserData(List<VUserDtoResponse> userList)
        {
            var checkedUsersMap = new Dictionary<int, VUserDtoResponse>();
            foreach (var user in UserListData)
            {
                checkedUsersMap[user.UserId] = user;
            }
            foreach (var newUser in userList)
            {
                if (checkedUsersMap.TryGetValue(newUser.UserId, out var checkedUser))
                {
                    newUser.Selected = checkedUser.Selected;
                    newUser.Copy = checkedUser.Copy;
                }
                else
                {
                    newUser.Selected = false;
                    newUser.Copy = false;
                }
            }
            return userList;
        }

        #endregion

        #endregion

        #region OthersMethods

        #region GetDataMethods
        private async Task GetUsersData()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                UserFilterDtoRequest.UserActiveState = true;
                var responseApi = await HttpClient.PostAsJsonAsync(UriFilterVUser, UserFilterDtoRequest);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<VUserDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null && deserializeResponse.Data.Data.Any())
                {
                    UserListData = UpdateListUserData(deserializeResponse.Data.Data);
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponetPost.ResetPagination(paginationInfo);
                }
                else
                {
                    UserListData = new();
                    paginationInfo = new();
                    paginationComponetPost.ResetPagination(paginationInfo);

                }
            }
            catch (Exception ex)
            {
                NotificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }

            StateHasChanged();

            SpinnerLoaderService.HideSpinnerLoader(Js);
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
                NotificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
        }

        public async Task GetDocumentalVersions()
        {
            try
            {
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<DocumentalVersionDtoResponse>>>("paramstrd/DocumentalVersions/ByDocumentalVersions");
                if (deserializeResponse!.Succeeded)
                {
                    docVersionList = deserializeResponse.Data!;
                    string toCurrent = Translation["ToCurrent"];
                    string documentaryVersion = Translation["DocumentaryVersion"];
                    docVersionList.ForEach(x =>
                    {
                        x.Code = $"{((x.EndDate == null || x.EndDate.Value > DateTime.Now) ? $"{x.StartDate!.Value.Year} {toCurrent} - {documentaryVersion} {x.Code}" : $"{x.StartDate!.Value.Year} a {x.EndDate.Value.Year} - {documentaryVersion} {x.Code} {x.VersionType}")}";
                    });
                    docVersionList = docVersionList.OrderBy(x => x.DocumentalVersionId).Reverse().ToList();
                    currentDocumentalVersionID = docVersionList.FirstOrDefault(x => x.EndDate == null || x.EndDate.Value > DateTime.Now).DocumentalVersionId;
                    await GetAdministrativeUnits(currentDocumentalVersionID);
                }
            }
            catch (Exception ex)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                NotificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }
        private async Task GetAdministrativeUnits(int id)
        {

            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");
                HttpClient?.DefaultRequestHeaders.Add("documentalVersionsId", $"{id}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeUnitsDtoResponse>>>("paramstrd/AdministrativeUnit/ByAdministrativeUnits");
                HttpClient?.DefaultRequestHeaders.Remove("documentalVersionsId");

                if (deserializeResponse!.Succeeded && (deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null))
                {
                    administrativeUnitList = deserializeResponse.Data;
                    isEnableProductionOffice = true;
                }
                else
                {
                    administrativeUnitList = new();
                }
            }
            catch (Exception ex)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                NotificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }

        }

        private async Task GetProducOffice(int id)
        {
            try
            {

                SpinnerLoaderService.ShowSpinnerLoader(Js);

                HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                HttpClient?.DefaultRequestHeaders.Add("AdministrativeUnitId", $"{id}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<ProductionOfficesDtoResponse>>>("paramstrd/ProductionOffice/ByProductionOffices");
                if (deserializeResponse!.Succeeded && (deserializeResponse.Data?.Count != 0 || deserializeResponse.Data != null))
                {
                    productionOfficeList = deserializeResponse.Data;

                }
                HttpClient?.DefaultRequestHeaders.Remove("AdministrativeUnitId");
                SpinnerLoaderService.HideSpinnerLoader(Js);

            }
            catch (Exception)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                NotificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }

        #endregion

        #region FilterMethods
        private async Task CleanFilter()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            UserFilterDtoRequest = new();
            UsersChecked = new();
            UserListData = new();

            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        private async Task SearchByFilter()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                if (!string.IsNullOrWhiteSpace(UserFilterDtoRequest.FirstName) ||
            !string.IsNullOrWhiteSpace(UserFilterDtoRequest.LastName) ||
            (!string.IsNullOrWhiteSpace(UserFilterDtoRequest.ChargeCode)) ||
            (UserFilterDtoRequest.AdministrativeUnitId > 0) ||
            (UserFilterDtoRequest.ProductionOfficeId > 0))
                {
                    await GetUsersData();
                }
                else
                {

                    NotificationModal.UpdateModal(ModalType.Information, Translation["AtLeastOneMessage"], true, Translation["Accept"], "");
                }
            }
            catch (Exception ex)
            {
                NotificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion

        #region SelectedMethods

        private async Task OnCheckBoxChangedAsync(bool newValue, VUserDtoResponse user, string action)
        {
            bool isValidUser = false;
            if (!AllowMultipleSelection && newValue)
            {
                UsersChecked.Clear();
            }
            if (action == "SelectedRecord")
            {
                if (IsValidateReplacements && newValue)
                {
                     isValidUser = await ValidateReplament(user.UserId);
                }

                if (user.Selected == true && !newValue)
                {
                    user.Selected = false;
                }
                else
                {
                    user.Selected = newValue;
                    user.Copy = !newValue;
                }
            }
            else if (action == "SelectedCopies")
            {
                if (user.Copy == true && !newValue)
                {
                    user.Copy = false;
                }
                else
                {
                    user.Copy = newValue;
                    user.Selected = !newValue;
                }
            }
            if (!AllowToCheckReplament && newValue && isValidUser)
            {
                user.Selected = !newValue;
                user.Copy = !newValue;
            }
            UpdateCheckedUsersList(user, newValue);
            if (!AllowMultipleSelection && UsersChecked.Count() > 0)
            {
                await OnDataChanged.InvokeAsync(UsersChecked);
            }
        }

        private void UpdateCheckedUsersList(VUserDtoResponse user, bool newValue)
        {
            if (UsersChecked == null)
            {
                UsersChecked = new List<VUserDtoResponse>();
            }

            var existingUser = UsersChecked.FirstOrDefault(u => u.UserId == user.UserId);

            if (existingUser != null)
            {
                existingUser.Selected = user.Selected;
                existingUser.Copy = user.Copy;

                if (!existingUser.Selected && !existingUser.Copy)
                {
                    UsersChecked.Remove(existingUser);
                }
            }
            else if (newValue && (user.Selected || user.Copy))
            {
                UsersChecked.Add(user);
            }

            if (!AllowMultipleSelection && UsersChecked.Count > 1)
            {
                UsersChecked.RemoveAll(u => u.UserId != user.UserId);
            }
        }




        #endregion

        #region ValidationsMethods
        private async Task<bool> ValidateReplament(int id)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("UserId");
                HttpClient?.DefaultRequestHeaders.Add("UserId", id.ToString());
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<string>>("administration/Replacement/ValidateUserReplacement");
                HttpClient?.DefaultRequestHeaders.Remove("UserId");

                if (deserializeResponse!.Succeeded)
                {
                    if (AllowToCheckReplament)
                    {
                        NotificationModal.UpdateModal(ModalType.Warning, string.Format(Translation["ReplacementNotMandatoryMessage"], deserializeResponse.Data), true, Translation["Accept"], buttonTextCancel: "");
                    }
                    else
                    {
                        NotificationModal.UpdateModal(ModalType.Warning, Translation["ReplacementUserMandatoryMessage"], true, Translation["Accept"], buttonTextCancel: "");
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                NotificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
                return false;
            }
            finally
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
        }


        #endregion

        #region CleanList

        public void ClearUserListData(int userId)
        {
            foreach (var user in UserListData)
            {
                if (user.UserId == userId)
                {
                    user.Selected = false;
                    user.Copy = false;
                }
            }
        }

        #endregion

        #endregion

        #endregion



    }
}
