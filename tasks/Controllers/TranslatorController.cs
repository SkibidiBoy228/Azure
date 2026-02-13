using Microsoft.AspNetCore.Mvc;
using tasks.Models;

namespace tasks.Controllers
{
    public class TranslatorController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(TranslatorViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Text))
            {
                ModelState.AddModelError("Text", "Будь ласка, введіть текст");
                return View(model);
            }

            model.Result = "Тут буде переклад";
            return View(model);
        }
    }
}
