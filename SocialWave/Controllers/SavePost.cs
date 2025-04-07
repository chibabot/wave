using Microsoft.AspNetCore.Mvc;

namespace SocialWave.Controllers
{

    [ServiceFilter(typeof(NotificationHelperActionFilter))]
    public class SavePost : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
