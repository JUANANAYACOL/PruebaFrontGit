using Control.Endeavour.FrontEnd.Models.Enums.Generic;
using Control.Endeavour.FrontEnd.Models.Models.Components.Language.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Interfaces.Storage;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using System;
using System.Net.Http.Json;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Layouts.NotFound
{
    public partial class NotFoundLayout
    {
        #region Variables

        #region Inject

        [Inject] private NavigationManager NavigationManager { get; set; }
        [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; }

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }


        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private ILocalStorage? LocalStorage { get; set; }

        #endregion Inject

        #region Environments

        #region Environments(String)

        private string? CurrentLanguage = "ES"; //Idioma predeterminado

        #endregion Environments(String)

        #region Environments(List & Dictionary)

        public static Dictionary<string, string>? LanguageCache { get; set; } = new();

        public List<PhraseDtoResponse> PhraseDtoResponses { get; set; } = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            try
            {
                //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
                LanguageCache = await LocalStorage.GetValue<Dictionary<string, string>>(ValuesKeysEnum.Diccionario);

                if (LanguageCache == null)
                {
                    LanguageCache = await LanguageSelected(CurrentLanguage);
                    await LocalStorage.SetValue(ValuesKeysEnum.Diccionario, LanguageCache);
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la inicialización de DropDownLanguageComponent: {ex.Message}");
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

        #region LanguageSelected

        public async Task<Dictionary<string, string>> LanguageSelected(string language)
        {
            bool validate = await LocalStorage.ContainsKey(ValuesKeysEnum.Diccionario);

            if (validate)
            {
                await LocalStorage.RemoveItem(ValuesKeysEnum.Diccionario);
            }

            var Peticion = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<PhraseDtoResponse>>>($"translation/Language/TranslationByCode?code={language}");
            if (Peticion.Succeeded)
            {
                PhraseDtoResponses = Peticion.Data;
                LanguageCache = PhraseDtoResponses!
                            .ToDictionary(item => item.KeyPhrase.KeyName, item => item.TextPhrase);

                await LocalStorage.SetValue(ValuesKeysEnum.Diccionario, LanguageCache);

                // Notificar a los componentes que el idioma ha cambiado
                //await EventAggregator.PublishLanguageChanged();

                return LanguageCache;
            }
            else
            {
                LanguageCache = new();
            }

            return LanguageCache;
        }

        #endregion LanguageSelected

        #region GetText

        public static string GetText(string key) =>
        LanguageCache?.GetValueOrDefault(key) ?? "key no encontrada";

        #endregion GetText

        #region RedirectPage

        public async Task RedirectPage()
        {
            var authState = await AuthenticationState;
            if (!authState.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo("/");
            }
            else
            {
                NavigationManager.NavigateTo("/Home");
            }
        }

        #endregion RedirectPage

        #endregion Methods
    }
}