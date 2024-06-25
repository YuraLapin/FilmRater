using FilmRaterMain.Controllers.UtilityClasses;
using Microsoft.AspNetCore.Mvc;

namespace FilmRaterMain.Controllers
{
    [ApiController]
    [Route("api/requests")]
    public class RequestController : ControllerBase
    {
        DatabaseRequestService databaseRequestService;

        public RequestController()
        {
            databaseRequestService = new DatabaseRequestService();
        }

        [HttpGet("GetGenres")]
        public async Task<IActionResult> GetGenres()
        {
            var genres = await databaseRequestService.GetGenres();

            return Ok(genres);
        }

        [HttpPost("GetLibrary")]
        public async Task<IActionResult> GetLibrary([FromBody] UserNameAndFilters userNameAndFilters)
        {
            var library = await databaseRequestService.GetLibrary(userNameAndFilters.UserName, userNameAndFilters.Genres, userNameAndFilters.MinScore, userNameAndFilters.MaxScore, userNameAndFilters.MinYear, userNameAndFilters.MaxYear, userNameAndFilters.Page, userNameAndFilters.NameFilter);

            return Ok(library);
        }

        [HttpGet("GetMinMaxYears")]
        public async Task<IActionResult> GetMinMaxYears()
        {
            var years = await databaseRequestService.GetMinMaxYears();

            return Ok(years);
        }

        [HttpPost("UpdateUserScore")]
        public async Task<IActionResult> UpdateUserScore([FromBody] FilmScoreUpdate filmScoreUpdate)
        {
            return Ok(await databaseRequestService.UpdateUserScore(filmScoreUpdate.FilmId, filmScoreUpdate.UserName, filmScoreUpdate.UserScore));
        }

        [HttpPost("GetTotalPages")]
        public async Task<IActionResult> GetTotalPages([FromBody] UserNameAndFilters userNameAndFilters)
        {
            return Ok(await databaseRequestService.GetTotalPages(userNameAndFilters.Genres, userNameAndFilters.MinScore, userNameAndFilters.MaxScore, userNameAndFilters.MinYear, userNameAndFilters.MaxYear, userNameAndFilters.NameFilter));
        }

        [HttpPost("GetFullFilmData")]
        public async Task<IActionResult> GetFullFilmData([FromBody] UserNameAndFilmId userNameAndFilmId )
        {
            return Ok(await databaseRequestService.GetFullFilmData(userNameAndFilmId.UserName, userNameAndFilmId.FilmId));
        }

        [HttpPost("GetFilmComments")]
        public async Task<IActionResult> GetFilmComments([FromBody] WrappedFilmId filmId)
        {
            return Ok(await databaseRequestService.GetFilmComments(filmId.filmId));
        }

        [HttpPost("UploadComment")]
        public async Task<IActionResult> UploadComment([FromBody] NewCommentData commentData)
        {
            return Ok(await databaseRequestService.UploadComment(commentData.FilmId, commentData.UserName, commentData.Text));
        }
    }
}
