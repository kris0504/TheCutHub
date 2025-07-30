using Microsoft.AspNetCore.Mvc;

public class ErrorController : Controller
{
    [Route("Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        return statusCode switch
        {
            404 => View("Error404"),
            500 => View("Error500"),
            _ => View("Error500"),
        };
    }

    [Route("Error")]
    public IActionResult Error() => View("Error500");
}
