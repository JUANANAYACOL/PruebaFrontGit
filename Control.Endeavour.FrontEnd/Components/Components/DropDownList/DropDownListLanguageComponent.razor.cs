using Control.Endeavour.FrontEnd.Models.Enums.Generic;
using Control.Endeavour.FrontEnd.Models.Models.Components.Language.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Services.Interfaces.Storage;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using System.Globalization;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Components.DropDownList
{
    public partial class DropDownListLanguageComponent
    {
        #region Variables
        #region Inject
        [Inject]
        private ILocalStorage? LocalStorage { get; set; }

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }
        #endregion Inject

        #endregion

        #region Private Fields

        private string? CurrentLanguage = "ES"; //Idioma predeterminado
        private string? CodeLanguage;

        public static Dictionary<string, string>? LanguageCache;

        public List<PhraseDtoResponse> PhraseDtoResponses { get; set; } = new List<PhraseDtoResponse>();
        private List<LanguageDtoResponse> LanguageDtoResponses { get; set; } = new List<LanguageDtoResponse>();

        private string? DefaultText;

        #endregion Private Fields

        #region Methods

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

        public static string GetText(string key) =>
        LanguageCache?.GetValueOrDefault(key) ?? "key no encontrada";

        #region Initialization

        protected override async Task OnInitializedAsync()
        {
            try
            {


                LanguageCache = await LocalStorage.GetValue<Dictionary<string, string>>(ValuesKeysEnum.Diccionario);

                if (LanguageCache == null)
                {
                    LanguageCache = await LanguageSelected(CurrentLanguage);
                    await LocalStorage.SetValue(ValuesKeysEnum.Diccionario, LanguageCache);

                }
                if (LanguageDtoResponses.Count == 0)
                {
                    await GetLanguages();
                }

                StateHasChanged();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la inicialización de DropDownLanguageComponent: {ex.Message}");
            }
        }

        #endregion Initialization

        private async Task GetLanguages()
        {
            try
            {
                var response = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<LanguageDtoResponse>>>("translation/Language/Get");
                //var response = await CallService.Get<List<LanguageDtoResponse>>("translation/Language/Get");
                LanguageDtoResponses = updateListText(response.Data).Result;
                DefaultText = LanguageDtoResponses.Where(s => s.CodeLanguage.Equals(CurrentLanguage)).Select(x => x.NameTraslated).First();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los lenguajes: {ex.Message}");
            }
        }

        private async Task<List<LanguageDtoResponse>> updateListText(List<LanguageDtoResponse> list)
        {
            foreach (var languageDto in list)
            {
                languageDto.NameTraslated = GetText(languageDto.Name);
            }
            return list;
        }

        private void ChangeLanguage(string value)
        {
            LanguageSelected(value);
            LanguageDtoResponses = updateListText(LanguageDtoResponses).Result;
            CurrentLanguage = value;
            DefaultText = LanguageDtoResponses.Where(s => s.CodeLanguage.Equals(value)).Select(x => x.NameTraslated).First();

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("it-IT");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("it-IT");
        }


        #endregion Methods
    }
}
