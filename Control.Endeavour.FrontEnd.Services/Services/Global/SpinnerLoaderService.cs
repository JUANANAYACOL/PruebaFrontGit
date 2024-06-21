using Microsoft.JSInterop;


namespace Control.Endeavour.FrontEnd.Services.Services.Global
{
    public class SpinnerLoaderService
    {
        public static async void ShowSpinnerLoader(IJSRuntime js)
        {
            await js.InvokeVoidAsync("ShowSpinnerLoader");
        }

        public static async void HideSpinnerLoader(IJSRuntime js)
        {
            await js.InvokeVoidAsync("HideSpinnerLoader");
        }
    }
}
