using System.ComponentModel.DataAnnotations;

namespace FilmRaterMain.Models
{
    public class LibraryModel
    {
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string UserName { get; set; }
    }
}
