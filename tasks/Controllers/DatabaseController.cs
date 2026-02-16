using Microsoft.AspNetCore.Mvc;
using tasks.Data;
using System.Linq;

namespace tasks.Controllers
{
    public class DatabaseController : Controller
    {
        private readonly AppDbContext _context;

        public DatabaseController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var data = _context.Messages.ToList();
            return View(data);
        }
    }
}
