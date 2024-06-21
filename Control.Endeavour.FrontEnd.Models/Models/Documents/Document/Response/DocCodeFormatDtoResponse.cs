

namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response
{
    public class DocCodeFormatDtoResponse
    {
        public int DocCodeFormatId { get; set; }
        public string ClassCode { get; set; } = null!;
        public string? ClassCodeName { get; set; }
        public string? Prefix { get; set; }
        public int PrefixLength { get; set; }
        public string? Data { get; set; }
        public int DataLength { get; set; } = 5;
        public string? Suffix { get; set; }
        public int SuffixLength { get; set; }
        public string? Separator { get; set; }
        public bool Active { get; set; }
        public bool ActiveState { get; set; } = true;
        public string? CreateUser { get; set; }

        public string? UpdateUser { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }



    }
}
