namespace NPMGame.API.Models.Config
{
    public class AppSettings
    {
        public string AuthCookieName { get; set; }

        public DatabaseSettings Database { get; set; }
    }

    public class DatabaseSettings
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}