namespace ChatGPT_Splitter_Blazor_New.TextComparer.Services;

using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

public class TextInputReader
{
    /// <summary>
    /// Legge l'input di testo HTML e restituisce il contenuto come stringa pulita.
    /// </summary>
    /// <param name="text">L'input HTML da cui estrarre il testo.</param>
    /// <returns>Una task che rappresenta il testo estratto e pulito.</returns>
    public async Task<string> ReadTextInput(string text)
    {
        return await Task.Run(() =>
        {
            var cleanedText = string.Empty;

            // Crea un nuovo elemento div per caricare l'HTML
            var div = new XElement("div", XElement.Parse(text));

            // Estrai il testo dai nodi HTML
            var textNode = _ExtractTextFromNode(div);

            // Se il testo non è vuoto o solo spazi bianchi
            if (!string.IsNullOrEmpty(textNode) && Regex.IsMatch(textNode, @"\S"))
            {
                cleanedText = textNode;

                // Rimuovi spazi multipli
                cleanedText = Regex.Replace(cleanedText, @"\n[\t\v ]*", "\n");
                // Rimuovi le nuove righe multiple
                cleanedText = Regex.Replace(cleanedText, @"\n{3,}", "\n\n");

                return cleanedText;
            }

            throw new Exception("HTML input has no valid text contents.");
        });
    }

    /// <summary>
    /// Esplora ricorsivamente i nodi figli e restituisce il contenuto di testo dell'HTML come stringa.
    /// </summary>
    /// <param name="node">L'elemento HTML da cui estrarre il testo.</param>
    /// <returns>Il contenuto di testo estratto.</returns>
    private string _ExtractTextFromNode(XElement node)
    {
        var str = string.Empty;

        // Regex per controllare le lettere (equivalente di XRegExp in JS)
        var letterRegex = new Regex(@"^\p{L}+$");

        // Funzione per verificare se un nodo deve essere saltato
        bool IsValidNode(string nodeName)
        {
            var skipNodes = new[] { "IFRAME", "NOSCRIPT", "SCRIPT", "STYLE" };
            return !Array.Exists(skipNodes, skipNode => skipNode == nodeName);
        }

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
                    var extractedContent = _ExtractTextFromNode(childElement);

                    // Aggiunge uno spazio tra nodi di testo non separati da spazi o newline
                    if (letterRegex.IsMatch(str.LastOrDefault().ToString()) && letterRegex.IsMatch(extractedContent.FirstOrDefault().ToString()))
                    {
                        str += " ";
                    }

                    str += extractedContent;
                }
            }
        }

        return str;
    }
}
