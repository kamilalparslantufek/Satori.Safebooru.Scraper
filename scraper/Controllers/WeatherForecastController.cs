using Microsoft.AspNetCore.Mvc;

namespace scraper.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "SafebooruImage")]
    public async Task<String?> Get()
    {
        // sayfaya giriş yap
        var url = "https://safebooru.org/index.php?page=post&s=list";
        var web = new HtmlAgilityPack.HtmlWeb();
        var doc = await web.LoadFromWebAsync(url);
        //content div xpath
        var node = doc.DocumentNode.SelectSingleNode("//*[@id=\"post-list\"]/div[2]");
        //son resimin urlsi
        var lastId = node?.ChildNodes[1]?.ChildNodes[0]?.ChildNodes[0]?.Attributes?.Where(f => f.Name == "href")?.FirstOrDefault()?.Value.Split("=").LastOrDefault();
        var itemUrl = $"https://safebooru.org/index.php?page=post&s=view&id={lastId}";
        //son resimin bilgileri
        var itemPage = web.Load(itemUrl);
        var i = itemPage.DocumentNode.SelectSingleNode("//*[@id=\"image\"]");
        Console.WriteLine(i.Attributes.Where(d => d.Name == "src").FirstOrDefault());
        var src = i?.Attributes?.Where(d => d.Name == "src")?.FirstOrDefault()?.Value.Split("?").First();
        var tagList = new List<string>();
        var sideBar = itemPage.DocumentNode.SelectSingleNode("//*[@id=\"tag-sidebar\"]");
        foreach (var child in sideBar.ChildNodes)
        {
            if (child.OuterHtml.Contains("tag-type-copyright") || child.OuterHtml.Contains("tag-type-character") || child.OuterHtml.Contains("tag-type-artist"))
            {
                tagList.Add(child.ChildNodes.Where(d => d.Name == "a").FirstOrDefault().FirstChild.GetDirectInnerText());
            }
        }
        
        return Newtonsoft.Json.JsonConvert.SerializeObject(new {itemUrl, src, tagList });
    }
}

