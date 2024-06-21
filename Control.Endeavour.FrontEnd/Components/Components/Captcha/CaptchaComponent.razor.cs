using Control.Endeavour.FrontEnd.Components.Components.DropDownList;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Toolbelt.Blazor.SpeechSynthesis;

namespace Control.Endeavour.FrontEnd.Components.Components.Captcha
{
    public partial class CaptchaComponent
    {
        #region Inject
        [Inject]
        private SpeechSynthesis? SpeechSynthesis { get; set; }

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { set; get; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion

        #region Parameters
        [Parameter]
        public EventCallback<string> OnCaptchaEntered { get; set; }
        #endregion

        #region Private Fields
        private string lastRequestTime = "Never";
        private Timer? timer;
        public string CaptchaValue => captcha;
        private string captcha = string.Empty;
        //CompareCaptchaDtoRequest compareCaptchaDtoRequest = new CompareCaptchaDtoRequest();
        #endregion

        #region Methods

        #region Language Handling
        // Método que se suscribe al método de publicación del servicio de traducción
        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        /*private string setKeyName(string key)
        {
            return Translation[key];
        }*/
        #endregion

        #region Event Handling
        private async Task OnCaptchaInputChanged(ChangeEventArgs e)
        {
            string userInput = e.Value.ToString();
            await OnCaptchaEntered.InvokeAsync(userInput);
        }
        #endregion 

        #region Initialization
        protected override async Task OnInitializedAsync()
        {
            //Ejecuta la primera petición inmediatamente y luego cada minuto
            TimerCallback timerCallback = _ => PeticionCaptcha();
            timer = new Timer(timerCallback, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }
        #endregion

        #region API Calls
        // Método para traer de la API el Captcha
        private async void PeticionCaptcha()
        {
            var captchaResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<string>>("security/Captcha/GetCaptcha");
            captcha = captchaResponse.Data!;
            lastRequestTime = DateTime.Now.ToString();
            StateHasChanged();
        }
        #endregion

        #region Cleanup
        public void Dispose() => timer.Dispose();
        #endregion

        #region Speech Synthesis
        private readonly float Speed = 0.5f; // Puedes ajustar la velocidad aquí

        private async Task OnClickSpeak()
        {
            var options = new SpeechSynthesisUtterance
            {
                Text = captcha,
                Rate = Speed // Ajusta la velocidad aquí (1.0 es la velocidad normal)
            };

            await this.SpeechSynthesis.SpeakAsync(options);
        }
        #endregion
        #endregion
    }
}
