using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.StateContainer.Authentication;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;

namespace Control.Endeavour.FrontEnd.Pages.Authentications.Login
{
    public partial class LoginPage
    {
        #region Variables
        #region Injects
       
        [Inject]
        private ProfileStateContainer? AuthenticationStateContainer { get; set; }

        #endregion
        #endregion

        

        public string ComponenteRenderizar { get; set; } = "CodeRecovery";

        protected override async Task OnInitializedAsync()
        {
            // Esperar a que DropDownLanguageComponent esté completamente inicializado
            //while (DropDownLanguageComponent.LanguageCache == null)
            //{
            //    await Task.Delay(100);
            //}

            ////EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            AuthenticationStateContainer.ComponentChange += StateHasChanged;
        }
        public void Dispose()
        {
            AuthenticationStateContainer.ComponentChange -= StateHasChanged;
        }

        //public string? ComponentViewRender { get; set; } = "Login";
        //public string? Uuid { get; set; } 
        //public string? Ip { get; set; } 
        //public string? User { get; set; } 

        //public void OnInit(EventCallbackArgs componentViewRender) {
        //    ComponentViewRender = componentViewRender.Vista;
        //    Console.WriteLine(componentViewRender.Vista);
        //    Console.WriteLine(componentViewRender.Uuid);
        //    Console.WriteLine(componentViewRender.Ip);
        //    Console.WriteLine(componentViewRender.User);
        //    Uuid = componentViewRender.Uuid;
        //    Ip = componentViewRender.Ip;
        //    User = componentViewRender.User;
        //    StateHasChanged();
        //}
    }
}
