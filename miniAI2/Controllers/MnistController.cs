using Microsoft.AspNetCore.Mvc;

public class MnistController : Controller
{

    public MnistController()
    {
    }

    public IActionResult Index()
    {
        return View();
    }
}
