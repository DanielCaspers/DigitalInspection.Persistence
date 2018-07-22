namespace DigitalInspectionNetCore21.Models.Web
{
	public class LoginResponse
	{
		public string authToken { get; set; }

		public string refreshToken { get; set; }
	}
}