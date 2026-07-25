using ESun.Business;
using ESun.Common.Dtos;
using ESun.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ESun.Web.Controllers
{
    public class PreferenceController : Controller
    {
        private readonly IPreferenceService _service;
        private const string SessionUserKey = "CurrentUserId";

        public PreferenceController(IPreferenceService service)
        {
            _service = service;
        }

        private string? CurrentUserId => HttpContext.Session.GetString(SessionUserKey);

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await _service.GetUsersAsync();

            var vm = new PreferenceIndexViewModel
            {
                CurrentUserId = CurrentUserId,
                UserOptions = users.Select(u => new SelectListItem
                {
                    Value = u.UserID,
                    Text = $"{u.UserName}（{u.UserID}）",
                    Selected = u.UserID == CurrentUserId
                })
            };

            if (!string.IsNullOrEmpty(CurrentUserId))
                vm.Items = await _service.GetListAsync(CurrentUserId);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectUser(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
                HttpContext.Session.SetString(SessionUserKey, userId);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (string.IsNullOrEmpty(CurrentUserId))
                return RedirectToAction(nameof(Index));

            return View(new PreferenceFormViewModel { UserID = CurrentUserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<IActionResult> Create(PreferenceFormViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            // UserID 一律採用 Session 的值，不信任表單傳來的（防止竄改冒用他人身分）
            if (string.IsNullOrEmpty(CurrentUserId))
                return RedirectToAction(nameof(Index));

            var dto = new PreferenceInputDto
            {
                UserID = CurrentUserId,
                ProductName = vm.ProductName,
                Price = vm.Price,
                FeeRate = vm.FeeRate,
                PurchaseQuantity = vm.PurchaseQuantity,
                Account = vm.Account
            };

            await _service.AddAsync(dto);
            TempData["Message"] = "新增成功！";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (string.IsNullOrEmpty(CurrentUserId))
                return RedirectToAction(nameof(Index));

            // GetById 的查詢條件已包含 UserID，查詢他人的 SN 會回傳 null（防 IDOR）
            var item = await _service.GetByIdAsync(id, CurrentUserId);
            if (item == null)
                return NotFound();

            var vm = new PreferenceFormViewModel
            {
                SN = item.SN,
                UserID = item.UserID,
                ProductName = item.ProductName,
                Price = item.Price,
                FeeRate = item.FeeRate,
                PurchaseQuantity = item.PurchaseQuantity,
                Account = item.Account
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PreferenceFormViewModel vm)
        {
            if(!ModelState.IsValid)
                  return View(vm);

            if (string.IsNullOrEmpty(CurrentUserId))
                return RedirectToAction(nameof(Index));

            var dto = new PreferenceInputDto
            { 
                SN = vm.SN,
                UserID = CurrentUserId,
                ProductName = vm.ProductName,
                Price = vm.Price,
                FeeRate = vm.FeeRate,
                PurchaseQuantity = vm.PurchaseQuantity,
                Account = vm.Account
            };
            await _service.UpdateAsync(dto);
            TempData["Message"] = "更新成功！";
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (string.IsNullOrEmpty(CurrentUserId))
                return RedirectToAction(nameof(Index));
            var item = await _service.GetByIdAsync(id, CurrentUserId);

            if (item == null)
                return NotFound();

            return View(item);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id )
        {
            if (string.IsNullOrEmpty(CurrentUserId))
                return RedirectToAction(nameof(Index));
            await _service.DeleteAsync(id, CurrentUserId);
            TempData["Message"] = "刪除成功！";
            return RedirectToAction(nameof(Index));
        }
    }
}