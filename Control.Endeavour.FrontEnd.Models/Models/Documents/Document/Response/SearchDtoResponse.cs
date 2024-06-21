namespace Control.Endeavour.FrontEnd.Models.Models.Documents.Document.Response
{
    public class SearchDtoResponse
    {
        public int DocumentId { get; set; }

        public int ControlId { get; set; }

        public string NroDocument { get; set; }

        public string FilingCode { get; set; }

        public DateTime DocDate { get; set; }

        public DateTime? DueDate { get; set; }

        public string DocDescription { get; set; }

        public int Year { get; set; }

        public string ClassCodeName { get; set; }
        public string ClassCode { get; set; }

        public string CreateDocumentUserFullName { get; set; }

        public string DocumentReceivers { get; set; }

        public string DocumentSignatories { get; set; }

        public DateTime? CreateDate { get; set; }

    }
}
