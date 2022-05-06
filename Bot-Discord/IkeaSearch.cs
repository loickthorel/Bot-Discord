using System.Text.RegularExpressions;
using Discord;
using Scraping.Web;

namespace Bot_Discord;

internal static class IkeaSearch
{
    private static readonly string WebSiteUrl;
    private static readonly string UrlSearch;
    private static EmbedBuilder _builder = null!;
    private static Embed _embed = null!;
    private static Embed[] _embeds = new Embed[10];
    private static readonly EmbedAuthorBuilder Author;

    static IkeaSearch()
    {
        var imageUrl = "https://tinyurl.com/2s4ybzw3";
        WebSiteUrl = "https://www.ikea.com";
        // https://www.ikea.com/fr/fr/search/products/?q=bureau
        UrlSearch = "/fr/fr/search/products/?q=";
        // UrlSearch = "/fr/fr/search/?q=";
        Author = new EmbedAuthorBuilder
        {
            Name = "MegActuBot",
            IconUrl = imageUrl,
            Url = imageUrl,
        };
    }

    public static Embed[]? UrlWithParameter(string parameters)
    {
        var words = parameters.Replace(' ', '+');
        var search = WebSiteUrl + UrlSearch + words;
        Console.WriteLine(search);
        
        var ret = new HttpRequestFluent(false)
            .FromUrl(search)
            .Load();
        
        var byClassContain =
            ret.HtmlPage.GetByClassNameContains("serp-grid search-grid");

        var i = 0;
        foreach (var result in byClassContain)
        {
            var urlPicture2 = string.Join("", Regex.Matches(result.InnerHtml, @"src=(.+?) srcset")
                    .Select(m => m.Groups[1].Value))
                .Replace('"', ' ')
                .Trim();
            
            var urlPicture = String.Join("", Regex.Matches(result.InnerHtml, @"src=(.+?) srcset")
                    .Select(m => m.Groups[1].Value))
                .Replace('"', ' ')
                .Trim();
            
            var urlRedirection2 = String.Join("", Regex.Matches(result.InnerHtml, @"a-link-normal s-no-outline(.+?)>")
                .Select(m => m.Groups[1].Value))
                .Replace("\u0022", "")
                .Replace("href=", "")
                .Replace(">", "")
                .Replace($"&amp;", "&")
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
                Author = Author,
                Title = title.Replace(">", "").Trim(),
                ImageUrl = urlPicture,
                ThumbnailUrl = urlPicture,
                Url = WebSiteUrl + urlRedirection2,
            };

            _embed = _builder.Build();

            if (i == 10)
                break;

            if (_embeds != null)
                _embeds[i] = _embed;

            i += 1;
        }

        return _embeds;
    }
}