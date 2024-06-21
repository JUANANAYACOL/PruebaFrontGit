namespace Control.Endeavour.FrontEnd.Models.Models.Pagination
{
    public class PaginationRequest
    {

        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? OrderBy { get; set; }
        public bool? OrderAsc { get; set; }

    }
}
