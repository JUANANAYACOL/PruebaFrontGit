using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Control.Endeavour.FrontEnd.Components.Components.Button
{
    public partial class ButtonComponent
    {
        #region Variables

        #region Inject 
        //[Inject]
        //private EventAggregatorService? EventAggregator { get; set; }*/
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }


        #endregion

        #region Parameters
        [Parameter] public string BtnText { get; set; } = "Save";
        [Parameter] public string BtnClassColor { get; set; } = "btnStyle--primary";
        [Parameter] public bool BtnDisabled { get; set; } = false;
        [Parameter] public string BtnClassModifiers { get; set; } = "";
        [Parameter] public string BtnIcon { get; set; } = "";
        [Parameter] public string BtnType { get; set; } = "button";
        [Parameter] public bool BtnVisible { get; set; } = true;
        [Parameter] public EventCallback<bool> BtnOnClick { get; set; }
        [Parameter] public string BtnTitle { get; set; } = string.Empty;

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
			////EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
		}
		#endregion

		#region Methods

		#region HandleMethods

		private async Task HandleLanguageChanged()
		{
			StateHasChanged();
		}
		#endregion

		#region OthersMethods

        private async Task OnClickBtnMethod() {
            await BtnOnClick.InvokeAsync(true);
        }

		#endregion

		#endregion
	}
}