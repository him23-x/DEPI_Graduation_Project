using DiscoverEgypt.Application.Experiences;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.WebApi.Controllers;

// The Controller is THIN: receive the request, call ONE Application method, return the result.
[ApiController]
[Route("api/experiences")]
public class ExperiencesController : ControllerBase
{
    private readonly ExperienceService _service;

    public ExperiencesController(ExperienceService service)
    {
        _service = service;
    }

    // GET /api/experiences
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ExperienceDto>>> GetPublished()
    {
        return Ok(await _service.GetPublishedAsync());
    }
}
