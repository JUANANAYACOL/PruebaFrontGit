using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.ManagementTray;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.ViewDocuments;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Filing.Response;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System;
using System.Net.Http.Json;

namespace Control.Endeavour.FrontEnd.Components.Modals.Documents.DocumentaryTask
{
    public partial class DocumentRelationModal
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

        #endregion Inject

        #region Modals

        private NotificationsComponentModal notificationModal = new();
        private ViewDocumentModal viewDocumentModal = new();

        #endregion Modals

        #region Parameters

        [Parameter]
        public EventCallback<MyEventArgs<int>> OnStatusChanged { get; set; }

        [Parameter]
        public bool IsGestionDocumentRelated { get; set; } = false;

        [Parameter]
        public EventCallback<int> OnValiteDocument { get; set; }

        [Parameter]
        public string Title { get; set; } = "";

        [Parameter]
        public EventCallback<bool> OnDocumentAssociated { get; set; }

        #endregion Parameters

        #region Modals

        private ImagePdfDtoResponse imgageDocument = new();
        private ValidateDocumentGeneralInfoModal validateDocumentInfo = new();

        #endregion Modals

        #region Environments

        #region Environments(String)

        private string nRadicado { get; set; } = string.Empty;

        private string FillingCodeDocument = string.Empty;
        private string placeHolder = "";
        private string hidenButton = "col-7";

        #endregion Environments(String)

        #region Environments(Numeric)

        private int controlId { get; set; } = 0;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool disAbleInput = false;
        private bool disAbleButton = false;
        private bool isEnableActionButton = true;
        private bool SeenDocRelation = false;
        private bool IsValidateDocumentVisible = false;
        private bool IsLinkDocumentVisible = false;

