namespace FilmRaterMain.Controllers.UtilityClasses
{
    public class DbConfiguration
    {
        public string server;
        public string uid;
        public string pwd;
        public string database;

        public DbConfiguration()
        {
            server = "localhost";
            uid = "client";
            pwd = "clientpassword192837";
            database = "film_db";
        }

        public DbConfiguration(string server, string uid, string pwd, string database)
        {
            this.server = server;
            this.uid = uid;
            this.pwd = pwd;
            this.database = database;
        }

        public string GetConnectionString()
        {
            return $"server={ server };uid={ uid };Port=3306;pwd={ pwd };database={ database };Allow User Variables=True;default command timeout=30";
        }
    }
}
