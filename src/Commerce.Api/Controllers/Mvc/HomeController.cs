using Microsoft.AspNetCore.Mvc;

namespace Commerce.Api.Controllers.Mvc;

public sealed class HomeController : Controller
{
    public IActionResult Index() => RedirectToAction("Index", "Customers");
}
