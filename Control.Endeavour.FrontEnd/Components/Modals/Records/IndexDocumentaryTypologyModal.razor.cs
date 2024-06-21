using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.MetaData.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Records.Request;
using Control.Endeavour.FrontEnd.Models.Models.Records.Response;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using OpenAI_API.Models;
using OpenAI_API.Moderation;
using System.Net.Http.Json;
using System.Security.Cryptography;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Components.Modals.Records
{
    public partial class IndexDocumentaryTypologyModal
    {
        #region Variables

        #region Inject 

        [Inject] private IJSRuntime Js { get; set; }
        [Inject] private HttpClient? HttpClient { get; set; }
        [Inject] private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion

        #region Components
        private InputModalComponent inputStartFolio { get; set; } = new();
        private InputModalComponent inputEndFolio { get; set; } = new();
        private string Start_Folio { get; set; } = string.Empty;
        private string End_Folio { get; set; } = string.Empty;
        private string Comment { get; set; } = string.Empty;

        #endregion

        #region Modals

        private NotificationsComponentModal NotificationModal = new();

        #endregion

        #region Parameters

        [Parameter] public EventCallback<MyEventArgs<DocumentaryTypologyTDMDtoRequest>> OnStatusChanged { get; set; } = new();

        #endregion

        #region Models

        IndexDocumentaryTypologyDtoRequest IndexDocumentaryTypologyDtoRequest = new();
        IndexDocumentaryTypologyUpdateDtoRequest _IndexDocumentaryTypologyUpdateDtoRequest = new();
        DocumentaryTypologyTDMDtoRequest DocumentaryTypologyTDMDtoRequest = new();

        #endregion

        #region Environments

        #region Environments(String)
        private string Color { get; set; } = "";
        private string Options { get; set; } = "";
        private string PanelTypeOptions { get; set; } = "d-none";
        private string PanelDescriptionOrigin { get; set; } = "d-none";
        private string IndexingType { get; set; } = "";
        #endregion

        #region Environments(Numeric)
        private decimal contadorcarac = 0;
        private decimal contadorcarac2 = 0;
        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)

        private bool ModalStatus = false;
        private bool Complete { get; set; } = false;
        private bool Absent { get; set; } = false;
        private bool Incomplete { get; set; } = false;
        private bool _Incomplete { get; set; } = false;
        private bool None { get; set; } = false;
        private bool IsEmail { get; set; } = false;
        private bool IsInformationSystem { get; set; } = false;
        private bool IsOther { get; set; } = false;
        private bool IsDisabledStartFolio { get; set; } = false;
        private bool IsDisabledEndFolio { get; set; } = false;

        #endregion

        #region Environments(List & Dictionary)

        private List<VSystemParamDtoResponse>? lstOrigin { get; set; } = new();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
		{
            await GetOrigin();
        }
        #endregion

        #region Methods

        #region HandleMethods

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if(args.ModalOrigin == "MDC,NE" || args.ModalOrigin == "CI" || args.ModalOrigin == "UI")
            {
                if (args.IsAccepted && args.ModalOrigin == "MDC,NE")
                {
                    Color = "MDC,NE";
                    IndexDocumentaryTypologyDtoRequest.Color = Color;
                    None = _Incomplete;
                    Absent = false;
                    Complete = false;
                    Incomplete = false;
                    Start_Folio = "0";
                    End_Folio = "0";
                }
            }
            else
            {
                await HandleModalClosedAsync(args.ModalStatus);
            }
        }

        private async Task HandleModalClosedAsync(bool status)
        {
            var result = new MyEventArgs<DocumentaryTypologyTDMDtoRequest>()
            {
                Data = DocumentaryTypologyTDMDtoRequest,
                ModalStatus = status
            };

            await OnStatusChanged.InvokeAsync(result);
            ModalStatus = status;
            StateHasChanged();
        }

        private void HandleCheckBoxes(bool newValue, int checkBoxCase)
        {
            switch (checkBoxCase)
            {
                case 1:
                    Color = "MDC,V";
                    IndexDocumentaryTypologyDtoRequest.Color = Color;
                    Complete = newValue;
                    Absent = false;
                    Incomplete = false;
                    None = false;
                    break;

                case 2:
                    Color = "MDC,AZ";
                    IndexDocumentaryTypologyDtoRequest.Color = Color;
                    Absent = newValue;
                    Complete = false;
                    Incomplete = false;
                    None = false;
                    break;

                case 3:
                    Color = "MDC,A";
                    IndexDocumentaryTypologyDtoRequest.Color = Color;
                    Incomplete = newValue;
                    Absent = false;
                    Complete = false;
                    None = false;
                    break;
                case 4:
                    _Incomplete = newValue;
                    NotificationModal.UpdateModal(ModalType.Warning, Translation["NotApplicableMessage"], true, Translation["Yes"], Translation["No"], modalOrigin: "MDC,NE");
                    break;
            }
        }

        private void HandleCheckTypeOptions(bool newValue, string checkBoxCase)
        {
            if (newValue)
            {
                switch (checkBoxCase)
                {
                    case "OPORI,CEL":
                        Options = "OPORI,CEL";
                        IsEmail = newValue;
                        IsInformationSystem = false;
                        IsOther = false;
                        PanelDescriptionOrigin = "";
                        break;

                    case "OPORI,SIF":
                        Options = "OPORI,SIF";
                        IsInformationSystem = newValue;
                        IsEmail = false;
                        IsOther = false;
                        PanelDescriptionOrigin = "";
                        break;

                    case "OPORI,OTR":
                        Options = "OPORI,OTR";
                        IsOther = newValue;
                        IsInformationSystem = false;
                        IsEmail = false;
                        PanelDescriptionOrigin = "";
                        break;
                    default:
                        PanelDescriptionOrigin = "d-none";
                        IsEmail = false;
                        IsInformationSystem = false;
                        IsOther = false;
                        break;
                }
            }
            else
            {
                PanelDescriptionOrigin = "d-none";
                Options = "";
                IsEmail = false;
                IsInformationSystem = false;
                IsOther = false;
            }
        }

        private async Task HandleSave()
        {
            if (DocumentaryTypologyTDMDtoRequest.DocumentId != 0)
            {
                _IndexDocumentaryTypologyUpdateDtoRequest.DocumentId = DocumentaryTypologyTDMDtoRequest.DocumentId;
                await UpdateIndexation();
            }
            else
            {
                await CreateIndexing();
            }
        }

        #endregion

        #region GetMethods
        private async Task GetOrigin()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");
                HttpClient?.DefaultRequestHeaders.Add("paramCode", "INDOR");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("paramCode");

                if (deserializeResponse!.Succeeded)
                {
                    lstOrigin = deserializeResponse.Data;
                }
                else { lstOrigin = new(); }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los origenes: {ex.Message}");
            }
        }

        #endregion

        #region OthersMethods

        #region ModalMethods
        public async Task UpdateModalStatusAsync(bool newValue)
        {
            ModalStatus = newValue;
            StateHasChanged();
        }

        public async Task GetValue(IndexDocumentaryTypologyDtoRequest model, int DocumentId, string _IndexingType)
        {
            try
            {
                IndexDocumentaryTypologyDtoRequest.VolumeId = model.VolumeId;
                DocumentaryTypologyTDMDtoRequest.VolumeId = model.VolumeId;
                DocumentaryTypologyTDMDtoRequest.DocumentId = DocumentId;
                IndexDocumentaryTypologyDtoRequest.DocumentaryTypologyId = model.DocumentaryTypologyId;
                IndexDocumentaryTypologyDtoRequest.StartFolio = model.StartFolio;
                IndexDocumentaryTypologyDtoRequest.EndFolio = model.EndFolio;
                Start_Folio = model.StartFolio.ToString();
                End_Folio = model.EndFolio.ToString();

                if(_IndexingType == "AU")
                {
                    IsDisabledStartFolio = true;
                    IsDisabledEndFolio = true;
                    IndexingType = _IndexingType;
                }
                else
                {
                    IsDisabledStartFolio = false;
                    IsDisabledEndFolio = false;
                    IndexingType = _IndexingType;
                }
                contadorcarac = model.Observation.Length;
                IndexDocumentaryTypologyDtoRequest.Observation = model.Observation;
                IndexDocumentaryTypologyDtoRequest.Color = model.Color;
                int idColor = model.Color == "MDC,V" ? 1 : model.Color == "MDC,AZ" ? 2 : model.Color == "MDC,A" || model.Color == "MDC,R" ? 3 : 4;
                HandleCheckBoxes(true, idColor);
                IndexDocumentaryTypologyDtoRequest.Origin = model.Origin;
                IndexDocumentaryTypologyDtoRequest.Options = model.Options == null ? "" : model.Options;
                
                if (model.Options != null) { 
                    HandleCheckTypeOptions(true, model.Options);
                }
                
                if(model.Origin == "INDOR,EL")
                {
                    PanelTypeOptions = "";
                    PanelDescriptionOrigin = "";
                }
                else
                {
                    PanelTypeOptions = "d-none";
                    PanelDescriptionOrigin = "d-none";
                }
                model.OptionValue = model.OptionValue == null ? "" : model.OptionValue;
                contadorcarac2 = model.OptionValue.Length;
                IndexDocumentaryTypologyDtoRequest.OptionValue = model.OptionValue;
            }
            catch (Exception ex)
            {
                NotificationModal.UpdateModal(ModalType.Error, Translation["ProcessErrorMessage"], true);
            }
        }

        #endregion

        #region ValidationMethods
        private void ContarCaracteres(ChangeEventArgs e, String procedencia = "")
        {
            String value = e.Value.ToString() ?? String.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                if(procedencia == "ORIGEN")
                {
                    contadorcarac2 = value.Length;
                }
                else
                {
                    contadorcarac = value.Length;
                }                
            }
            else
            {
                if (procedencia == "ORIGEN")
                {
                    contadorcarac2 = 0;
                }
                else
                {
                    contadorcarac = 0;
                }
            }
        }
        #endregion

        private void ActivarPanel(string value)
        {
            IndexDocumentaryTypologyDtoRequest.Origin = value;

            if (!string.IsNullOrEmpty(value) && value == "INDOR,EL")
            {
                PanelTypeOptions = "";
            }
            else
            {
                PanelTypeOptions = "d-none";
                PanelDescriptionOrigin = "d-none";
                IsEmail = false;
                IsInformationSystem = false;
                IsOther = false;
            }
        }

        private async Task CreateIndexing()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                bool _continue = true;
                IndexDocumentaryTypologyDtoRequest.StartFolio = Convert.ToInt16(Start_Folio);
                IndexDocumentaryTypologyDtoRequest.EndFolio = Convert.ToInt16(End_Folio);
                IndexDocumentaryTypologyDtoRequest.Color = Color;
                IndexDocumentaryTypologyDtoRequest.Options = String.IsNullOrEmpty(Options) ? "" : Options;
                IndexDocumentaryTypologyDtoRequest.OptionValue = String.IsNullOrEmpty(IndexDocumentaryTypologyDtoRequest.OptionValue) ? "" : IndexDocumentaryTypologyDtoRequest.OptionValue;

                if (IndexDocumentaryTypologyDtoRequest.EndFolio > IndexDocumentaryTypologyDtoRequest.StartFolio)
                {
                    if (IndexDocumentaryTypologyDtoRequest.Origin == "INDOR,EL")
                    {
                        _continue = IsEmail ? true : IsInformationSystem ? true : IsOther ? true : false;
                    }

                    if (_continue)
                    {
                        var responseApi = await HttpClient.PostAsJsonAsync("records/ControlBoard/IndexDocumentaryTypology", IndexDocumentaryTypologyDtoRequest);
                        var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<IndexDocumentaryTypologyDtoResponse>>();

                        if (deserializeResponse.Succeeded)
                        {

                            DocumentaryTypologyTDMDtoRequest.DocumentId = deserializeResponse.Data.DocumentId;
                            DocumentaryTypologyTDMDtoRequest.VolumeId = IndexDocumentaryTypologyDtoRequest.VolumeId;

                            NotificationModal.UpdateModal(ModalType.Success, Translation["SuccessfulIndexing"], true, Translation["Accept"]);
                            SpinnerLoaderService.HideSpinnerLoader(Js);
                        }
                        else
                        {
                            NotificationModal.UpdateModal(ModalType.Error, Translation["IndexingError"], true, Translation["Accept"], modalOrigin:"CI");
                            SpinnerLoaderService.HideSpinnerLoader(Js);
                        }
                    }
                    else
                    {
                        NotificationModal.UpdateModal(ModalType.Error, Translation["OriginErrorMessage"], true, Translation["Accept"], modalOrigin: "CI");
                        SpinnerLoaderService.HideSpinnerLoader(Js);
                    }
                }
                else
                {
                    NotificationModal.UpdateModal(ModalType.Error, Translation["IndexDocumentErrorMessage"], true, Translation["Accept"], modalOrigin: "CI");
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar la indexación: {ex.Message}");
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                throw;
            }
        }
        private async Task UpdateIndexation()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                bool _continue = true;
                _IndexDocumentaryTypologyUpdateDtoRequest.StartFolio = Convert.ToInt16(Start_Folio);
                _IndexDocumentaryTypologyUpdateDtoRequest.EndFolio = Convert.ToInt16(End_Folio);
                _IndexDocumentaryTypologyUpdateDtoRequest.Observation = IndexDocumentaryTypologyDtoRequest.Observation;
                _IndexDocumentaryTypologyUpdateDtoRequest.Color = Color;
                _IndexDocumentaryTypologyUpdateDtoRequest.Origin = String.IsNullOrEmpty(IndexDocumentaryTypologyDtoRequest.Origin) ? "" : IndexDocumentaryTypologyDtoRequest.Origin;
                _IndexDocumentaryTypologyUpdateDtoRequest.Options = String.IsNullOrEmpty(Options) ? "" : Options;
                _IndexDocumentaryTypologyUpdateDtoRequest.OptionValue = String.IsNullOrEmpty(IndexDocumentaryTypologyDtoRequest.OptionValue) ? "" : IndexDocumentaryTypologyDtoRequest.OptionValue;
                               
                if (_IndexDocumentaryTypologyUpdateDtoRequest.EndFolio > _IndexDocumentaryTypologyUpdateDtoRequest.StartFolio)
                {
                    if (_IndexDocumentaryTypologyUpdateDtoRequest.Origin == "INDOR,EL")
                    {
                        _continue = IsEmail ? true : IsInformationSystem ? true : IsOther ? true : false;
                    }

                    if (_continue)
                    {
                        var responseApi = await HttpClient.PostAsJsonAsync("records/ControlBoard/UpdateIndexDocumentaryTypology", _IndexDocumentaryTypologyUpdateDtoRequest);
                        var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<IndexDocumentaryTypologyDtoResponse>>();

                        if (deserializeResponse.Succeeded)
                        {

                            DocumentaryTypologyTDMDtoRequest.DocumentId = deserializeResponse.Data.DocumentId;
                            DocumentaryTypologyTDMDtoRequest.VolumeId = IndexDocumentaryTypologyDtoRequest.VolumeId;

                            NotificationModal.UpdateModal(ModalType.Success, Translation["SuccessfulIndexing"], true, Translation["Accept"]);
                            SpinnerLoaderService.HideSpinnerLoader(Js);
                        }
                        else
                        {
                            NotificationModal.UpdateModal(ModalType.Error, Translation["IndexingError"], true, Translation["Accept"], modalOrigin: "UI");
                            SpinnerLoaderService.HideSpinnerLoader(Js);
                        }
                    }
                    else
                    {
                        NotificationModal.UpdateModal(ModalType.Error, Translation["OriginErrorMessage"], true, Translation["Accept"], modalOrigin: "UI");
                        SpinnerLoaderService.HideSpinnerLoader(Js);
                    }
                }
                else
                {
                    NotificationModal.UpdateModal(ModalType.Error, Translation["IndexDocumentErrorMessage"], true, Translation["Accept"], modalOrigin: "UI");
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar la indexación: {ex.Message}");
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                throw;
            }
        }

        #endregion

        #endregion


    }
}
