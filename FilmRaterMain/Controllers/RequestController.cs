using FilmRaterMain.Controllers.UtilityClasses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FilmRaterMain.Controllers
{
    public class LoginWithPassword
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class UserNameAndFilters
    {
        public string UserName { get; set; }
        public List<string>? Genres { get; set; }
        public float? MinScore { get; set; }
        public float? MaxScore { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
    }

    public class MinMaxYears
    {
        public int MinYear { get; set;}
        public int MaxYear { get; set;}
    }

    public class FilmScoreUpdate
    {
        public int FilmId { get; set; }
        public string UserName { get; set; }
        public int UserScore { get; set; }
    }

    [ApiController]
    [Route("Home/api/requests")]
    public class RequestController : ControllerBase
    {
        DatabaseRequestService requestClass;
        PasswordHashService passwordHashService;

        public RequestController()
        {
            requestClass = new DatabaseRequestService();
            passwordHashService = new PasswordHashService();
        }

        [HttpPost("TryLogIn")]
        public async Task<IActionResult> TryLogIn([FromBody] LoginWithPassword loginWithPassword)
        {
            if (await requestClass.UserNameIsFree(loginWithPassword.Login))
            {
                return Ok(false);
            }

            string storedHash = await requestClass.GetStoredHash(loginWithPassword.Login);

            bool success = passwordHashService.CheckHash(storedHash, loginWithPassword.Password);

            return Ok(success);
        }

        [HttpPost("TryRegister")]
        public async Task<IActionResult> TryRegister([FromBody] LoginWithPassword loginWithPassword)
        {
            string hashedPassword = passwordHashService.HashPassword(loginWithPassword.Password);

            bool res = await requestClass.TryRegister(loginWithPassword.Login, hashedPassword);

            return Ok(res);
        }

        [HttpGet("GetGenres")]
        public async Task<IActionResult> GetGenres()
        {
            var genres = await requestClass.GetGenres();

            return Ok(genres);
        }

        [HttpPost("GetLibrary")]
        public async Task<IActionResult> GetLibrary([FromBody] UserNameAndFilters userNameAndFilters)
        {
            var library = await requestClass.GetLibrary(userNameAndFilters.UserName, userNameAndFilters.Genres, userNameAndFilters.MinScore, userNameAndFilters.MaxScore, userNameAndFilters.MinYear, userNameAndFilters.MaxYear);

            return Ok(library);
        }

        [HttpGet("GetMinMaxYears")]
        public async Task<IActionResult> GetMinMaxYears()
        {
            var years = await requestClass.GetMinMaxYears();

            return Ok(years);
        }

        [HttpPost("UpdateUserScore")]
        public async Task<IActionResult> UpdateUserScore([FromBody] FilmScoreUpdate filmScoreUpdate)
        {
            return Ok(await requestClass.UpdateUserScore(filmScoreUpdate.FilmId, filmScoreUpdate.UserName, filmScoreUpdate.UserScore));
        }
    }
}
