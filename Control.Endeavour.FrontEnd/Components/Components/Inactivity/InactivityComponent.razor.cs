using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Interfaces.Authentication;
using Control.Endeavour.FrontEnd.StateContainer.Authentication;
using Control.Endeavour.FrontEnd.StateContainer.Documents;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Control.Endeavour.Frontend.Client.Models.ComponentViews.Menu.Request;

using Control.Endeavour.FrontEnd.Components.Components.Modals;

using Control.Endeavour.FrontEnd.Components.Views.Menu;
using Control.Endeavour.FrontEnd.Models.Enums.Generic;
using Control.Endeavour.FrontEnd.Components.Components.User;
using Control.Endeavour.FrontEnd.Models.Models.Authentication.CodeRecovery.Request;

using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Interfaces.Authentication;

using Control.Endeavour.FrontEnd.Services.Interfaces.Storage;
using Control.Endeavour.FrontEnd.Services.Services.Authentication;
using Control.Endeavour.FrontEnd.Services.Services.Language;

using Control.Endeavour.FrontEnd.StateContainer.Authentication;
using Microsoft.AspNetCore.Components;

using Microsoft.AspNetCore.Components.Authorization;

using Microsoft.JSInterop;

using System.Net.Http.Json;
using Telerik.SvgIcons;

using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;

using Control.Endeavour.FrontEnd.Components.Components.Captcha;
using Control.Endeavour.FrontEnd.StateContainer.Filing;
using Control.Endeavour.FrontEnd.Models.Models.Menu.Request;
using System;

using Control.Endeavour.FrontEnd.Resources;
using Microsoft.Extensions.Localization;

using Control.Endeavour.FrontEnd.Models.Models.Administration.User.Response;

using Control.Endeavour.FrontEnd.StateContainer.Documents;

using Control.Endeavour.FrontEnd.Models.Models.Administration.Profile.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Response;
using Control.Endeavour.FrontEnd.Components.Components.Inactivity;

namespace Control.Endeavour.FrontEnd.Components.Components.Inactivity
{
    public partial class InactivityComponent
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private IConfiguration Configuration { get; set; }

        [Inject]
        private IAuthenticationJWT? AuthenticationJWT { get; set; }

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject] private ProfileStateContainer authenticationStateContainer { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        //private static Thread _timerThread;

        private static bool _isTimerCancelled { get; set; } = false;
        private const long DEFAULT_MILLISECOND = 900000;
        private const int DEFAULT_SECONDS = 15000;
        private const int CONVERT_MINUTES = 60000;
        private const int CONVERT_SECONDS = 1000;
        private const string MINUTES = "Minutes";
        private const string SECTION_INACTIVITY = "Inactivity";
        private const string WAIT_TIME = "WaitingTimeInSeconds";
        private int MinutesQuantity { get; set; } = 0;

        private long milliseconds;
        private int waitTime;
        private int waitTimeInSeconds;
        private DateTime lastInteractionTime;

        private NotificationsComponentModal? notificationModal { get; set; }

        protected override async Task OnInitializedAsync()
        {
            milliseconds = await ParameterizedMillisecond();
            waitTime = ParameterizedSeconds();

            lastInteractionTime = DateTime.Now;

            await SetInactivityTimer();
        }

        private async Task SetInactivityTimer()
        {
            await JSRuntime.InvokeVoidAsync("resetInactivityTimer", DotNetObjectReference.Create(this), milliseconds);
        }

        [JSInvokable]
        public async Task LogOutDueToInactivity()
        {
            Console.WriteLine($"Ingreso al método LogOutDueToInactivity");

            string currentUrl = NavigationManager.Uri;
            var isHomePage = currentUrl.EndsWith("/");
            if (isHomePage) return;
            _isTimerCancelled = false;
            notificationModal.UpdateModal(ModalType.Information, string.Format(Translation["InactvityQuestion"], waitTimeInSeconds), true, Translation["Accept"], Translation["Cancel"]);

            _isTimerCancelled = false;

            await TimerNotification(waitTimeInSeconds);
        }

        private async Task TimerNotification(int seconds)
        {
            await Task.Delay(waitTimeInSeconds * 1000);
            if (!_isTimerCancelled)
            {
                notificationModal.Visible
                    = false;
                StateHasChanged();

                await CloseSession();
            }
            else
            {
                _isTimerCancelled = true;
            }
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type.Equals(ModalType.Information) && args.IsCancelled)
            {
                _isTimerCancelled = true;
            }
            if (notificationModal.Type.Equals(ModalType.Information) && args.IsAccepted)
            {
                await CloseSession();
            }
        }

        private async Task HandleUserInteraction()
        {
            lastInteractionTime = DateTime.Now;
            await SetInactivityTimer();
        }

        private async Task<long> ParameterizedMillisecond()
        {
            long milliseconds = DEFAULT_MILLISECOND;
            await GetData();
            int minutesConf = ( MinutesQuantity != 0 ) ? MinutesQuantity : Configuration
                .GetSection(SECTION_INACTIVITY)
                .GetValue<int>(MINUTES);

            if (minutesConf is not default(int))
                milliseconds = minutesConf * CONVERT_MINUTES;

            return milliseconds;
        }

        private int ParameterizedSeconds()
        {
            var seconds = DEFAULT_SECONDS;
            waitTimeInSeconds = Configuration
                .GetSection(SECTION_INACTIVITY)
                .GetValue<int>(WAIT_TIME);

            if (waitTimeInSeconds is not default(int))
                return waitTimeInSeconds * CONVERT_SECONDS;

            return seconds;
        }

        private async Task CloseSession()
        {
            var response = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<bool>>("security/Session/UpdateLogout");

            if (response.Succeeded)
            {
                await AuthenticationJWT.LogoutToken();
                NavigationManager?.NavigateTo("");
                authenticationStateContainer.SelectedComponentChanged("Login");
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["SigningOutError"], true, Translation["Accept"], "", "", "");
            }
            StateHasChanged();
        }

        private async Task GetData()
        {
            var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<AppKeysDtoResponse>>>("security/System/ReadUsers");
            if (deserializeResponse.Succeeded)
            {
                var data = deserializeResponse.Data;

                var minute = data.Where(apk => apk.KeyName.Equals("MinutesQuantity")).FirstOrDefault();

                MinutesQuantity = int.Parse(minute.Value1 ?? "0");
            }
        }
    }
}