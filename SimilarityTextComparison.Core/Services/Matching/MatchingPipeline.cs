﻿using SimilarityTextComparison.Core.Models.Comparison;

namespace SimilarityTextComparison.Core.Services.Matching
{
    public class MatchingPipeline
    {
        private readonly IEnumerable<IMatchStep> _steps;

        public MatchingPipeline(IEnumerable<IMatchStep> steps)
        {
            _steps = steps;
        }

        public async Task<List<List<MatchSegment>>> ExecuteAsync(MatchingContext context)
        {
            foreach (var step in _steps)
            {
                await step.ExecuteAsync(context);
            }

            return context.MatchingSegments;
        }
    }
}