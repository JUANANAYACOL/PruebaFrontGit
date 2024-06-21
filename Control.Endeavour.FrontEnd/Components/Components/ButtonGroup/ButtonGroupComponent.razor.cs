using Control.Endeavour.FrontEnd.Models.Enums.Components.Inputs;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;

namespace Control.Endeavour.FrontEnd.Components.Components.ButtonGroup
{
    public partial class ButtonGroupComponent
    {
        #region Variables

        #region Inject 
        //[Inject]
        //private EventAggregatorService? EventAggregator { get; set; }*/

        #endregion

        #region Parameters
        [Parameter] public string LabelText { get; set; } = "";
        [Parameter] public string BtnClassColor { get; set; } = "btnStyle--primary";
        [Parameter] public string BtnClassModifiers { get; set; } = "";
        [Parameter] public string BtnIcon { get; set; } = "";
        [Parameter] public string InputPlaceholder { get; set; } = "";
        [Parameter] public string BtnTitle { get; set; } = "";
        [Parameter] public bool InputRequired { get; set; } = false;
        [Parameter] public string InputValue
        {
            get => _inputValue;
            set
            {
                if (_inputValue != value)
                {
                    _inputValue = value;
                    InputValueChanged.InvokeAsync(value); // Notifica al componente padre
                    MethodValueChanged.InvokeAsync(value); // Notifica al componente padre

                }
            }
        }
        [Parameter] public EventCallback<string> InputValueChanged { get; set; }
        [Parameter] public EventCallback<string> MethodValueChanged { get; set; }
        [Parameter] public EventCallback<bool> BtnOnClick { get; set; }
        [Parameter] public InputModalTypeEnum FieldType { get; set; } = InputModalTypeEnum.None;

        #endregion

        #region Environments

        #region Environments(String)
        private string? _inputValue;
        #endregion

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
        private Dictionary<string, object> GetInputAttributes()
        {
            var attributes = new Dictionary<string, object>
            {
                { "placeholder", InputPlaceholder },
                { "value", InputValue }
            };

            if (InputRequired)
            {
                attributes.Add("required", "required");
            }

            return attributes;
        }

        private async Task OnClickBtnMethod()
        {
            await BtnOnClick.InvokeAsync(true);
        }
        public bool IsInputValid
        {
            get
            {
                return IsValid();
            }
        }
        private bool IsValid()
        {
            if (Validate(FieldType, InputValue))
            {
                return true;
            }

            // Aquí puedes mostrar un mensaje de error específico para cada tipo de validación si lo deseas.
            return false;
        }

        private static bool Validate(InputModalTypeEnum validationType, string value)
        {
            return validationType switch
            {
                InputModalTypeEnum.None => true,
                InputModalTypeEnum.NotEmpty => IsNotEmpty(value),
                InputModalTypeEnum.Name => IsNameValid(value),
                _ => true,
            };
        }

        #region ValidationMethods

        // Verifica si el valor contiene solo números.
        private static bool IsNumbersOnlyValid(string value)
        {
            return Regex.IsMatch(value, "^[0-9]+$");
        }

        // Verifica si el valor no está vacío.
        private static bool IsNotEmpty(string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        // Verifica si el valor contiene solo letras.
        private static bool IsNameValid(string value)
        {
            return Regex.IsMatch(value, "^[a-zA-Z\\s]+$");
        }

        #endregion

        #endregion

        #endregion
    }
}
