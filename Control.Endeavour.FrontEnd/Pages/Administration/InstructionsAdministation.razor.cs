
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.SystemConfiguration;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.SvgIcons;


namespace Control.Endeavour.FrontEnd.Pages.Administration
{
	public partial class InstructionsAdministation
	{

        #region Variables

        #region Inject 
        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private IJSRuntime Js { get;set; }
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion

        #region Components



        #endregion

        #region Modals
        private NotificationsComponentModal notificationModal = new();
        private InstructionsModal instructionsModal = new();

        #endregion

        #region Parameters


        #endregion

        #region Models

        private InstructionsDtoResponse recordToDelete = new();
        private InstructionsFilterDtoRequest instructionsFilter = new();


        #endregion

        #region Environments

        #region Environments(String)

        #endregion

        #region Environments(Numeric)

        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        #endregion

        #region Environments(List & Dictionary)
        private List<InstructionsDtoResponse> instructionsList = new();
        private List<VSystemParamDtoResponse> classCodeList = new();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            try
            {
                await GetInstructions();
                await GetClasses();
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }


        }


        #endregion

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }
        private async Task HandleRefreshGridData(bool refresh)
        {
            await GetInstructions();
        }
        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            try
            {
                if (args.IsAccepted && args.ModalOrigin.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new();
                    request.Id = (recordToDelete.InstructionId.HasValue) ? recordToDelete.InstructionId.Value : 0;
                    request.User = "Front"; // Cambiar por la varibale de session del usuario
                    var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/Instruction/DeleteInstruction", request);

                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse.Succeeded)
                    {
                        await GetInstructions();
                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            notificationModal.UpdateModal(ModalType.Success, "¡Se ha eliminado la instrucción!", true);
                        }
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true, Translation["Accept"]);
                    }
                }
            }
            catch (Exception ex)
            {
                notificationModal.UpdateModal(ModalType.Error, ex.Message, true);
            }

        }
        #endregion

        #region OthersMethods

        #region ModalMethods

        private void ShowModalCreate()
        {
            instructionsModal.UpdateModalStatus(true);
        }
        private void ShowModalEdit(InstructionsDtoResponse record)
        {
            instructionsModal.UpdateModalStatus(true);
            instructionsModal.ReceiveRecord(record);
        }
        private void ShowModalDelete(InstructionsDtoResponse record)
        {
            recordToDelete = record;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, @Translation["Yes"], @Translation["No"], modalOrigin: "DeleteModal");
        }

        #endregion

        #region GetData

        
        private async Task GetInstructions()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                var responseApi = await HttpClient!.PostAsJsonAsync("documentmanagement/Instruction/ByFilter", instructionsFilter);

                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<InstructionsDtoResponse>>>();
                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null)
                {
                    instructionsList = deserializeResponse.Data;
                }
                else
                {
                    instructionsList = new List<InstructionsDtoResponse>();
                    notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores en caso de fallo al obtener las sucursales.
                Console.WriteLine($"Error al obtener las sucursales: {ex.Message}");
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }
        private async Task GetClasses()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
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

                notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion

        #region FilterMethods

        private async Task CleanFilter()
        {
            instructionsFilter = new();
            await GetInstructions();

        }
        private async Task SearchByFilter()
        {
            if (!string.IsNullOrEmpty(instructionsFilter.Name) || !string.IsNullOrEmpty(instructionsFilter.ClassCode))
            {

                await GetInstructions();

            }
            else
            {
                notificationModal.UpdateModal(ModalType.Warning, Translation["EnterParameterSearch"], true, buttonTextCancel: "");
            }
        }

        #endregion
        #endregion
        #endregion


    }
}
