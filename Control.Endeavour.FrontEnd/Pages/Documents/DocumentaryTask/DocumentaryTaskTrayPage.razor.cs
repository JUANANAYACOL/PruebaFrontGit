using Control.Endeavour.FrontEnd.Components.Components.Inputs;
using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Components.Components.Pagination;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.DocumentaryTask;
using Control.Endeavour.FrontEnd.Components.Modals.Documents.ManagementTray;
using Control.Endeavour.FrontEnd.Components.Modals.Generic.GenericSearch;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Administration.City.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Request;
using Control.Endeavour.FrontEnd.Models.Models.Administration.SystemConfiguration.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VDocumentaryTypology.Response;
using Control.Endeavour.FrontEnd.Models.Models.Administration.VUser.Response;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Request;
using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.GenericDtos;
using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using Control.Endeavour.FrontEnd.Models.Models.Pagination;
using Control.Endeavour.FrontEnd.Resources;
using Control.Endeavour.FrontEnd.Services.Services.Global;
using Control.Endeavour.FrontEnd.Services.Services.Language;
using Control.Endeavour.FrontEnd.StateContainer.Documents;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using System.Diagnostics;
using System.Drawing.Text;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telerik.Blazor.Components;
using Telerik.DataSource;
using Telerik.SvgIcons;

namespace Control.Endeavour.FrontEnd.Pages.Documents.DocumentaryTask
{
    public partial class DocumentaryTaskTrayPage
    {
        #region Variables

        #region Inject

        /*[Inject]
        private EventAggregatorService? EventAggregator { get; set; }*/

        [Inject]
        private HttpClient? HttpClient { get; set; }

        [Inject]
        private DocumentsStateContainer? documentsStateContainer { get; set; }

        [Inject]
        private NavigationManager navigation { get; set; }

        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        [Inject]
        private IJSRuntime Js { get; set; }

        #endregion Inject

        #region Components

        private NewPaginationComponent<VDocumentaryTaskDtoResponse, FilterManagementDtoRequest> paginationComponent = new();

        #endregion Components

        #region Modals

        private NotificationsComponentModal notificationModal = new();
        private NotificationsComponentModal notificationModalSucces = new();
        private GenericSearchModal genericSearchModal = new();
        private ValidateDocumentGeneralInfoModal generalInformationModal = new();
        private DocTaskWorkFlowModal docTaskWorkFlowModal = new();
        private GenericUserSearchModal genericUserSearch = new();

        #endregion Modals

        #region Models

        private VDocumentaryTaskDtoResponse docToDelete = new();
        private RetunUserSearch vUserSelected = new();
        private DataCardDocTaskDtoResponse dataCardsDocTask = new DataCardDocTaskDtoResponse();
        private PaginationInfo paginationInfo = new();
        private FilterManagementDtoRequest filter { get; set; } = new();
        public InputModalComponent docTaskInput { get; set; } = new();
        public InputModalComponent userInput { get; set; } = new();

        #endregion Models

        #region Environments

        #region Environments(String)

        public string taskIdValue { get; set; } = "";
        private string created = "";
        private string review = "";
        private string approve = "";
        private string toSign = "";
        private string signed = "";
        private string involved = "";
        private string title = "";
        private string user = "UserForwardName";
        private string codeRV = "TAINS,RV";
        private string codeAP = "TAINS,AP";
        private string codeFR = "TAINS,FR";
        private string codePR = "TAINS,PR";
        private string id1 = "UserTaskId";
        private string id2 = "UserForwardId";
        private string codeP = "ProcessCode";
        private string codeI = "InstructionCode";
        private string UriFilterDocs = "documentarytasks/DocumentaryTask/ByFilter";
        public string descriptionInput { get; set; } = "";

        #endregion Environments(String)

        #region Environments(Numeric)

        private int characterCounter = 0;

        #endregion Environments(Numeric)

        #region Environments(DateTime)

        public DateTime? StartValue { get; set; } = DateTime.Now;
        public DateTime? EndValue { get; set; } = DateTime.Now.AddDays(10);

        private DateTime Min = new DateTime(1990, 1, 1, 8, 15, 0);
        private DateTime Max = new DateTime(2025, 1, 1, 19, 30, 45);

        #endregion Environments(DateTime)

        #region Environments(Bool)

        private bool startDate = false;
        private bool endDate = false;
        private bool activeState = false;

