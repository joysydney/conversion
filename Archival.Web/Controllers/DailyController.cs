using Archival.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Archival.Web.Controllers
{
    public class DailyController : Controller
    {
        private readonly IDailyConversion _daily;
        public DailyController(IDailyConversion daily)
        {
            _daily = daily;
        }

        public IActionResult Index()
        {
            _daily.Execute();
            return View();
        }
    }
}
