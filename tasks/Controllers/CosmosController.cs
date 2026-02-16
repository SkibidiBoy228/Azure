using Microsoft.AspNetCore.Mvc;
using tasks.Data;
using tasks.Models;

namespace tasks.Controllers
{
    public class CosmosController : Controller
    {
        private readonly CosmosService _service;

        public CosmosController(CosmosService service)
        {
            _service = service;
        }

        [HttpGet("/Cosmos")]
        public async Task<IActionResult> Index([FromQuery] List<string>? selectedGroups)
        {
            var allGroups = await _service.GetAllGroupIdsAsync();
            ViewBag.Groups = allGroups;


            if (selectedGroups?.Any() != true)
                return View(new List<CosmosMessage>());

            var guidList = selectedGroups.Select(Guid.Parse).ToList();
            var data = await _service.GetByGroupIdsAsync(guidList);

            return View(data);
        }
    }
}
