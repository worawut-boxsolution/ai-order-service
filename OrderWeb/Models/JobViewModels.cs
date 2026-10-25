namespace OrderWeb.Models;

public enum JobStatus
{
    Pending,
    InProgress,
    Submitted,
    Completed
}

public class JobDto
{
    public string JobId { get; set; } = string.Empty;
    public string BranchCode { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string JobTypeName { get; set; } = string.Empty;
    public string AssignedTeamName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public JobStatus Status { get; set; }
    public DateTime? CheckInTime { get; set; }
}

public class DashboardSummary
{
    public int TotalJobs { get; set; }
    public int PendingJobs { get; set; }
    public int InProgressJobs { get; set; }
    public int CompletedJobs { get; set; }
}
