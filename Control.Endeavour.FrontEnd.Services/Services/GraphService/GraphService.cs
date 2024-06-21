using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Services.Interfaces.DriveService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Services.Services.DriveService
{
    public class GraphService : IGraphService
    {
        public async Task<string> GetAccessTokenAsync(string clientId, string clientSecret, string tenantId)
        {
            try
            {
                var authorityUri = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
                var resource = "https://graph.microsoft.com/.default";

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                    var formData = new Dictionary<string, string>
                    {
                        { "client_id", clientId },
                        { "client_secret", clientSecret },
                        { "grant_type", "client_credentials" },
                        { "scope", resource },
                    };

                    HttpContent contentHttp = new FormUrlEncodedContent(formData);

                    var response = await httpClient.PostAsync(authorityUri, contentHttp);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        dynamic tokenResponse = JsonConvert.DeserializeObject(content);
                        var accessToken = tokenResponse.access_token;

                        return accessToken;
                    }
                    else
                    {
                        Console.WriteLine($"Error al obtener el token de acceso: {response.ReasonPhrase}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el token de acceso: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Value>> GetUserDocs(string objectId, string token)
        {
            List<Value> documents = new();
            try
            {

                var graphApiUrl = $"https://graph.microsoft.com/v1.0/users/{objectId}/drive/root/children";

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await httpClient.GetAsync(graphApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponseCountry = await response.Content.ReadAsStringAsync();
                        var responseCountryList = System.Text.Json.JsonSerializer.Deserialize<GraphFolderDtoResponse>(jsonResponseCountry);

                        var result = await GetFoldersByUser(responseCountryList, objectId, token);

                        return result;
                    }
                    else
                    {
                        Console.WriteLine($"Error al enviar el correo: {response.ReasonPhrase}");
                        return documents;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo: {ex.Message}");
                return documents;
            }
        }

        public async Task<List<Value>> GetFoldersByUser(GraphFolderDtoResponse folder, string objectId, string token)
        {
            List<Value> documents = new();

            try
            {
                string key = folder.value[3].cTag;
                string pattern = @"{([^}]*)}";
                string itemId = "";

                Match match = Regex.Match(key, pattern);
                if (match.Success)
                {
                    Console.WriteLine("Dato encontrado: " + match.Groups[1].Value);
                    itemId = match.Groups[1].Value;
                }
                else
                {
                    Console.WriteLine("No se encontró el dato.");
                }

                var graphApiUrl = $"https://graph.microsoft.com/v1.0/users/{objectId}/drives/{folder.value[3].parentReference.driveId}/items/{itemId}/children";

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await httpClient.GetAsync(graphApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponseCountry = await response.Content.ReadAsStringAsync();
                        var responseCountryList = System.Text.Json.JsonSerializer.Deserialize<GraphDocumentDtoResponse>(jsonResponseCountry);
                        documents = responseCountryList.value;
                        return documents;
                    }
                    else
                    {
                        Console.WriteLine($"Error al enviar el correo: {response.ReasonPhrase}");
                        return documents;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo: {ex.Message}");
                return documents;
            }
        }

        public async Task<List<Value>> getGroupDocuments(string groupId, string token)
        {
            List<Value> documents = new();
            try
            {
                var graphApiUrl = $"https://graph.microsoft.com/v1.0/groups/{groupId}/drive/root/children";

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await httpClient.GetAsync(graphApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var responseData = System.Text.Json.JsonSerializer.Deserialize<GraphFolderDtoResponse>(jsonResponse);

                        var result = await getFolderInfo(responseData, groupId, token);

                        return result;
                    }
                    else
                    {
                        Console.WriteLine($"Error al enviar el correo: {response.ReasonPhrase}");
                        return documents;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo: {ex.Message}");
                return documents;
            }
        }

        public async Task<List<Value>> getFolderInfo(GraphFolderDtoResponse folder, string groupId, string token)
        {
            List<Value> documents = new();

            try
            {
                string key = folder.value[0].cTag;
                string pattern = @"{([^}]*)}";
                string folderId = "";

                Match match = Regex.Match(key, pattern);
                if (match.Success)
                {
                    Console.WriteLine("Dato encontrado: " + match.Groups[1].Value);
                    folderId = match.Groups[1].Value;
                }
                else
                {
                    Console.WriteLine("No se encontró el dato.");
                }

                var graphApiUrl = $"https://graph.microsoft.com/v1.0/groups/{groupId}/drive/items/{folderId}/children";

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await httpClient.GetAsync(graphApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var responseData = System.Text.Json.JsonSerializer.Deserialize<GraphDocumentDtoResponse>(jsonResponse);

                        documents = await getFolderData(responseData, groupId, token);
                        return documents;
                    }
                    else
                    {
                        Console.WriteLine($"Error al enviar el correo: {response.ReasonPhrase}");
                        return documents;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo: {ex.Message}");
                return documents;
            }
        }

        public async Task<List<Value>> getFolderData(GraphDocumentDtoResponse folder, string groupId, string token)
        {
            List<Value> documents = new();

            try
            {
                string key = folder.value[4].cTag;
                string pattern = @"{([^}]*)}";
                string tagFolder = "";

                Match match = Regex.Match(key, pattern);
                if (match.Success)
                {
                    Console.WriteLine("Dato encontrado: " + match.Groups[1].Value);
                    tagFolder = match.Groups[1].Value;
                }
                else
                {
                    Console.WriteLine("No se encontró el dato.");
                }

                var graphApiUrl = $"https://graph.microsoft.com/v1.0/groups/{groupId}/drive/items/{tagFolder}/children";

                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await httpClient.GetAsync(graphApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponseCountry = await response.Content.ReadAsStringAsync();
                        var responseCountryList = System.Text.Json.JsonSerializer.Deserialize<GraphDocumentDtoResponse>(jsonResponseCountry);
                        documents = responseCountryList.value;
                        return documents;
                    }
                    else
                    {
                        Console.WriteLine($"Error al enviar el correo: {response.ReasonPhrase}");
                        return documents;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar el correo: {ex.Message}");
                return documents;
            }
        }
    }
}
