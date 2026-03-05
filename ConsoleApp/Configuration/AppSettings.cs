namespace Task1.ConsoleApp.Configuration
{
    public class AppSettings
    {
        public string ApiType { get; set; } = "Http";
        public HttpSettings HttpSettings { get; set; } = new();
        public GrpcSettings GrpcSettings { get; set; } = new();
        public ConnectionStrings ConnectionStrings { get; set; } = new();
    }

    public class HttpSettings
    {
        public string Endpoint { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class GrpcSettings
    {
        public string Endpoint { get; set; } = string.Empty;
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; } = string.Empty;
    }
}
