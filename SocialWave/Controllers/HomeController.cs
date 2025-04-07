using Microsoft.AspNetCore.Mvc;
using SocialWave.Models.Services;
using SocialWave.Models.ViewModels;
using System.Diagnostics;
using SocialWave.Exceptions;
using SocialWave.Models.ConcreteClasses;
using System.Threading.Tasks;
using System.Collections.Generic;
using SocialWave.Helpers;
using System.Globalization;
using SocialWave.Models.AbstractClasses;

namespace SocialWave.Controllers
{
    /// <summary>
    /// Controller responsible for managing home-related functionalities.
    /// </summary>
    [ServiceFilter(typeof(NotificationHelperActionFilter))]
    public class HomeController : Controller
    {
        private readonly UserService _userService;
        private readonly PostService _postService;
        private readonly AmountOfPostsHelper _amountOfPostsHelper;
        private readonly SearchService _searchService;
        private readonly NotificationService _notificationService;
        private readonly NotificationHelper _notificationHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="userService">The service for handling user-related operations.</param>
        /// <param name="postService">The service for handling post-related operations.</param>
        /// <param name="amountOfPostsHelper">The helper for managing the amount of posts to display.</param>
        /// <param name="searchService">The service for handling search-related operations.</param>
        /// <param name="notificationService">The service for handling notification-related operations.</param>
        /// <param name="notificationHelper">The helper for managing notification-related functionalities.</param>
        public HomeController(UserService userService,
               PostService postService,
               AmountOfPostsHelper amountOfPostsHelper,
               SearchService searchService,
               NotificationService notificationService,
               NotificationHelper notificationHelper)
        {
            _userService = userService;
            _postService = postService;
            _amountOfPostsHelper = amountOfPostsHelper;
            _searchService = searchService;
            _notificationService = notificationService;
            _notificationHelper = notificationHelper;
        }
        /// <summary>
        /// Displays the home page with a list of posts authored by the logged-in user.
        /// </summary>
        /// <returns>The home page view.</returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                int amountOfPosts = _amountOfPostsHelper.ReturnAmountOfPosts();
                User user = await _userService.FindUserByNameAsync(User.Identity.Name);
                IEnumerable<Post> posts = await _postService.FindPostsByGenerateTrendingAsync(user, amountOfPosts);
                ViewData["CurrentUser"] = user;
                ViewData["AmountOfPostsHelper"] = _amountOfPostsHelper;
                return View(posts);
            }
            catch (PostException)
            {
                return View();
            }
            catch (UserException)
            {
                return View();
            }
        }

        /// <summary>
        /// Displays the home page with posts admired by the logged-in user.
        /// </summary>
        /// <returns>The home page view with admired posts.</returns>
        [HttpGet]
        public async Task<IActionResult> IndexWithPostsAdmired()
        {
            int amountPosts = _amountOfPostsHelper.ReturnAmountOfPosts();
            var user = await _userService.FindUserByNameAsync(User.Identity.Name);
            var posts = await _searchService.SearchPostByAdmiredAsync(user);
            ViewData["PostAdmired"] = "����� �� ����� �����";
            ViewData["CurrentUser"] = user;
            return View("Index", posts);
        }

        /// <summary>
        /// Sets the amount of posts to display on the home page.
        /// </summary>
        /// <param name="amount">The amount of posts to set.</param>
        /// <returns>Redirects to the home page.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SetAmountOfPosts(int amount)
        {
            if (Request.Method != "POST")
            {
                return NotFound();
            }
            _amountOfPostsHelper.SetAmountOfPosts(amount);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Displays the About page.
        /// </summary>
        /// <returns>The About page view.</returns>
        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Displays the error page with the error details.
        /// </summary>
        /// <returns>The error page view.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(string message)
        {
            return View(new ErrorViewModel { Message = message, RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}