        private bool grid1 = false;
        private bool grid2 = false;
        private bool grid3 = false;
        private bool grid4 = false;

        private bool positionCard = false;
        private bool UserModalStatus = false;

        #endregion Environments(Bool)

        #region Environments(List & Dictionary)

        private List<VDocumentaryTaskDtoResponse> documentaryTaskList = new();

        #endregion Environments(List & Dictionary)

        #endregion Environments

        #endregion Variables

        #region OnInitializedAsync

        protected override async Task OnInitializedAsync()
        {
            title = Translation["SentTo"];

            if (string.IsNullOrEmpty(documentsStateContainer.Code))
            {
                await GetDataCards(codeP, id1, new List<string>() { codePR, codeFR }, new List<bool>() { true, false, true, true });
            }
            else
            {
                await GetDataCards(documentsStateContainer.Code, documentsStateContainer.UserType, documentsStateContainer.Codes, documentsStateContainer.Values);
            }

            //EventAggregator.LanguageChangedEvent += HandleLanguageChanged;
            await GetNumericDataCards();
        }

        #endregion OnInitializedAsync

        #region Methods

        #region HandleMethods

        private async Task HandleLanguageChanged()
        {
            StateHasChanged();
        }

        private void ShowUsersModal(bool value)
        {
            //genericSearchModal.UpdateModalStatus(true);
            UserModalStatus = true;
        }

        private async Task ShowModalGeneralInformation(VDocumentaryTaskDtoResponse model)
        {
            await generalInformationModal.GeneralInformation(model.ControlId.Value);
            generalInformationModal.UpdateModalStatus(true);
        }

        private async Task ShowModalDocTaskWorkFlow(VDocumentaryTaskDtoResponse doc)
        {
            await docTaskWorkFlowModal.GetWorkFlowDocumentaryTask(doc.TaskId);
            docTaskWorkFlowModal.UpdateModalStatus(true);
        }

        private void HandlePaginationGrid(List<VDocumentaryTaskDtoResponse> newDataList)
        {
            documentaryTaskList = newDataList;
        }

        private async Task HandleTaskManagementSubmit(VDocumentaryTaskDtoResponse doc)
        {
            try
            {
                AppKeysFilterDtoRequest appKeysFilter = new();
                appKeysFilter.FunctionName = "ContenedorStorage";
                var responseApi = await HttpClient.PostAsJsonAsync("params/AppKeys/ByFilter", appKeysFilter);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<List<AppKeysDtoResponse>>>();
                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    var docTemp = new
                    {
                        Identification = doc.TaskId,
                        PositionCard = positionCard
                    };

                    //documentsStateContainer.ParametrosVisor(doc.TaskId, positionCard);
                    await Js.InvokeVoidAsync("encryptDataReturn", docTemp, "data", deserializeResponse.Data[0].Value1);
                    await CheckReadDocument(doc);
                    navigation.NavigateTo("/TaskManagement");
                }

                //var docTemp = new
                //{
                //    Identification = doc.TaskId,
                //    PositionCard = positionCard
                //};
                //EncriptService.Encrypt(HttpClient, Js, docTemp, "container");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los tipos de documento: {ex.Message}");
            }
        }

        private void HandleUserChanged(List<RetunUserSearch> user)
        {
            //genericSearchModal.UpdateModalStatus(user.ModalStatus);
            vUserSelected = user[0];
        }

        #endregion HandleMethods

        #region OthersMethods

        #region GetNumericDataCardsDocumentTask

        private async Task GetNumericDataCards()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                var deserializeResponse = await HttpClient.GetFromJsonAsync<HttpResponseWrapperModel<DataCardDocTaskDtoResponse>>("documentarytasks/DocumentaryTask/GetCountTask");
                dataCardsDocTask = deserializeResponse.Data;

