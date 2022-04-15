using System.Collections;
using System.Xml.XPath;
using HtmlAgilityPack;

namespace FinancialStorage.BackgroundWorkers.Application.WebScrapers.Extensions;

public static class HtmlAgilityExtensions
{
    public static string? EvaluateFirstText(this XPathNavigator navigator, string xpath)
    {
        var expression = navigator.Compile(xpath);

        var enumerator = (navigator.Evaluate(expression) as IEnumerable)?.GetEnumerator();
        if (!enumerator?.MoveNext() ?? false)
        {
            return null;
        }

        var currentNode = enumerator?.Current as HtmlNodeNavigator;

        return currentNode?.Value;
    }

    public static IEnumerable<HtmlNodeNavigator> EvaluateAllOccurrences(this XPathNavigator navigator, string xpath)
    {
        var expression = navigator.Compile(xpath);

        var enumeration = navigator.Evaluate(expression) as IEnumerable;

        foreach (var elem in enumeration!)
        {
            yield return (HtmlNodeNavigator)elem;
        }
    }
}