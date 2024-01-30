using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;
using MIST155.Models;
using MIST155.Models.DTO;
using System.Text;
using System.Xml.Linq;
using static Azure.Core.HttpHeader;

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
        
        public IActionResult Content()
        {
            Thread.Sleep(5000);
            //int x = 10;
            //int y = 0;
            //int z = x / y;
            
            return Content("<h2>你好Content</h2>", "text/html",Encoding.UTF8);
            //return Content("<h2>你好Content</h2>", "text/plain", Encoding.UTF8);
            //plain純文字  //html的格式
        }
        //public IActionResult Rigister(string name,int age = 28)
        //{
        //    if(string.IsNullOrEmpty(name))
        //    {
        //        name = "guest";
        //    }
        //    return Content($"你好{name},你{age}歲了","text/plain",Encoding.UTF8);
        //}
        public IActionResult Rigister(UserDTO _user)
        {
            if (string.IsNullOrEmpty(_user.Name))
            {
                _user.Name = "guest";
            }
            return Content($"你好{_user.Name},你{_user.Age}歲了,E-mail{_user.Email}", "text/plain", Encoding.UTF8);
        }
        public IActionResult CheckAccount(UserDTO _user) 
        {
            
            // 使用 Entity Framework Core 的 LINQ 查詢檢查帳號是否存在
            if (_context.Members.Any(p => p.Name == _user.Name))
            {
                // 如果帳號存在，返回帳號已存在的訊息
                return Content($"{_user.Name}帳號已存在");
            }
            else
            {
                // 如果帳號不存在，返回帳號可以使用的訊息
                return Content($"{_user.Name}可以使用");
            }
           
            
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
