using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel;
using System.Globalization;

namespace Control.Endeavour.FrontEnd.Components.Components.Captcha
{
    public partial class ReCaptchaGoogleComponent
    {
        #region Variables

        #region Inject 

        [Inject]
        private IJSRuntime JS { get; set; }




        #endregion

        #region Components


        #endregion

        #region Modals


        #endregion

        #region Parameters

        [Parameter]
        public string SiteKey { get; set; }

        [Parameter]
        public EventCallback<string> OnSuccess { get; set; }

        [Parameter]
        public EventCallback OnExpired { get; set; }
        [Parameter]
        public string Lang { get; set; }

        #endregion

        #region Models

        #endregion

        #region Environments

        #region Environments(String)
        private string UniqueId = Guid.NewGuid().ToString();
        private string loadMessage = "Loading...";
        #endregion

        #region Environments(Numeric)
        private int WidgetId;
        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)
        private bool isLoadedScript = false;
        #endregion

        #region Environments(List & Dictionary)

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    if (string.IsNullOrEmpty(Lang))
                        Lang = CultureInfo.DefaultThreadCurrentCulture?.Name ?? "es";

                    var timeoutTimeSpan = new TimeSpan(0, 0, 5); // 5 secs
                    var ress = await JS.InvokeAsync<object>("My.reCAPTCHA.init", timeout: timeoutTimeSpan);
                    this.WidgetId = await JS.InvokeAsync<int>("My.reCAPTCHA.render", timeout: timeoutTimeSpan, DotNetObjectReference.Create(this), UniqueId, SiteKey, Lang);
                    isLoadedScript = true;
                    loadMessage = "";
                }
                catch (Exception ex)
                {
                    loadMessage = "Error on Load Captcha.";
                    isLoadedScript = false;
                }

                StateHasChanged();
            }
        }


        #endregion

        #region Methods

        #region HandleMethods


        #endregion

        #region OthersMethods
        [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
        public void CallbackOnSuccess(string response)
        {
            if (OnSuccess.HasDelegate)
            {
                OnSuccess.InvokeAsync(response);
            }
        }

        [JSInvokable, EditorBrowsable(EditorBrowsableState.Never)]
        public void CallbackOnExpired()
        {
            if (OnExpired.HasDelegate)
            {
                OnExpired.InvokeAsync(null);
            }
        }

        public ValueTask<string> GetResponseAsync()
        {
            return JS.InvokeAsync<string>("My.reCAPTCHA.getResponse", WidgetId);
        }

        #endregion

        #endregion
    }
}