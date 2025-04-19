namespace TravelBridgeAPI.Security
{
    public class ApiKeyValidation : IApiKeyValidation
    {
        private readonly IConfiguration _configuration;
        public ApiKeyValidation(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public bool IsValidApiKey(string userApiKey)
        {
            if(string.IsNullOrEmpty(userApiKey))
            {
                return false;
            }
            string? apiKey = _configuration.GetValue<string>(Constants.ApiKeyName);
            Console.WriteLine($"[INFO] apiKey: {apiKey}");
            Console.WriteLine($"[INFO] user-apiKeY: {userApiKey}");
            if (apiKey == null || apiKey != userApiKey)
                return false;
            return true;
        }
    }
}
