using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.CodeRecovery.Request;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.Login.Request;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.PasswordRecovery.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Interfaces.Authentication;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Components.Timers
{
    public partial class TimerComponent : ComponentBase
    {
        #region Variables

        #region Inject

        [Inject]
        private ProfileStateContainer? AuthenticationStateContainer { get; set; }

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Parameter

        [Parameter]
        public TimeSpan TimeDuration { get; set; } = TimeSpan.FromSeconds(30); // Tiempo por defecto

        [Parameter]
        public string ResendButtonText { get; set; } = "ResendCode"; // Texto por defecto del botón

        [Parameter]
        public EventCallback OnTimerEnd { get; set; }

        [Parameter] public EventCallback<bool> ReSenderCode {  get; set; } = new();

        #endregion Parameter

        #region Entorno

        private CancellationTokenSource cts = new CancellationTokenSource();
        private TimeSpan timeLeft;
        private bool isButtonDisabled = true;
        private NotificationsComponentModal? NotificationModal { get; set; }

        #endregion Entorno

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            await InitializeTimer();
        }

        #endregion OnInitializedAsync

        #region Initialize Timer

        private async Task InitializeTimer()
        {
            timeLeft = TimeDuration;
            isButtonDisabled = true;
            await RunTimer();
        }

        #endregion Initialize Timer

        #region Run Timer

        private async Task RunTimer()
        {
            while (timeLeft > TimeSpan.Zero)
            {
                try
                {
                    await Task.Delay(1000, cts.Token);
                }
                catch (TaskCanceledException)
                {
                    return; // Cancelar si el token de cancelación se activa
                }

                timeLeft = timeLeft.Add(TimeSpan.FromSeconds(-1));
                StateHasChanged(); // Re-render the component
            }

            isButtonDisabled = false;
            StateHasChanged(); // Re-render the component

            if (OnTimerEnd.HasDelegate)
            {
                await OnTimerEnd.InvokeAsync(null);
            }
        }

        #endregion Run Timer

        #region Resend Code

        private async Task ReSendTaskEvent()
        {
            await ReSenderCode.InvokeAsync(true);
            cts.Cancel();
            cts.Dispose();
            cts = new CancellationTokenSource();
            await InitializeTimer();
        }

        #endregion Resend Code

        #region Dispose

        public void Dispose()
        {
            cts.Cancel();
            cts.Dispose();
        }

        

       

        #endregion Dispose
    }
}