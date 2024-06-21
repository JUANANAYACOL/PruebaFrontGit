using Control.Endeavour.FrontEnd.Pages.Administration;
using Control.Endeavour.FrontEnd.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Control.Endeavour.FrontEnd.Components.Components.User
{
    public partial class UserCardComponent
    {
        #region Variables

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #region Parameters

        [Parameter]
        public string FullName { get; set; } = "NameNotFound";

        [Parameter]
        public string AdministrativeUnitName { get; set; } = "AdministrativeUnitNotFound";

        [Parameter]
        public string ProductionOfficeName { get; set; } = "ProductionOfficeNotFound";

        [Parameter]
        public string Positionname { get; set; } = "PositionNotFound";

        [Parameter]
        public int? LeadManagerId { get; set; }

        [Parameter]
        public bool ValidLeadManager { get; set; } = false;

        #endregion Parameters

        #endregion Variables

        protected override async Task OnInitializedAsync()
        {
            if (FullName == "NameNotFound")
                FullName = Translation[FullName];
            if (AdministrativeUnitName == "AdministrativeUnitNotFound")
                AdministrativeUnitName = Translation[AdministrativeUnitName];
            if (ProductionOfficeName == "ProductionOfficeNotFound")
                ProductionOfficeName = Translation[ProductionOfficeName];
            if (Positionname == "PositionNotFound")
                Positionname = Translation[Positionname];
        }
    }
}