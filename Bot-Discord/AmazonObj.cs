using Discord;
using Scraping.Web;

namespace Bot_Discord;

public struct AmazonObj
{
    public string Name { get; set; }

    public string UrlPicture { get; set; }

    public string Price { get; set; }

    public string Stars  { get; set; }
    
    public string UrlRedirection { get; set; }
}