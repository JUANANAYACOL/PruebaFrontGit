using Control.Endeavour.FrontEnd.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Control.Endeavour.FrontEnd.Components.Components.Switch;


public partial class SwitchComponent : ComponentBase
{
    [Inject]
    private IStringLocalizer<Translation>? Translation { get; set; }

    #region Parameters
    [Parameter] public string Width { get; set; } = "60px";
    [Parameter] public string Height { get; set; } = "30px";
    [Parameter] public string BackgroundColorDisabled { get; set; } = "#D1D1D6"; // Color de fondo cuando esta inactivo el switch
    [Parameter] public string BackgroundColorActive { get; set; } = "#2196F3"; // Color de fondo cuando esta activo el switch
    [Parameter] public string TextStateActive { get; set; } = "Enabled"; // Palabra que aparece a la derecha del switch cuando esta activo
    [Parameter] public string TextStateDisabled { get; set; } = "Disabled"; // Palabra que aparece a la derecha del switch cuando esta inactivo
    [Parameter] public string ColorTextState { get; set; } = "#424242"; // Color de Palabra que aparece a la derecha del 
    [Parameter] public string SizeTextState { get; set; } = "14px"; // Tamaño de Palabra que aparece a la derecha del switch
    [Parameter] public string FontFamilyTextState { get; set; } = "Roboto"; // Font-family de Palabra que aparece a la derecha del switch
    [Parameter] public bool ShowText { get; set; } // True si se quiere mostrar las 2 palabras dentro del switch
    [Parameter] public string ValueYes { get; set; } = "Sí"; // Palabra cuando está activo dentro del switch
    [Parameter] public string ValueNot { get; set; } = "No"; // Palabra cuando está desactivado dentro del switch
    [Parameter] public string ColorValueYes { get; set; } = "#FFFFFF"; // Color de Palabra cuando está activo dentro del switch
    [Parameter] public string ColorValueNot { get; set; } = "#FFFFFF"; // Color de Palabra cuando está desactivado dentro del switch
    [Parameter] public string SizeValueYes { get; set; } = "14px"; // Tamaño de Palabra cuando está activo dentro del switch
    [Parameter] public string SizeValueNot { get; set; } = "14px"; // Tamaño de Palabra cuando está desactivado dentro del switch
    [Parameter] public string FontFamilyValueYes { get; set; } = "Roboto"; // Font-family de Palabra cuando está activo dentro del switch
    [Parameter] public string FontFamilyValueNot { get; set; } = "Roboto"; // Font-family de Palabra cuando está desactivado dentro del switch
    [Parameter] public bool CurrentValue { get; set; }
    [Parameter] public EventCallback<bool> CurrentValueChanged { get; set; }
    [Parameter] public EventCallback<bool> MethodValueChanged { get; set; }

    #endregion Parameters

    private async Task ToggleSwitch()
    {
        CurrentValue = !CurrentValue;
        await CurrentValueChanged.InvokeAsync(CurrentValue); // Mantiene la lógica original para notificar el cambio de valor
        await MethodValueChanged.InvokeAsync(CurrentValue);  // Notifica al componente padre cuando cambia el valor
        StateHasChanged();
    }
}