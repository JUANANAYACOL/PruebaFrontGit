namespace Control.Endeavour.FrontEnd.Models.Models.Administration.Audit.Response
{
    public class VWLogsAuditDtoBugResponse
    {
        public int Log202301Id { get; set; }

        public DateTime Date { get; set; }

        public int ClientId { get; set; }

        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string Ip { get; set; } = null!;

        public string? Module { get; set; }

        public string? Micro { get; set; }

        public string? Class { get; set; }

        public string? Method { get; set; }

        public string? TableName { get; set; }

        public string? Script { get; set; }

        public string? Params { get; set; }

        public string? LogType { get; set; }

        public int? Audit202301Id { get; set; }

        public string? AuditType { get; set; }

        public string? Message { get; set; }

        public string? Detail { get; set; }

        public int? Bug202301Id { get; set; }

        public string? ErrorType { get; set; }

        public string? Exception { get; set; }

        public string? MessageError { get; set; }

        public string? StackTrace { get; set; }
    }
}