        #endregion Environments(Bool)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            placeHolder = Translation["FilingNumber"];
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            if (IsGestionDocumentRelated)
            {
                hidenButton = "col-10";
            }
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
            nRadicado = "";
            isEnableActionButton = true;
            IsValidateDocumentVisible = false;
            IsLinkDocumentVisible = false;
        }

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        public void getDocumentFillCode(string value)
        {
            FillingCodeDocument = value;
            nRadicado = "";
            IsValidateDocumentVisible = false;
            IsLinkDocumentVisible = false;
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if(args.ModalOrigin.Equals("EnableButton") && args.IsCancelled)
            {
                IsValidateDocumentVisible = false;
            }

            if (!IsGestionDocumentRelated && args.ModalOrigin.Equals("DocTaskDocument"))
            {
                if (args.IsAccepted)
                {
                    var eventArgs = new MyEventArgs<int>
                    {
                        Data = controlId,
                        ModalStatus = false,
                    };

                    nRadicado = "";
                    isEnableActionButton = true;
                    IsValidateDocumentVisible = false;
                    IsLinkDocumentVisible = false;

                    await OnStatusChanged.InvokeAsync(eventArgs);
                }
                else
                {
                    var eventArgs = new MyEventArgs<int>();
                    await OnStatusChanged.InvokeAsync(eventArgs);
                }
            }
            else
            {
                if (args.ModalOrigin.Equals("LinkNewDocument"))
                {
                    await OnDocumentAssociated.InvokeAsync(true);
                }
            }
        }

        #endregion HandleMethods

        #region OthersMethods

        #region SendDocumentRelation

        private async Task SelectDocRelationAsync()
        {
            if (nRadicado != null)
            {
                SearchDtoRequest filtro = new SearchDtoRequest()
                {
                    FilingCode = nRadicado,
                };

                var responseApi = await HttpClient.PostAsJsonAsync("documents/Document/SearchEngineDocument", filtro);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<SearchDtoResponse>>>();

                if (deserializeResponse.Data.Data.Count > 0 && deserializeResponse.Data != null)
                {
                    controlId = deserializeResponse.Data.Data[0].ControlId;
                    IsValidateDocumentVisible = true;
                    notificationModal.UpdateModal(ModalType.Information, Translation["ConfirmAction"] + "\n" + Translation["WishContinue"], true, modalOrigin: "EnableButton");
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Information, Translation["AssociatedDocumentNotFound"], true, buttonTextCancel: "");
                }
            }
        }

        #endregion SendDocumentRelation

        #region ResetModal

        public void resetModal()
        {
            //nRadicado = "";
            hidenButton = "col-10";
        }

        #endregion ResetModal

        #region UpdateDocumentRelation

        public async Task UpdateDocumentRelation(int id, bool value)
        {
            SeenDocRelation = value;
            disAbleInput = !SeenDocRelation;

            HttpClient?.DefaultRequestHeaders.Remove("TaskId");
            HttpClient?.DefaultRequestHeaders.Add("TaskId", $"{id}");
            var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<FilingDtoResponse>>("documentarytasks/DocumentaryTask/GetDocumentRelation");
            HttpClient?.DefaultRequestHeaders.Remove("TaskId");

            if (deserializeResponse.Data != null)
            {
                hidenButton = "col-7";
                disAbleButton = true;
                //placeHolder = deserializeResponse.Data.ExternalFiling;
                nRadicado = deserializeResponse.Data.ExternalFiling;
                controlId = deserializeResponse.Data.ControlId.Value;
            }
            else
            {
                hidenButton = "col-10";
                nRadicado = "";
                placeHolder = Translation["NoRelatedDocumentFound"];
                disAbleButton = false;
            }
            StateHasChanged();
        }

        public void DocRelationMT(DocumentResponseComentaryClosedDtoResponse record, bool value)
        {
            SeenDocRelation = value;
            disAbleInput = !SeenDocRelation;

            hidenButton = "col-7";
            disAbleButton = true;
            placeHolder = record.FilingCode;
            controlId = record.ControlId;
        }

        #endregion UpdateDocumentRelation

        #region GetImageDocument

        private async Task GetImageDocument(int id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                HttpClient?.DefaultRequestHeaders.Remove("controlId");
                HttpClient?.DefaultRequestHeaders.Add("controlId", $"{id}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<ImagePdfDtoResponse>>("documents/Document/GetImageDocument");
                HttpClient?.DefaultRequestHeaders.Remove("controlId");

                if (deserializeResponse.Succeeded)
                {
                    imageValid = true;
                    imgageDocument = deserializeResponse.Data;
                }
                else
                {
                    imageValid = false;
                    notificationModal.UpdateModal(ModalType.Information, Translation["NoImage4Document"], true, modalOrigin: "imagen", buttonTextCancel: "");
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener archivo: {ex.Message}");
            }
        }

        #endregion GetImageDocument

        #region ViewAttachment

        private bool imageValid = true;

        private async Task HandleViewAttachment()
        {
            if (controlId != 0)
            {
                await validateDocumentInfo.GeneralInformation(controlId);
                validateDocumentInfo.UpdateModalStatus(true);
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["NoDocumentSelected"], true);
            }
        }

        #endregion ViewAttachment

        #region ShowModalInfo

        private async Task ShowModalInfo()
        {
            IsLinkDocumentVisible = true;

            switch (IsGestionDocumentRelated)
            {
                case true:
                    if (controlId != 0)
                    {
                        await OnValiteDocument.InvokeAsync(controlId);
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["NoDocumentSelected"], true);
                    }
                    break; 
                case false:
                    await HandleViewAttachment();
                    break;

            }

            
        }

        #endregion ShowModalInfo

        #region LinkDocument

        private async Task LinkNewDocument()
        {
            switch (IsGestionDocumentRelated)
            {
                case true:
                    await LinkNewDocumentManagement();
                    break;
                case false:
                    notificationModal.UpdateModal(ModalType.Information, Translation["ConfirmAction"] + "\n" + Translation["WishContinue"], true, modalOrigin: "DocTaskDocument");
                    break;

            }
        }

        private async Task LinkNewDocumentManagement()
        {
            
            if (nRadicado != null)
            {
                DocumentRelationDtoRequest filtro = new()
                {
                    FilingCode1 = FillingCodeDocument,
                    FilingCode2 = nRadicado,
                    RelationOrigin = "ORI,GES",
                    CreateUser = "JohanC" //to-do cambiar por el nombre de sesion
                };

                var responseApi = await HttpClient.PostAsJsonAsync("documentmanagement/Document/CreateDocumentRelation", filtro);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<SearchDtoResponse>>();

                if (deserializeResponse.Data != null)
                {
                    notificationModal.UpdateModal(ModalType.Success, string.Format(Translation["DocumentWasAssociated"], nRadicado), true, modalOrigin: "LinkNewDocument");
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Error, Translation["AssociatedDocumentNotFound"], true);
                }
            }
        }

        #endregion LinkDocument

        #endregion OthersMethods

        #endregion Methods
    }
}