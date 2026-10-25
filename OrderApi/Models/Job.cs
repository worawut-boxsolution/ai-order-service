using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderApi.Models;

public enum JobStatus
{
    Pending,
    InProgress,
    Submitted,
    Completed
}

public class Job
{
    [Key]
    public string Job_ID { get; set; } = default!;

    public string Branch_Code { get; set; } = default!;
    [ForeignKey("Branch_Code")]
    public MasterBranch? Branch { get; set; }

    public string JobType_ID { get; set; } = default!;
    [ForeignKey("JobType_ID")]
    public MasterJobType? JobType { get; set; }

    public string Assigned_Team_ID { get; set; } = default!;
    [ForeignKey("Assigned_Team_ID")]
    public User? AssignedTeam { get; set; }

    public DateTime Start_Date { get; set; }
    public DateTime End_Date { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Pending;

    public DateTime? Check_in_Time { get; set; }
    public string? Check_in_LatLong { get; set; }
    public DateTime? Finished_Time { get; set; }

    public ICollection<JobPhoto> Photos { get; set; } = new List<JobPhoto>();
    public ICollection<IssueLog> Issues { get; set; } = new List<IssueLog>();
    public ICollection<BillingExpense> Expenses { get; set; } = new List<BillingExpense>();
}

public class JobPhoto
{
    [Key]
    public int Photo_ID { get; set; }
    public string Job_ID { get; set; } = default!;
    public string Image_URL { get; set; } = default!;
    public string Caption { get; set; } = default!;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public class IssueLog
{
    [Key]
    public int Issue_ID { get; set; }
    public string Job_ID { get; set; } = default!;
    public string Issue_Detail { get; set; } = default!;
    public string Reported_By { get; set; } = default!;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}

public class BillingExpense
{
    [Key]
    public int Expense_ID { get; set; }
    public string Job_ID { get; set; } = default!;
    public string Item_Name { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Receipt_URL { get; set; } = default!;
}
