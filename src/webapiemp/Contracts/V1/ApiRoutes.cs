namespace webapiemp.Contracts.V1;

public class ApiRoutes
{
    public const string Root = "api";
    public const string Version = "v1";
    public const string Base = Root + "/" + Version;

    public static class Cards
    {
        public const string Create = Base + "/cards";
    }

    public static class Groups
    {
        public const string GetCards = Base + "/group/cards";
    }
}
