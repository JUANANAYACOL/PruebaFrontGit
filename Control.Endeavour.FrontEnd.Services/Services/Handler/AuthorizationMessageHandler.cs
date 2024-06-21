using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Newtonsoft.Json;

namespace Control.Endeavour.FrontEnd.Services.Services.Handler
{
    public class AuthorizationMessageHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
               
                response.StatusCode = HttpStatusCode.OK;



                response.Content =
                    new StringContent(JsonConvert.SerializeObject(new HttpResponseWrapperModel<object>() { Succeeded = false, Message=string.Empty,CodeError=null, Errors=null})); 
               
                return response;
            }
            return response;
        }
    }
}
