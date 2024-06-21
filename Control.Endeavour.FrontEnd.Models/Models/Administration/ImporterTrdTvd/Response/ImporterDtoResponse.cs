using Control.Endeavour.FrontEnd.Models.Models.HttpResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Administration.ImporterTrdTvd.Response
{
    public class ImporterDtoResponse
    {
        public int ImporterHistoryId { get; set; }
        public int DocumentalVersionId { get; set; }
        public int FileId { get; set; }
        public string Description { get; set; } = null;

        //Registros Creados
        public int contUnity { get; set; }
        public int contOffice { get; set; }
        public int contSeries { get; set; }
        public int contSubseries { get; set; }
        public int contTypologies { get; set; }
        public int contRetentions { get; set; }
        public int contTRD { get; set; }
        public int contTRDC { get; set; }

    }
}
