using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.DocumentaryTask
{
    public partial class SecondPasswordModal
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

        private NotificationsComponentModal notificationModal;
        #endregion

        #region Modals



        #endregion

        #region Parameters
        [Parameter] public EventCallback<MyEventArgs<bool>> OnStatusChanged { get; set; }
        #endregion

        #region Models


        #endregion

        #region Environments

        #region Environments(String)
        public string tempPassInput = "";
        #endregion

        #region Environments(Numeric)
        private int UserId = 4076;
        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool validate = false;
        private bool isEnableActionButton = true;

        #endregion

        #region Environments(List & Dictionary)



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
            validate = false;
            tempPassInput = string.Empty;
            StateHasChanged();
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted && notificationModal.ModalOrigin.Equals("ValidatePassword"))
            {
                var eventArgs = new MyEventArgs<bool>
                {
                    Data = validate,
                    ModalStatus = false
                };
                tempPassInput = "";

                await OnStatusChanged.InvokeAsync(eventArgs);

            }
        }


        #endregion

        #region OthersMethods
        public async Task ValidatePassword()
        {
            SecondPasswordDtoRequest validateRequest = new() { SecondPassword = tempPassInput };
            var responseApi = await HttpClient.PostAsJsonAsync("security/User/CheckSecondPassword", validateRequest);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<bool>>();

            if (deserializeResponse.Succeeded)
            {
                validate = deserializeResponse.Data;

            }

            if (!validate)
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["WrongPassword"], true);

            }
            else
            {
                notificationModal.UpdateModal(ModalType.Success, Translation["SuccessfulValidation"], true, modalOrigin: "ValidatePassword");
            }


        }

        #endregion

        #endregion
    }
}
