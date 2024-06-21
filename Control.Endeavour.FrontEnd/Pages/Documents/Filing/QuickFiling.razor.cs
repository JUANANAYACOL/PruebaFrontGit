using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.Attachments;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.Label;
using Control.Endeavour.FrontEnd.Models.Enums.Attachments;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Filing.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Filing.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos.Attachments.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.StateContainer.Filing;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using Microsoft.VisualBasic;
using System.Net.Http.Json;
using System.Net.Mail;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Pages.Documents.Filing
{
    public partial class QuickFiling
    {

        #region Variables

        #region Inject 
        [Inject] private IJSRuntime Js { get; set; }
        [Inject] private IStringLocalizer<Translation>? Translation { get; set; }

        [Inject]
		private HttpClient? HttpClient { get; set; }
        [Inject] private FilingStateContainer? FilingSC { get; set; }
        #endregion

        #region Components


        #endregion

        #region Modals
        private NotificationsComponentModal notificationModal = new();
        private AttachmentsModal ModalAttachments = new();
        private LabelModal LabelModal = new();

        #endregion

        #region Parameters


        #endregion

        #region Models
        private AttachmentsDtoRequest RecordToDelete = new();
        private AttachmentsType Filing = AttachmentsType.Filing;
        private QuickFilingDtoRequest quickFilingDtoRequest = new();
        #endregion

        #region Environments

        #region Environments(String)
        private string titleView { get; set; } = "Comunicaciones - ";

        private string TablaAdjuntos = "d-none";
        private string Radicado = "";
        private string IdDocumento = "";
        private string NumberOfRecievers  = "1";
        private string panel_6 = "d-none";
        private string panel_5 = "d-none";
        #endregion

        #region Environments(Numeric)
        private decimal contadorcarac = 0;
        #endregion

        #region Environments(DateTime)

        #endregion

        #region Environments(Bool)
        private bool DisableButtons { get; set; } = false;
        private bool DisableDropDown { get; set; } = true;
        private bool isTextareaDisabled = false;





        #endregion

        #region Environments(List & Dictionary)
        private List<AttachmentsDtoRequest> listAttachment { get; set; } = new();
        private List<AttachmentsDtoRequest> Attachments { get; set; } = new();
        private List<VSystemParamDtoResponse> classCodeList = new();

        #endregion

        #endregion

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            titleView = Translation["Comunications"] + " - " + Translation["QuickFiling"];
            await GetClasses();
        }


        #endregion

        #region Methods

        #region HandleMethods

        #region HandleNotificationsModal
        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                if (args.ModalOrigin == "RemoveFile")
                {
                    listAttachment.Remove(RecordToDelete);

                    if (listAttachment.Count == 0)
                    {
                        TablaAdjuntos = "d-none";
                    }
                    else
                    {
                        TablaAdjuntos = "";
                    }
                }
            }
            else
            {
                Console.WriteLine("Registro No eliminado");
            }
        }
        #endregion

        #region HandleModals
        private void HandleModalClosed()
        {
            LabelModal.UpdateModalStatus(false);
        }
        private async Task HandleAttachments(MyEventArgs<List<AttachmentsDtoRequest>> data)
        {
            Attachments = data.Data;

            if (Attachments?.Count > 0)
            {
                listAttachment = new();

                foreach (var item in Attachments)
                {
                    AttachmentsDtoRequest attachmentsData = new AttachmentsDtoRequest()
                    {
                        DataFile = item.DataFile,
                        ArchiveName = item.ArchiveName,
                        ArchiveExt = item.ArchiveExt.ToString().Replace(".", ""), //Es importante que no lleve punto
                        ExhibitCode = item.ExhibitCode,
                        AttCode = item.AttCode,
                        AttDescription = item.AttDescription,
                        IconPath = item.IconPath,
                        Size = item.Size,
                    };

                    listAttachment.Add(attachmentsData);
                }

                if (listAttachment.Count > 0)
                {
                    notificationModal.UpdateModal(ModalType.Success, Translation["AttachmentUploadSuccessfulMessage"], true, Translation["Accept"], "", "", "");
                    TablaAdjuntos = "";
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["LoadErrorMessage"], true);
                    TablaAdjuntos = "d-none";
                }
            }
            else
            {
                listAttachment = new List<AttachmentsDtoRequest>();
            }

            await Task.Delay(1000);
        }

        #endregion

        #endregion

        #region HandleFormCreate
        private async Task HandleFormCreate()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                DisableButtons = true;
                panel_6 = String.Empty;
                isTextareaDisabled = true;
                DisableDropDown = false;
                quickFilingDtoRequest.NumberOfRecipients = int.Parse(NumberOfRecievers);
                
               
                var responseApi = await HttpClient.PostAsJsonAsync("documents/Filing/QuickFiling", quickFilingDtoRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<FilingDtoResponse>>();

                if (deserializeResponse.Succeeded)
                {
                    notificationModal.UpdateModal(ModalType.Success, string.Format(Translation["CreateSuccessfullFilingMessage"], deserializeResponse.Data.FilingCode), true);
                    Radicado = deserializeResponse.Data.FilingCode;
                    IdDocumento = deserializeResponse.Data.ControlId.ToString();
                    #region FillOutFiling

                    FilingSC?.Parametros(false);
                    FilingSC.FilingNumber = Radicado;
                    FilingSC.DocumentId = IdDocumento;
                    

                    #endregion FillOutFiling
                }















                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar el radicado: {ex.Message}");
                SpinnerLoaderService.HideSpinnerLoader(Js);
                throw;
            }
        }

        private void ResetQuickFiling()
        {
            DisableButtons = false;
            panel_6 = "d-none";
            panel_5 = "d-none";
            isTextareaDisabled = false;
            DisableDropDown = true;
            quickFilingDtoRequest = new();
            NumberOfRecievers = "1";
        }
        #endregion

        #region OthersMethods

        #region ValidationMethods
        private Dictionary<string, object> GetDisabledAttribute()
        {
            var attributes = new Dictionary<string, object>();
            if (isTextareaDisabled)
            {
                attributes.Add("disabled", "disabled");
            }
            return attributes;
        }

        private void ContarCaracteres(ChangeEventArgs e)
        {
            String value = e.Value.ToString() ?? String.Empty;

            if (!string.IsNullOrEmpty(value))
            {
                contadorcarac = value.Length;
                ValidateFields();


            }
            else
            {
                contadorcarac = 0;
                
            }
        }

        private void ValidateFields()
        {
            if(DisableDropDown && !string.IsNullOrEmpty(NumberOfRecievers) && !string.IsNullOrEmpty(quickFilingDtoRequest.PhysicalDescription) && !string.IsNullOrEmpty(quickFilingDtoRequest.ClassCode))
            {
                panel_5 = String.Empty;
            }
            else
            {
                panel_5 = "d-none";
                
            }
        }
        #endregion

        #region GetData
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

        #region ShowModalMethods
        private void showModalLabel()
        {
            LabelModal.GetDataQuickFiling(NumberOfRecievers, quickFilingDtoRequest.PhysicalDescription);
            LabelModal.UpdateModalStatus(true);

        }
        private void showModalAttachments()
        {
            ModalAttachments.UpdateModalStatus(true, listAttachment);

        }

        #endregion

        #region Attachments
        private void RemoverFile(AttachmentsDtoRequest fileInfo)
        {
            RecordToDelete = fileInfo;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, @Translation["Yes"], @Translation["No"], modalOrigin: "RemoveFile");

        }

        #endregion

        #endregion

        #endregion
    }
}
