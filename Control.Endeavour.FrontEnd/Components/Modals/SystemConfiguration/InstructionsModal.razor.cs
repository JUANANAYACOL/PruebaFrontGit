using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.SystemConfiguration
{
    public partial class InstructionsModal
    {

        #region Variables

        #region Inject 
        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }
        [Inject]
        private IJSRuntime Js { get; set; }
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion

        #region Components


        #endregion

        #region Modals
        private NotificationsComponentModal notificationModal = new();

        #endregion

        #region Parameters
        [Parameter] public EventCallback<bool> OnChangeData { get; set; }


        #endregion

        #region Models
        private InstructionsDtoResponse instruccionDtoResponse = new();
        private InstructionsDtoResponse instructionDtoResponseEdit = new();

        #endregion

        #region Environments

        #region Environments(String)
        
        private string ClassNameText = "SelectAnOption";
        

        #endregion

        #region Environments(Numeric)


        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool IsEditForm = false;
        private bool modalStatus = false;
        private bool enableButton = true;

        #endregion

        #region Environments(List & Dictionary)

        private List<VSystemParamDtoResponse> classCodeList = new();
        private List<VSystemParamDtoResponse> flowStateList = new();
        private List<VSystemParamDtoResponse> originList = new();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            try
            {
                await GetClasses();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al realizar la inicialización: {ex.Message}");
            }

        }


        #endregion

        #region Methods

        #region HandleMethods
        private void HandleModalClosed(bool status)
        {

            modalStatus = status;
            instruccionDtoResponse = new();
            ClassNameText = "SelectAnOption";
            IsEditForm = false;
            enableButton = true;
            ResetFormAsync();
            StateHasChanged();
        }

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
            }


        }
        #region FormMethods
        private async Task HandleValidSubmit()
        {
            // Lógica de envío del formulario
            if (IsEditForm)
            {
                await HandleFormUpdate();
            }
            else
            {
                await HandleFormCreate();
            }

            StateHasChanged();

        }
        private async Task HandleFormCreate()
        {
            try
            {
                if (ValidateFields())
                {
                    
                    var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/Instruction/CreateInstruction", instruccionDtoResponse);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<InstructionsDtoResponse>>();

                    if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                    {
                        notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                        await OnChangeData.InvokeAsync(true);
                        IsEditForm = false;
                        ResetFormAsync();
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
                    }
                }

            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }
        }
        private async Task HandleFormUpdate()
        {
            try
            {
                if (ValidateFields())
                {
                    
                    var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/Instruction/UpdateInstruction", instruccionDtoResponse);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<InstructionsDtoResponse>>();

                    if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                    {
                        notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                        await OnChangeData.InvokeAsync(true);
                        IsEditForm = false;
                        ResetFormAsync();
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
                    }
                }

            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }

        }
        private async Task ResetFormAsync()
        {
            if (!IsEditForm)
            {
                instruccionDtoResponse = new();
            }
            else
            {
                //instruccionDtoResponse = instructionDtoResponseEdit;
                instruccionDtoResponse = new() { 
                    InstructionId = instructionDtoResponseEdit.InstructionId,
                    Name = instructionDtoResponseEdit.Name,
                    ClassCode = instructionDtoResponseEdit.ClassCode,
                    ActiveState = instructionDtoResponseEdit.ActiveState
                
                };
            }
        }
        #endregion

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        #endregion

        #region OthersMethods

        #region ValidateFields
        private bool ValidateFields()
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(instruccionDtoResponse.Name))
            {
                errors.Add(string.Format(Translation["CharacterEmptyValidation"], Translation["Name"]));
            }

            if (string.IsNullOrEmpty(instruccionDtoResponse.ClassCode))
            {
                errors.Add(string.Format(Translation["CharacterEmptyValidation"], Translation["Class"]));
            }


            if (errors.Count > 0)
            {
                var message = string.Join(" ", errors);
                notificationModal.UpdateModal(ModalType.Error, Translation["TheFollowingFieldsHaveErrors"] + "\n" + message, true);
                return false;
            }

            return true;
        }
        public void ValidateEnableButton()
        {
            
            enableButton = string.IsNullOrWhiteSpace(instruccionDtoResponse.Name) ||
                           string.IsNullOrWhiteSpace(instruccionDtoResponse.ClassCode);

            
        }


        #endregion

        #region GetData

        private async Task GetClasses()
        {
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "CL");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    classCodeList = deserializeResponse.Data;
                }
                else { classCodeList = new(); }

            }
            catch (Exception ex)
            {

                notificationModal.UpdateModal(ModalType.Error, $"Error al obtener los tipos de clase: {ex.Message}", true);
            }
        }
        
        

        

        #endregion



        #region ModalMethods

        public async void ReceiveRecord(InstructionsDtoResponse response)
        {
            IsEditForm = true;
            instruccionDtoResponse.InstructionId = response.InstructionId;
            instruccionDtoResponse.Name = response.Name;
            instruccionDtoResponse.ClassCode = response.ClassCode;
            ClassNameText = "SelectAnOption";
            instruccionDtoResponse.ActiveState = response.ActiveState;
            instructionDtoResponseEdit = new() { 
                InstructionId = response.InstructionId,
                Name = response.Name,
                ClassCode = response.ClassCode,
                ActiveState = response.ActiveState
            };
        }
        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }
        #endregion

        #endregion

        #endregion

    }
}
