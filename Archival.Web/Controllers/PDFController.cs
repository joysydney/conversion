using Archival.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Archival.Web.Controllers
{
    public class PDFController : Controller
    {
        private readonly IPDFConversion _pdf;
        public PDFController(IPDFConversion pdf)
        {
            _pdf = pdf;
        }

        public IActionResult Index()
        {
            _pdf.Execute();
            return View();
        }
    }
}
