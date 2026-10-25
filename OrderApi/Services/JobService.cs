using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.DTOs;
using OrderApi.Models;

namespace OrderApi.Services;

public interface IJobService
{
    Task<IEnumerable<Job>> GetJobsAsync(string role, string userId);
    Task<bool> SaveCheckInAsync(string jobId, string latLong);
    Task<string> UploadPhotoAsync(string jobId, string base64, string caption);
    Task<bool> SubmitJobAsync(string jobId, string issueDetail);
    Task<string> CreateJobAsync(CreateJobRequest data);
}

public class JobService : IJobService
{
    private readonly ApplicationDbContext _context;

    public JobService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Job>> GetJobsAsync(string role, string userId)
    {
        var query = _context.Jobs
            .Include(j => j.Branch)
            .Include(j => j.JobType)
            .AsQueryable();

        if (role == UserRole.Technician.ToString())
        {
            query = query.Where(j => j.Assigned_Team_ID == userId);
        }

        return await query.ToListAsync();
    }

    public async Task<bool> SaveCheckInAsync(string jobId, string latLong)
    {
        var job = await _context.Jobs.FindAsync(jobId);
        if (job == null) return false;

        job.Check_in_Time = DateTime.Now;
        job.Check_in_LatLong = latLong;
        job.Status = JobStatus.InProgress;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string> UploadPhotoAsync(string jobId, string base64, string caption)
    {
        // ในระบบจริงจะอัปโหลดไป Google Drive ตาม SRS
        // นี่เป็น Mock-up URL
        var imageUrl = $"https://storage.googleapis.com/7-11-jobs/{jobId}/{Guid.NewGuid()}.jpg";
        
        var photo = new JobPhoto
        {
            Job_ID = jobId,
            Image_URL = imageUrl,
            Caption = caption,
            Timestamp = DateTime.Now
        };

        _context.JobPhotos.Add(photo);
        await _context.SaveChangesAsync();
        return imageUrl;
    }

    public async Task<bool> SubmitJobAsync(string jobId, string issueDetail)
    {
        var job = await _context.Jobs.FindAsync(jobId);
        if (job == null) return false;

        if (!string.IsNullOrEmpty(issueDetail))
        {
            _context.IssueLogs.Add(new IssueLog
            {
                Job_ID = jobId,
                Issue_Detail = issueDetail,
                Reported_By = job.Assigned_Team_ID,
                Timestamp = DateTime.Now
            });
        }

        job.Status = JobStatus.Submitted;
        job.Finished_Time = DateTime.Now;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string> CreateJobAsync(CreateJobRequest data)
    {
        var jobId = "JOB-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        var job = new Job
        {
            Job_ID = jobId,
            Branch_Code = data.BranchCode,
            JobType_ID = data.JobTypeID,
            Assigned_Team_ID = data.AssignedTeamID,
            Start_Date = data.StartDate,
            End_Date = data.EndDate,
            Status = JobStatus.Pending
        };

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();
        return jobId;
    }
}
