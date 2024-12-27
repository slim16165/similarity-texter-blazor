using SimilarityTextComparison.Domain.Interfaces.TextProcessing;

namespace SimilarityTextComparison.Application.Pipeline.PipelineSteps;

public class TextCleaningStep : IPipelineStep
{
    private readonly ITextProcessor _textProcessor;

    public TextCleaningStep(ITextProcessor textProcessor)
    {
        _textProcessor = textProcessor;
    }

    public async Task ExecuteAsync(MatchingContext context)
    {
        context.SourceText.Text = _textProcessor.CleanText(context.SourceText.Text);
        context.TargetText.Text = _textProcessor.CleanText(context.TargetText.Text);
    }
}