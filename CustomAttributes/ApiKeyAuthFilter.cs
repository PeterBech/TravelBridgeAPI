using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TravelBridgeAPI.Security;

namespace TravelBridgeAPI.CustomAttributes
{
    public class ApiKeyAuthFilter : IAuthorizationFilter
    {
        private readonly IApiKeyValidation _apiKeyValidation;
        public ApiKeyAuthFilter(IApiKeyValidation apiKeyValidation)
        {
            _apiKeyValidation = apiKeyValidation;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue(Constants.ApiKeyHeaderName, out var extractedApiKey))
            {
                context.Result = new BadRequestObjectResult("API key header mangler.");
                return;
            }

            if (!_apiKeyValidation.IsValidApiKey(extractedApiKey))
            {
                context.Result = new UnauthorizedObjectResult("Ugyldig API key.");
                return;
            }
        }

    }
}
