using System.Text.RegularExpressions;
using System.Xml.Linq;
using SimilarityTextComparison.Domain.Interfaces.TextProcessing;
using SimilarityTextComparison.Infrastructure.Services;

namespace SimilarityTextComparison.Domain.Services.TextPreProcessing;

/// <summary>
/// Converte input HTML o testo in una stringa pulita per il processamento.
/// </summary>
public class TextInputReader : ITextInputReader
{
    private readonly TextComparisonConfiguration _config;

    public TextInputReader(TextComparisonConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Legge l'input di testo HTML o normale e restituisce il contenuto come stringa pulita.
    /// </summary>
    public async Task<string> ReadTextInputAsync(string input)
    {
        if (_config.IsHtmlInput)
        {
            return await Task.Run(() => CleanHtmlInput(input));
        }
        else
        {
            return NormalizeWhitespace(input);
        }
    }

    public string CleanHtmlInput(string htmlInput)
    {
        try
        {
            var div = new XElement("div", XElement.Parse(htmlInput));
            var extractedText = ExtractTextRecursively(div);

            if (string.IsNullOrWhiteSpace(extractedText))
            {
                // Tratta come testo normale se non c'è testo valido
                return NormalizeWhitespace(htmlInput);
            }

            return NormalizeWhitespace(extractedText);
        }
        catch (Exception)
        {
            // Tratta come testo normale in caso di parsing fallito
            return NormalizeWhitespace(htmlInput);
        }
    }

    private static string NormalizeWhitespace(string text)
    {
        text = Regex.Replace(text, @"\s+", " ").Trim();
        return text;
    }

    private static string ExtractTextRecursively(XElement node)
    {
        var str = string.Empty;

        // Regex per controllare le lettere
        var letterRegex = new Regex(@"^\p{L}+$");

        if (IsValidNode(node.Name.LocalName) && node.HasElements)
        {
            foreach (var child in node.Nodes())
            {
                if (child is XText textNode)
                {
                    str += textNode.Value;
                }
                else if (child is XElement childElement)
                {
                    var extractedContent = ExtractTextRecursively(childElement);

                    // Aggiunge uno spazio tra nodi di testo non separati da spazi
                    if (!string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(extractedContent) &&
                        char.IsLetter(str[^1]) && char.IsLetter(extractedContent[0]))
                    {
                        str += " ";
                    }

                    str += extractedContent;
                }
            }
        }

        return str;

        bool IsValidNode(string nodeName)
        {
            var skipNodes = new[] { "IFRAME", "NOSCRIPT", "SCRIPT", "STYLE" };
            return !skipNodes.Contains(nodeName, StringComparer.OrdinalIgnoreCase);
        }
    }
}