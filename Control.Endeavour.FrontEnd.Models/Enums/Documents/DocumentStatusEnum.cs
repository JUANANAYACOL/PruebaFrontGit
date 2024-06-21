using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Enums.Documents;

public enum DocumentStatusEnum : short
{
    /// <summary>
    /// En Proceso
    /// </summary>
    [ControlEnum("ES,ENP", "InProgress")]
    WithoutProcessingWord = 1,

    /// <summary>
    /// En Transito
    /// </summary>
    [ControlEnum("ES,ETR", "InTransit")]
    InProgressWord = 2,

    /// <summary>
    /// Gestion Exitosa
    /// </summary>
    [ControlEnum("ES,GEX", "SuccessfulManagement")]
    SuccessfullManagementWord = 3,

    /// <summary>
    /// Copias
    /// </summary>
    [ControlEnum("", "Copies")]
    Copy = 4,
}
