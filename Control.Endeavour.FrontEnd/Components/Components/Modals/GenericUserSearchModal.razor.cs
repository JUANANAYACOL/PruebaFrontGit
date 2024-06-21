using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Telerik.Blazor.Common.Filter;

namespace Control.Endeavour.FrontEnd.Components.Components.Modals
{
    public partial class GenericUserSearchModal
    {
        #region Parameters

        [Parameter]
        public string Title { get; set; } = string.Empty;

        [Parameter]
        public bool IsVisible { get; set; } = false;

        [Parameter]
        public string Width { get; set; } = "50%";

        [Parameter]
        public bool AllowCopiesSelection { get; set; } = false;

        [Parameter]
        public bool AllowMultipleSelection { get; set; } = false;

        [Parameter] public bool IsModalExpand { get; set; } = true;

        /// <summary>
        /// Define el tipo de búsqueda a realizar en el sistema.
        /// </summary>
        /// <value>
        /// Entero que representa los diferentes tipos de búsqueda:
        /// 1 - Búsqueda de usuarios.
        /// 2 - Búsqueda de usuarios de terceros.
        /// 3 - Búsqueda de destinatarios y remitentes de una radicación enviada.
        /// 4 - Búsqueda de destinatarios y remitentes de una radicación interna.
        /// 5 - Búsqueda de destinatarios y remitentes de una radicación recibida.
        /// </value>

        [Parameter] public int TypeOfSearch { get; set; } = 1;

        [Parameter] public bool IsValidateReplacements { get; set; } = false;

        [Parameter] public bool AllowToCheckReplament { get; set; } = true;
        [Parameter] public bool ValidateSenderAndRecipient {  get; set; } = true;

        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #region EventCallBacks

        [Parameter]
        public EventCallback<bool> OnModalClosed { get; set; }

        [Parameter]
        public EventCallback<List<RetunUserSearch>> OnSelectedUserData { get; set; }

        #endregion EventCallBacks

        #region Modals

        private NotificationsComponentModal notificationModal = new();

        #endregion

        #endregion Parameters

        private string regularHeight = "fit-content";
        private string expandedHeight = "100vh";
        private string expandedWidth = "100%";
        private string regularWidth = string.Empty;
        private string modalExpanded = string.Empty;
        private string regularMaxHeight = "90%";
        private string classExpand = string.Empty;
        private bool IsExpanded = false;
        private bool IsButtonVisible = false;
        private bool IsSavaData = false;
        private bool ShowCopiesFiling = true;
        private bool ValidateReplacementsfiling = true;

        private List<RetunUserSearch> selectedUsers = new();

        private List<ThirdUserDtoResponse> ThirdUsersChecked = new();
        private List<ThirdPartyDtoResponse> ThirdPartyUserCheckeds = new();
        private List<VUserDtoResponse> UserCheckeds = new();

        private List<ThirdUserDtoResponse> ThirdUsersSenderChecked = new();
        private List<ThirdPartyDtoResponse> ThirdPartyUserSenderCheckeds = new();
        private List<VUserDtoResponse> UserSenderCheckeds = new();

        private List<ThirdUserDtoResponse> ThirdUsersRecipientChecked = new();
        private List<ThirdPartyDtoResponse> ThirdPartyUserRecipientCheckeds = new();
        private List<VUserDtoResponse> UserRecipientCheckeds = new();

        private UsersSearchModal userSearhModalRef = new();
        private ThirdPartyUserSearchModal thirPartyUserSearchModalRef = new();


        private int ActiveTabIndex = 0;

        #region ActionsMethods

        protected override async Task OnInitializedAsync()
        {
            regularWidth = Width;
            if (!IsModalExpand)
            {
                modalExpanded = "d-md-none";
            }
            if (AllowMultipleSelection)
            {
                IsButtonVisible = true;
            }
            if (!ValidateSenderAndRecipient)
            {
                ShowCopiesFiling = false;
                ValidateReplacementsfiling = false;
            }
        }

        private async Task CloseModal()
        {

            await OnModalClosed.InvokeAsync(false);
            regularHeight = "fit-content";
            regularWidth = Width;
            IsExpanded = false;
            classExpand = string.Empty;
            regularMaxHeight = "90%";
            if (!IsSavaData)
            {
                UserRecipientCheckeds = new();
                ThirdUsersChecked = new();
                ThirdPartyUserCheckeds = new();
                UserCheckeds = new();

                ThirdUsersSenderChecked = new();
                ThirdPartyUserSenderCheckeds = new();
                UserSenderCheckeds = new();

                ThirdUsersRecipientChecked = new();
                ThirdPartyUserRecipientCheckeds = new();
            }
        }

