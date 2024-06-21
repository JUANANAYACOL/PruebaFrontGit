using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Interfaces.AIService;
using Control.Endeavour.FrontEnd.Services.Services.AIService;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using OpenAI_API.Moderation;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.DocumentaryTask
{
    public partial class AIServiceModal
    {
        #region Variables

        #region Inject

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        [Inject]
        private IAnswerGeneratorService answerGeneratorService { get; set; }

        #endregion Inject

        #region Parameters

        [Parameter]
        public EventCallback<string> AIresponse { get; set; }

        #endregion Parameters

        #region Models

        private Assistant assistant = new();
        private ElementReference animatedElement;
        private List<FileInfoData> file = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        public string descriptionInput { get; set; } = "";
        private string answers;
        private string url;
        private string modelType;
        public const string FormatDateTimeStamp = "yyyy_MM_dd__HH_mm_ss_ffff";
        public string TitleButton = "fa-solid fa-microphone";

        #endregion Environments(String)

        #region Environments(Numeric)

        private int contadorcarac = 0;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool _mDisableRecordAudioStart = true;
        private bool animate = false;

        #endregion Environments(Bool)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await Js.InvokeVoidAsync("BlazorAudioRecorder.Initialize", DotNetObjectReference.Create(this));
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        public void UpdateModalStatus(bool newValue, string value)
        {
            modalStatus = newValue;
            modelType = value;
            StateHasChanged();
        }

        private void HandleModalClosed(bool status)
        {
            assistant.Prompt = "";
            file = new();
            modalStatus = status;
        }

        private async Task HandleFileData(List<FileInfoData> newFile)
        {
            if (newFile.Any())
            {
                file = newFile;
                await speechToText(Convert.ToBase64String(file[0].Base64Data));
            }
        }

        #endregion HandleMethods

        #region OthersMethods

        #region CountChar

        private void ContarCaracteres(ChangeEventArgs e)
        {
            String value = e.Value.ToString() ?? String.Empty;
            descriptionInput = value;

            if (!string.IsNullOrEmpty(e.Value.ToString()))
            {
                contadorcarac = e.Value.ToString().Length;
            }
            else
            {
                contadorcarac = 0;
            }
        }

        #endregion CountChar

        #region Consumo AI text generation

        private async Task pruebaAI()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            var response = await answerGeneratorService.GenerateAnswer(assistant.Prompt);

            answers = response;
            await SendAIResponse();
            assistant = new Assistant();
            contadorcarac = 0;
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion Consumo AI text generation

        #region Consumo AI speech to text

        private async Task ButRecordAudioStart_Click()
        {
            if (_mDisableRecordAudioStart)
            {
                url = string.Empty;
                _mDisableRecordAudioStart = false;
                await Js.InvokeVoidAsync("BlazorAudioRecorder.StartRecord");
                animate = true;
                TitleButton = "fa-solid fa-microphone-slash";
            }
            else
            {
                _mDisableRecordAudioStart = true;
                await Js.InvokeVoidAsync("BlazorAudioRecorder.StopRecord");
                animate = false;
                TitleButton = "fa-solid fa-microphone";
            }
        }

        //[JSInvokable]
        //public async Task TakeAudio(string? url, string base64)
        //{
        //    this.url = url;
        //    await InvokeAsync(() => StateHasChanged());
        //    await speechToText(base64);
        //}

        private async Task speechToText(string base64)
        {
            var response = await answerGeneratorService.GenerateAnswerSpeechToText(base64, "record.mp3");
            answers = response;
            await SendAIResponse();
            url = string.Empty;
        }

        #endregion Consumo AI speech to text

        #region Consumo AI speech to answer

        [JSInvokable]
        public async Task TakeAudio(string? url, string base64)
        {
            this.url = url;
            await InvokeAsync(() => StateHasChanged());
            await speechToAnswer(base64);
        }

        private async Task speechToAnswer(string base64)
        {
            var response = await answerGeneratorService.GenerateAnswerSpeechToAnswer(base64, "record.mp3");
            answers = response;
            await SendAIResponse();
            url = string.Empty;
        }

        #endregion Consumo AI speech to answer

        #region Send AIResponse

        private async Task SendAIResponse()
        {
            if (!string.IsNullOrEmpty(answers))
            {
                await AIresponse.InvokeAsync(answers);
            }
            assistant.Prompt = "";
            file = new();
        }

        #endregion Send AIResponse

        #endregion OthersMethods

        #endregion Methods
    }
}