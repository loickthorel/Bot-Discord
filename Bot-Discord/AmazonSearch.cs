using System.Text.RegularExpressions;
using Discord.WebSocket;
using Scraping.Web;

namespace Bot_Discord;

public static class AmazonSearch
{
    public static Task UrlWithParameter(string parameters, SocketMessage message)
    {
        var words = parameters.Replace(' ', '+');
        var url = "https://www.amazon.fr/s?k=" + words;

        var ret = new HttpRequestFluent(false)
            .FromUrl(url)
            .Load();
        
        var byClassContain =
            ret.HtmlPage.GetByClassNameContains("sg-col-4-of-12 " +
                                                "s-result-item s-asin sg-col-4-of-16 " +
                                                "sg-col s-widget-spacing-small sg-col-4-of-20");
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

            var listAmzObj = new List<AmazonObj>(){
                obj
            };
            
            return Task.FromResult(message.Channel.SendMessageAsync(
                $"Voici ce que j'ai trouvé : {obj.Name}: - Prix : {obj.Price} - Photo : {obj.UrlPicture} - Rating : {obj.Stars}"));
        }
        return Task.FromResult(message.Channel.SendMessageAsync("c raté"));
    }
}