using Control.Endeavour.FrontEnd.Components.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Enums.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Models.Models.Components.Modals;
using Control.Endeavour.FrontEnd.Models.Models.Components.UploadFiles;
using Control.Endeavour.FrontEnd.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;


namespace Control.Endeavour.FrontEnd.Components.Components.UploadFiles
{
    public partial class DragAndDropTemplateComponent : ComponentBase
    {
        #region Inject
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }

        #endregion

        #region Parameters

        [Parameter] public int MaxFileCount { get; set; } = 5;
        [Parameter] public string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".svg", ".pdf", ".xlsx", ".docx", ".xls", ".doc" };
        [Parameter] public int MaxFileSizeMB { get; set; } = 30;
        [Parameter] public bool VisibleFiles { get; set; } = true;
        [Parameter] public EventCallback<List<FileInfoData>> OnFileListChanged { get; set; }
        [Parameter] public TypeOfFilesEnum TypeOfFiles { get; set; } = TypeOfFilesEnum.Files;
        [Parameter] public List<FileInfoData> FileInfos { get; set; } = new List<FileInfoData>();
        [Parameter] public string IdInputElement { get; set; } = "file-input-element";

        #endregion

        #region Variables
        Dictionary<string, string> bootstrapIcons = new Dictionary<string, string>
        {
            { ".aac", "bi bi-filetype-aac" },
            { ".ai", "bi bi-filetype-ai" },
            { ".bmp", "bi bi-filetype-bmp" },
            { ".cs", "bi bi-filetype-cs" },
            { ".css", "bi bi-filetype-css" },
            { ".csv", "bi bi-filetype-csv" },
            { ".doc", "bi bi-filetype-doc" },
            { ".docx", "bi bi-filetype-docx" },
            { ".exe", "bi bi-filetype-exe" },
            { ".gif", "bi bi-filetype-gif" },
            { ".heic", "bi bi-filetype-heic" },
            { ".html", "bi bi-filetype-html" },
            { ".java", "bi bi-filetype-java" },
            { ".jpg", "bi bi-filetype-jpg" },
            { ".jpeg", "bi bi-filetype-jpg" },
            { ".js", "bi bi-filetype-js" },
            { ".json", "bi bi-filetype-json" },
            { ".jsx", "bi bi-filetype-jsx" },
            { ".key", "bi bi-filetype-key" },
            { ".m4p", "bi bi-filetype-m4p" },
            { ".md", "bi bi-filetype-md" },
            { ".mdx", "bi bi-filetype-mdx" },
            { ".mov", "bi bi-filetype-mov" },
            { ".mp3", "bi bi-filetype-mp3" },
            { ".mp4", "bi bi-filetype-mp4" },
            { ".otf", "bi bi-filetype-otf" },
            { ".pdf", "bi bi-filetype-pdf" },
            { ".php", "bi bi-filetype-php" },
            { ".png", "bi bi-filetype-png" },
            { ".ppt", "bi bi-filetype-ppt" },
            { ".pptx", "bi bi-filetype-pptx" },
            { ".psd", "bi bi-filetype-psd" },
            { ".py", "bi bi-filetype-py" },
            { ".raw", "bi bi-filetype-raw" },
            { ".rb", "bi bi-filetype-rb" },
            { ".sass", "bi bi-filetype-sass" },
            { ".scss", "bi bi-filetype-scss" },
            { ".sh", "bi bi-filetype-sh" },
            { ".sql", "bi bi-filetype-sql" },
            { ".svg", "bi bi-filetype-svg" },
            { ".tiff", "bi bi-filetype-tiff" },
            { ".tsx", "bi bi-filetype-tsx" },
            { ".ttf", "bi bi-filetype-ttf" },
            { ".txt", "bi bi-filetype-txt" },
            { ".wav", "bi bi-filetype-wav" },
            { ".woff", "bi bi-filetype-woff" },
            { ".xls", "bi bi-filetype-xls" },
            { ".xlsx", "bi bi-filetype-xlsx" },
            { ".xml", "bi bi-filetype-xml" },
            { ".yml", "bi bi-filetype-yml" },

        };
        private NotificationsComponentModal notificationModal;
        private string hoverClass = string.Empty;
        private ElementReference dropContainer;
        private InputFile? inputFile;
        private bool isLoading = false;
        private FileInfoData recordToDelete;
        IJSObjectReference? pasteFileModule;
        IJSObjectReference? initializePasteFileFunction;
        private string containerClass { get; set; } = "dADTemplate";


        #endregion

        #region Methods
        protected override async Task OnInitializedAsync()
        {
            if (TypeOfFiles == TypeOfFilesEnum.ProfilePicture)
            {
                containerClass = "";
            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("initializeFilePaste", dropContainer, inputFile!.Element);
            }
        }

        #region Js 
        private async Task TriggerFileInputClick()
        {
            await JSRuntime.InvokeVoidAsync("triggerClick", IdInputElement);
        }
        void OnDragEnter(DragEventArgs e) => hoverClass = "hover";
        void OnDragLeave(DragEventArgs e) => hoverClass = string.Empty;

        private async Task NotifyFileListChanged()
        {
            await OnFileListChanged.InvokeAsync(FileInfos.ToList());
        }
        public async ValueTask DisposeAsync()
        {
            if (initializePasteFileFunction is not null)
            {
                await initializePasteFileFunction.InvokeVoidAsync("dispose");
                await initializePasteFileFunction.DisposeAsync();
            }

            if (pasteFileModule is not null)
            {
                await pasteFileModule.DisposeAsync();
            }
        }
        #endregion

        #region File Processing Methods

        async Task OnChange(InputFileChangeEventArgs e)
        {
            isLoading = true;
            StateHasChanged();
            foreach (var file in e.GetMultipleFiles())
            {
                if (MaxFileCount == 1)
                {
                    FileInfos.Clear();
                }
                if (IsValidFile(file))
                {

                    await ProcessValidFile(file);
                }
                else
                {

                    await HandleInvalidFile(file);
                }
            }

            isLoading = false;
            StateHasChanged();
        }
        private bool IsValidFile(IBrowserFile file)
        {
            return IsSizeValid(file) && IsExtensionValid(file) && IsFileCountValid();
        }

        private async Task ProcessValidFile(IBrowserFile file)
        {
            var fileInfo = await ConvertFileToFileInfo(file);
            FileInfos.Add(fileInfo);
            await NotifyFileListChanged();
        }
        private async Task HandleInvalidFile(IBrowserFile file)
        {
            if (!IsFileCountValid())
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["NumberFilesExceededMessage"], true, Translation["Accept"]);
            }
            else
            {
                notificationModal.UpdateModal(ModalType.Error, Translation["FileNotAllowedMessage"], true, Translation["Accept"]);
            }
            
            await NotifyFileListChanged();
        }
        private bool IsSizeValid(IBrowserFile file)
        {

            return file.Size <= MaxFileSizeMB * 1024 * 1024;
        }

        private bool IsExtensionValid(IBrowserFile file)
        {

            var fileExtension = Path.GetExtension(file.Name).ToLowerInvariant();
            return AllowedExtensions.Contains(fileExtension);
        }

        private bool IsFileCountValid()
        {
            return FileInfos.Count < MaxFileCount;
        }
        private async Task<FileInfoData> ConvertFileToFileInfo(IBrowserFile file)
        {

            using var stream = file.OpenReadStream(MaxFileSizeMB * 1024 * 1024);
            using var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            var base64Data = ms.ToArray();
            return new FileInfoData
            {
                Name = Path.GetFileNameWithoutExtension(file.Name),
                Extension = Path.GetExtension(file.Name),
                Size = file.Size,
                IconPath = GetBootstrapIconByExtension(Path.GetExtension(file.Name)),
                Base64Data = base64Data,
            };
        }
        private string GetBootstrapIconByExtension(string extension)
        {
            return bootstrapIcons.TryGetValue(extension, out var icon) ? icon : "bi bi-file";
        }
        private async Task DeleteFile(FileInfoData fileInfo)
        {
            FileInfos.Remove(fileInfo);
            await NotifyFileListChanged();
        }
        #endregion

        #region Modal
        private async Task DeleteFileModalOpen(FileInfoData fileInfo)
        {
            recordToDelete = fileInfo;
            notificationModal.UpdateModal(ModalType.Warning, Translation["DeleteAttachmentMessage"], true, Translation["Yes"], Translation["No"]);
        }
        private async Task ChangeProfilePicture(FileInfoData fileInfo)
        {
            FileInfos.Remove(fileInfo);
            await NotifyFileListChanged();
            await TriggerFileInputClick();



        }


        private async Task HandleModalNotiClose(ModalClosedEventArgs args)
        {
            if (args.IsAccepted)
            {
                await DeleteFile(recordToDelete);
            }
            else
            {
                Console.WriteLine(Translation["RecordNotDeleted"]);
            }
        }

        #endregion

        #endregion
    }
}
