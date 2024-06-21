using Control.Endeavour.FrontEnd.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Control.Endeavour.FrontEnd.Components.Components.Cards
{
    public partial class AttachmentCardComponent
    {
        [Inject]
        private IStringLocalizer<Translation>? Translation { get; set; }


        #region Parameters

        [Parameter]
        public string ArchiveName { get; set; } = "NameNotFound";
        [Parameter]
        public string ArchiveExt { get; set; } = "DocumentTypeNotFound";
        [Parameter]
        public string AttDescription { get; set; } = "DescriptionNotFound";
        [Parameter]
        public string CreateUser { get; set; } = "UserNotFound";
        [Parameter]
        public string CreateDate { get; set; } = "DateNotFound";

        #endregion

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            if (ArchiveName == "NameNotFound") 
                ArchiveName = Translation[ArchiveName];
            if (ArchiveExt == "DocumentTypeNotFound")
                ArchiveExt = Translation[ArchiveExt];
            if (AttDescription == "DescriptionNotFound")
                AttDescription = Translation[AttDescription];
            if (CreateUser == "UserNotFound")
                CreateUser = Translation[CreateUser];
            if (CreateDate == "DateNotFound")
                CreateDate = Translation[CreateDate];
        }


        #endregion
    }
}
