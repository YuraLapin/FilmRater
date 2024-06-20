using System.ComponentModel.DataAnnotations;

namespace FilmRaterMain.Models
{
    public class MoreInfoModel
    {
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public bool UserLogged { get; set; }
        public int FilmId { get; set; } 
    }
}
