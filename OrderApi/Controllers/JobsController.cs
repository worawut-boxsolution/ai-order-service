using Microsoft.AspNetCore.Mvc;
using OrderApi.DTOs;
using OrderApi.Services;

namespace OrderApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobsController(IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpGet]
    public async Task<IActionResult> GetJobs([FromQuery] string role, [FromQuery] string userId)
    {
        var jobs = await _jobService.GetJobsAsync(role, userId);
        return Ok(jobs);
    }

    [HttpPost("{id}/checkin")]
    public async Task<IActionResult> CheckIn(string id, [FromBody] CheckInRequest request)
    {
        var success = await _jobService.SaveCheckInAsync(id, request.LatLong);
        if (!success) return NotFound();
        return Ok(new { success, time = DateTime.Now });
    }

    [HttpPost("{id}/upload-photo")]
    public async Task<IActionResult> UploadPhoto(string id, [FromBody] PhotoUploadRequest request)
    {
        var url = await _jobService.UploadPhotoAsync(id, request.Base64Image, request.Caption);
        return Ok(new { success = true, url });
    }

    [HttpPost("{id}/submit")]
    public async Task<IActionResult> SubmitJob(string id, [FromBody] SubmitJobRequest request)
    {
        var success = await _jobService.SubmitJobAsync(id, request.IssueDetail);
        if (!success) return NotFound();
        return Ok(new { success });
    }

    [HttpPost]
    public async Task<IActionResult> CreateJob([FromBody] CreateJobRequest request)
    {
        var jobId = await _jobService.CreateJobAsync(request);
        return Ok(new { success = true, jobId });
    }
}
