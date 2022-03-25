using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Business.Interfaces.Tools;

namespace Business.Services.Tools;

public class ClearHtmlServiceService : IClearHtmlService
{
    private static readonly Regex Tags = new(@"<[^>]+?>", RegexOptions.Multiline | RegexOptions.Compiled);
    private static readonly Regex NotOkCharacter = new(@"[^\w;&#@.:/\\?=|%!() -]", RegexOptions.Compiled);

    public string UnHtml(string html)
    {
        html = HttpUtility.UrlDecode(html);
        html = HttpUtility.HtmlDecode(html);

        html = RemoveTag(html, "<!--", "-->");
        html = RemoveTag(html, "<script", "</script>");
        html = RemoveTag(html, "<style", "</style>");

        //replace matches of these regexes with space
        html = Tags.Replace(html, " ");
        html = NotOkCharacter.Replace(html, " ");
        html = SingleSpacedTrim(html);

        return html;
    }

    private string RemoveTag(string html, string startTag, string endTag)
    {
        bool bAgain;
        do
        {
            bAgain = false;
            var startTagPos = html.IndexOf(startTag, 0, StringComparison.CurrentCultureIgnoreCase);
            if (startTagPos < 0)
                continue;
            var endTagPos = html.IndexOf(endTag, startTagPos + 1, StringComparison.CurrentCultureIgnoreCase);
            if (endTagPos <= startTagPos)
                continue;
            html = html.Remove(startTagPos, endTagPos - startTagPos + endTag.Length);
            bAgain = true;
        } while (bAgain);

        return html;
    }

    private string SingleSpacedTrim(string inString)
    {
        var sb = new StringBuilder();
        var inBlanks = false;
        foreach (var c in inString)
        {
            switch (c)
            {
                case '\r':
                case '\n':
                case '\t':
                case ' ':
                    if (!inBlanks)
                    {
                        inBlanks = true;
                        sb.Append(' ');
                    }

                    continue;
                default:
                    inBlanks = false;
                    sb.Append(c);
                    break;
            }
        }

        return sb.ToString().Trim();
    }
}