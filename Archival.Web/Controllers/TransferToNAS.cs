using Archival.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Archival.Controllers
{
    public class TransferToNAS : Controller
    {
        private readonly INASConversion _nas;
        public TransferToNAS(INASConversion nas)
        {
            _nas = nas;
        }

        public IActionResult Index()
        {
            _nas.Execute();
            return View();
        }
    }
}
