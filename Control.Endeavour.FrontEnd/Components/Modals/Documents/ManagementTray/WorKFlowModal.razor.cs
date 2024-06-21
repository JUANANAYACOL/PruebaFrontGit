using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdParty.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.ThirdUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using System.Reflection.Metadata;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.ManagementTray;
public partial class WorKFlowModal : ComponentBase
{
    #region Variables

    #region Inject

    /*[Inject]
    private EventAggregatorService? EventAggregator { get; set; }*/

    [Inject]
    private IJSRuntime Js { get; set; }

    [Inject]
    private HttpClient? HttpClient { get; set; }
    [Inject]
    private IStringLocalizer<Translation>? Translation { get; set; }

    #endregion Inject

    #region Parameters
    [Parameter] public string? Title { get; set; }
    private bool modalStatus { get; set; } = false;

    #endregion Parameters

    #region Models
    private WorKFlowDtoResponse worKFlowDtoResponse = new();
    private MetaModel meta { get; set; } = new() { PageSize = 10 };
    private NotificationsComponentModal notificationModal { get; set; } = new();
    #endregion Models

    #region Environments
    private int controlId { get; set; } = 0;
    private bool dataChargue { get; set; } = false;

    private bool isGenerationDocumentVisible = false;

    private List<DocumentWorkFlowDtoResponse> documentWorkFlowLst = new();

    #endregion Environments



    #endregion Variables

    #region OnInitializedAsync

    protected override async Task OnInitializedAsync()
    {
        Title = Translation["DocumentManagementFlow"];
        //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
    }

    #endregion OnInitializedAsync

    #region Methods

    #region HandleMethods

    private async Task HandleLanguageChanged()
    {
        StateHasChanged();
    }
    private void HandleModalClosed(bool value)
    {
        UpdateModalStatus(value);
        isGenerationDocumentVisible = false;
    }
    private void HandleModalNotiClose(ModalClosedEventArgs args)
    {
        UpdateModalStatus(args.ModalStatus);
    }
    #endregion HandleMethods

    #region MethodsGeneral
    public void UpdateModalStatus(bool newValue)
    {
        modalStatus = newValue;
        StateHasChanged();
    }

    #endregion MethodsGeneral

    #region MethodsAsync
    async Task GetWorKFlowAsync()
    {
        try
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            if (controlId > 0)
            {
                HttpClient?.DefaultRequestHeaders.Remove("ControlId");
                HttpClient?.DefaultRequestHeaders.Add("ControlId", $"{controlId}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<WorKFlowDtoResponse>>("documentmanagement/DocumentManagement/ByFilterControlId").ConfigureAwait(false);
                HttpClient?.DefaultRequestHeaders.Remove("ControlId");
                if (deserializeResponse!.Succeeded)
                {
                    worKFlowDtoResponse = deserializeResponse.Data!;
                    HttpClient?.DefaultRequestHeaders.Remove("ControlId");
                    HttpClient?.DefaultRequestHeaders.Add("ControlId", $"{controlId}");
                    var validateResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<bool>>("documentarytasks/DocumentaryTask/ValidateDocumentConnection").ConfigureAwait(false);
                    HttpClient?.DefaultRequestHeaders.Remove("ControlId");
                    if(validateResponse!.Succeeded)
                    {
                        isGenerationDocumentVisible = validateResponse.Data;
                        if(isGenerationDocumentVisible)
                        {
                            DocumentWorkFlowFilterDtoRequest filter = new()
                            {
                                ControlId = controlId,
                                TaskId = null
                            };
                            var responseApi = await HttpClient.PostAsJsonAsync("documentarytasks/DocumentaryTask/GetWorkFlow", filter);
                            var responseApideserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<DocumentWorkFlowDtoResponse>>>();
                            if (responseApideserializeResponse.Succeeded && responseApideserializeResponse.Data.Count > 0)
                            {
                                documentWorkFlowLst = responseApideserializeResponse.Data;
                            }
                            else
                            {
                                notificationModal.UpdateModal(ModalType.Information, Translation["LoadErrorMessageS"], true, buttonTextCancel: string.Empty);
                            }

                        }
                    }
                }
            }
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }
        catch (Exception ex)
        {
            notificationModal.UpdateModal(ModalType.Warning, Translation["Accept"] + $": {ex.Message}", true, Translation["Accept"], Translation["Cancel"]);
            Console.WriteLine(Translation["Accept"] + $": {ex.Message}");
        }
    }
    public async Task WorKFlowAsync(int Idcontrol)
    {
        dataChargue = false;
        controlId = Idcontrol;
        await GetWorKFlowAsync();
        StateHasChanged();
    }
    #endregion MethodsGeneral

    #endregion Methods

}
