using System.Net.Http.Json;
using OrderWeb.Models;

namespace OrderWeb.Services;

public class JobApiService
{
    private readonly HttpClient _http;

    public JobApiService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<JobDto>> GetJobsAsync()
    {
        try 
        {
            // ในสถานการณ์จริงจะเรียก api/jobs แต่ตอนนี้เราจะจำลองข้อมูลบางส่วนก่อนหาก API ยังไม่พร้อม 
            // หรือเรียกจริงๆ ถ้า API ทำงานอยู่
            var jobs = await _http.GetFromJsonAsync<List<JobDto>>("api/jobs");
            return jobs ?? new List<JobDto>();
        }
        catch
        {
            // Fallback mock data for UI demo if API is not reachable
            return GetMockJobs();
        }
    }

    private List<JobDto> GetMockJobs()
    {
        return new List<JobDto>
        {
            new JobDto { JobId = "JOB-001", BranchCode = "B001", BranchName = "สาขา สุขุมวิท 11", JobTypeName = "ซ่อมไฟฟ้า", AssignedTeamName = "ทีมช่าง A", Status = JobStatus.Pending, StartDate = DateTime.Now },
            new JobDto { JobId = "JOB-002", BranchCode = "B015", BranchName = "สาขา ลาดพร้าว 101", JobTypeName = "ประปา", AssignedTeamName = "ทีมช่าง B", Status = JobStatus.InProgress, StartDate = DateTime.Now.AddDays(-1), CheckInTime = DateTime.Now.AddHours(-2) },
            new JobDto { JobId = "JOB-003", BranchCode = "B042", BranchName = "สาขา สยามสแควร์", JobTypeName = "โครงสร้าง", AssignedTeamName = "ทีมช่าง A", Status = JobStatus.Completed, StartDate = DateTime.Now.AddDays(-5), EndDate = DateTime.Now.AddDays(-4) }
        };
    }
}
