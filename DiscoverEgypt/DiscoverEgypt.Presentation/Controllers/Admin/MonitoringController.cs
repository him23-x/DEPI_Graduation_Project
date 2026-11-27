using DiscoverEgypt.Application.Features.AdminMonitoring.Requests.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Monitoring")]
    public sealed class MonitoringController : Controller
    {
        private readonly IMediator _mediator;

        public MonitoringController(IMediator mediator) => _mediator = mediator;

        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var summary = await _mediator.Send(new GetAdminMonitoringDashboardQuery(), cancellationToken);
            ViewData["Title"] = "Transaction monitoring";
            return View("~/Views/Admin/Monitoring/Index.cshtml", summary);
        }
    }
}
