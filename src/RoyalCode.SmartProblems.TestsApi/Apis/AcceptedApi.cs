using RoyalCode.SmartProblems.HttpResults;

namespace RoyalCode.SmartProblems.TestsApi.Apis;

/// <summary>
/// Endpoints exercising <see cref="AcceptedMatch"/> and <see cref="AcceptedMatch{T}"/>:
/// 202 with and without body, with and without <c>Location</c>, and problems.
/// </summary>
public static class AcceptedApi
{
    public static IEndpointRouteBuilder MapAcceptedApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/accepted").WithTags("Accepted API");

        group.MapPost("/queue", Queue);
        group.MapPost("/queue-located", QueueLocated);
        group.MapPost("/ticket", Ticket);
        group.MapPost("/ticket-located", TicketLocated);

        return group;
    }

    private static async Task<AcceptedMatch> Queue(QueueRequest request)
    {
        await Task.Delay(1); // simula enfileiramento assíncrono

        if (request.Fail)
            return Problems.InvalidParameter("Queue rejected.", nameof(request.Fail));

        return Result.Ok();
    }

    private static async Task<AcceptedMatch> QueueLocated(QueueRequest request)
    {
        await Task.Delay(1);

        Result result = request.Fail
            ? Problems.InvalidParameter("Queue rejected.", nameof(request.Fail))
            : Result.Ok();

        return result.AcceptedMatch("/api/accepted/status/fixed");
    }

    private static async Task<AcceptedMatch<TicketDetails>> Ticket(QueueRequest request)
    {
        await Task.Delay(1);

        if (request.Fail)
            return Problems.InvalidParameter("Queue rejected.", nameof(request.Fail));

        Result<TicketDetails> result = new TicketDetails { Ticket = "T-42", Status = "queued" };
        return result;
    }

    private static async Task<AcceptedMatch<TicketDetails>> TicketLocated(QueueRequest request)
    {
        await Task.Delay(1);

        Result<TicketDetails> result = request.Fail
            ? Problems.InvalidParameter("Queue rejected.", nameof(request.Fail))
            : new TicketDetails { Ticket = "T-42", Status = "queued" };

        return result.AcceptedMatch(t => $"/api/accepted/status/{t.Ticket}");
    }
}

public class QueueRequest
{
    public bool Fail { get; set; }
}

public class TicketDetails
{
    public string Ticket { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
