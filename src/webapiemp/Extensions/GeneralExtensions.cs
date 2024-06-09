namespace webapiemp.Extensions;

public static class GeneralExtensions
{
    public static int GetUserId(this HttpContext httpContext)
    {
        //var user = await context.User.Claims.FirstOrDefault(x => x.Type == "id");
        if (httpContext.User == null)
        {
            return -1;
        }

        return int.Parse(httpContext.User.Claims.Single(x => x.Type == "id").Value); 
    }
}
