using Control.Endeavour.FrontEnd.Models.Enums.Generic;
using Control.Endeavour.FrontEnd.Services.Interfaces.Storage;
using Microsoft.JSInterop;
using System.Text.Json;

namespace Control.Endeavour.FrontEnd.Services.Services.Storage
{
    public class SessionStorageService : ISessionStorage
    {
        #region Atributos
        private readonly IJSRuntime JSRuntime;
        private readonly string tipoDeAlmacenamiento = "sessionStorage.";
        #endregion

        #region Constructor
        public SessionStorageService(IJSRuntime jSRuntime)
        {
            JSRuntime = jSRuntime;
        }
        #endregion

        #region Metodos
        /// <summary>
        /// El metodo realiza la limpieza de session storage.
        /// </summary>
        /// <returns></returns>
        public async Task ClearAll()
        {
            await JSRuntime.InvokeVoidAsync($"{tipoDeAlmacenamiento}clear").ConfigureAwait(false);
        }

        /// <summary>
        /// El metodo se encarga de obtener la información almacenada en la session storage segun la llave.
        /// </summary>
        /// <typeparam name="T">Valor de la llave con que almacena cierta informacion.</typeparam>
        /// <param name="key">Modelo o objeto que se deselizara.</param>
        /// <returns></returns>
        public async Task<T> GetValue<T>(ValuesKeysEnum key)
        {
            var data = await JSRuntime.InvokeAsync<string>($"{tipoDeAlmacenamiento}getItem", key.ToString()).ConfigureAwait(false);
            return data == null ? default : JsonSerializer.Deserialize<T>(data);
        }

        /// <summary>
        /// El metodo remuevo la información almacenada en el session storage.
        /// </summary>
        /// <param name="key">Valor de la llave con que almacena cierta informacion.</param>
        /// <returns></returns>
        public async Task RemoveItem(ValuesKeysEnum key)
        {
            await JSRuntime.InvokeVoidAsync($"{tipoDeAlmacenamiento}removeItem", key.ToString());
        }

        /// <summary>
        /// El metodo permita almacenar la información en session storage.
        /// </summary>
        /// <typeparam name="T">Modelo o objeto con la información cargada para almacenar.</typeparam>
        /// <param name="key">Valor de la llave con que almacena cierta informacion.</param>
        /// <param name="value">El valor, modelo o objeto con la información almacenada.</param>
        /// <returns></returns>
        public async Task SetValue<T>(ValuesKeysEnum key, T value)
        {
            await JSRuntime.InvokeVoidAsync($"{tipoDeAlmacenamiento}setItem", key.ToString(), JsonSerializer.Serialize(value)).ConfigureAwait(false);
        }

        /// <summary>
        /// Verifica si una clave existe en la session storage.
        /// </summary>
        /// <param name="key">La clave a verificar.</param>
        /// <returns>True si la clave existe, False si no.</returns>
        public async Task<bool> ContainsKey(ValuesKeysEnum key)
        {
            var data = await JSRuntime.InvokeAsync<string>($"{tipoDeAlmacenamiento}getItem", key.ToString()).ConfigureAwait(false);
            return data != null;
        }
        #endregion
    }
}
