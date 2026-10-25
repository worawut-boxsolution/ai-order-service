using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderApi.Data;
using OrderApi.Models;

namespace OrderApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MasterDataController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MasterDataController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("branch")]
    public async Task<IActionResult> AddBranch([FromBody] MasterBranch branch)
    {
        _context.Branches.Add(branch);
        await _context.SaveChangesAsync();
        return Ok(new { success = true });
    }

    [HttpGet("branches")]
    public async Task<IActionResult> GetBranches()
    {
        return Ok(await _context.Branches.ToListAsync());
    }

    [HttpGet("jobtypes")]
    public async Task<IActionResult> GetJobTypes()
    {
        return Ok(await _context.JobTypes.ToListAsync());
    }
}
