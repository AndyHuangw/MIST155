using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Scaffolding;
using MIST155.Models;
using System.Text;

namespace MIST155.Controllers
{
    public class ApiController : Controller
    {
        private readonly MyDBContext _context;
        public ApiController(MyDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Cities()
        {
            var cities = _context.Addresses.Select(x => x.City).Distinct();
            return Json(cities);
        }
        [HttpPost]
        public IActionResult Content()
        {
            return Content("<h2>你好Content</h2>", "text/html",Encoding.UTF8);
            //return Content("<h2>你好Content</h2>", "text/plain", Encoding.UTF8);
            //plain純文字  //html的格式
        }
        public IActionResult Image(int id=1)
        {
            Member? member = _context.Members.Find(id);
            if (member != null)
            {
                byte[] img = member.FileData;
                    if (img != null) 
                {
                return File(img,"image/jpeg");
                }
            }
            return NotFound();
        }
    }
}
