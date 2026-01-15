using AISupportTicketSystem.Application.Features.Categories.Queries.GetAllCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AISupportTicketSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all active categories
    /// </summary>
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        if (includeInactive && !User.IsInRole("Admin")) includeInactive = false;

        var categories = await _mediator.Send(new GetAllCategoriesQuery(includeInactive));
        return  Ok(categories);
    }
}