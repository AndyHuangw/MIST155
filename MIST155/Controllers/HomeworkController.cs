
using Microsoft.AspNetCore.Mvc;
using MIST155.Models;
namespace MIST155.Controllers
{
    public class HomeworkController : Controller
    {
       
        public IActionResult Index()
        {
            return View();
        }
        
    }
}
