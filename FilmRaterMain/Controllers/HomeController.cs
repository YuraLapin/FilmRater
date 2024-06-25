using FilmRaterMain.Controllers.UtilityClasses;
using FilmRaterMain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

        public IActionResult LogIn()
        {
            return View(new LoginModel() { ErrorType = "NoError" });
        }

        [HttpPost("Home/TryLogin")]
        public async Task<IResult> TryLogIn([FromBody] UserNameAndPassword userNameAndPassword)
        {
            if (await databaseRequestService.UserNameIsFree(userNameAndPassword.UserName))
            {
                return Results.Ok(false);
            }

            string storedHash = await databaseRequestService.GetStoredHash(userNameAndPassword.UserName);

            bool success = passwordHashService.CheckHash(storedHash, userNameAndPassword.Password);

            if (success)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, userNameAndPassword.UserName) };

                var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new
                {
                    access_token = encodedJwt,
                    username = userNameAndPassword.UserName,
                };

                return Results.Json(response);
            }

            return Results.Ok(false);
        }

        [HttpGet("Home/CheckToken")]
        public IResult CheckToken()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Results.Ok(true);
            }

            return Results.Ok(false);
        }

        public IActionResult Library(int id)
        {
            if (id == 0)
            {
                return View();
            }

            return View("MoreInfo", new MoreInfoModel() { FilmId = id });
        }

        [HttpPost("Home/TryRegister")]
        public async Task<IResult> TryRegister([FromBody] UserNameAndPassword userNameAndPassword)
        {
            string hashedPassword = passwordHashService.HashPassword(userNameAndPassword.Password);

            bool success = await databaseRequestService.TryRegister(userNameAndPassword.UserName, hashedPassword);

            if (success)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, userNameAndPassword.UserName) };

                var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new
                {
                    access_token = encodedJwt,
                    username = userNameAndPassword.UserName,
                };

                return Results.Json(response);
            }

            return Results.Unauthorized();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
