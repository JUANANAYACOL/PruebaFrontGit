using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Generic;
using Control.Endeavour.FrontEnd.Models.Models.Components.Language.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Interfaces.Storage;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Components.DropDownList
{
    public partial class DDLLangComponent
    {
        [Inject]
        private IJSRuntime? js { get; set; }

        [Inject]
        private NavigationManager? navigationManager { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        private NotificationsComponentModal notificationModal { get; set; } = new();

        private string currentCulture { get; set; } = CultureInfo.DefaultThreadCurrentCulture?.Name ?? "es";

        private string cultureInfo { get; set; } = CultureInfo.DefaultThreadCurrentCulture?.Name ?? "es";

        private Dictionary<string, string> culturesTranslated = new Dictionary<string, string> {
            { new string("es"),new string("Español") },
            //{ new string("de"),new string("Deutsche") },
            { new string("en"),new string("English") },
            //{ new string("fr"),new string("Française") },
            //{ new string("it"),new string("Italiano") },
            //{ new string("pt"),new string("Português") }
        };

        protected override async Task OnInitializedAsync()
        {
            
            
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) 
            {
                if (string.IsNullOrEmpty(currentCulture) || currentCulture.Contains("es-"))
                {
                    var a = CultureInfo.DefaultThreadCurrentCulture?.Name;
                    currentCulture = CultureInfo.DefaultThreadCurrentCulture?.Name ?? "es";
                    StateHasChanged(); 
                }
            }
        }

        private void HandleModalNotiClose(ModalClosedEventArgs args) {
            if (notificationModal.Type == ModalType.Warning && args.IsAccepted) {
                var culture = new CultureInfo(cultureInfo);
                var jsInProcessRuntime = (IJSInProcessRuntime)js;
                jsInProcessRuntime.InvokeVoid("culture.set", culture.Name);
                navigationManager.NavigateTo(navigationManager.Uri, forceLoad: true);
            }
        }

        private void ChangeLanguage(string value)
        {
            if (!string.IsNullOrEmpty(value)){
                cultureInfo = value;
                notificationModal.UpdateModal(ModalType.Warning, Translation["LanguageChangeMessage"], true, Translation["Accept"], Translation["Cancel"]);
            }
            else
            {
                cultureInfo = value;
            }
            
        }


    }
}
