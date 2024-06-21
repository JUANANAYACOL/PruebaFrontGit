using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.RegularExpressions;

namespace Control.Endeavour.FrontEnd.Components.Components.Inputs
{
    public partial class CodeInputComponent : ComponentBase
    {
        #region Variables
        #region Inject 
        [Inject]
        protected IJSRuntime? JSRuntime { get; set; }
        #endregion

        #region Parameter
        [Parameter]
        public EventCallback<bool> OnValidation { get; set; }
        #endregion

        #region Entorno
        private string inputValue = "";
        public bool IsInvalid { get; private set; }

        private string? displayValue;
        public string DisplayValue
        {
            get => displayValue;
            set
            {
                if (value != null)
                {
                    // Trunca el valor para asegurarse de que no exceda los 11 caracteres deseados
                    string truncatedValue = new string(value.Take(11).ToArray());

                    // Elimina los guiones y guarda el valor real
                    InputValue = new string(truncatedValue.Where(char.IsLetterOrDigit).ToArray());

                    // Añade guiones para la presentación visual
                    displayValue = Regex.Replace(InputValue, ".{1}", "$0-").TrimEnd('-');
                }
            }
        }
        public string InputValue
        {
            get => inputValue;
            set
            {
                if (inputValue != value)
                {
                    inputValue = value;
                }
            }
        }
        #endregion
        #endregion

        #region Validation
        public void ValidateInput()
        {
            ValidateCode();
            OnValidation.InvokeAsync(IsInvalid);
        }

        private void ValidateCode()
        {
            IsInvalid = inputValue.Length != 6;
        }

        private void ResetValidation()
        {
            IsInvalid = false;
        }
        #endregion

        #region Methods
        public void HandleInput(ChangeEventArgs e)
        {
            DisplayValue = e.Value?.ToString().ToUpper();
            StateHasChanged();
        }
        public void Reset()
        {
            DisplayValue = ""; 
            IsInvalid = false;
            StateHasChanged();
        }

        #endregion
    }
}
