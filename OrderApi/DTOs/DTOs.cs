using OrderApi.Models;

namespace OrderApi.DTOs;

public record LoginRequest(string Phone, string Password);
public record AuthResponse(bool Success, string Message, User? User, string? Token);

public record JobResponse(
    string JobID, 
    string BranchName, 
    string JobTypeName, 
    string Status, 
    DateTime StartDate, 
    DateTime EndDate
);

public record CheckInRequest(string LatLong);
public record PhotoUploadRequest(string Base64Image, string Caption);
public record SubmitJobRequest(string IssueDetail);
public record CreateJobRequest(
    string BranchCode, 
    string JobTypeID, 
    string AssignedTeamID, 
    DateTime StartDate, 
    DateTime EndDate
);
