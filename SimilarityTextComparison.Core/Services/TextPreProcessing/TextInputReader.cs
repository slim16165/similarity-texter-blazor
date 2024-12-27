using System.Text.RegularExpressions;
using System.Xml.Linq;
using SimilarityTextComparison.Domain.Interfaces.TextProcessing;

namespace SimilarityTextComparison.Domain.Services.TextPreProcessing;

/// <summary>
/// Converte input HTML o testo in una stringa pulita per il processamento.
/// </summary>
public class TextInputReader : ITextInputReader
{
    /// <summary>
    /// Legge l'input di testo HTML e restituisce il contenuto come stringa pulita.
    /// </summary>
    public async Task<string> ReadTextInputAsync(string htmlInput)
    {
        var cleanedText = await Task.Run(() => CleanHtmlInput(htmlInput));
        return cleanedText;
    }

    public string CleanHtmlInput(string htmlInput)
    {
        var div = new XElement("div", XElement.Parse(htmlInput));
        var extractedText = ExtractTextRecursively(div);

        if (string.IsNullOrWhiteSpace(extractedText))
        {
            throw new Exception("HTML input has no valid text contents.");
        }

        extractedText = NormalizeWhitespace(extractedText);
        return extractedText;
    }

    private static string NormalizeWhitespace(string text)
    {
        text = Regex.Replace(text, @"\n[\t\v ]*", "\n");
        text = Regex.Replace(text, @"\n{3,}", "\n\n");
        return text;
    }

    /// <summary>
    /// Esplora ricorsivamente i nodi figli e restituisce il contenuto di testo dell'HTML come stringa.
    /// </summary>
    private static string ExtractTextRecursively(XElement node)
    {
        var str = string.Empty;

        // Regex per controllare le lettere (equivalente di XRegExp in JS)
        var letterRegex = new Regex(@"^\p{L}+$");

        if (IsValidNode(node.Name.LocalName) && node.HasElements)
        {
            foreach (var child in node.Nodes())
            {
                // Se è un nodo di testo
                if (child is XText textNode)
                {
                    str += textNode.Value;
                }
                else if (child is XElement childElement)
                {
                    var extractedContent = ExtractTextRecursively(childElement);

                    // Aggiunge uno spazio tra nodi di testo non separati da spazi o newline
                    if (str.Length > 0 && extractedContent.Length > 0 &&
                        letterRegex.IsMatch(str[^1].ToString()) &&
                        letterRegex.IsMatch(extractedContent[0].ToString()))
                    {
                        str += " ";
                    }

                    str += extractedContent;
                }
            }
        }

        return str;

        // Funzione per verificare se un nodo deve essere saltato
        bool IsValidNode(string nodeName)
        {
            var skipNodes = new[] { "IFRAME", "NOSCRIPT", "SCRIPT", "STYLE" };
            return !skipNodes.Contains(nodeName, StringComparer.OrdinalIgnoreCase);
        }
    }
}