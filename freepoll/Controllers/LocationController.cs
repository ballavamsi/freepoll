using freepoll.Helpers;
using freepoll.UserModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace freepoll.Controllers
{
    [Route("api/location")]
    public class LocationController : Controller
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        public LocationController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        [Route("ip")]
        [HttpGet]
        public IPLocation GetUserIPAddress()
        {
            return LocationHelper.GetIpAndLocation(_httpContextAccessor);
        }

    }
}