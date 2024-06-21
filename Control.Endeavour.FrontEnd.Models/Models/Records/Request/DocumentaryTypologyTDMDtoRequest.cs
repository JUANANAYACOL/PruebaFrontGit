using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Records.Request
{
    public class DocumentaryTypologyTDMDtoRequest
    {
        public int ControlId { get; set; }
        public int DocumentaryTypologyId { get; set; }
        public string? TypologyName { get; set; }
        public string? DocDescription { get; set; }
        public string? Color { get; set; }
        public int StartFolio { get; set; }
        public int EndFolio { get; set; }
        public int NumberFolio { get; set; }
        public string? OriginCode { get; set; }
        public string? OriginName { get; set; }
        public string? Opcion { get; set; }
        public string? OpcionValor { get; set; }
        public string? IndexingType { get; set; }
        public int VolumeId { get; set; }
        public int DocumentId { get; set; }
        public string? FilingCode { get; set; }
        public string? CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string? UpdateUser { get; set; }
        public DateTime UpdateDate { get; set; }
    }
}
