using Microsoft.AspNetCore.Mvc;

namespace TravelBridgeAPI.CustomAttributes
{
    public class ApiKeyAttribute : ServiceFilterAttribute
    {
        public ApiKeyAttribute()
            :base(typeof(ApiKeyAuthFilter))
        {

        }
    }
}