        private void ExpandModal()
        {
            if (IsExpanded)
            {
                regularHeight = "fit-content";
                regularWidth = Width;
                IsExpanded = false;
                classExpand = string.Empty;
                regularMaxHeight = "90%";
            }
            else
            {
                regularMaxHeight = "100%";
                regularWidth = expandedWidth;
                regularHeight = expandedHeight;
                classExpand = "expanded-modal";
                IsExpanded = true;
            }
        }

        private void HandleModalOpenChanged(bool isOpen)
        {
            if (isOpen)
            {
                IsButtonVisible = false;
            }
            else
            {
                IsButtonVisible = true;
            }
        }

        private async Task TabChangedHandler(int newIndex)
        {
            ActiveTabIndex = newIndex;
        }

        #endregion ActionsMethods

        #region ReturnMethods

        #region SimpleSelection

        private async Task HandleDataChangedAsync(List<ThirdUserDtoResponse> updatedUsers)
        {
            notificationModal.UpdateModal(ModalType.Information, Translation["SelectionQuestion"], true, Translation["Yes"], Translation["No"], modalOrigin: "SimpleSelectionThirdUser");
            if (!AllowMultipleSelection)
            {
                selectedUsers.Clear();
            }
            RetunUserSearch retunUserSearch = new()
            {
                UserId = updatedUsers[0].ThirdUserId,
                TypeOfUser = "TDF,TU",
                IsSelected = updatedUsers[0].Selected,
                IsCopy = updatedUsers[0].Copy,
                UserName = updatedUsers[0].Names,
                UserEmail = updatedUsers[0].Email,
                UserIdentificationNumber = updatedUsers[0].IdentificationNumber,
            };
            selectedUsers.Add(retunUserSearch);
        }

        private async Task HandleDataChangedThirdPartyAsync(List<ThirdPartyDtoResponse> updatedUsers)
        {
            notificationModal.UpdateModal(ModalType.Information, Translation["SelectionQuestion"], true, Translation["Yes"], Translation["No"], modalOrigin: "SimpleSelectionThirdParty");
            if (!AllowMultipleSelection)
            {
                selectedUsers.Clear();
            }
            RetunUserSearch retunUserSearch = new()
            {
                UserId = updatedUsers[0].ThirdPartyId,
                TypeOfUser = "TDF,T",
                IsSelected = updatedUsers[0].Selected,
                IsCopy = updatedUsers[0].Copy,
                UserName = updatedUsers[0].Names,
                UserEmail = updatedUsers[0].Email1 ?? updatedUsers[0].Email2,
                UserIdentificationNumber = updatedUsers[0].IdentificationNumber
            };
            selectedUsers.Add(retunUserSearch);
        }

        private async Task HandleDataChangedUserAsync(List<VUserDtoResponse> updatedUsers)
        {
            notificationModal.UpdateModal(ModalType.Information, Translation["SelectionQuestion"], true, Translation["Yes"], Translation["No"], modalOrigin: "SimpleSelectionThirdParty");
            if (!AllowMultipleSelection)
            {
                selectedUsers.Clear();
            }
            RetunUserSearch retunUserSearch = new()
            {
                UserId = updatedUsers[0].UserId,
                TypeOfUser = "TDF,U",
                IsSelected = updatedUsers[0].Selected,
                IsCopy = updatedUsers[0].Copy,
                UserName = updatedUsers[0].FullName,
                UserEmail = updatedUsers[0].Email,
                UserPosition = updatedUsers[0].Charge,
                UserAdministrativeUnitName = updatedUsers[0].AdministrativeUnitName,
                UserProductionOfficeName = updatedUsers[0].ProductionOfficeName
            };
            selectedUsers.Add(retunUserSearch);
        }

        #endregion SimpleSelection

        #region MassiveSelection

        private async Task ReturnMassiveSelectedUserAsync()
        {
            selectedUsers.Clear();
            if ((TypeOfSearch == 3 || TypeOfSearch == 4 || TypeOfSearch == 5) && ValidateSenderAndRecipient)
            {
                if (!await ValidateUsersSelection())
                {
                    return;
                }
            }
            switch (TypeOfSearch)
            {
                case 1:
                    ProcessUsers();
                    break;
                case 2:
                    ProcessThirdPartyUsers();
                    break;
                case 3:
                    ProcessFilingSents();
                    break;
                case 4:
                    ProcessInternalFilingInternals();
                    break;
                case 5:
                    ProcessFilling();
                    break;
            }
            notificationModal.UpdateModal(ModalType.Information, Translation["SelectionQuestion"], true, Translation["Yes"], Translation["No"], modalOrigin: "SendUser");
        }


        private void ProcessFilling()
        {
            //Senders
            ProcessThirdPartyUsers(ThirdPartyUserSenderCheckeds, "TDF,T", true, false);
            ProcessThirdUsers(ThirdUsersSenderChecked, "TDF,TU", true, false);
            //Recipients
            ProcessUsers(UserRecipientCheckeds, "TDF,U", false, true);
        }

