using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch
{
    public partial class ThirdPartyUsersSearchModal
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


        #endregion

        #region Modals
        private NotificationsComponentModal notificationModal = new();

        #endregion

        #region Parameters
        [Parameter]
        public EventCallback<bool> OnModalOpenChanged { get; set; }
        [Parameter]
        public bool AllowCopiesSelection { get; set; } = false;

        [Parameter]
        public bool AllowMultipleSelection { get; set; } = false;
        [Parameter]
        public EventCallback<List<ThirdUserDtoResponse>> OnDataChanged { get; set; }
        [Parameter]
        public List<ThirdUserDtoResponse> ThirdUsersChecked { get; set; } = new();


        #endregion

        #region Models

        #endregion

        #region Environments

        #region Environments(String)

        #endregion

        #region Environments(Numeric)

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)
        private bool modalStatus = false;
        #endregion

        #region Environments(List & Dictionary)

        private List<ThirdUserDtoResponse> ThirdUsers = new();


        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {


        }


        #endregion

        #region Methods

        #region HandleMethods

        private void HandleModalClosed(bool status)
        {
            OnModalOpenChanged.InvokeAsync(false);
            modalStatus = status;
        }
        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {



        }
        #endregion

        #region OthersMethods

        #region ModalMethods

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }
        public void UpdateListThirdUser(List<ThirdUserDtoResponse> userList)
        {
            var checkedUsersMap = new Dictionary<int, ThirdUserDtoResponse>();


            foreach (var user in ThirdUsersChecked)
            {
                checkedUsersMap[user.ThirdUserId] = user;
            }


            foreach (var newUser in userList)
            {
                if (checkedUsersMap.TryGetValue(newUser.ThirdUserId, out var checkedUser))
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


            ThirdUsers = userList;
        }





        #endregion

        #region SelectionMethods

        private async Task OnCheckBoxChangedAsync(bool newValue, ThirdUserDtoResponse thirdUser, string action)
        {
            if (!AllowMultipleSelection && newValue)
            {
                ThirdUsersChecked.Clear();
            }

            if (action == "SelectedRecord")
            {
                thirdUser.Selected = newValue;
                thirdUser.Copy = !newValue;
            }
            else if (action == "SelectedCopies")
            {
                thirdUser.Copy = newValue;
                thirdUser.Selected = !newValue;
            }

            UpdateCheckedUsersList(thirdUser, newValue);

            if (!AllowMultipleSelection)
            {
                await OnDataChanged.InvokeAsync(ThirdUsersChecked);
                UpdateModalStatus(false);
            }
        }

        private void UpdateCheckedUsersList(ThirdUserDtoResponse thirdUser, bool newValue)
        {
            if (ThirdUsersChecked == null)
            {
                ThirdUsersChecked = new List<ThirdUserDtoResponse>();
            }

            var existingUser = ThirdUsersChecked.FirstOrDefault(u => u.ThirdUserId == thirdUser.ThirdUserId);

            if (existingUser != null)
            {
                existingUser.Selected = thirdUser.Selected;
                existingUser.Copy = thirdUser.Copy;
            }
            else if (newValue)
            {
                ThirdUsersChecked.Add(thirdUser);
            }

            if (!AllowMultipleSelection && ThirdUsersChecked.Count > 1)
            {
                ThirdUsersChecked.RemoveAll(u => u.ThirdUserId != thirdUser.ThirdUserId);
            }
        }





        #endregion

        #region CleanList
        public void ClearThirUsers(int userId)
        {
            foreach (var user in ThirdUsers)
            {
                if (user.ThirdUserId == userId)
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
