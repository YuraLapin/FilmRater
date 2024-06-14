using System.ComponentModel.DataAnnotations;

namespace FilmRaterMain.Models
{
    public class LoginModel
    {
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ErrorType { get; set; }
        public string CameFrom { get; set; }
        public int FilmId { get; set; }
    }
}
