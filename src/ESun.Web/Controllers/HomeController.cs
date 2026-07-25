using System.Diagnostics;
using ESun.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace ESun.Web.Controllers
{
    // 本系統首頁改為 Preference/Index，Home 僅保留全域例外處理頁。
    // Program.cs 的 UseExceptionHandler("/Home/Error") 會導向這裡。
    public class HomeController : Controller
    {
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
