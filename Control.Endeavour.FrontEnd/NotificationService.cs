using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Microsoft.Extensions.Localization;

namespace Control.Endeavour.FrontEnd.Services.Services.Global
{
    public class NotificationService
    {
        public NotificationEvalutionDtoResponse NotificationDecrypt<T>(IStringLocalizer<Translation> Translation, HttpResponseWrapperModel<T> deseriaizationResponse)
        {
            try
            {
                NotificationEvalutionDtoResponse result = new();
                var succeed = deseriaizationResponse.Succeeded;

                string response;
                if (deseriaizationResponse.Errors != null && deseriaizationResponse.Errors.Any())
                {
                    var translatedErrors = deseriaizationResponse.Errors.Select(error => Translation[error]).ToArray();


                    response = string.Format(succeed ? Translation[$"{deseriaizationResponse.Message}"] : Translation[$"{deseriaizationResponse.CodeError}"], translatedErrors);
                }
                else
                {
                    response = succeed ? Translation[$"{deseriaizationResponse.Message}"] : Translation[$"{deseriaizationResponse.CodeError}"];
                }

                result = new()
                {
                    Succeed = succeed,
                    ModalType = succeed ? ModalType.Success : ModalType.Information,
                    NotificationMessage = response
                };
                return result;
            }
            catch (Exception ex)
            {
                return new()
                {
                    Succeed = deseriaizationResponse.Succeeded,
                    ModalType = deseriaizationResponse.Succeeded ? ModalType.Success : ModalType.Information,
                    NotificationMessage = string.IsNullOrEmpty(deseriaizationResponse.Message) ? deseriaizationResponse.CodeError : deseriaizationResponse.Message
                };
            }
        }
    }
}