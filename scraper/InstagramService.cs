using System;
namespace scraper
{
	public class InstagramService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly HttpClient client;
		private const string accessToken = "EAAJ95XCmN78BOZBlncp4K6KXGd5bYH4kw1pG2ZAy8ZAszrZCHDsNpRk5XF2fOfam845nMlY0BZCGT7ufooTpKGn9NumeF5LaEWcLGdYqaru68HkHonjkgWIG7idNvyhQECc4ZCr1ZB2AKa8ZAveNKUBaJaPZAkAkkZAMDpomrcl7vLoOF3OLjvMIynAW1aBYGwF2DJROrjXjwd";
		private const string igId = "17841453217849679";
		public InstagramService(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
			client = _httpClientFactory.CreateClient();
        }

        public async Task<string> CreateContainer(string caption, string imageUrl)
		{
			var res = await client.PostAsync($"https://graph.facebook.com/v19.0/{igId}/media?image_url={imageUrl}&is_carousel_item={false}&caption={"automation test"}&access_token={accessToken}", null);
			var str = await res.Content.ReadAsStringAsync();
			return str;
		}

		public async Task<string> CheckContainerStatus(string containerId)
		{
			var res = await client.GetAsync($"https://graph.facebook.com/v19.0/{containerId}?fields=status&access_token={accessToken}");
			var str = await res.Content.ReadAsStringAsync();
			return str;
        }

		public async Task<string> PublishContainer(string containerId)
		{
			var res = await client.PostAsync($"https://graph.facebook.com/v19.0/{igId}/media_publish?creation_id={containerId}&access_token={accessToken}", null);
			var str = await res.Content.ReadAsStringAsync();
			return str;
		}
	}
}