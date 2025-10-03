using Microsoft.Extensions.Logging;
using KafkaDotnetSample.Domain.Repositories;

namespace KafkaDotnetSample.Domain.UseCases;

public class GetProcessedEventsUseCase
{
    private readonly IProcessedEventsRepository _repository;
    private readonly ILogger<GetProcessedEventsUseCase> _logger;
    public GetProcessedEventsUseCase(IProcessedEventsRepository repository, ILogger<GetProcessedEventsUseCase> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<(string Topic, string Data)>> ExecuteAsync()
    {
        var events = new List<(string Topic, string Data)>();
        try
        {
            await foreach (var evt in _repository.Events.ReadAllAsync())
            {
                events.Add(evt);
            }
            _logger.LogInformation("Fetched {Count} processed events", events.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching processed events");
        }
        return events;
    }
}
