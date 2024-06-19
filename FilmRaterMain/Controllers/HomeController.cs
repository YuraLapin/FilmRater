using FilmRaterMain.Controllers.UtilityClasses;
using FilmRaterMain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FilmRaterMain.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        DatabaseRequestService databaseRequestService;
        PasswordHashService passwordHashService;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            databaseRequestService = new DatabaseRequestService();
            passwordHashService = new PasswordHashService();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LogIn(string cameFrom, int filmId)
        {
            return View(new LoginModel() { ErrorType = "NoError", CameFrom = cameFrom, FilmId = filmId });
        }

        [HttpPost("Home/Library")]
        public IActionResult Library(string? userName)
        {
            if (userName == null)
            {
                return View(new LibraryModel() { UserName = "" });
            }

            return View(new LibraryModel() { UserName = userName });
        }

        [HttpPost("Home/MoreInfo")]
        public IActionResult MoreInfo(string? userName, int filmId)
        {
            if (userName == null)
            {
                return View(new MoreInfoModel() { UserName = "", FilmId = filmId });
            }

            return View(new MoreInfoModel() { UserName = userName, FilmId = filmId });
        }

        [HttpPost("Home/TryLogIn")]
        public async Task<IActionResult> TryLogIn(string userName, string password, string cameFrom, int filmId)
        {
            if (await databaseRequestService.UserNameIsFree(userName))
            {
                return View("LogIn", new LoginModel() { ErrorType = "WrongCridentialsError", CameFrom = cameFrom, FilmId = filmId });
            }

            string storedHash = await databaseRequestService.GetStoredHash(userName);

            bool success = passwordHashService.CheckHash(storedHash, password);

            if (success)
            {

                if (cameFrom == "Library")
                {
                    return View("Library", new LibraryModel() { UserName = userName });
                }

                if (cameFrom == "MoreInfo" && filmId != 0)
                {
                    return View("MoreInfo", new MoreInfoModel() { UserName = userName, FilmId = filmId });
                }

            }

            return View("LogIn", new LoginModel() { ErrorType = "WrongCridentialsError", CameFrom = cameFrom, FilmId = filmId });
        }

        [HttpPost("Home/GoBack")]
        public async Task<IActionResult> GoBack(string cameFrom, int filmId)
        {
            if (cameFrom == "MoreInfo" && filmId != 0)
            {
                return View("MoreInfo", new MoreInfoModel() { UserName = "", FilmId = filmId });
            }

            return View("Library", new LibraryModel() { UserName = "" });
        }

        [HttpPost("Home/TryRegister")]
        public async Task<IActionResult> TryRegister(string userName, string password, string cameFrom, int filmId)
        {
            string hashedPassword = passwordHashService.HashPassword(password);

            bool success = await databaseRequestService.TryRegister(userName, hashedPassword);

            if (success)
            {
                if (cameFrom == "Library")
                {
                    return View("Library", new LibraryModel() { UserName = userName });
                }

                if (cameFrom == "MoreInfo" && filmId != 0)
                {
                    return View("MoreInfo", new MoreInfoModel() { UserName = userName, FilmId = filmId });
                }
            }

            return View("LogIn", new LoginModel() { ErrorType = "LoginTakenError", CameFrom = cameFrom, FilmId = filmId });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
