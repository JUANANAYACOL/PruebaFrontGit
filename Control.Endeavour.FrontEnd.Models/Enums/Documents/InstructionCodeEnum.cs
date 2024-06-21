using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Endeavour.FrontEnd.Models.Enums.Documents
{
    public enum InstructionCodeEnum : short
    {
        /// <summary>
        /// Revisar
        /// </summary>
        [ControlEnum("TAINS,RV", "Review")]
        Review = 1,

        /// <summary>
        /// Aprobar
        /// </summary>
        [ControlEnum("TAINS,AP", "Approve")]
        Approve = 2,

        /// <summary>
        /// Firmar
        /// </summary>
        [ControlEnum("TAINS,FR", "Sign")]
        Signature = 3,
    }
}
