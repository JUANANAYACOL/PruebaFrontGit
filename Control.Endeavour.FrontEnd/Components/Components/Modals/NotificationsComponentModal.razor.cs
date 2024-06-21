using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Components.Components.Modals
{
    public partial class NotificationsComponentModal
    {
        #region Variables

        #region Inject

        //[Inject] private EventAggregatorService? EventAggregator { get; set; }
        [Inject] private HttpClient? HttpClient { get; set; }

        [Inject] private IJSRuntime? Js { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Parameters

        [Parameter] public ModalType Type { get; set; } = ModalType.Error;
        [Parameter] public string Title { get; set; } = "";
        [Parameter] public string ClassColor { get; set; } = "";
        [Parameter] public string Message { get; set; } = "";
        [Parameter] public string IdModal { get; set; } = "";
        [Parameter] public bool Visible { get; set; } = false;
        [Parameter] public string ButtonTextAceptar { get; set; } = "Accept";
        [Parameter] public string ButtonTextCancel { get; set; } = "Cancel";
        [Parameter] public string ModalOrigin { get; set; } = "";
        [Parameter] public string Width { get; set; } = "512px";
        [Parameter] public string Height { get; set; } = "410px";
        [Parameter] public string NotificationModal { get; set; } = "";
        [Parameter] public RenderFragment? Body { get; set; }
        [Parameter] public EventCallback<ModalClosedEventArgs> OnModalClosed { get; set; }

        #endregion Parameters

        #region Models

        private ModalType Success = ModalType.Success;
        private ModalType Information = ModalType.Information;
        private ModalType Warning = ModalType.Warning;
        private ModalType Error = ModalType.Error;

        #endregion Models

        #region Environments

        #region Environments(String)

        private string ButtonClassMargin { get; set; } = "";

        #endregion Environments(String)

        #region Environments(Bool)

        private bool IsDeleted = false;

        #endregion Environments(Bool)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;

            if (Body != null)
            {
                Width = "886px";
                Height = "auto";
                ButtonClassMargin = "mb-3";
            }
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #endregion HandleMethods

        #region GetMethods

        private string GetIconPath(ModalType modalType)
        {
            switch (modalType)
            {
                case ModalType.Error:
                    ClassColor = "title-modal-error";
                    return "../img/alerts/IconoModalError.svg"; // Cambiar a la ruta correcta si es necesario
                case ModalType.Success:
                    ClassColor = "title-modal-success";
                    return "../img/alerts/IconoModalExitoso.svg"; // Cambiar a la ruta correcta si es necesario
                case ModalType.Information:
                    ClassColor = "title-modal-information";
                    return "../img/alerts/IconoModalInformacion.svg";

                case ModalType.Warning:
                    ClassColor = "title-modal-warning";
                    return "../img/alerts/IconoModalAdvertencia.svg";

                default:
                    ClassColor = "title-modal-warning";
                    return "../img/alerts/IconoModalAdvertencia.svg"; // O alguna imagen predeterminada
            }
        }

        private string GetStyleModalAlert(ModalType modalType)
        {
            switch (modalType)
            {
                case ModalType.Error:
                    return NotificationModal = "k-window-content--position notificationModal notificationModal--Error";

                case ModalType.Success:
                    return NotificationModal = "k-window-content--position notificationModal  notificationModal--Success";

                case ModalType.Information:
                    return NotificationModal = "k-window-content--position notificationModal notificationModal--Information";

                case ModalType.Warning:
                    return NotificationModal = "k-window-content--position notificationModal notificationModal--Warning";

                default:
                    return NotificationModal = "k-window-content--position notificationModal notificationModal--Warning";
            }
        }

        #endregion GetMethods

        #region OthersMethods

        public void UpdateModal(ModalType type, string message, bool visible, string buttonTextAceptar = "Accept", string buttonTextCancel = "Cancel", string title = "", string modalOrigin = "")
        {
            Type = type;
            Title = title;
            Message = message;
            Visible = visible;
            ButtonTextAceptar = String.IsNullOrEmpty(buttonTextAceptar) ? Translation["Accept"] : buttonTextAceptar;
            ButtonTextCancel = buttonTextCancel ?? "Cancelar";
            ModalOrigin = modalOrigin;
            NotificationModal = GetStyleModalAlert(Type);
            StateHasChanged();
        }

        private async Task CloseModal(bool value)
        {
            var args = new ModalClosedEventArgs
            {
                IsAccepted = value,
                IsCancelled = !value,
                ModalStatus = false,
                ModalOrigin = ModalOrigin
            };
            Visible = false;
            await OnModalClosed.InvokeAsync(args);
        }

        #endregion OthersMethods

        #endregion Methods
    }
}