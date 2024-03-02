using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Paymob.Services
{
	public class PaymentService
	{
		private readonly IHttpClientFactory httpClientFactory;

		public PaymentService(IHttpClientFactory httpClientFactory)
        {
			this.httpClientFactory = httpClientFactory;
		}
		public async Task<T> Pay<T>(object payload, string url)
		{

			var client = httpClientFactory.CreateClient();
			var request = new HttpRequestMessage(HttpMethod.Post, url);
			var content = new StringContent(JsonConvert.SerializeObject(payload), null, "application/json");
			request.Content = content;
			var response = await client.SendAsync(request);
			response.EnsureSuccessStatusCode();
			var result = JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
			return result;
		}
    }
}
