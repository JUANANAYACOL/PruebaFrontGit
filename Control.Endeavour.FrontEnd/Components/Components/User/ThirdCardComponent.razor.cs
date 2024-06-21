using Control.Endeavour.FrontEnd.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Control.Endeavour.FrontEnd.Components.Components.User
{
    public partial class ThirdCardComponent
    {
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #region Parameters

        [Parameter]
        public string IdentificationNumber { get; set; } = "TINNotFound";
        [Parameter]
        public string Names { get; set; } = "NameNotFound";
        [Parameter]
        public string CompanyName { get; set; } = "CompanyNotFound";
        [Parameter]
        public string Email { get; set; } = "EmailNotFound";
        [Parameter]
        public string Positionname { get; set; } = "PositionNotFound";

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            if(IdentificationNumber == "TINNotFound")
                IdentificationNumber = Translation[IdentificationNumber];
            if (Names == "NameNotFound") 
                Names = Translation[Names];
            if (CompanyName == "CompanyNotFound") 
                CompanyName = Translation[CompanyName];
            if (Names == "EmailNotFound") 
                Email = Translation[Email];
            if (Names == "PositionNotFound") 
                Positionname = Translation[Positionname];
        }
        #endregion
    }
}
