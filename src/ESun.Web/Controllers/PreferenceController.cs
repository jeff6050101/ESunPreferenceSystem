using ESun.Business;
using ESun.Common.Dtos;
using ESun.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ESun.Web.Controllers
{
    public class PreferenceController : Controller   // ← 繼承 Controller
    {
        private readonly IPreferenceService _service;
        private const string SessionUserKey = "CurrentUserId";

        public PreferenceController(IPreferenceService service)
        {
            _service = service;
        }

        // 小工具：目前使用者 ID（未選擇時為 null）
        private string? CurrentUserId => HttpContext.Session.GetString(SessionUserKey);


        // ===== Index（範本，直接抄）=====
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

        // ===== SelectUser：切換使用者（範本，直接抄）=====
        [HttpPost]
        [ValidateAntiForgeryToken] //防偽機制
        public IActionResult SelectUser(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
                HttpContext.Session.SetString(SessionUserKey, userId);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Create()
        {
            // 1. 還沒選使用者 → 趕他回清單頁去選
            if (string.IsNullOrEmpty(CurrentUserId))
                return RedirectToAction(nameof(Index));

            // 2. 給一張空白表單，UserID 先填好目前使用者
            return View(new PreferenceFormViewModel { UserID = CurrentUserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<IActionResult> Create(PreferenceFormViewModel vm)
        {
            // 步驟 1：驗證沒過 → 回表單（錯誤訊息會自動顯示）
            if (!ModelState.IsValid)
                return View(vm);

            // 步驟 2：【安全】UserID 一律用 Session 的，不信任表單傳來的
            if (string.IsNullOrEmpty(CurrentUserId))
                return RedirectToAction(nameof(Index));

            // 步驟 3：ViewModel → DTO（Service 要的是 DTO）
            var dto = new PreferenceInputDto
            {
                UserID = CurrentUserId,
                ProductName = vm.ProductName,
                Price = vm.Price,
                FeeRate = vm.FeeRate,
                PurchaseQuantity = vm.PurchaseQuantity,
                Account = vm.Account
            };

            // 步驟 4：呼叫業務層 → 成功後導回清單
            await _service.AddAsync(dto);
            TempData["Message"] = "新增成功！";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // 1. 還沒選使用者 → 趕他回清單頁去選
            if (string.IsNullOrEmpty(CurrentUserId))
                return RedirectToAction(nameof(Index));

            // 2. 查出這一筆（GetById 有 UserID 條件，查別人的會回 null）
            var item = await _service.GetByIdAsync(id, CurrentUserId);
            if (item == null)
                return NotFound();

            // 3. 把查到的值「填進」表單 ViewModel

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
            var item = await _service.GetByIdAsync(id, CurrentUserId); // 不用再用PreferenceListItemDto，因為回傳的就是了

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