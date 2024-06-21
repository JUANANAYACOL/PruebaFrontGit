using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Net.Http.Json;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.DocumentaryTask
{
    public partial class CopiesModal
    {

        #region Variables

        #region Inject 
        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion

        #region Components

        private NotificationsComponentModal notificationModal = new();
        #endregion

        #region Modals

        private GenericUserSearchModal genericUserSearch = new();
        #endregion

        #region Parameters

        [Parameter] public EventCallback<MyEventArgs<CopyDtoResponse>> OnStatusChanged { get; set; }

        #endregion

        #region Models

        private CopyDtoResponse destinationCopys = new();


        #endregion

        #region Environments

        #region Environments(String)

        private string modalTitle = "";

        #endregion

        #region Environments(Numeric)

        public int ActiveTabIndex = 0;
        private int changeModal = 1;

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool isuser = false;
        private bool seenCopys = false;
        private bool UserModalStatus = false;

        #endregion

        #region Environments(List & Dictionary)

        private List<UserClasification> userList = new();
        private List<AdministrationUsers> thirdList = new();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            modalTitle = Translation["UserSearch"];
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;

        }


        #endregion

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
        }

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted && args.ModalOrigin.Equals("DestinationCopies"))
            {

                var eventArgs = new MyEventArgs<CopyDtoResponse>
                {
                    Data = destinationCopys,
                    ModalStatus = false
                };

                await OnStatusChanged.InvokeAsync(eventArgs);

            }
            else
            {
                var eventArgs = new MyEventArgs<CopyDtoResponse>();

                await OnStatusChanged.InvokeAsync(eventArgs);
            }

        }

        private async Task HandleCopys()
        {
            destinationCopys = new();

            if (userList.Count > 0) { destinationCopys.DestinationsUser = new(); destinationCopys.DestinationsUser.AddRange(userList.Where(x => x.TaskId == null).Select(x => x).ToList()); }
            if (thirdList.Count > 0) { destinationCopys.DestinationsAdministration = new(); destinationCopys.DestinationsAdministration.AddRange(thirdList.Where(x => x.TaskId == null).Select(x => x).ToList()); }

            notificationModal.UpdateModal(ModalType.Warning, Translation["ConfirmAction"] + "\n " +

                " \n " + Translation["WishContinue"], true, modalOrigin:"DestinationCopies");
        }

        #endregion

        #region OthersMethods

        private void HandleSelectedModal(int changemodal)
        {
            try
            {
                changeModal = changemodal;

                if (changemodal == 2)
                {
                    modalTitle = Translation["UserSearch"] + " - " + Translation["NaturalLegalPersons"];
                }
                else
                {
                    modalTitle = Translation["UserSearch"];
                }
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine($"{nameof(OperationCanceledException)} thrown with message: {e.Message}");
            }
        }

        private void HandleSelectedUserData(List<RetunUserSearch> selectedUsers)
        {
            if (changeModal == 1)
            {
                var userRecords = selectedUsers
                .Where(user => user.TypeOfUser == "TDF,U")
                .Select(user => new UserClasification
                {
                    UserId = user.UserId,
                    Type = user.TypeOfUser,
                    FullName = user.UserName,
                    AdministrativeUnitName = user.UserAdministrativeUnitName,
                    ProductionOfficeName = user.UserProductionOfficeName,
                    Charge = user.UserPosition
                }).ToList();

                userList.AddRange(userRecords);
                userList = userList.DistinctBy(x => x.UserId).ToList();

            }
            else
            {
                var thirdRecords = selectedUsers
                .Where(user => user.TypeOfUser == "TDF,T" || user.TypeOfUser == "TDF,TU")
                .Select(user => new AdministrationUsers
                {
                    ThirdPartyId = user.TypeOfUser == "TDF,T" ? user.UserId : (int?)null,
                    ThirdUserId = user.TypeOfUser == "TDF,TU" ? user.UserId : (int?)null,
                    FullName = user.UserName,
                    Type = user.TypeOfUser,
                    CompanyName = user.UserName,
                    IdentificationNumber = user.UserIdentificationNumber,
                    IdentificationTypeName = null,
                    Email = user.UserEmail
                }).ToList();

                thirdList.AddRange(thirdRecords);
                thirdList = thirdList.DistinctBy(x => new { x.ThirdPartyId, x.ThirdUserId }).ToList();
            }


        }

        

        #region UpdateCopies - TaskManagement

        public async Task UpdateCopys(int id, bool value, CopyDtoResponse copies)
        {
            seenCopys = !value;

            HttpClient?.DefaultRequestHeaders.Remove("TaskId");
            HttpClient?.DefaultRequestHeaders.Add("TaskId", id.ToString());
            var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<CopyDtoResponse>>("documentarytasks/DocumentaryTask/GetCopies");
            HttpClient?.DefaultRequestHeaders.Remove("TaskId");

            if (deserializeResponse.Data != null)
            {
                if (deserializeResponse.Data.DestinationsUser != null) { userList = deserializeResponse.Data.DestinationsUser; }
                if (deserializeResponse.Data.DestinationsAdministration != null) { thirdList = deserializeResponse.Data.DestinationsAdministration; }

            }

            if (copies != null)
            {
                if (copies.DestinationsUser != null && copies.DestinationsUser.Count > 0)
                {
                    userList.AddRange(copies.DestinationsUser);
                }
                if (copies.DestinationsAdministration != null && copies.DestinationsAdministration.Count > 0)
                {
                    thirdList.AddRange(copies.DestinationsAdministration);
                }
            }
        }

        #endregion

        #region DataContainerMethods
        

        #endregion

        #region ManageTabComponent
        private void OpenNewModalUser()
        {
            if (ActiveTabIndex == 0)
            {
                isuser = true;
            }
            else { isuser = false; }

            HandleSelectedModal(isuser ? 1 : 2);
            UserModalStatus = true;
        }
        #endregion

        #region DeleteUser/UserAdministration

        private async Task DeleteThirdParty(AdministrationUsers user)
        {
            if (user.TaskId != null)
            {
                var deleteValidation = await DeleteCopys(user.TaskId.Value, (int)(user.ThirdPartyId ?? user.ThirdUserId), user.Type);

                if (deleteValidation)
                {
                    thirdList.Remove(user);
                }
            }
            else
            {
                thirdList.Remove(user);
            }
        }


        private async Task DeleteUser(UserClasification user)
        {
            if (user.TaskId != null)
            {
                var deleteValidation = await DeleteCopys(user.TaskId.Value, user.UserId, user.Type);

                if (deleteValidation)
                {
                    userList.Remove(user);
                }
            }
            else
            {
                userList.Remove(user);
            }
        }

        private async Task<bool> DeleteCopys(int taskId, int id, string type)
        {

            UserDeleteDtoRequest Copies = new()
            {
                TaskId = taskId,
                UserId = id,
                TypeUser = type
            };

            var responseApi = await HttpClient.PostAsJsonAsync("documentarytasks/DocumentaryTask/DeleteCopies", Copies);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<bool>>();

            if (deserializeResponse.Succeeded)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region CloseModalSearchUser
        private void HandleModalUserClosed(bool status)
        {

            UserModalStatus = status;

        }

        #endregion

        #endregion

        #endregion

    }
}
