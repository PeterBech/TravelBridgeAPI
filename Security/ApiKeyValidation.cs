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
            Console.WriteLine("[INFO] Testing apiKey vs user-apiKey");
            if(string.IsNullOrEmpty(userApiKey))
            {
                Console.WriteLine("[ERROR] user-apiKey is null or empty");
                return false;
            }
            string? apiKey = _configuration.GetValue<string>(Constants.ApiKeyName);
            Console.WriteLine($"[INFO] apiKey: {apiKey}");
            Console.WriteLine($"[INFO] user-apiKeY: {userApiKey}");
            if (apiKey == null || apiKey != userApiKey)
            {
                Console.WriteLine("[ERROR] user-apiKey != apiKey");
                return false;
            }
            Console.WriteLine("[SUCCESS] user-apiKey == apiKey");    
            return true;
        }
    }
}
