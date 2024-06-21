using Control.Endeavour.FrontEnd.Models.Models.Pagination;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.DocumentaryTypologiesBag.Request
{
    public class DocumentaryTypologiesBagFitlerDtoRequest : PaginationRequest
    {
        public string TypologyName { get; set; }
    }
}