﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Models.Records.Request
{
    public class RecordsOpeningDtoRequest
    {
        public int? RecordId { get; set; }
        public string? Justification { get; set; }
        public int? SubSeriesId { get; set; }
    }

}
