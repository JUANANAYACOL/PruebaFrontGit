using Control.Endeavour.FrontEnd.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Control.Endeavour.FrontEnd.Components.Components.User
{
    public partial class UserComponent
    {
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #region Parameters
        [Parameter] public string? UserName { get; set; }
        [Parameter] public string? UserImage { get; set; }
        #endregion
    }
}
