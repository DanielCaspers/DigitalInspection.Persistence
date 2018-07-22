using System.Net.Http;
using System.Threading.Tasks;
using DigitalInspectionNetCore21.Models.DTOs;
using DigitalInspectionNetCore21.Models.Mappers;
using DigitalInspectionNetCore21.Models.Store;
using DigitalInspectionNetCore21.Models.Web;
using Newtonsoft.Json;

namespace DigitalInspectionNetCore21.Services.Web
{
	public class StoreInfoService: HttpClientService
	{
		public static async Task<HttpResponse<StoreInfo>> GetStoreInfo(string companyNumber)
		{
			using (HttpClient httpClient = InitializeAnonymousHttpClient())
			{
				var url = $"conos/{companyNumber}";
				HttpResponseMessage response = await httpClient.GetAsync(url);
				string json = await response.Content.ReadAsStringAsync();

				return CreateStoreInfoResponse(response, json);
			}
		}

		private static HttpResponse<StoreInfo> CreateStoreInfoResponse(HttpResponseMessage httpResponse, string responseContent)
		{
			HttpResponse<StoreInfo> storeInfoResponse = new HttpResponse<StoreInfo>(httpResponse, responseContent);

			if (httpResponse.IsSuccessStatusCode && responseContent != string.Empty)
			{
				StoreInfoDTO dto = JsonConvert.DeserializeObject<StoreInfoDTO>(responseContent);
				storeInfoResponse.Entity = StoreInfoMapper.mapToStoreInfo(dto);
			}

			return storeInfoResponse;
		}
	}
}
