using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.IdentityModel.Tokens;
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
        private readonly IWebHostEnvironment _environment;
        public ApiController(MyDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
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
        public IActionResult Districts(string city)
        {
            var districts = _context.Addresses.Where(a => a.City == city).Select(a => a.SiteId).Distinct();
            return Json(districts);
        }
        public IActionResult Road(string district)
        {// 從 _context.Addresses 中查詢 SiteId// 過濾出城市等於輸入參數 city 的地址
            // 選擇每個地址的 SiteId// 去除重複的 SiteId

            var road = _context.Addresses.Where(a => a.SiteId == district).Select(a => a.Road).Distinct();
            return Json(road);
        }
        public IActionResult Address()
        {
            return View();
        }
        public IActionResult SpotTitle(string title)
        {
            var titles = _context.Spots.Where(s => s.SpotTitle.Contains(title)).Select(s=>s.SpotTitle).Take(8);

            return Json(titles);
        }

        public IActionResult Content()
        {
            Thread.Sleep(5000);
            //int x = 10;
            //int y = 0;
            //int z = x / y;

            return Content("<h2>你好Content</h2>", "text/html", Encoding.UTF8);
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
        //public IActionResult Rigister(UserDTO _user)
        public IActionResult Rigister(Member _user, IFormFile Avatar)
        {
            if (string.IsNullOrEmpty(_user.Name))
            {
                _user.Name = "guest";
            }


            // 預設檔案名稱為 "empty.jpg"
            string fileName = "empty.jpg";
            // 如果使用者的頭像不為空，則使用實際檔案名稱
            if (Avatar != null)
            {
                fileName = Avatar.FileName;
            }
            // 構建上傳路徑，結合 WebRootPath、"uploads" 資料夾和檔案名稱
            string uploadPath = Path.Combine(_environment.WebRootPath, "uploads", fileName);
            // 使用 FileStream 創建檔案，如果檔案已存在，則覆蓋
            using (var fileStream = new FileStream(uploadPath, FileMode.Create))
            {
                Avatar?.CopyTo(fileStream);
            }
            //return Content($"你好{_user.Name},你{_user.Age}歲了,E-mail是{_user.Email}", "text/plain", Encoding.UTF8);
            //return Content($"{_user.Avatar?.FileName}-{_user.Avatar?.ContentType}-{_user.Avatar?.Length};");


            _user.FileName = fileName;
            byte[]? imagebyte = null;
            using (var memoryStream = new MemoryStream())
            {
                Avatar?.CopyTo(memoryStream);
                imagebyte = memoryStream.ToArray();
            }
            _user.FileData = imagebyte;
            _context.Members.Add(_user);
            _context.SaveChanges();

            return Content(uploadPath);
        }
        [HttpPost] //使用[HttpPost] 屬性標記的 Action，處理 POST 請求，接收 JSON 格式的請求主體
        public IActionResult Spot([FromBody] SearchDTO _search)
        {
           
            // 將接收到的 SearchDTO 物件轉換為 JSON 格式並返回
            var spots = _search.categoryId == 0 ? _context.SpotImagesSpots : _context.SpotImagesSpots.Where(s=>s.CategoryId == _search.categoryId);
            //關鍵字搜尋
            if(!string.IsNullOrEmpty(_search.keyword))
            {
                // 過濾條件：場景標題包含搜尋關鍵字或場景描述包含搜尋關鍵字
                spots = spots.Where(s => s.SpotTitle.Contains(_search.keyword) || s.SpotDescription.Contains(_search.keyword));
            }
            //排序
            switch(_search.sortBy)
            {
                // 如果排序類型為升序，則按場景標題升序排序；否則按場景標題降序排序
                case "spotTitle":
                    spots = _search.sortType == "asc" ?spots.OrderBy(s=>s.SpotTitle):spots.OrderByDescending(s=>s.SpotTitle);
                    break;
                // 如果排序類型為升序，則按類別 ID 升序排序；否則按類別 ID 降序排序
                case "categoryId":
                    spots = _search.sortType == "asc" ? spots.OrderBy(s => s.CategoryId) : spots.OrderByDescending(s => s.CategoryId);
                    break;
                // 如果未指定排序條件或排序條件不匹配，默認按場景 ID 進行排序
                default:
                    spots = _search.sortType == "asc" ? spots.OrderBy(s => s.SpotId) : spots.OrderByDescending(s => s.SpotId);
                    break;
            }
            //分頁
            // 獲取場景集合的總數
            int totalCount = spots.Count();
            // 設定每頁顯示的項目數，如果未指定，則默認為 9
            int pageSize = _search.pageSize ?? 9;
            // 計算總頁數，採用上取整以確保足夠的頁數容納所有項目
            int totalPage = (int)Math.Ceiling((decimal)totalCount / pageSize);
            // 獲取要顯示的頁碼，如果未指定，則默認為第 1 頁
            int page = _search.page ?? 1;
            // 使用 Skip 和 Take 方法實現分頁，計算要跳過的項目數並取出指定數量的項目
            spots = spots.Skip((page-1)*pageSize).Take(pageSize);

            SpotsPagingDTO spotsPaging = new SpotsPagingDTO();
            spotsPaging.TotalPages = totalPage;
            spotsPaging.SpotsResult = spots.ToList();
            return Json(spotsPaging);

            //return Json(spots);
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
