using AutoMapper;
using BookTracking.API.Models;
using BookTracking.Application.Dtos;
using BookTracking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookTracking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditLogController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;
    private readonly IMapper _mapper;

    public AuditLogController(IAuditLogService auditLogService, IMapper mapper)
    {
        _auditLogService = auditLogService;
        _mapper = mapper;
    }

    [HttpGet("filter")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AuditLogResponse>>>> Filter([FromQuery] FilterAuditLogRequest request)
    {
        try
        {
            var filterDto = _mapper.Map<AuditLogFilterCriteriaDto>(request);
            var logs = await _auditLogService.GetFilteredAuditLogsAsync(filterDto);
            var response = _mapper.Map<IEnumerable<AuditLogResponse>>(logs);
            
            return Ok(ApiResponse<IEnumerable<AuditLogResponse>>.Success(response, 200, "Audit logs retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<IEnumerable<AuditLogResponse>>.Failure("An unexpected error occurred while retrieving audit logs.", 500));
        }
    }
}
