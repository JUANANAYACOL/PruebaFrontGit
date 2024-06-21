using Control.Endeavour.FrontEnd.Models.Enums.Components.Inputs;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;

namespace Control.Endeavour.FrontEnd.Components.Components.Inputs
{
    public partial class InputModalComponent : ComponentBase
    {
        #region Component Parameters

        // Indica si el campo es obligatorio.
        [Parameter] public bool IsRequired { get; set; } = false;

        // Texto de etiqueta del campo.
        [Parameter] public string LabelText { get; set; } = "Text not provided";

        // Placeholder del campo.
        [Parameter] public string Placeholder { get; set; } = "No text provided";

        // Tipo de validación para el campo.
        [Parameter] public InputModalTypeEnum FieldType { get; set; } = InputModalTypeEnum.None;

        [Parameter] public string Style { get; set; } = "";

        [Parameter] public bool IsVisible { get; set; } = true;
        [Parameter] public string InputType { get; set; } = "text";
        [Parameter] public int InputMaxLength { get; set; } = 1000;
        [Parameter] public int InputMinValue { get; set; } = 0;
        [Parameter] public bool IsDisabled { get; set; } = false;
        [Parameter] public string InputMaxNumberValue { get; set; } = "9999";
        [Parameter] public EventCallback<string> InputValueChanged { get; set; }
        [Parameter] public EventCallback<string> MethodValueChanged { get; set; }

        private Dictionary<string, object> GetInputAttributes()
        {
            var attributes = new Dictionary<string, object>
        {
            { "placeholder", Placeholder },
            { "value", InputValue } // Remueve @bind="InputValue" y maneja los cambios manualmente si es necesario
        };

            if (IsRequired)
            {
                attributes.Add("required", "required");
            }

            if (IsDisabled)
            {
                attributes.Add("disabled", "disabled");
            }

            return attributes;
        }

        #endregion Component Parameters

        #region Private Variables

        // Valor de entrada del campo.

        private bool IsValid()
        {
            if (Validate(FieldType, InputValue))
            {
                return true;
            }

            // Aquí puedes mostrar un mensaje de error específico para cada tipo de validación si lo deseas.
            return false;
        }

        private string _inputValue = string.Empty;

        [Parameter]
        public string InputValue
        {
            get => _inputValue;
            set
            {

                string newValue = value ?? string.Empty;

                if (FieldType == InputModalTypeEnum.Filing)
                {

                    newValue = new string(newValue.Where(char.IsDigit).Take(InputMaxLength).ToArray());
                }


                if (_inputValue != newValue)
                {
                    _inputValue = newValue;
                    InputValueChanged.InvokeAsync(newValue); 
                    MethodValueChanged.InvokeAsync(newValue); 
                }
            }
        }

        public bool IsInputValid
        {
            get
            {
                return IsValid();
            }
        }

        #endregion Private Variables

        #region Validation Methods

        // Subregión para microvalidaciones

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
            return Regex.IsMatch(value, "^[a-zA-Z0-9\\s]+$");
        }

        // Verifica si el valor es válido para un código.
        private static bool IsCodeValid(string value)
        {
            return value.Length <= 5 && Regex.IsMatch(value, "^[a-zA-Z0-9]+$");
        }

        // Verifica si el valor es una URL válida.
        private static bool IsWebsiteValid(string value)
        {
            return Uri.TryCreate(value, UriKind.Absolute, out _);
        }

        // Verifica si el valor es una dirección de correo electrónico válida.
        private static bool IsEmailValid(string value)
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(value);
                return address.Address == value;
            }
            catch
            {
                return false;
            }
        }

        // Subregión para la validación principal basada en el tipo de validación seleccionado.
        private static bool Validate(InputModalTypeEnum validationType, string value)
        {
            return validationType switch
            {
                InputModalTypeEnum.Filing => true,
                InputModalTypeEnum.None => true,
                InputModalTypeEnum.NotEmpty => IsNotEmpty(value),
                InputModalTypeEnum.Name => IsNameValid(value),
                InputModalTypeEnum.Code => IsCodeValid(value),
                InputModalTypeEnum.Website => IsWebsiteValid(value),
                InputModalTypeEnum.Email => IsEmailValid(value),
                InputModalTypeEnum.NumbersOnly => IsNumbersOnlyValid(value),
                _ => true,
            };
        }

        #endregion Validation Methods

        #region Component Methods

        // Limpia el campo estableciendo su valor en vacío.
        public void ClearField()
        {
            InputValue = string.Empty;
        }

        public void UpdateIsRequired(bool newValue)
        {
            IsRequired = newValue;
            StateHasChanged();
        }

        public void UpdateInputValue(string newValue)
        {
            InputValue = newValue;
            StateHasChanged();
        }

        public void UpdateIsDisabled(bool newValue)
        {
            IsDisabled = newValue;
        }

        #endregion Component Methods
    }
}