                if (dataCardsDocTask != null)
                {
                    created = dataCardsDocTask.Created.ToString();
                    review = dataCardsDocTask.Review.ToString();
                    approve = dataCardsDocTask.Approve.ToString();
                    toSign = dataCardsDocTask.ToSign.ToString();
                    signed = dataCardsDocTask.Signed.ToString();
                    involved = dataCardsDocTask.Involved.ToString();
                }
                else
                {
                    created = "0";
                    review = "0";
                    approve = "0";
                    toSign = "0";
                    signed = "0";
                    involved = "0";
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener N° de tareas documentales: {ex.Message}");
            }
        }

        #endregion GetNumericDataCardsDocumentTask

        #region GetDataCardsDocumentTask

        private async Task GetDataCards(string code, string userId, List<string> codes, List<bool> value)
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                filter = new();

                filter.UserForwardId = (userId.Equals(id2)) ? true : false;
                filter.UserTaskId = (userId.Equals(id1)) ? true : false;
                filter.InstructionCode = (code.Equals(codeI)) ? codes : new();
                filter.ProcessCode = (code.Equals(codeP)) ? codes : new();

                if (codes.Count > 1)
                {
                    filter.Indicted = null;
                    positionCard = false;
                }
                else
                {
                    filter.Indicted = value[3];
                    positionCard = true;

                    if (filter.ProcessCode.Any() && filter.ProcessCode[0].Equals(codeFR))
                    {
                        positionCard = false;
                    }
                }

                grid1 = value[0];
                grid2 = value[1];
                grid3 = value[2];
                grid4 = value[0];

                if ((grid1 && grid3) && codes[0].Equals(codeFR))
                {
                    user = "UserTaskName";
                    title = Translation["SignedBy"];
                    grid4 = false;
                }
                else
                {
                    user = "UserForwardName";
                    title = Translation["SentTo"];
                }

                filter.OrderBy = "TaskDate";
                filter.OrderAsc = false;

                var responseApi = await HttpClient.PostAsJsonAsync(UriFilterDocs, filter);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<VDocumentaryTaskDtoResponse>>>();

                if (deserializeResponse.Succeeded && deserializeResponse.Data.Data != null)
                {
                    documentaryTaskList = deserializeResponse.Data.Data.DistinctBy(x => x.TaskId).ToList();
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                }
                else
                {
                    documentaryTaskList = new();
                    Console.WriteLine(deserializeResponse.Message);
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener tareas documentales: {ex.Message}");
            }
        }

        #endregion GetDataCardsDocumentTask

        #region GetDocumenTaskFilter

        private async Task GetDocumentsTaskFilter()
        {
            try
            {
                SpinnerLoaderService.ShowSpinnerLoader(Js);
                int taskId = string.IsNullOrEmpty(taskIdValue) ? 0 : int.Parse(taskIdValue);
                string description = string.IsNullOrEmpty(descriptionInput) ? "" : descriptionInput;
                DateTime? stDate = StartValue;
                DateTime? enDate = EndValue;

                if (!(startDate && endDate))
                {
                    stDate = null;
                    enDate = null;
                }

                filter.TaskId = taskId;
                filter.StartDate = stDate;
                filter.EndDate = enDate;
                filter.TaskDescription = description;
                //filtro.UserTaskId = true;
                filter.UserId = vUserSelected.UserId;

                filter.OrderBy = "TaskDate";
                filter.OrderAsc = false;

                var responseApi = await HttpClient.PostAsJsonAsync(UriFilterDocs, filter);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<VDocumentaryTaskDtoResponse>>>();

                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    documentaryTaskList = deserializeResponse.Data != null ? deserializeResponse.Data.Data.ToList() : new List<VDocumentaryTaskDtoResponse>();
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                }
                else
                {
                    documentaryTaskList = new();
                    Console.WriteLine("something wrong will hapen");
                    notificationModal.UpdateModal(ModalType.Error, Translation["CannotFindRecords"], true);
                }
                SpinnerLoaderService.HideSpinnerLoader(Js);
            }
            catch (Exception ex)
            {
                SpinnerLoaderService.HideSpinnerLoader(Js);
                Console.WriteLine($"Error al obtener tareas documentales: {ex.Message}");
                notificationModal.UpdateModal(ModalType.Error, Translation["UnexpectedError"], true);
            }
        }

        #endregion GetDocumenTaskFilter

        #region Check Read Document

        private async Task CheckReadDocument(VDocumentaryTaskDtoResponse doc)
        {
            try
            {
                ViewStateDtoRequest viewDoc = new()
                {
                    TaskManagementId = doc.TaskManagementId,
                    UpdateUserId = doc.UserTaskId.Value
                };

                var responseApi = await HttpClient.PostAsJsonAsync("documentarytasks/DocumentaryTask/ViewStateTasksManagement", viewDoc);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<bool>>();
                if (deserializeResponse.Succeeded)
                {
                    Console.WriteLine("checkbox marcado con exito");
                }
                else
                {
                    Console.WriteLine("no se pudo marcar el checkbox");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar el checkbox: {ex.Message}");
            }
        }

        #endregion Check Read Document

        #region DeleteDocumentTask

        private void ShowDeleteModal(VDocumentaryTaskDtoResponse doc)
        {
            docToDelete = doc;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteQuestionMessage"], true, @Translation["Yes"], @Translation["No"], modalOrigin: "DeleteModal");
        }

        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                DeleteGeneralDtoRequest Task = new DeleteGeneralDtoRequest()
                {
                    Id = docToDelete.TaskId,
                };

                var responseApi = await HttpClient.PostAsJsonAsync("documentarytasks/DocumentaryTask/DeleteDocumentaryTask", Task);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<bool>>();

                if (deserializeResponse.Succeeded)
                {
                    notificationModalSucces.UpdateModal(ModalType.Success, Translation["DeleteSuccessfulMessage"], true);
                    await GetDataCards(codeP, id1, new List<string>() { codePR, codeFR }, new List<bool>() { true, false, true });
                    await GetNumericDataCards();
                }
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["CannotDelete"], true);
            }
        }

        #endregion DeleteDocumentTask

        #region resetModal

        public async Task ResetGrid()
        {
            taskIdValue = string.Empty;
            descriptionInput = string.Empty;
            vUserSelected = new();
            filter.TaskId = 0;
            characterCounter = 0;
            filter.StartDate = null;
            filter.EndDate = null;
            filter.TaskDescription = string.Empty;
            filter.UserId = null;
            await GetDocumentsTaskFilter();
        }

        #endregion resetModal

        #region CountChar

        private void ContarCaracteres(ChangeEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value.ToString()))
            {
                characterCounter = e.Value.ToString().Length;
            }
            else
            {
                characterCounter = 0;
            }
        }

        #endregion CountChar

        #region CloseModalSearchUser
        private void HandleModalUserClosed(bool status)
        {
            UserModalStatus = status;
            genericUserSearch.RemoveUserSelectedById(vUserSelected.UserId);
        }

        #endregion

        #region ExportToExcel

        public bool CancelExport { get; set; }

        private List<TemplateDocumentDtoResponse> allRecords { get; set; }

        bool ExportAllPages { get; set; }

        public async Task GetAllRecords()
        {
            try
            {
                int taskId = string.IsNullOrEmpty(taskIdValue) ? 0 : int.Parse(taskIdValue);
                string description = string.IsNullOrEmpty(descriptionInput) ? "" : descriptionInput;
                DateTime? stDate = StartValue;
                DateTime? enDate = EndValue;

                if (!(startDate && endDate))
                {
                    stDate = null;
                    enDate = null;
                }

                filter.TaskId = taskId;
                filter.StartDate = stDate;
                filter.EndDate = enDate;
                filter.TaskDescription = description;
                //filtro.UserTaskId = true;
                filter.UserId = vUserSelected.UserId;

                filter.OrderBy = "TaskDate";
                filter.OrderAsc = false;

                var responseApi = await HttpClient.PostAsJsonAsync(UriFilterDocs, filter);
                var deserializeResponse = await responseApi.Content.ReadFromJsonAsync<HttpResponseWrapperModel<PaginationResponse<VDocumentaryTaskDtoResponse>>>();

                if (deserializeResponse.Succeeded && deserializeResponse.Data != null)
                {
                    documentaryTaskList = deserializeResponse.Data != null ? deserializeResponse.Data.Data.ToList() : new List<VDocumentaryTaskDtoResponse>();
                    paginationInfo = deserializeResponse.Data.ExtractPaginationInfo();
                    paginationComponent.ResetPagination(paginationInfo);
                }
                else
                {
                    documentaryTaskList = new();
                    Console.WriteLine("something wrong will hapen");
                    notificationModal.UpdateModal(ModalType.Error, Translation["CannotFindRecords"], true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener tareas documentales: {ex.Message}");
                notificationModal.UpdateModal(ModalType.Error, Translation["UnexpectedError"], true);
            }
        }

        public async Task OnBeforeExcelExport(GridBeforeExcelExportEventArgs args)
        {

            if (ExportAllPages)
            {
                await GetAllRecords();

                if (allRecords.Any())
                {
                    args.Data = allRecords;
                }
            }
            else { args.Data = documentaryTaskList; }

            args.IsCancelled = CancelExport;
        }

        #endregion

        #endregion OthersMethods

        #endregion Methods
    }
}