        private void ProcessInternalFilingInternals()
        {
            //Senders
            ProcessUsers(UserSenderCheckeds, "TDF,U", true, false);
            //Recipients
            ProcessUsers(UserRecipientCheckeds, "TDF,U", false, true);

        }

        private void ProcessFilingSents()
        {
            //Senders
            ProcessUsers(UserSenderCheckeds, "TDF,U", true, false);
            //Recipients
            ProcessThirdPartyUsers(ThirdPartyUserRecipientCheckeds, "TDF,T", false, true);
            ProcessThirdUsers(ThirdUsersRecipientChecked, "TDF,TU", false, true);

        }

        private void ProcessUsers()
        {
            if (UserCheckeds.Any())
            {
                foreach (var user in UserCheckeds)
                {
                    RetunUserSearch userSearch = new()
                    {
                        UserId = user.UserId,
                        TypeOfUser = "TDF,U",
                        IsSelected = user.Selected,
                        IsCopy = user.Copy,
                        UserAdministrativeUnitName = user.AdministrativeUnitName,
                        UserProductionOfficeName = user.ProductionOfficeName,
                        UserEmail = user.Email,
                        UserName = user.FullName,
                        UserPosition = user.Charge
                    };
                    selectedUsers.Add(userSearch);
                }
            }

        }

        private void ProcessThirdPartyUsers()
        {
            if (ThirdUsersChecked.Any())
            {
                foreach (var user in ThirdUsersChecked)
                {
                    RetunUserSearch returnUserSearch = new()
                    {
                        UserId = user.ThirdUserId,
                        TypeOfUser = "TDF,TU",
                        IsSelected = user.Selected,
                        IsCopy = user.Copy,
                        UserName = user.Names,
                        UserEmail = user.Email,
                        UserIdentificationNumber = user.IdentificationNumber
                    };
                    selectedUsers.Add(returnUserSearch);
                }
            }
            if (ThirdPartyUserCheckeds.Any())
            {
                foreach (var party in ThirdPartyUserCheckeds)
                {
                    RetunUserSearch returnUserSearch = new()
                    {
                        UserId = party.ThirdPartyId,
                        TypeOfUser = "TDF,T",
                        IsSelected = party.Selected,
                        IsCopy = party.Copy,
                        UserName = party.Names,
                        UserEmail = party.Email1 ?? party.Email2,
                        UserIdentificationNumber = party.IdentificationNumber
                    };
                    selectedUsers.Add(returnUserSearch);
                }
            }

        }

        #endregion MassiveSelection



        #endregion ReturnMethods

        #region ValidationMethods
        private async Task<bool> ValidateUsersSelection()
        {
            bool senderChecks = false;
            bool recipientChecks = false;

            switch (TypeOfSearch)
            {
                case 3:
                    senderChecks = UserSenderCheckeds.Any();
                    recipientChecks = ThirdPartyUserRecipientCheckeds.Any() || ThirdUsersRecipientChecked.Any();
                    break;

                case 4:
                    senderChecks = UserSenderCheckeds.Any();
                    recipientChecks = UserRecipientCheckeds.Any();
                    break;

                case 5:
                    senderChecks = ThirdPartyUserSenderCheckeds.Any() || ThirdUsersSenderChecked.Any();
                    recipientChecks = UserRecipientCheckeds.Any();
                    break;
            }

            if (!senderChecks && !recipientChecks)
            {
                notificationModal.UpdateModal(ModalType.Warning, Translation["MustSelectUserSearch"], true, Translation["Accept"], buttonTextCancel: "");
                return false;
            }
            else if (!senderChecks)
            {
                notificationModal.UpdateModal(ModalType.Warning, Translation["MustSelectSender"], true, Translation["Accept"], buttonTextCancel: "");
                ActiveTabIndex = 0;
                return false;
            }
            else if (!recipientChecks)
            {
                notificationModal.UpdateModal(ModalType.Warning, Translation["MustSelectRecipient"], true, Translation["Accept"], buttonTextCancel: "");
                ActiveTabIndex = 1;
                return false;
            }

            return true;
        }

        #endregion

        #region ProcessMethos
        private void ProcessThirdPartyUsers(List<ThirdPartyDtoResponse> thirdPartyUserCheckeds, string typeOfUser, bool? isSender, bool? isRecipient)
        {
            foreach (var party in thirdPartyUserCheckeds)
            {
                RetunUserSearch returnUserSearch = new()
                {
                    UserId = party.ThirdPartyId,
                    TypeOfUser = typeOfUser,
                    IsSelected = party.Selected,
                    IsCopy = party.Copy,
                    IsSender = party.Copy ? null : isSender,
                    IsRecipient = party.Copy ? null : isRecipient,
                    UserName = party.Names,
                    UserEmail = party.Email1 ?? party.Email2,
                    UserIdentificationNumber = party.IdentificationNumber
                };
                selectedUsers.Add(returnUserSearch);
            }
        }

