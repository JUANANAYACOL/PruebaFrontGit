using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.AdministracionTRD.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.Company.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using Telerik.Blazor.Components;

namespace Control.Endeavour.FrontEnd.Components.Modals.Administration.AdministrationTRD
{
    public partial class AdministrativeActModal : ComponentBase
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

        #region Components

        private NotificationsComponentModal notificationModal { get; set; } = new();

        #endregion Components

        #region Models

        public TelerikPdfViewer PdfViewerRef { get; set; } = new();
        public DocumentalVersionDtoResponse docVersionDtoResponse { get; set; } = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        public string FileBase64Data;
        private string displayTableHide = "";
        private string displayPdfViewerHide = "";
        private string displayTable = "col-lg-6";
        private string displayDocument = "col-lg-6";
        private string docIcon = "fa-solid fa-eye";
        private string tableIcon = "fa-solid fa-eye";
        private string tableText = "ShowInformation";
        private string docText = "ShowDocument";
        private string displayViewer { get; set; } = "";
        private string extensionMessage { get; set; } = string.Empty;

        #endregion Environments(String)

        #region Environments(Numeric)

        private int recordDeleteId = 0;
        private int documentalVersionId = 0;
        private int FileSize = 100;

        #endregion Environments(Numeric)

        #region Environments(Bool)

        private bool modalStatus = false;
        private bool validDisplayDocument = false;
        private bool validDisplayTable = false;
        private bool showTableActive = false;
        private bool showDocumentActive = false;
        private bool saveIsDisable { get; set; } = true;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        public List<AdministrativeActDtoRequest> listAdminActDtoRequest { get; set; } = new();
        public List<AdministrativeActDtoResponse> listAdminActDtoResponse { get; set; } = new();
        private List<FileInfoData> listNewAdminActs { get; set; } = new();

        private byte[]? SelectedPDF { get; set; }
        private string[] AllowedExtensions { get; set; } = { ".pdf" };

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            extensionMessage = GetUploadMessage();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        #region HandleFormCreate

