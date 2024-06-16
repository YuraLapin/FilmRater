namespace FilmRaterMain.Controllers
{
    public class UserNameAndFilters
    {
        public string UserName { get; set; }
        public List<string> Genres { get; set; }
        public float MinScore { get; set; }
        public float MaxScore { get; set; }
        public int MinYear { get; set; }
        public int MaxYear { get; set; }
        public int Page { get; set; }
        public string NameFilter { get; set; }
    }
}
