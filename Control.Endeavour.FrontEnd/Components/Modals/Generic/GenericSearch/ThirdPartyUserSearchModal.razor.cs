using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdParty.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Models.Models.Records.Request;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch
{
    public partial class ThirdPartyUserSearchModal
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
        NewPaginationComponent<ThirdPartyDtoResponse, ThirdPartyFilterDtoRequest> PaginationComponentThirdPartyPN = new();
        NewPaginationComponent<ThirdPartyDtoResponse, ThirdPartyFilterDtoRequest> PaginationComponentThirdPartyPJ = new();

        #endregion

        #region Modals
        private ThirdPartyUsersSearchModal ViewThirdPartyUserModal = new();
        private NotificationsComponentModal notificationModal = new();

        #endregion

        #region Parameters
        [Parameter]
        public EventCallback<bool> OnModalOpenChanged { get; set; }
        [Parameter]
        public EventCallback<List<ThirdUserDtoResponse>> OnDataChanged { get; set; }
        [Parameter]
        public EventCallback<List<ThirdPartyDtoResponse>> OnDataChangedThirdParty { get; set; }
        [Parameter]
        public bool AllowCopiesSelection { get; set; } = false;

        [Parameter]
        public bool AllowMultipleSelection { get; set; } = false;
        [Parameter]
        public List<ThirdPartyDtoResponse> ThirdPartyChecked { get; set; } = new();

        [Parameter]
        public List<ThirdUserDtoResponse> ThirdUsersChecked { get; set; } = new();


        #endregion

        #region Models
        private ThirdPartyFilterDtoRequest thirdPartyFilterPN = new();
        private ThirdPartyFilterDtoRequest thirdPartyFilterPJ = new();
        private NotificationsComponentModal NotificationModal = new();
        private RecordFilterDtoRequest RecordFilter = new();
        private PaginationInfo PaginationInfoThirdPartyPN = new();
        private PaginationInfo PaginationInfoThirdPartyPJ = new();

        #endregion

        #region Environments

        #region Environments(String)
        private string ThirdPartyFilterUri = "administration/ThirdParty/ByFilterWithUsers";
        private string PanelPnPerson = string.Empty;
        private string PanelPjPerson = "d-none";
        private string TypePerson = "PN";
        #endregion

        #region Environments(Numeric)


        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        #endregion

        #region Environments(List & Dictionary)
        private List<ThirdPartyDtoResponse> thirdPartyListPN { get; set; } = new();
        private List<ThirdPartyDtoResponse> thirdPartyListPJ { get; set; } = new();



        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            thirdPartyFilterPN.PersonType = "PN";
            thirdPartyFilterPJ.PersonType = "PJ";
            await GetThirdUser("PN");
        }


        #endregion

        #region Methods

        #region HandleMethods


        #endregion

        #region OthersMethods

        #region GetDataMethods

        public async Task GetThirdUser(string TypePerson = "")
        {
            try
            {
                ThirdPartyFilterDtoRequest filterRequest = new();
                switch (TypePerson)
                {
                    case "PN":
                        filterRequest = thirdPartyFilterPN;
                        break;
                    case "PJ":
                        filterRequest = thirdPartyFilterPJ;
                        break;

                }
                var responseApi = await HttpClient.PostAsJsonAsync(ThirdPartyFilterUri, filterRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<ThirdPartyDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data.Data != null)
                {
                    switch (TypePerson)
                    {
                        case "PN":
                            thirdPartyListPN = UpdateListThirdUser(deserializeResponse.Data.Data);
                            PaginationInfoThirdPartyPN = deserializeResponse.Data.ExtractPaginationInfo();
                            PaginationComponentThirdPartyPN.ResetPagination(PaginationInfoThirdPartyPN);
                            break;
                        case "PJ":
                            thirdPartyListPJ = UpdateListThirdUser(deserializeResponse.Data.Data);
                            PaginationInfoThirdPartyPJ = deserializeResponse.Data.ExtractPaginationInfo();
                            PaginationComponentThirdPartyPJ.ResetPagination(PaginationInfoThirdPartyPJ);
                            break;

                    }

                }

                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                NotificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }

        #endregion

        #region ModalMethods

        private void ShowModalThirdUser(ThirdPartyDtoResponse record)
        {
            if (record.ThirdUsers != null && record.ThirdUsers.Count > 0)
            {
                ViewThirdPartyUserModal.UpdateListThirdUser(record.ThirdUsers);
                ViewThirdPartyUserModal.UpdateModalStatus(true);
                OnModalOpenChanged.InvokeAsync(true);
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Information, Translation["NoThirdPartyUserMessage"], true, buttonTextCancel: "");
            }
        }

        #endregion

        #region FilterMethods
        public bool ValidateThirdPartyFilter(ThirdPartyFilterDtoRequest thirdPartyFilter)
        {
            if (!string.IsNullOrWhiteSpace(thirdPartyFilter.Names) ||
                !string.IsNullOrWhiteSpace(thirdPartyFilter.IdentificationNumber) ||
                !string.IsNullOrWhiteSpace(thirdPartyFilter.Email))
            {

                return true;
            }
            return false;
        }

        private async Task CleanFilter()
        {
            ThirdUsersChecked = new();
            ThirdPartyChecked = new();
            switch (TypePerson)
            {
                case "PN":
                    thirdPartyFilterPN = new();
                    await GetThirdUser(TypePerson);
                    break;
                case "PJ":
                    thirdPartyFilterPJ = new();
                    await GetThirdUser(TypePerson);
                    break;

            }
        }
        private async Task SearchByFilter()
        {
            ThirdPartyFilterDtoRequest filterRequest = new();
            switch (TypePerson)
            {
                case "PN":
                    filterRequest = thirdPartyFilterPN;
                    break;
                case "PJ":
                    filterRequest = thirdPartyFilterPJ;
                    break;

            }
            bool hasValidData = ValidateThirdPartyFilter(filterRequest);
            if (hasValidData)
            {
                await GetThirdUser(TypePerson);
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Warning, Translation["EnterParameterSearch"], true, buttonTextCancel: "");
            }
        }


        #endregion

        #region ActionsMethods
        private void HandleModalOpenChanged(bool isOpen)
        {
            if (isOpen)
            {
                OnModalOpenChanged.InvokeAsync(true);
            }
            else
            {
                OnModalOpenChanged.InvokeAsync(false);
            }

        }
        private async Task TabChangedHandler(string type)
        {

            if (type.Equals("PJ"))
            {
                await GetThirdUser("PJ");
                PanelPnPerson = "d-none";
                PanelPjPerson = string.Empty;
                TypePerson = "PJ";
            }
            else
            {
                await GetThirdUser("PN");
                PanelPnPerson = string.Empty;
                PanelPjPerson = "d-none";
                TypePerson = "PN";
            }
        }
        #region SelectionMethods

        private async Task OnCheckBoxChanged(bool newValue, ThirdPartyDtoResponse thirdParty, string action)
        {
            if (!AllowMultipleSelection && newValue)
            {
                ThirdPartyChecked.Clear();
            }
            if (action == "SelectedRecord")
            {
                if (thirdParty.Selected == true && !newValue)
                {
                    thirdParty.Selected = false;
                }
                else
                {
                    thirdParty.Selected = newValue;
                    thirdParty.Copy = !newValue;
                }
            }
            else if (action == "SelectedCopies")
            {
                if (thirdParty.Copy == true && !newValue)
                {
                    thirdParty.Copy = false;
                }
                else
                {
                    thirdParty.Copy = newValue;
                    thirdParty.Selected = !newValue;
                }
            }
            UpdateCheckedPartyList(thirdParty, newValue);

            if (!AllowMultipleSelection)
            {
                await OnDataChangedThirdParty.InvokeAsync(ThirdPartyChecked);

            }
        }

        private void UpdateCheckedPartyList(ThirdPartyDtoResponse thirdParty, bool newValue)
        {
            if (ThirdPartyChecked == null)
            {
                ThirdPartyChecked = new List<ThirdPartyDtoResponse>();
            }

            var existingParty = ThirdPartyChecked.FirstOrDefault(u => u.ThirdPartyId == thirdParty.ThirdPartyId);
            if (existingParty != null)
            {
                existingParty.Selected = thirdParty.Selected;
                existingParty.Copy = thirdParty.Copy;
            }
            else if (newValue)
            {
                ThirdPartyChecked.Add(thirdParty);
            }
            if (!AllowMultipleSelection && ThirdPartyChecked.Count > 1)
            {
                ThirdPartyChecked.RemoveAll(u => u.ThirdPartyId != thirdParty.ThirdPartyId);
            }
        }




        #endregion

        #endregion



        #region PaginationMethods
        public List<ThirdPartyDtoResponse> UpdateListThirdUser(List<ThirdPartyDtoResponse> userList)
        {
            var checkedUsersMap = new Dictionary<int, ThirdPartyDtoResponse>();


            foreach (var user in ThirdPartyChecked)
            {
                checkedUsersMap[user.ThirdPartyId] = user;
            }

            foreach (var newUser in userList)
            {
                if (checkedUsersMap.TryGetValue(newUser.ThirdPartyId, out var checkedUser))
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
        private void HandlePaginationGridPN(List<ThirdPartyDtoResponse> newDataList)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            thirdPartyListPN = UpdateListThirdUser(newDataList);

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }
        private void HandlePaginationGridPJ(List<ThirdPartyDtoResponse> newDataList)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            thirdPartyListPJ = UpdateListThirdUser(newDataList);

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion


        #region ReturnDataMethods
        private async void HandleDataChanged(List<ThirdUserDtoResponse> updatedUsers)
        {
            await OnDataChanged.InvokeAsync(updatedUsers);
        }
        #endregion

        #region CleanList
        public void ClearThirdPartyUserFlags(int userId)
        {
            var itemPN = thirdPartyListPN.FirstOrDefault(item => item.ThirdPartyId == userId);
            if (itemPN != null)
            {
                itemPN.Selected = false;
                itemPN.Copy = false;
            }


            var itemPJ = thirdPartyListPJ.FirstOrDefault(item => item.ThirdPartyId == userId);
            if (itemPJ != null)
            {
                itemPJ.Selected = false;
                itemPJ.Copy = false;
            }
        }

        public void ClearThirUser(int userId)
        {
            ViewThirdPartyUserModal.ClearThirUsers(userId);
        }
        #endregion

        #endregion

        #endregion

    }
}
