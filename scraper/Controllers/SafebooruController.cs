using Microsoft.AspNetCore.Mvc;
using scraper.Entity;

namespace scraper.Controllers;

[ApiController]
[Route("[controller]")]
public class SafebooruController : ControllerBase
{
    InstagramService _service;
    private readonly Random random;


    public SafebooruController(IHttpClientFactory _factory, Random random)
    {
        _service = new InstagramService(_factory);
        this.random = random;
    }

    
    private async Task<String> maxId()
    {
        // sayfaya giriş yap
        var url = "https://safebooru.org/index.php?page=post&s=list";
        var web = new HtmlAgilityPack.HtmlWeb();
        var doc = await web.LoadFromWebAsync(url);
        //content div xpath
        var node = doc.DocumentNode.SelectSingleNode("//*[@id=\"post-list\"]/div[2]");
        //son resimin urlsi
        var lastId = node?.ChildNodes[1]?.ChildNodes[0]?.ChildNodes[0]?.Attributes?.Where(f => f.Name == "href")?.FirstOrDefault()?.Value.Split("=").LastOrDefault();
        return lastId;
    }

    private async Task<RandomImage> getRandomImage()
    {
        var lastId = await maxId();
        var integer = int.Parse(lastId);
        var id = random.Next(integer);
        var itemUrl = $"https://safebooru.org/index.php?page=post&s=view&id={id}";
        //resim bilgileri
        var web = new HtmlAgilityPack.HtmlWeb();
        var itemPage = web.Load(itemUrl);
        var i = itemPage.DocumentNode.SelectSingleNode("//*[@id=\"image\"]");

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

        return new RandomImage(){ ItemUrl= itemUrl, Src = src, TagList = tagList.ToArray() };
    }

    [HttpGet("Random")]
    public async Task<RandomImage?> GetMax()
    {
        
        return await getRandomImage();
    }
    [HttpPost("Container/Create")]
    public async Task<String?> CreateRandomContainer()
    {
        var image = await getRandomImage();
        var res = await _service.CreateContainer("caption, automation test", image.Src);
        return res;
    }

    [HttpGet("Container/Status/{containerId}")]
    public async Task<String?> GetContainerStatus(string containerId)
        => await _service.CheckContainerStatus(containerId);

    [HttpPost("Container/Publish")]
    public async Task<String?> PublishContainer(string containerId)
        => await _service.PublishContainer(containerId);
}