namespace FilmRaterMain.Controllers
{
    public class LibraryEntry
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
}