        private async Task HandleFormCreate()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                if (listAdminActDtoRequest.Count > 0)
                {
                    listAdminActDtoRequest.ForEach(x => x.DocumentalVersionId = documentalVersionId);
                    listAdminActDtoRequest.ForEach(x =>
                    {
                        var act = listAdminActDtoResponse.Find(item => item.DataFile != null && item.DataFile.SequenceEqual(x.DataFile));
                        if (act != null)
                        {
                            x.AdminActDate = act.AdminActDate;
                            x.ActName = act.ActName;
                        }
                    });

                    var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/AdministrativeAct/CreateAdministrativeAct", listAdminActDtoRequest);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeActDtoResponse>>>();
                    if (deserializeResponse!.Succeeded)
                    {
                        listAdminActDtoRequest.Clear();
                        await GetActs();

                        SpinnerLoaderService.HideSpinnerLoader(Js);
                        notificationModal.UpdateModal(ModalType.Success, Translation["CreateSuccessfulMessage"], true, Translation["Accept"]);
                    }
                    else
                    {
                        SpinnerLoaderService.HideSpinnerLoader(Js);
                        notificationModal.UpdateModal(ModalType.Error, Translation["CreateErrorMessage"], true, Translation["Accept"]);
                    }
                }
                else
                {
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                    notificationModal.UpdateModal(ModalType.Error, Translation["NoAdministrativeActsToCreate"], true, Translation["Accept"]);
                }
                EnableSaveButton();
            }
            catch
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, Translation["ErrorCreatingData"], true, Translation["Accept"]);
            }
        }

        #endregion HandleFormCreate

        #region HandleNewActsToCreate

        private void HandleNewActsToCreate(List<FileInfoData> data)
        {
            if (listAdminActDtoResponse.Count < 10)
            {
                var file = data.LastOrDefault();
                if (file != null)
                {
                    AdministrativeActDtoRequest act = new()
                    {
                        DocumentalVersionId = docVersionDtoResponse.DocumentalVersionId,
                        FileName = file.Name!,
                        FileExt = file.Extension!.Replace(".", ""),
                        DataFile = file.Base64Data!
                    };

                    if (listAdminActDtoRequest.Find(x => x.DataFile.SequenceEqual(act.DataFile)) == null)
                    {
                        listAdminActDtoRequest.Add(act);

                        AdministrativeActDtoResponse actNew = new()
                        {
                            DataFile = file.Base64Data!
                        };

                        listAdminActDtoResponse.Add(actNew);
                        StateHasChanged();
                    }
                    else
                    {
                        notificationModal.UpdateModal(ModalType.Error, $"{Translation["FileAlreadyAttachedError"]}", true, Translation["Yes"], null);
                    }
                }

                listNewAdminActs.Clear();
                EnableSaveButton();
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["NumberFilesExceededMessage"]}", true, Translation["Yes"], null);
            }
        }

        #endregion HandleNewActsToCreate

        #region HandleDeleteRecord

        #region DeleteRecordOfLists

        private void RemoveAdministrativeAct(AdministrativeActDtoResponse act)
        {
            SelectedPDF = new byte[0];
            displayViewer = "d-none";
            listAdminActDtoResponse.Remove(act);

            var fileN = listAdminActDtoRequest.Find(x => x.DataFile.SequenceEqual(act.DataFile)).FileName;
            listAdminActDtoRequest.RemoveAll(x => x.FileName.Equals(fileN));
            StateHasChanged();
        }

        #endregion DeleteRecordOfLists

        #region ModalWarningDelete

        private void ShowModalDelete(AdministrativeActDtoResponse value)
        {
            if (value.AdministrativeActId != 0)
            {
                recordDeleteId = value.AdministrativeActId;
                notificationModal.UpdateModal(ModalType.Warning, $"{Translation["DeleteQuestionMessage"]}: {value.ActName}", true, Translation["Yes"], Translation["No"], modalOrigin: "DeleteModal");
            }
            else
            {
                RemoveAdministrativeAct(value);
            }
            EnableSaveButton();
        }

        #endregion ModalWarningDelete

        #region ModalDeleteRecord

        private async Task HandleModalNotiCloseAsync(ModalClosedEventArgs args)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                if (args.IsAccepted && args.ModalOrigin!.Equals("DeleteModal"))
                {
                    DeleteGeneralDtoRequest request = new() { Id = recordDeleteId, User = "user" };

                    var responseApi = await HttpClient!.PostAsJsonAsync("paramstrd/AdministrativeAct/DeleteAdministrativeAct", request);
                    var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<int>>();
                    if (deserializeResponse!.Succeeded)
                    {
                        SelectedPDF = new byte[0];
                        displayViewer = "d-none";
                        await GetActs();

                        if (args.ModalOrigin.Equals("DeleteModal"))
                        {
                            SpinnerLoaderService.HideSpinnerLoader(Js);
                            notificationModal.UpdateModal(ModalType.Success, Translation["DeleteSuccessfulMessage"], true, Translation["Accept"]);
                        }
                    }
                    else
                    {
                        SpinnerLoaderService.HideSpinnerLoader(Js);
                        notificationModal.UpdateModal(ModalType.Error, Translation["DeleteErrorMessage"], true, Translation["Accept"]);
                    }

                    StateHasChanged();
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["DeleteErrorMessage"]}", true, Translation["Yes"], null);
            }
        }

        #endregion ModalDeleteRecord

        #endregion HandleDeleteRecord

        #endregion HandleMethods

        #region OthersMethods

        #region HandleModalClosed

        private void HandleModalClosed(bool status)
        {
            modalStatus = status;
            StateHasChanged();
            ResetFormAsync();
        }

        #endregion HandleModalClosed

        #region ResetFormAsync

        public void ResetFormAsync()
        {
            documentalVersionId = 0;
            listAdminActDtoResponse = new();
            listAdminActDtoRequest = new();
            docVersionDtoResponse = new();
            docVersionDtoResponse = new DocumentalVersionDtoResponse();
            listNewAdminActs.Clear();
            PdfViewerRef = new TelerikPdfViewer();
            SelectedPDF = new byte[0];
            displayViewer = "d-none";
        }

        public void Clean()
        {
            listAdminActDtoResponse.RemoveAll(x => x.FileId == 0);
            listAdminActDtoRequest.Clear();
            listNewAdminActs.Clear();

            SelectedPDF = new byte[0];
            displayViewer = "d-none";
        }

        #endregion ResetFormAsync

        #region Gets

        #region GetFile

        private async Task<FileDtoResponse?> GetFile(int? id)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                HttpClient?.DefaultRequestHeaders.Add("FileId", $"{id}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<FileDtoResponse>>("file/File/ByIdBase");
                HttpClient?.DefaultRequestHeaders.Remove("FileId");
                if (deserializeResponse!.Succeeded)
                {
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                    return deserializeResponse.Data!;
                }
                else
                {
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                    return null;
                }
            }
            catch
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                return null;
            }
        }

        #endregion GetFile

        #region GetAct

        #region GetActsOfDocumentalVersion

        private async Task GetActs()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            try
            {
                HttpClient?.DefaultRequestHeaders.Remove("documentalVersionId");
                HttpClient?.DefaultRequestHeaders.Add("documentalVersionId", $"{documentalVersionId}");
                var deserializeResponse = await HttpClient!.GetFromJsonAsync<HttpResponseWrapperModel<List<AdministrativeActDtoResponse>>>("paramstrd/AdministrativeAct/ByFilterId");
                HttpClient?.DefaultRequestHeaders.Remove("documentalVersionId");

                if (deserializeResponse!.Succeeded && deserializeResponse.Data != null && deserializeResponse.Data.Count > 0)
                {
                    var listAdminActDtoResponseCopy = listAdminActDtoResponse;
                    listAdminActDtoResponse = deserializeResponse.Data ?? new();

                    if (listAdminActDtoRequest.Count > 0)
                    {
                        listAdminActDtoRequest.ToList().ForEach(x =>
                        {
                            var search = listAdminActDtoResponseCopy.FindAll(y => y.DataFile != null).FirstOrDefault();
                            if (search != null)
                            {
                                x.AdminActDate = search.AdminActDate;
                                x.ActName = search.ActName;
                            }

                            AdministrativeActDtoResponse actNew = new()
                            {
                                ActName = x.FileName!,
                                DataFile = x.DataFile!,
                                AdminActDate = x.AdminActDate
                            };

                            listAdminActDtoResponse.Add(actNew);
                        });
                    }
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }
                else
                {
                    var listAdminActDtoResponseCopy = listAdminActDtoResponse;
                    listAdminActDtoResponse = new();
                    if (listAdminActDtoRequest.Count > 0)
                    {
                        listAdminActDtoRequest.ToList().ForEach(x =>
                        {
                            var search = listAdminActDtoResponseCopy.FindAll(y => y.DataFile != null).FirstOrDefault();
                            if (search != null)
                            {
                                x.AdminActDate = search.AdminActDate;
                                x.ActName = search.ActName;
                            }

                            AdministrativeActDtoResponse actNew = new()
                            {
                                ActName = x.ActName!,
                                DataFile = x.DataFile!,
                                AdminActDate = x.AdminActDate
                            };

                            listAdminActDtoResponse.Add(actNew);
                        });
                    }
                    SpinnerLoaderService.HideSpinnerLoader(Js);
                }

                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                notificationModal.UpdateModal(ModalType.Error, $"{Translation["LoadErrorMessage"]}", true, Translation["Yes"], null);
            }
        }

        #endregion GetActsOfDocumentalVersion

        #endregion GetAct

        #endregion Gets

        #region TableAndPdf

        #region SelectToPreviewPdf

        public async Task SelectToPreviewPdf(AdministrativeActDtoResponse pdfPath)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            if (pdfPath.FileId != 0)
            {
                var file = await GetFile(pdfPath.FileId);
                SelectedPDF = file!.DataFile;
                displayViewer = "";
            }
            else
            {
                SelectedPDF = pdfPath.DataFile!;
                displayViewer = "";
            }

            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion SelectToPreviewPdf

        #region HandleVisibilityButtons

        #region HandleVisibilityTable

        private void ShowTable()
        {
            validDisplayTable = !validDisplayTable;

            if (validDisplayTable && displayDocument.Equals("col-lg-6") || !validDisplayTable && displayDocument.Equals("col-lg-12") || validDisplayTable && displayDocument.Equals("col-lg-12"))
            {
                displayTable = "col-lg-12";
                displayDocument = "d-none";
                tableIcon = "fa-solid fa-eye-slash";
                tableText = Translation["HideInformation"];
                docIcon = "fa-solid fa-eye";
                docText = Translation["ShowDocument"];
            }
            else if (!validDisplayTable && displayDocument.Equals("col-lg-6"))
            {
                docText = Translation["ShowDocument"];
                displayTable = "col-lg-12";
                displayDocument = "d-none";
                tableIcon = "fa-solid fa-eye-slash";
                tableText = Translation["HideInformation"];
                docIcon = "fa-solid fa-eye";
            }
            else
            {
                tableText = Translation["ShowInformation"];
                displayTable = "col-lg-6";
                displayDocument = "col-lg-6";
                tableIcon = "fa-solid fa-eye";
            }
        }

        #endregion HandleVisibilityTable

        #region HandleVisibilityPdf

        private void ShowDocument()
        {
            validDisplayDocument = !validDisplayDocument;

            if (validDisplayDocument && displayTable.Equals("col-lg-6") || !validDisplayDocument && displayTable.Equals("col-lg-12") || validDisplayDocument && displayTable.Equals("col-lg-12"))
            {
                displayTable = "d-none";
                displayDocument = "col-lg-12";
                docIcon = "fa-solid fa-eye-slash";
                docText = Translation["HideInformation"];
                tableIcon = "fa-solid fa-eye";
                tableText = "Mostrar información";
            }
            /*else if (!validDisplayDocument && displayTable.Equals("col-lg-12") || validDisplayDocument && displayTable.Equals("col-lg-12"))
            {
                docText = "Ocultar documento";
                displayTable = "d-none";
                displayDocument = "col-lg-12";
                docIcon = "fa-solid fa-eye-slash";
                tableIcon = "fa-solid fa-eye";
                tableText = "Mostrar información";
            }
            else if (validDisplayDocument && displayTable.Equals("col-lg-12"))
            {
                displayTable = "col-lg-6";
                displayDocument = "col-lg-6";
                docText = "Mostrar documento";
                docIcon = "fa-solid fa-eye";
            }*/
            else if (!validDisplayDocument && displayTable.Equals("col-lg-6"))
            {
                tableText = Translation["ShowInformation"];
                docText = Translation["HideInformation"];
                displayTable = "d-none";
                displayDocument = "col-lg-12";
                docIcon = "fa-solid fa-eye-slash";
                tableIcon = "fa-solid fa-eye";
            }
            else
            {
                docText = Translation["ShowDocument"];
                displayTable = "col-lg-6";
                displayDocument = "col-lg-6";
                docIcon = "fa-solid fa-eye";
            }
        }

        #endregion HandleVisibilityPdf

        #endregion HandleVisibilityButtons

        #endregion TableAndPdf

        #region UpdateModalStatus

        public void UpdateModalStatus(bool newValue)
        {
            modalStatus = newValue;
            StateHasChanged();
        }

        #endregion UpdateModalStatus

        #region ReceiveDocumentalVersion

        public async Task ReceiveDocumentalVersionAsync(int documentalVersionIdRecord)
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            documentalVersionId = documentalVersionIdRecord;
            await GetActs();

            if (listAdminActDtoResponse.Count != 0)
            {
                await SelectToPreviewPdf(listAdminActDtoResponse[0]);
            }
            StateHasChanged();
            SpinnerLoaderService.HideSpinnerLoader(Js);
        }

        #endregion ReceiveDocumentalVersion

        #region EnableSaveButton

        public void EnableSaveButton()
        {
            var everyoneHasDateAndName = false;
            listAdminActDtoRequest.ToList().ForEach(x =>
            {
                var search = listAdminActDtoResponse.FindAll(y => y.DataFile != null && y.DataFile.SequenceEqual(x.DataFile)).FirstOrDefault();
                if (search != null && search.AdminActDate == null || string.IsNullOrEmpty(search!.ActName))
                {
                    everyoneHasDateAndName = true;
                }
            });

            if ((listAdminActDtoRequest == null || listAdminActDtoRequest.Count == 0) || everyoneHasDateAndName)
            {
                saveIsDisable = true;
            }
            else
            {
                saveIsDisable = false;
            }
            StateHasChanged();
        }

        #endregion EnableSaveButton

        #region GetUploadMessage

        public string GetUploadMessage()
        {
            SpinnerLoaderService.ShowSpinnerLoader(Js);
            string extensions = string.Join(" " + Translation["Or"] + " ", AllowedExtensions).Replace(".", "").ToUpper();
            SpinnerLoaderService.HideSpinnerLoader(Js);

            return string.Format(Translation["AllowedFormatFiles"], extensions, FileSize);
        }

        #endregion GetUploadMessage

        #endregion OthersMethods

        #endregion Methods
    }
}