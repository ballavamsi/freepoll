using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace freepoll.Controllers
{
    [Route("api/location")]
    public class LocationController : Controller
    {

        private readonly IActionContextAccessor _accessor;
        public LocationController(IActionContextAccessor accessor){
            _accessor = accessor;
        }

        [Route("ip")]
        public string GetUserIPAddress(){
            return _accessor.ActionContext.HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}