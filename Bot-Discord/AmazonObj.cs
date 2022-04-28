using Scraping.Web;

namespace Bot_Discord;

public struct AmazonObj
{
    public string Name { get; init; }

    public string UrlPicture { get; init; }

    public string Price { get; init; }

    public string Stars  { get; init; }
    
    public string UrlRedirection { get; set; }
}