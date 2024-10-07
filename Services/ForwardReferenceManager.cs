using ChatGPT_Splitter_Blazor_New.Pages.TextComparer.Model.TextProcessing;
using ChatGPT_Splitter_Blazor_New.TextComparer.Model.TextProcessing;

namespace ChatGPT_Splitter_Blazor_New.TextComparer.Services;

public class ForwardReferenceManager
{
    private readonly Configuration _config;

    public ForwardReferenceManager(Configuration config)
    {
        _config = config;
    }

    public Dictionary<int, int> CreateForwardReferences(MyText text, List<Token> tokens)
    {
        var mtsTags = new Dictionary<string, int>();
        var forwardReferences = new Dictionary<int, int>();

        for (int i = text.TokenkBeginPos; (i + _config.MinMatchLength - 1) < text.TokenEndPos; i++)
        {
            var tag = GenerateTag(i, tokens);

            if (mtsTags.TryGetValue(tag, out int previousPos))
            {
                forwardReferences[previousPos] = i;
            }

            mtsTags[tag] = i;
        }

        return forwardReferences;
    }

    private string GenerateTag(int position, List<Token> tokens)
    {
        var selectedTokens = tokens.Skip(position).Take(_config.MinMatchLength);
        return string.Concat(selectedTokens.Select(token => token.Text));
    }

}