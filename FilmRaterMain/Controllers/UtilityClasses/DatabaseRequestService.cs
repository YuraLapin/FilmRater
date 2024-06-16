using System.Data.SqlTypes;
using System.Text;

namespace FilmRaterMain.Controllers.UtilityClasses
{
    public class DatabaseRequestService
    {
        private DbConfiguration config;

        public DatabaseRequestService()
        {
            config = new DbConfiguration();
        }

        public const int pageSize = 30;

        public async Task<string> GetStoredHash(string login)
        {
            var storedHash = new List<string>();

            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(config.GetConnectionString()))
            {
                await conn.OpenAsync();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT user_password FROM user WHERE user_name = @user_name;";
                cmd.Parameters.AddWithValue("@user_name", login);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        storedHash.Add(reader.GetString(0));
                    }
                }

                await conn.CloseAsync();
            }

            return storedHash[0];
        }

        public async Task<bool> TryRegister(string login, string hashedPassword)
        {
            bool res;

            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(config.GetConnectionString()))
            {
                await conn.OpenAsync();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT user_name FROM user WHERE user_name = @user_name_check;";
                cmd.Parameters.AddWithValue("@user_name_check", login);

                var foundUsers = new List<string>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        foundUsers.Add(reader.GetString(0));
                    }
                }

                if (foundUsers.Count == 0)
                {
                    cmd.CommandText = "INSERT INTO user (user_name, user_password) VALUES (@user_name_register, @user_password)";
                    cmd.Parameters.AddWithValue("@user_name_register", login);
                    cmd.Parameters.AddWithValue("@user_password", hashedPassword);

                    await cmd.ExecuteNonQueryAsync();

                    res = true;
                }
                else
                {
                    res = false;
                }

                await conn.CloseAsync();
            }

            return res;
        }

        public async Task<bool> UserNameIsFree(string userName)
        {
            bool res;

            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(config.GetConnectionString()))
            {
                await conn.OpenAsync();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT user_name FROM user WHERE user_name = @user_name;";
                cmd.Parameters.AddWithValue("@user_name", userName);

                var foundUsers = new List<string>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        foundUsers.Add(reader.GetString(0));
                    }
                }

                if (foundUsers.Count == 0)
                {
                    res = true;
                }
                else
                {
                    res = false;
                }

                await conn.CloseAsync();
            }

            return res;
        }

        public async Task<List<string>> GetGenres()
        {
            var genres = new List<string>();

            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(config.GetConnectionString()))
            {
                await conn.OpenAsync();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT genre_name FROM genre;";

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        genres.Add(reader.GetString(0));
                    }
                }

                await conn.CloseAsync();
            }

            return genres;
        }

        public async Task<List<LibraryEntry>> GetLibrary(string userName, List<string> genres, float minScore, float maxScore, int minYear, int maxYear, int page)
        {
            var library = new List<LibraryEntry>();

            var genreFilter = new StringBuilder();
            foreach (var genre in genres)
            {
                genreFilter = genreFilter.Append(genre);
                genreFilter = genreFilter.Append(",");
            }

            var s = genreFilter.ToString();

            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(config.GetConnectionString()))
            {
                await conn.OpenAsync();

                var cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT film.film_id, film_name, film_year, film_duration, \r\nGROUP_CONCAT(DISTINCT director.director_name SEPARATOR ', ') AS directors,\r\nGROUP_CONCAT(DISTINCT country.country_name SEPARATOR ', ') AS countries,\r\nAVG(user_score.score) AS rating\r\n\tFROM film\r\n\t\tJOIN film_genre ON film_genre.film_id = film.film_id\r\n\t\tJOIN genre ON genre.genre_id = film_genre.genre_id\r\n        JOIN film_director ON film_director.film_id = film.film_id\r\n        JOIN director ON director.director_id = film_director.director_id\r\n        JOIN film_country ON film_country.film_id = film.film_id\r\n        JOIN country ON country.country_id = film_country.country_id\r\n        LEFT JOIN user_score ON user_score.film_id = film.film_id\r\n\tWHERE FIND_IN_SET(genre.genre_name, @genre_filter) OR @genre_filter = ''\r\n\tGROUP BY film.film_id\r\n\tHAVING film.film_year >= @min_year AND film.film_year <= @max_year\r\n    AND (AVG(user_score.score) >= @min_rating AND AVG(user_score.score) <= @max_rating) OR COUNT(user_score.score) = 0\r\n    LIMIT { (page - 1) * pageSize }, { pageSize };";
                cmd.Parameters.AddWithValue("@min_rating", minScore);
                cmd.Parameters.AddWithValue("@max_rating", maxScore);
                cmd.Parameters.AddWithValue("@min_year", minYear);
                cmd.Parameters.AddWithValue("@max_year", maxYear);
                cmd.Parameters.AddWithValue("@genre_filter", genreFilter.ToString());

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var newFilm = new LibraryEntry()
                        {
                            Id = reader.GetString(0),
                            Name = reader.GetString(1),
                            Year = reader.GetInt32(2),
                            Duration = reader.GetInt32(3),
                            Directors = reader.GetString(4).Split(", ").ToList(),
                            Countries = reader.GetString(5).Split(", ").ToList(),
                        };
                        try
                        {
                            newFilm.Rating = reader.GetFloat(6);
                        }
                        catch (SqlNullValueException)
                        {
                            newFilm.Rating = 0;
                        }
                        library.Add(newFilm);
                    }
                }

                foreach (var film in library)
                {
                    var currentGenres = new List<string>();
                    int currentUserRating = 0;

                    cmd.CommandText = "SELECT genre.genre_name FROM film_genre JOIN genre ON genre.genre_id = film_genre.genre_id WHERE film_genre.film_id = @film_id;";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@film_id", film.Id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            currentGenres.Add(reader.GetString(0));
                        }
                    }

                    if (userName != "")
                    {
                        cmd.CommandText = "SELECT user_score.score FROM user_score WHERE user_score.film_id = @film_id AND user_score.user_name = @user_name;";
                        cmd.Parameters.AddWithValue("@user_name", userName);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                currentUserRating = reader.GetInt32(0);
                            }
                        }
                    }
                    else
                    {
                        currentUserRating = 0;
                    }
                    

                    film.Genres = currentGenres;
                    film.CurrentUserRating = currentUserRating;
                }

                await conn.CloseAsync();
            }

            return library;
        }

        public async Task<MinMaxYears> GetMinMaxYears()
        {
            var years = new MinMaxYears();

            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(config.GetConnectionString()))
            {
                await conn.OpenAsync();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT MIN(film.film_year), MAX(film.film_year) FROM film;";

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        years.MinYear = reader.GetInt32(0);
                        years.MaxYear = reader.GetInt32(1);
                    }
                }
            }

            return years;
        }

        public async Task<bool> UpdateUserScore(int filmId, string userName, int userScore)
        {
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(config.GetConnectionString()))
            {
                await conn.OpenAsync();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT score FROM user_score WHERE film_id = @film_id AND user_name = @user_name;";
                cmd.Parameters.AddWithValue("@film_id", filmId);
                cmd.Parameters.AddWithValue("@user_name", userName);

                var foundScores = new List<string>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        foundScores.Add(reader.GetString(0));
                    }
                }

                if (foundScores.Count == 0)
                {
                    cmd.CommandText = "INSERT INTO user_score (film_id, user_name, score) VALUES (@film_id, @user_name, @score);";
                }

                else
                {
                    cmd.CommandText = "UPDATE user_score SET score = @score WHERE (user_name = @user_name AND film_id = @film_id);";
                }

                cmd.Parameters.AddWithValue("@score", userScore);

                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }

        public async Task<int> GetTotalPages(List<string> genres, float minScore, float maxScore, int minYear, int maxYear)
        {
            int pages = 1;

            var genreFilter = new StringBuilder();
            foreach (var genre in genres)
            {
                genreFilter = genreFilter.AppendLine(genre);
                genreFilter = genreFilter.AppendLine(",");
            }

            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(config.GetConnectionString()))
            {
                await conn.OpenAsync();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM\r\n(\r\nSELECT film.film_id, film.film_name, film.film_year, film.film_duration, \r\nGROUP_CONCAT(DISTINCT director.director_name SEPARATOR ', ') AS directors,\r\nGROUP_CONCAT(DISTINCT country.country_name SEPARATOR ', ') AS countries,\r\nAVG(user_score.score) AS rating\r\n\tFROM film\r\n\t\tJOIN film_genre ON film_genre.film_id = film.film_id\r\n\t\tJOIN genre ON genre.genre_id = film_genre.genre_id\r\n        JOIN film_director ON film_director.film_id = film.film_id\r\n        JOIN director ON director.director_id = film_director.director_id\r\n        JOIN film_country ON film_country.film_id = film.film_id\r\n        JOIN country ON country.country_id = film_country.country_id\r\n        LEFT JOIN user_score ON user_score.film_id = film.film_id\r\n\tWHERE FIND_IN_SET(genre.genre_name, @genre_filter) OR @genre_filter = \"\"\r\n\tGROUP BY film.film_id\r\n\tHAVING film.film_year >= @min_year AND film.film_year <= @max_year\r\n    AND (AVG(user_score.score) >= @min_rating AND AVG(user_score.score) <= @max_rating) OR COUNT(user_score.score) = 0\r\n) AS a;";
                cmd.Parameters.AddWithValue("@min_rating", minScore);
                cmd.Parameters.AddWithValue("@max_rating", maxScore);
                cmd.Parameters.AddWithValue("@min_year", minYear);
                cmd.Parameters.AddWithValue("@max_year", maxYear);
                cmd.Parameters.AddWithValue("@genre_filter", genreFilter.ToString());

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        pages = (int)MathF.Ceiling((float)reader.GetInt32(0) / (float)pageSize);
                    }
                }                

            }

            return pages;
        }

        public async Task<FullFilmData> GetFullFilmData(string userName, string filmId)
        {
            var filmData = new FullFilmData();

            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(config.GetConnectionString()))
            {
                await conn.OpenAsync();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT film.film_id, film.film_name, film.film_year, film.film_duration, film.film_slogan, film.film_synopsis, \r\nGROUP_CONCAT(DISTINCT director.director_name SEPARATOR ', ') AS directors,\r\nGROUP_CONCAT(DISTINCT country.country_name SEPARATOR ', ') AS countries,\r\nGROUP_CONCAT(DISTINCT genre.genre_name SEPARATOR ', ') AS genres,\r\nAVG(user_score.score) AS rating\r\n\tFROM film\r\n\t\tLEFT JOIN film_genre ON film_genre.film_id = film.film_id\r\n\t\tLEFT JOIN genre ON genre.genre_id = film_genre.genre_id\r\n        LEFT JOIN film_director ON film_director.film_id = film.film_id\r\n        LEFT JOIN director ON director.director_id = film_director.director_id\r\n        LEFT JOIN film_country ON film_country.film_id = film.film_id\r\n        LEFT JOIN country ON country.country_id = film_country.country_id\r\n        LEFT JOIN user_score ON user_score.film_id = film.film_id\r\n\tWHERE film.film_id = @film_id\r\n\tGROUP BY film.film_id;";
                cmd.Parameters.AddWithValue("@film_id", filmId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        filmData.Id = filmId;
                        filmData.Name = reader.GetString(1);
                        filmData.Year = reader.GetInt32(2);
                        filmData.Duration = reader.GetInt32(3);
                        filmData.Slogan = reader.GetString(4);
                        filmData.Synopsis = reader.GetString(5);
                        filmData.Directors = reader.GetString(6).Split(", ").ToList();
                        filmData.Countries = reader.GetString(7).Split(", ").ToList();
                        filmData.Genres = reader.GetString(8).Split(", ").ToList();
                        try
                        {
                            filmData.Rating = reader.GetFloat(9);
                        }
                        catch (SqlNullValueException)
                        {
                            filmData.Rating = 0;
                        }
                    }
                }

                if (userName != "")
                {
                    cmd.CommandText = "SELECT score FROM user_score WHERE film_id = @film_id AND user_name = @user_name";
                    cmd.Parameters.AddWithValue("@user_name", userName);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            try
                            {
                                filmData.CurrentUserRating = reader.GetInt32(0);
                            }
                            catch (SqlNullValueException)
                            {
                                filmData.CurrentUserRating = 0;
                            }
                        }
                    }
                }
                else
                {
                    filmData.CurrentUserRating = 0;
                }

            }

            return filmData;
        }

        public async Task<List<Comment>> GetFilmComments(string filmId)
        {
            var comments = new List<Comment>();

            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(config.GetConnectionString()))
            {
                await conn.OpenAsync();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT film_comment.user_name, film_comment.comment_text, user_score.score\r\n\tFROM film_comment\r\n\t\tLEFT JOIN user_score ON user_score.film_id = film_comment.film_id AND user_score.user_name = film_comment.user_name\r\n\tWHERE film_comment.film_id = @film_id;";
                cmd.Parameters.AddWithValue("@film_id", filmId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var newComment = new Comment()
                        {
                            UserName = reader.GetString(0),
                            Text = reader.GetString(1),
                        };
                        try
                        {
                            newComment.UserScore = reader.GetInt32(2);
                        }
                        catch (SqlNullValueException)
                        {
                            newComment.UserScore = 0;
                        }
                        comments.Add(newComment);  
                    }
                }
            }

            return comments;
        }

        public async Task<bool> UploadComment(string filmId, string userName, string commentText)
        {
            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(config.GetConnectionString()))
            {
                await conn.OpenAsync();
                var cmd = conn.CreateCommand();

                cmd.CommandText = "INSERT INTO film_comment (film_id, user_name, comment_text) VALUES (@film_id, @user_name, @comment_text);";
                cmd.Parameters.AddWithValue("@film_id", filmId);
                cmd.Parameters.AddWithValue("@user_name", userName);
                cmd.Parameters.AddWithValue("@comment_text", commentText);

                await cmd.ExecuteNonQueryAsync();
            }

            return true;
        }
    }
}
