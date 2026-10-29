using System.Globalization;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using DiscoverEgypt.Application.DTOs.Earnings;
using DiscoverEgypt.Application.Features.Earnings.Requests.Queries;
using DiscoverEgypt.Domain.Enums.Payments;
using DiscoverEgypt.Presentation.ViewModels.Earnings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers.Earnings;

[Authorize(Roles = "Host")]
public sealed class EarningsController(IMediator mediator, IMapper mapper) : Controller
{
    private const int PageSize = 10;

    public async Task<IActionResult> Index(PayoutStatus? status, int page = 1, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();
        if (status.HasValue && !Enum.IsDefined(status.Value)) status = null;
        page = Math.Clamp(page, 1, 1_000_000);

        var result = await mediator.Send(new GetHostEarningsQuery(userId, status, page, PageSize), cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(result.TotalCount / (double)PageSize));
        if (page > totalPages)
            return RedirectToAction(nameof(Index), new { status, page = totalPages });

        ViewData["Section"] = "Earnings";
        return View(new HostEarningsIndexViewModel
        {
            Payouts = mapper.Map<IReadOnlyList<HostPayoutItemViewModel>>(result.Payouts),
            PendingAmount = result.PendingAmount,
            PaidAmount = result.PaidAmount,
            CancelledAmount = result.CancelledAmount,
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize,
            SelectedStatus = status
        });
    }

    public async Task<IActionResult> Export(PayoutStatus? status, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();
        if (status.HasValue && !Enum.IsDefined(status.Value)) status = null;

        const int exportPageSize = 50;
        var firstPage = await mediator.Send(
            new GetHostEarningsQuery(userId, status, 1, exportPageSize), cancellationToken);
        var payouts = new List<HostPayoutListItemDto>(firstPage.Payouts);
        var totalPages = (int)Math.Ceiling(firstPage.TotalCount / (double)exportPageSize);
        for (var page = 2; page <= totalPages; page++)
        {
            var result = await mediator.Send(
                new GetHostEarningsQuery(userId, status, page, exportPageSize), cancellationToken);
            payouts.AddRange(result.Payouts);
        }

        var csv = new StringBuilder();
        csv.AppendLine("Booking ID,Experience,Experience date,Amount (EGP),Eligible at,Status,Paid at");
        foreach (var payout in payouts)
        {
            csv.Append(Csv(payout.BookingId.ToString(CultureInfo.InvariantCulture))).Append(',')
                .Append(Csv(payout.ExperienceTitle)).Append(',')
                .Append(Csv(FormatDate(payout.ScheduledStart))).Append(',')
                .Append(payout.Amount.ToString("0.00", CultureInfo.InvariantCulture)).Append(',')
                .Append(Csv(FormatDate(payout.EligibleAt))).Append(',')
                .Append(Csv(payout.Status.ToString())).Append(',')
                .AppendLine(Csv(payout.PaidAt.HasValue ? FormatDate(payout.PaidAt.Value) : string.Empty));
        }

        return File(Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray(),
            "text/csv; charset=utf-8", $"host-payouts-{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    private static string FormatDate(DateTime value) =>
        DateTime.SpecifyKind(value, DateTimeKind.Utc).ToString("O", CultureInfo.InvariantCulture);

    private static string Csv(string value)
    {
        if (value.Length > 0 && value[0] is '=' or '+' or '-' or '@' or '\t' or '\r')
            value = $"'{value}";
        return $"\"{value.Replace("\"", "\"\"")}\"";
    }
}
