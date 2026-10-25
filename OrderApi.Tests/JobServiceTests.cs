using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.Models;
using OrderApi.Services;
using OrderApi.DTOs;
using Xunit;

namespace OrderApi.Tests;

public class JobServiceTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private async Task SeedMasterData(ApplicationDbContext context)
    {
        context.Branches.Add(new MasterBranch 
        { 
            Branch_Code = "B1", 
            Branch_Name = "Branch 1",
            Province = "Bangkok",
            District = "Pathum Wan",
            Location = "13.7465,100.5348"
        });
        context.JobTypes.Add(new MasterJobType 
        { 
            JobType_ID = "T1", 
            JobType_Name = "Electrical",
            Status = "Active"
        });
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task SaveCheckInAsync_ShouldChangeStatusToInProgress_WhenValid()
    {
        // Arrange
        using var context = GetDbContext();
        var jobId = "JOB-001";
        context.Jobs.Add(new Job
        {
            Job_ID = jobId,
            Status = JobStatus.Pending,
            Branch_Code = "B1",
            JobType_ID = "T1",
            Assigned_Team_ID = "TECH-01"
        });
        await context.SaveChangesAsync();

        var service = new JobService(context);

        // Act
        var result = await service.SaveCheckInAsync(jobId, "13.7563,100.5018");

        // Assert
        Assert.True(result);
        var updatedJob = await context.Jobs.FindAsync(jobId);
        Assert.NotNull(updatedJob);
        Assert.Equal(JobStatus.InProgress, updatedJob.Status);
    }

    [Fact]
    public async Task SubmitJobAsync_ShouldChangeStatusToSubmitted_AndRecordIssue()
    {
        // Arrange
        using var context = GetDbContext();
        var jobId = "JOB-002";
        context.Jobs.Add(new Job
        {
            Job_ID = jobId,
            Status = JobStatus.InProgress,
            Assigned_Team_ID = "TECH-01",
            Branch_Code = "B1",
            JobType_ID = "T1"
        });
        await context.SaveChangesAsync();

        var service = new JobService(context);
        var issueDetail = "พบก๊อกน้ำรั่วซึมเพิ่มเติม";

        // Act
        var result = await service.SubmitJobAsync(jobId, issueDetail);

        // Assert
        Assert.True(result);
        var updatedJob = await context.Jobs.FindAsync(jobId);
        Assert.NotNull(updatedJob);
        Assert.Equal(JobStatus.Submitted, updatedJob.Status);

        var issue = await context.IssueLogs.FirstOrDefaultAsync(i => i.Job_ID == jobId);
        Assert.NotNull(issue);
        Assert.Equal(issueDetail, issue.Issue_Detail);
    }

    [Fact]
    public async Task GetJobsAsync_ShouldReturnOnlyAssignedJobs_ForTechnician()
    {
        // Arrange
        using var context = GetDbContext();
        await SeedMasterData(context);

        context.Jobs.AddRange(new List<Job>
        {
            new Job { Job_ID = "J1", Assigned_Team_ID = "T1", Status = JobStatus.Pending, Branch_Code="B1", JobType_ID="T1" },
            new Job { Job_ID = "J2", Assigned_Team_ID = "T2", Status = JobStatus.Pending, Branch_Code="B1", JobType_ID="T1" },
            new Job { Job_ID = "J3", Assigned_Team_ID = "T1", Status = JobStatus.InProgress, Branch_Code="B1", JobType_ID="T1" }
        });
        await context.SaveChangesAsync();

        var service = new JobService(context);

        // Act
        var result = await service.GetJobsAsync(UserRole.Technician.ToString(), "T1");

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SaveCheckInAsync_ShouldReturnFalse_WhenJobNotFound()
    {
        // Arrange
        using var context = GetDbContext();
        var service = new JobService(context);

        // Act
        var result = await service.SaveCheckInAsync("NON-EXISTENT", "0,0");

        // Assert
        Assert.False(result);
    }
}
