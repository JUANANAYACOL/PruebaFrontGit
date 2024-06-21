using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.ManagementTray
{
    public partial class CopiesDocumentModal
    {

        #region Variables

        #region Inject 
        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }
        [Inject] private IJSRuntime Js { get; set; }
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion

        #region Components


        #endregion

        #region Modals
        private NotificationsComponentModal notificationModal = new();
        private GenericUserSearchModal genericUserSearch = new();

        #endregion

        #region Parameters


        #endregion

        #region Models

        #endregion

        #region Environments

        #region Environments(String)
        private string filingCode = string.Empty;

        #endregion

        #region Environments(Numeric)

        private int controlId = 0;
        private int TypeOfSearchUserModal = 1;

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)
        private bool modalStatus = false;
        private bool UserModalStatus = false;
        #endregion

        #region Environments(List & Dictionary)

        private List<DocumentCopiesDtoResponse> copiesUserDocument = new();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;

        }


        #endregion

        #region Methods

        #region HandleMethods

        private void HandleModalClosed(bool value)
        {
            UpdateModalStatus(value);

        }
        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            
        }
        private async Task HandleOnModalUserChangeAsync(List<RetunUserSearch> selectedUsers)
        {
            try
            {
                DocumentCopiesDtoRequest request = new()
                {
                    ControlId = controlId,
                    UserCopyId = selectedUsers[0].UserId,
                    UserType = selectedUsers[0].TypeOfUser 
                };
                var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/Document/CreateNewCopy", request);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<string>>();
                if (deserializeResponse.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["CopySentTo"] + selectedUsers[0].UserName, true);
                    await GetDocumentCopies();
                }
                
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }

        }

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        #endregion

        #region OthersMethods

        #region GetDataMethods

        private async Task GetDocumentCopies()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("ControlId");
                HttpClient?.DefaultRequestHeaders.Add("ControlId", $"{controlId}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<DocumentCopiesDtoResponse>>>("documentmanagement/Document/ReadCopies");
                HttpClient?.DefaultRequestHeaders.Remove("ControlId");

                if (deserializeResponse!.Succeeded && deserializeResponse!.Message.Equals(string.Empty))
                {
                    copiesUserDocument = deserializeResponse.Data;
                }
                else { copiesUserDocument = new(); }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
            }
        }

        #endregion

        #region ModalMethods
        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }
        public async Task LoadDocumentcopies(int ControlId,  string FilingCode)
        {
            controlId = ControlId;
            filingCode = FilingCode;
            await GetDocumentCopies();
        }

        private void ShowModalSearchUser()
        {
             UserModalStatus = true;
        }
        #endregion

        #region ActionsMethods

        private void OpenModalSearchUser(int typeOfSearch)
        {
            TypeOfSearchUserModal = typeOfSearch;
            UserModalStatus = true;

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
