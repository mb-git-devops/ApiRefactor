using ApiRefactor.Contracts.Requests;
using ApiRefactor.Contracts.Responses;
using ApiRefactor.Services;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ApiRefactor.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/waves")]
public sealed class WavesController(IWaveService waveService) : ControllerBase
{
    /// <summary>List all waves.</summary>
    [HttpGet]
    [SwaggerOperation(
        Summary = "List waves",
        Description = "Returns every wave in the system, ordered by wave date, for store picking operations.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Successful response containing zero or more waves.", typeof(WavesListResponse))]
    [Produces("application/json")]
    public async Task<ActionResult<WavesListResponse>> GetAsync(CancellationToken cancellationToken)
    {
        var result = await waveService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>Get a single wave by identifier.</summary>
    [HttpGet("{id:guid}", Name = "GetWaveById")]
    [SwaggerOperation(
        Summary = "Get wave by id",
        Description = "Returns the wave with the specified unique identifier.")]
    [SwaggerResponse(StatusCodes.Status200OK, "The wave was found.", typeof(WaveResponse))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "No wave exists for the given id.")]
    [Produces("application/json")]
    public async Task<ActionResult<WaveResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var wave = await waveService.GetByIdAsync(id, cancellationToken);
        if (wave is null)
        {
            return NotFound();
        }

        return Ok(new WaveResponse(wave.Id, wave.Name, wave.WaveDate));
    }

    /// <summary>Create or update a wave.</summary>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Upsert wave",
        Description =
            "Creates a wave when the id is omitted or unknown, or updates an existing wave. After a successful save, a WaveUpserted integration event is published (MassTransit).")]
    [SwaggerResponse(StatusCodes.Status201Created, "A new wave was inserted.", typeof(WaveResponse))]
    [SwaggerResponse(StatusCodes.Status200OK, "An existing wave was updated.", typeof(WaveResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Validation failed for the request body.", typeof(ProblemDetails))]
    [Produces("application/json")]
    public async Task<ActionResult<WaveResponse>> UpsertAsync(
        [FromBody] UpsertWaveRequest request,
        CancellationToken cancellationToken)
    {
        var (wave, wasInserted) = await waveService.UpsertAsync(request, cancellationToken);
        var response = new WaveResponse(wave.Id, wave.Name, wave.WaveDate);
        var version = HttpContext.GetRouteData().Values.TryGetValue("version", out var v) ? v?.ToString() : null;
        version ??= "1.0";

        if (wasInserted)
        {
            return CreatedAtRoute("GetWaveById", new { version, id = wave.Id }, response);
        }

        return Ok(response);
    }
}
