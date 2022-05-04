using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using Scraping.Web;
using static System.Console;

namespace Bot_Discord;

public static class AmazonSearch
{
    private static string ImageUrl = "https://tinyurl.com/2s4ybzw3";
    private static Embed _embed = null!;
    private static Embed[] _embeds = new Embed[10];
    private static EmbedBuilder _builder = null!;
    private static string WebSiteUrl = "https://www.amazon.fr"; 
    static EmbedAuthorBuilder _author = new()
    {
        Name = "MegActuBot", 
        IconUrl = ImageUrl, 
        Url = ImageUrl,  
    };
    
    public static Task UrlWithParameter(string parameters, SocketMessage message)
    {
        var words = parameters.Replace(' ', '+');
        var search = WebSiteUrl + "/s?k=" + words;

        var ret = new HttpRequestFluent(false)
            .FromUrl(search)
            .Load();

        var byClassContain =
            ret.HtmlPage.GetByClassNameContains("sg-col-4-of-12 " +
                                                "s-result-item s-asin sg-col-4-of-16 " +
                                                "sg-col s-widget-spacing-small sg-col-4-of-20");

        var i = 0;
        foreach (var result in byClassContain)
        {
            if (!result.InnerHtml.Contains(
                    "<i class=\"a-icon a-icon-prime a-icon-medium\" role=\"img\" aria-label=\"Amazon Prime\"></i>"))
                continue;
            
            var urlPicture = String.Join("", Regex.Matches(result.InnerHtml, @"src=(.+?) srcset")
                    .Select(m => m.Groups[1].Value))
                .Replace('"', ' ')
                .Trim();

            var urlRedirection = String.Join("", Regex.Matches(result.InnerHtml, @"href="+'\u0022'+"(.+?)<div class="+'\u0022'+"a-section aok-relative")
                    .Select(m => m.Groups[1].Value))
                .Replace('"', ' ')
                .Replace(">", "")
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
            
            if (title.Length == 0)
                title = parameters;
            
            _builder = new EmbedBuilder
            {
                Description = price + " € / Rating : " + stars,
                Color = Color.DarkRed,
                Author = _author,
                Title = title.Replace(">", "").Trim(), 
                ImageUrl = urlPicture,
                ThumbnailUrl = urlPicture,
                Url = WebSiteUrl+urlRedirection,
            };

            _embed = _builder.Build();

            if (i == 10)
            {
                break;
            }
            
            if (_embeds != null) 
                _embeds[i] = _embed;

            i += 1;
        }
        
        return Task.FromResult(message.Channel.SendMessageAsync(embeds: _embeds));
    }
}