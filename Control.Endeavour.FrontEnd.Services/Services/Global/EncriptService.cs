using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Services.Services.Global
{
    public class EncriptService
    {

        public static async void Encrypt(HttpClient? HttpClient, IJSRuntime js, object data, string keyName)
        {
            AppKeysFilterDtoRequest appKeysFilter = new();
            appKeysFilter.FunctionName = "ContenedorStorage";
            var responseApi = await HttpClient.PostAsJsonAsync("params/AppKeys/ByFilter", appKeysFilter);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<AppKeysDtoResponse>>>();

            if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
            {
                await js.InvokeVoidAsync("encryptDataReturn", data, keyName, deserializeResponse.Data[0].Value1);
            }
        }

        public static async Task<T> DecryptData<T>(HttpClient? HttpClient, IJSRuntime js, string keyName)
        {
            AppKeysFilterDtoRequest appKeysFilter = new();
            appKeysFilter.FunctionName = "ContenedorStorage";
            var responseApi = await HttpClient.PostAsJsonAsync("params/AppKeys/ByFilter", appKeysFilter);
            var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<AppKeysDtoResponse>>>();
            if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
            {
                return await js.InvokeAsync<T>("decryptDataReturn", keyName, deserializeResponse.Data[0].Value2);
            }
            else { throw new InvalidOperationException("No se pudo descifrar la respuesta correctamente."); }
        }
    }
}