        private void ProcessThirdUsers(List<ThirdUserDtoResponse> thirdUsersChecked, string typeOfUser, bool? isSender, bool? isRecipient)
        {
            foreach (var user in thirdUsersChecked)
            {
                RetunUserSearch returnUserSearch = new()
                {
                    UserId = user.ThirdUserId,
                    TypeOfUser = typeOfUser,
                    IsSelected = user.Selected,
                    IsCopy = user.Copy,
                    IsSender = user.Copy ? null : isSender,
                    IsRecipient = user.Copy ? null : isRecipient,
                    UserName = user.Names,
                    UserEmail = user.Email,
                    UserIdentificationNumber = user.IdentificationNumber
                };
                selectedUsers.Add(returnUserSearch);
            }
        }

        private void ProcessUsers(List<VUserDtoResponse> userCheckeds, string typeOfUser, bool? isSender, bool? isRecipient)
        {
            foreach (var user in userCheckeds)
            {
                RetunUserSearch userSearch = new()
                {
                    UserId = user.UserId,
                    TypeOfUser = typeOfUser,
                    IsSelected = user.Selected,
                    IsCopy = user.Copy,
                    IsSender = user.Copy ? null : isSender,
                    IsRecipient = user.Copy ? null : isRecipient,
                    UserAdministrativeUnitName = user.AdministrativeUnitName,
                    UserProductionOfficeName = user.ProductionOfficeName,
                    UserEmail = user.Email,
                    UserName = user.FullName,
                    UserPosition = user.Charge
                };
                selectedUsers.Add(userSearch);
            }
        }


        #endregion

        #region HandleModalNotiClose

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            switch (args.ModalOrigin)
            {
                case "SendUser":
                    if (args.IsAccepted)
                    {
                        IsSavaData = true;
                        await OnSelectedUserData.InvokeAsync(selectedUsers);
                        await CloseModal();
                        
                    }
                    break;
                case "SimpleSelectionThirdUser":
                    if (args.IsAccepted)
                    {
                        IsSavaData = true;
                        await OnSelectedUserData.InvokeAsync(selectedUsers);
                        await CloseModal();
                        ;
                    }
                    else
                    {
                        thirPartyUserSearchModalRef.ClearThirUser(selectedUsers[0].UserId);
                        selectedUsers.Clear();
                        ThirdUsersChecked.Clear();
                    }
                    break;
                case "SimpleSelectionThirdParty":
                    if (args.IsAccepted)
                    {
                        IsSavaData = true;
                        await OnSelectedUserData.InvokeAsync(selectedUsers);
                        await CloseModal();
                    }
                    else
                    {
                        thirPartyUserSearchModalRef.ClearThirdPartyUserFlags(selectedUsers[0].UserId);
                        selectedUsers.Clear();
                        ThirdPartyUserCheckeds.Clear();
                    }
                    break;
                case "SimpleSelectionUser":
                    if (args.IsAccepted)
                    {
                        IsSavaData = true;
                        await OnSelectedUserData.InvokeAsync(selectedUsers);
                        await CloseModal();
                    }
                    else
                    {
                        userSearhModalRef.ClearUserListData(selectedUsers[0].UserId);
                        selectedUsers.Clear();
                        UserCheckeds.Clear();
                    }
                    break;

            }
        }

        #endregion HandleModalNotiClose

        #region DeleteUsers
        public void ClearUserCheckeds()
        {
            UserCheckeds = new();
        }
        public void RemoveUserSelectedById(int userId)
        {
            selectedUsers.RemoveAll(u => u.UserId == userId);

            if (TypeOfSearch == 1 || TypeOfSearch == 2)
            {
                ThirdUsersChecked.RemoveAll(u => u.ThirdUserId == userId);
                ThirdPartyUserCheckeds.RemoveAll(u => u.ThirdPartyId == userId);
                UserCheckeds.RemoveAll(u => u.UserId == userId);
            }
            else if (TypeOfSearch == 3 || TypeOfSearch == 4 || TypeOfSearch == 5)
            {
                ThirdUsersSenderChecked.RemoveAll(u => u.ThirdUserId == userId);
                ThirdPartyUserSenderCheckeds.RemoveAll(u => u.ThirdPartyId == userId);
                UserSenderCheckeds.RemoveAll(u => u.UserId == userId);

                ThirdUsersRecipientChecked.RemoveAll(u => u.ThirdUserId == userId);
                ThirdPartyUserRecipientCheckeds.RemoveAll(u => u.ThirdPartyId == userId);
                UserRecipientCheckeds.RemoveAll(u => u.UserId == userId);
            }
                

        }
        #endregion
    }
}