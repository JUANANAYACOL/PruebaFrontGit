namespace Control.Endeavour.FrontEnd.Models.Models.Pagination
{
    public class PaginationInfo
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }

        public PaginationInfo(int pageNumber = 0, int pageSize = 0, int totalPages = 0, int totalCount = 0)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = totalPages;
            TotalCount = totalCount;
        }
    }
}
