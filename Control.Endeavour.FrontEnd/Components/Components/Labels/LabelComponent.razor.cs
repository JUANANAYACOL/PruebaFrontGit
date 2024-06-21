using Microsoft.AspNetCore.Components;
using System.Text;

namespace Control.Endeavour.FrontEnd.Components.Components.Labels
{
    public partial class LabelComponent
    {
        #region Dependencies
        //[Inject]
        //private EventAggregatorService EventAggregator { get; set; }
        #endregion

        #region Parameters
        [Parameter]
        public string KeyName { get; set; } = "Ingrese su info";

        [Parameter]
        public string? For { get; set; }

        [Parameter]
        public string AdditionalClasses { get; set; } = string.Empty;

        [Parameter]
        public string? FontSize { get; set; } = "16px"; // Valor predeterminado de 16px

        [Parameter]
        public bool IsBold { get; set; } = false; // No es negrita por defecto

        [Parameter]
        public bool IsCentered { get; set; } = false; // No está centrado por defecto
        #endregion

        #region Styling Methods
        private string GetCustomStyles()
        {
            var styles = new StringBuilder("color: #FFFFFF; font-family: Poppins;");

            if (!string.IsNullOrEmpty(FontSize))
            {
                styles.Append($" font-size:{FontSize};");
            }

            if (IsBold)
            {
                styles.Append(" font-weight: bold;");
            }

            return styles.ToString();
        }

        private string GetAdditionalClasses()
        {
            return IsCentered ? "text-center" : "";
        }

        private string GetCombinedClasses()
        {
            var classes = new StringBuilder("custom-label");
            classes.Append($" {GetAdditionalClasses()}");

            if (!string.IsNullOrEmpty(AdditionalClasses))
            {
                classes.Append($" {AdditionalClasses}");
            }

            return classes.ToString();
        }
        #endregion

        #region Language Handling
        protected override async Task OnInitializedAsync()
        {
            ////EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        //private string SetLabel(string value)
        //{
            //return DropDownLanguageComponent.GetText(value);
        //}
        #endregion
    }
}