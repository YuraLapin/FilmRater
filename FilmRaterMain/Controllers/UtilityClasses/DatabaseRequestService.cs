using System.Data.SqlTypes;

namespace FilmRaterMain.Controllers.UtilityClasses
{
    public class FilmData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public int Duration { get; set; }
    }

    public class CompleteFilmData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Year { get; set; }
        public int Duration { get; set; }
        public List<string> Genres { get; set; }
        public List<string> Directors { get; set; }
        public List<string> Countries { get; set; }
        public float Rating { get; set; }
        public int CurrentUserRating { get; set; }
    }

    public class DatabaseRequestService
    {
        private DbConfiguration config;

        public DatabaseRequestService()
        {
            config = new DbConfiguration();
        }

        public async Task<string> GetStoredHash(string login)
        {
            var storedHash = new List<string>();

            MySql.Data.MySqlClient.MySqlConnection conn;

            using (conn = new MySql.Data.MySqlClient.MySqlConnection(config.GetConnectionString()))
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
            MySql.Data.MySqlClient.MySqlConnection conn;

            using (conn = new MySql.Data.MySqlClient.MySqlConnection(config.GetConnectionString()))
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
            MySql.Data.MySqlClient.MySqlConnection conn;

            using (conn = new MySql.Data.MySqlClient.MySqlConnection(config.GetConnectionString()))
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

            MySql.Data.MySqlClient.MySqlConnection conn;

            using (conn = new MySql.Data.MySqlClient.MySqlConnection(config.GetConnectionString()))
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

        public async Task<List<CompleteFilmData>> GetLibrary(string userName, List<string>? genres, float? minScore, float? maxScore, int? minYear, int? maxYear)
        {
            var library = new List<CompleteFilmData>();

            MySql.Data.MySqlClient.MySqlConnection conn;

            using (conn = new MySql.Data.MySqlClient.MySqlConnection(config.GetConnectionString()))
            {
                await conn.OpenAsync();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT film_id, film_name, film_year, film_duration FROM film;";

                var filmList = new List<FilmData>();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        filmList.Add(new FilmData()
                        {
                            Id = reader.GetString(0),
                            Name = reader.GetString(1),
                            Year = reader.GetInt32(2),
                            Duration = reader.GetInt32(3),
                        });
                    }
                }

                int i = 0;
                foreach (var film in filmList)
                {
                    var currentGenres = new List<string>();
                    var currentDirectors = new List<string>();
                    var currentCountries = new List<string>();
                    float rating = 0;
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

                    cmd.CommandText = "SELECT director.director_name FROM film_director JOIN director ON director.director_id = film_director.director_id WHERE film_director.film_id = @film_id;";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            currentDirectors.Add(reader.GetString(0));
                        }
                    }

                    cmd.CommandText = "SELECT country.country_name FROM film_country JOIN country ON country.country_id = film_country.country_id WHERE film_country.film_id = @film_id;";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            currentCountries.Add(reader.GetString(0));
                        }
                    }

                    cmd.CommandText = "SELECT AVG(user_score.score) FROM user_score WHERE user_score.film_id = @film_id;";
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        try
                        {
                            while (await reader.ReadAsync())
                            {
                                rating = MathF.Round(reader.GetFloat(0), 2);
                            }
                        }
                        catch (SqlNullValueException)
                        {
                            rating = 0;
                        }
                    }

                    cmd.CommandText = "SELECT user_score.score FROM user_score WHERE user_score.film_id = @film_id AND user_score.user_name = @user_name;";
                    cmd.Parameters.AddWithValue("@user_name", userName);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            currentUserRating = reader.GetInt32(0);
                        }
                    }

                    var completeFilm = new CompleteFilmData()
                    {
                        Id = film.Id,
                        Name = film.Name,
                        Year = film.Year,
                        Duration = film.Duration,
                        Genres = currentGenres,
                        Directors = currentDirectors,
                        Countries = currentCountries,
                        Rating = rating,
                        CurrentUserRating = currentUserRating,
                    };

                    library.Add(completeFilm);

                    ++i;
                }

                await conn.CloseAsync();
            }

            if (genres != null && genres.Count() > 0)
            {
                foreach (var film in library.ToList())
                {
                    bool deleting = true;
                    foreach (var genre in genres)
                    {
                        if (film.Genres.Contains(genre))
                        {
                            deleting = false;
                        }
                    }
                    if (deleting)
                    {
                        library.Remove(film);
                    }
                }
            }

            if (minScore != null && maxScore != null)
            {
                foreach (var film in library.ToList())
                {
                    if (film.Rating > maxScore || film.Rating < minScore)
                    {
                        library.Remove(film);
                    }
                }
            }

            if (minYear != null && maxYear != null)
            {
                foreach (var film in library.ToList())
                {
                    if (film.Year > maxYear || film.Year < minYear)
                    {
                        library.Remove(film);
                    }
                }
            }

            return library;
        }

        public async Task<MinMaxYears> GetMinMaxYears()
        {
            var years = new MinMaxYears();
            MySql.Data.MySqlClient.MySqlConnection conn;

            using (conn = new MySql.Data.MySqlClient.MySqlConnection(config.GetConnectionString()))
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
            MySql.Data.MySqlClient.MySqlConnection conn;

            using (conn = new MySql.Data.MySqlClient.MySqlConnection(config.GetConnectionString()))
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
    }
}
