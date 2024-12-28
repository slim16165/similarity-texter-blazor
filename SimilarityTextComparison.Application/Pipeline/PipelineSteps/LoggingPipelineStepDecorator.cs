namespace SimilarityTextComparison.Application.Pipeline.PipelineSteps;

public class LoggingPipelineStepDecorator : IPipelineStep
{
    private readonly IPipelineStep _inner;

    public LoggingPipelineStepDecorator(IPipelineStep inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public async Task ExecuteAsync(MatchingContext context)
    {
        Console.WriteLine($"[START] {_inner.GetType().Name}");
        try
        {
            await _inner.ExecuteAsync(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {_inner.GetType().Name}: {ex.Message}");
            throw;
        }
        Console.WriteLine($"[END] {_inner.GetType().Name}");
    }
}