using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Attachments;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Country.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Country.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VSystemParam.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos.Attachments.Request;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using DevExpress.Data.Filtering.Helpers;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.IO;
using System.Net.Http.Json;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.FormatMasterList
{
    public partial class TemplateModal
    {
        #region Variables

        #region Inject

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject] private IJSRuntime Js { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion Inject

        #region Components

        private InputModalComponent CodeInput;
        private InputModalComponent NameTemplateInput;
        private InputModalComponent ProcessInput;
        private InputModalComponent VersionInput;
        private MetaModel meta = new();

        #endregion Components

        #region Modals

        private NotificationsComponentModal notificationModal = new();

        #endregion Modals

        #region Parameters

        [Parameter] public bool ModalStatus { get; set; }
        [Parameter] public EventCallback<bool> OnChangeData { get; set; }

        #endregion Parameters

        #region Models

        private TemplateDocumentDtoResponse DocTemplateFormResponse = new();
        private DocTemplateDtoRequest DocTemplateformRequest = new();
        private TemplateDocumentDtoResponse _selectedRecord = new();
        private FileInfoData fileData = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        private string ValueFormatDoc = "";
        private string value = "";
        private string version = "";
        private string textFD = "SelectFormatType";
        private string paramCol = "col";
        private string fileInfoSpan = "";
        private string versionInput = "";

        #endregion Environments(String)

        #region Environments(Numeric)

        private int tempId = 0;
        private int contadorcarac = 0;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool upFile = false;
        private bool modalStatus = false;
        private bool IsEditForm = false;
        private bool activeState = true;
        private bool visible = false;
        private bool enableButton = true;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<FileInfoData> fileDataList = new();
        private AttachmentsDtoRequest attachments = new();
        private List<VSystemParamDtoResponse> FormatDoc = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            await GetFormatDoc();
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #region CloseModal

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
            contadorcarac = 0;
            paramCol = "col";
            IsEditForm = false;
            visible = false;
            versionInput = "";
            ValueFormatDoc = "";
            activeState = true;
            fileDataList = new();
            fileInfoSpan = "";
            enableButton = true;
            DocTemplateFormResponse = new TemplateDocumentDtoResponse();
            StateHasChanged();
        }

        #endregion CloseModal

        #region Identifier Method

        private async Task HandleValidSubmit()
        {
            if (IsEditForm)
            {
                await UpdateTemplateDoc();
            }
            else
            {
                await CreateTemplateDoc();
            }
            StateHasChanged();
        }

        #endregion Identifier Method

        private void HandleFilesList(List<FileInfoData> newList)
        {
            if (newList.Any())
            {
                if (!string.IsNullOrEmpty(ValueFormatDoc))
                {
                    if (ValueFormatDoc.Equals("TFOR,XLSX") && newList[0].Extension.Equals(".xlsx"))
                    {
                        fileDataList = newList;
                    }
                    else if (ValueFormatDoc.Equals("TFOR,DTXT") && newList[0].Extension.Equals(".docx"))
                    {
                        fileDataList = newList;
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Warning, "El archivo no coincide con el tipo de formato seleccionado", true, Translation["Accept"], buttonTextCancel: "");
                        fileDataList = new();
                    }
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Warning, "Para poder cargar un archivo es necesario definir el tipo de formato", true, Translation["Accept"], buttonTextCancel: "");
                    fileDataList = new();
                }
            }
            ValidateEnableButton();
        }

        #region ModalNotifcation

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        private void HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (notificationModal.Type == ModalType.Success)
            {
                UpdateModalStatus(args.ModalStatus);
                ResetFiltersAsync();
            }
            else if (notificationModal.Type == ModalType.Warning && notificationModal.ModalOrigin.Equals("updateFile"))
            {
                visible = true;
            }
        }

        #endregion ModalNotifcation

        #endregion HandleMethods

        #region OthersMethods

        #region GetFormatDoc

        public async Task GetFormatDoc()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                HttpClient?.DefaultRequestHeaders.Remove("ParamCode");
                HttpClient?.DefaultRequestHeaders.Add("ParamCode", "TFOR");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<List<VSystemParamDtoResponse>>>("generalviews/VSystemParams/ByParamCode");
                HttpClient?.DefaultRequestHeaders.Remove("ParamCode");

                if (deserializeResponse.Data != null)
                {
                    FormatDoc = deserializeResponse.Data;
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las acciones: {ex.Message}");
            }
        }

        #endregion GetFormatDoc

        #region CreateTemplatedocuments

        private async Task CreateTemplateDoc()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);

            if (fileDataList.Any())
            {
                DocTemplateformRequest.TempCode = CodeInput.InputValue;
                DocTemplateformRequest.TempVersion = int.Parse(versionInput);
                DocTemplateformRequest.TempName = NameTemplateInput.InputValue;
                DocTemplateformRequest.TempType = ValueFormatDoc;
                DocTemplateformRequest.Process = DocTemplateFormResponse.Process;
                DocTemplateformRequest.Archivo = fileDataList[0].Base64Data;
                DocTemplateformRequest.ActiveState = activeState;
                var responseApi = await HttpClient.PostAsJsonAsync("documents/TemplateDocuments/CreateTemplateDocument", DocTemplateformRequest);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<DocTemplateDtoRequest>>();

                if (deserializeResponse.Succeeded)
                {
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                    notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                    //ResetFiltersAsync();
                    await OnChangeData.InvokeAsync(true);
                }
                else
                {
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                    notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
                }
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Warning, "Para crear una plantilla es necesario agregar un archivo", true, Translation["Accept"], buttonTextCancel: "");
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion CreateTemplatedocuments

        #region UpdateTemplateDocuments

        public async Task UpdateTemplateDoc()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                DocTemplateformRequest.TemplateId = tempId;
                DocTemplateformRequest.CompanyId = 17;
                DocTemplateformRequest.User = "Admin";
                DocTemplateformRequest.TempName = NameTemplateInput?.InputValue;
                DocTemplateformRequest.TempCode = CodeInput.InputValue;
                DocTemplateformRequest.TempType = ValueFormatDoc;
                DocTemplateformRequest.Process = DocTemplateFormResponse.Process ?? "";
                DocTemplateformRequest.ActiveState = activeState;

                if (fileDataList[0].Extension.Equals(fileData.Extension))
                {
                    DocTemplateformRequest.Archivo = fileDataList[0].Base64Data.Equals(fileData.Base64Data) ? null : fileDataList[0].Base64Data;

                    var responseApi = await HttpClient.PostAsJsonAsync("documents/TemplateDocuments/UpdateTemplateDocument", DocTemplateformRequest);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<TemplateDocumentDtoResponse>>();

                    if (deserializeResponse.Succeeded)
                    {
                        notificationModal.UpdateModal(ModalType.Success, Translation["UpdateSuccesfulMessage"], true, Translation["Accept"]);
                        IsEditForm = false;
                        visible = false;
                        //ResetFiltersAsync();
                        await OnChangeData.InvokeAsync(true);
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, Translation["UpdateErrorMessage"], true, Translation["Accept"]);
                    }
                }
                else
                {
                    notificationModal.UpdateModal(ModalType.Warning, Translation["WrongFormatType"], true, Translation["Accept"], buttonTextCancel: "", modalOrigin: "updateFile");
                    fileDataList = new();
                    fileDataList.Add(fileData);
                }

                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener el listado maestro de formatos: {ex.Message}");
            }
        }

        #endregion UpdateTemplateDocuments

        #region CountChar

        private void ContarCaracteres(ChangeEventArgs e)
        {
            String value = e.Value.ToString() ?? String.Empty;
            DocTemplateFormResponse.Process = value;
            if (!string.IsNullOrEmpty(e.Value.ToString()))
            {
                contadorcarac = e.Value.ToString().Length;
            }
            else
            {
                contadorcarac = 0;
            }
            ValidateEnableButton();
        }

        #endregion CountChar

        #region GetFile

        private async Task GetFile(int id)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);

                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", $"{id}");
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");

                if (deserializeResponse.Succeeded)
                {
                    fileData = new()
                    {
                        Name = deserializeResponse.Data.FileName,
                        Extension = "." + deserializeResponse.Data.FileExt,
                        Base64Data = deserializeResponse.Data.DataFile
                    };
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener archivo: {ex.Message}");
            }
        }

        #endregion GetFile

        #region UpdateRecords

        public async Task RecibirRegistro(TemplateDocumentDtoResponse response)
        {
            paramCol = "row";
            _selectedRecord = response;
            visible = true;
            upFile = false;
            fileDataList = new();
            tempId = response.TemplateId;
            ValueFormatDoc = _selectedRecord.TempType;
            GetValue(_selectedRecord.TempType);
            FormatType(ValueFormatDoc);
            activeState = _selectedRecord.ActiveState;
            DocTemplateFormResponse.TempCode = _selectedRecord.TempCode;
            DocTemplateFormResponse.TempName = _selectedRecord.TempName;
            versionInput = _selectedRecord.TempVersion.ToString();
            DocTemplateFormResponse.TempType = _selectedRecord.TempType;
            DocTemplateFormResponse.Process = _selectedRecord.Process;
            DocTemplateFormResponse.ActiveState = _selectedRecord.ActiveState;
            DocTemplateformRequest.TempCode = _selectedRecord.TempCode;
            DocTemplateformRequest.TempName = _selectedRecord.TempName;
            DocTemplateformRequest.TempType = _selectedRecord.TempType;
            DocTemplateformRequest.Process = _selectedRecord.Process;
            DocTemplateformRequest.ActiveState = _selectedRecord.ActiveState;
            contadorcarac = DocTemplateformRequest.Process.Count();
            await GetFile(response.FileId);
            fileDataList.Add(fileData);
            IsEditForm = true;
        }

        #endregion UpdateRecords

        #region ResetFilter

        public void ResetFiltersAsync()
        {
            if (visible)
            {
                DocTemplateFormResponse.TempCode = "";
                DocTemplateFormResponse.Process = "";
                DocTemplateFormResponse.TempName = "";
                activeState = true;
                contadorcarac = 0;
                ValueFormatDoc = "";
                fileDataList = new();
                fileInfoSpan = "";
            }
            else
            {
                DocTemplateFormResponse.TempCode = "";
                DocTemplateFormResponse.Process = "";
                DocTemplateFormResponse.TempName = "";
                versionInput = "";
                activeState = true;
                contadorcarac = 0;
                ValueFormatDoc = "";
                fileDataList = new();
                fileInfoSpan = "";
            }

            StateHasChanged();
        }

        #endregion ResetFilter

        #region FormatType

        public void FormatType(string value)
        {
            ValueFormatDoc = value;

            if (ValueFormatDoc != null)
            {
                if (ValueFormatDoc.Equals("TFOR,DTXT"))
                {
                    fileInfoSpan = "sólo se permite cargar archivos en formato docx";
                }
                else if (ValueFormatDoc.Equals("TFOR,XLSX"))
                {
                    fileInfoSpan = "sólo se permite cargar archivos en formato xlsx";
                }
                else { fileInfoSpan = ""; }
            }
            ValidateEnableButton();
        }

        #endregion FormatType

        #region GetValues

        public void GetValue(string data)
        {
            foreach (var file in FormatDoc)
            {
                if (file.Code.Equals(data))
                {
                    value = file.Value;
                }
            }
        }

        #endregion GetValues

        #region ValidationMethods

        private void ValidateEnableButton()
        {
            enableButton = string.IsNullOrWhiteSpace(ValueFormatDoc) ||
                   string.IsNullOrWhiteSpace(versionInput) ||
                   string.IsNullOrWhiteSpace(DocTemplateFormResponse.TempCode) ||
                   string.IsNullOrWhiteSpace(DocTemplateFormResponse.TempName) ||
                   (string.IsNullOrWhiteSpace(DocTemplateFormResponse.Process) || DocTemplateFormResponse.Process.Length < 10) ||
                   fileDataList.Count == 0;
        }

        #endregion ValidationMethods

        #endregion OthersMethods

        #endregion Methods
    }
}