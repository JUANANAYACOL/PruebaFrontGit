using Control.Endeavour.FrontEnd.Models.Models.Documents.DocumentsTask.Response;
using Control.Endeavour.FrontEnd.Models.Models.Documents.Filing.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Records.Response
{
    public class RecordHistoryDtoResponse
    {
        public int RecordHistoryId { get; set; }
        public int RecordId { get; set; }
        public string? RecordsFileType { get; set; }
        public string? RecordFileTypeName { get; set; }
        public DateTime ChangeDate { get; set; }
        public bool Active { get; set; } = true;
        public string? CreateUser { get; set; }
        public string? UpdateUser { get; set; }
        public DateTime? CreateDate { get; set; } = DateTime.Now;
        public DateTime? UpdateDate { get; set; }
        public string? ActionType { get; set; }
        public string? ActionTypeName { get; set; }
        public int? SubSeriesId { get; set; }
        public string? SubSeries { get; set; }
        public string? Series { get; set; }
        public string? ProductionOffice { get; set; }
        public string? AdministrativeUnits { get; set; }
        public string? HistoryType { get; set; }
        public string? HistoryTypeName { get; set; }
        public string? ClosedType { get; set; }
        public string? ClosedTypeName { get; set; }
        public string? Operation { get; set; }
        public string? OperationName { get; set; }
        public string? Justification { get; set; }
        //public virtual Recorddto Record { get; set; } = null!;
    }


}
