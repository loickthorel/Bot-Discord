using System.Text.RegularExpressions;
using Scraping.Web;
using TestCode;

namespace Bot_Discord;

public class AmazonSearch
{
    void UrlWithParameter(string parameters, bool withPrime, bool onlyPromo)
    {
        string words = parameters.Replace(' ', '+');
        string url = "https://www.amazon.fr/s?k=" + words;

        var ret = new HttpRequestFluent(true)
            .FromUrl(url)
            .Load();

        Console.WriteLine(url);

        var byClassContain =
            ret.HtmlPage.GetByClassNameContains("sg-col-4-of-12 " +
                                                "s-result-item s-asin sg-col-4-of-16 " +
                                                "sg-col s-widget-spacing-small sg-col-4-of-20");

        var list = new List<AmazonObj>();
        foreach (var result in byClassContain)
        {
            if (!result.InnerHtml.Contains(
                    "<i class=\"a-icon a-icon-prime a-icon-medium\" role=\"img\" aria-label=\"Amazon Prime\"></i>"))
                continue;

            var urlPicture = String.Join("", Regex.Matches(result.InnerHtml, @"src=(.+?) srcset")
                    .Select(m => m.Groups[1].Value))
                .Replace('"', ' ')
                .Trim();

            var urlRedirection = String.Join("", Regex.Matches(result.InnerHtml, @"/gp/(.+?)>")
                    .Select(m => m.Groups[1].Value))
                .Replace('"', ' ')
                .Trim();

            var title = String.Join("",
                    Regex.Matches(result.InnerHtml, $@"a-size-medium a-color-base a-text-normal(.+?)</span>")
                        .Select(m => m.Groups[1].Value))
                .Replace('"', ' ')
                .Trim();

            if (title == "")
            {
                title = String.Join("",
                        Regex.Matches(result.InnerHtml, $@"a-size-base a-color-base a-text-normal(.+?)</span>")
                            .Select(m => m.Groups[1].Value))
                    .Replace('"', ' ')
                    .Trim();
            }

            var price = String.Join("", Regex.Matches(result.InnerHtml, @"a-price-whole"">(.+?)</span>")
                    .Select(m => m.Groups[1].Value))
                .Replace('"', ' ')
                .Trim();

            var stars = String.Join("", Regex.Matches(result.InnerHtml, @"a-icon-alt(.+?)étoiles")
                    .Select(m => m.Groups[1].Value))
                .Replace('"', ' ')
                .Replace('>', ' ')
                .Trim();

            // result.InnerText.Replace("\n", " ").TrimStart()

            AmazonObj obj = new AmazonObj()
            {
                Name = title.Replace(">", ""),
                UrlPicture = urlPicture,
                UrlRedirection = urlRedirection,
                Price = price,
                Stars = stars
            };

            list.Add(obj);
        }
    }
}