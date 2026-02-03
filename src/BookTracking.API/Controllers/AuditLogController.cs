using AutoMapper;
using BookTracking.API.Models;
using BookTracking.Application.Dtos;
using BookTracking.Application.Interfaces;
using BookTracking.Domain.Enums;
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
    public async Task<ActionResult<ApiResponse<IEnumerable<GroupedAuditLogResponse>>>> Filter([FromQuery] FilterAuditLogRequest request)
    {
        try
        {
            var filterDto = _mapper.Map<AuditLogFilterCriteriaDto>(request);
            var groupedLogs = await _auditLogService.GetFilteredAuditLogsGroupedAsync(filterDto);
            var groupedResponse = _mapper.Map<IEnumerable<GroupedAuditLogResponse>>(groupedLogs);
            return Ok(ApiResponse<IEnumerable<GroupedAuditLogResponse>>.Success(groupedResponse, 200, "Grouped audit logs retrieved successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.Failure("An unexpected error occurred while retrieving audit logs.", 500));
        }
    }
}
