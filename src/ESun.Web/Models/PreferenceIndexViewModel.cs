using ESun.Common.Dtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ESun.Web.Models
{
    /// <summary>
    /// 清單頁（需求 2）的檢視模型。
    /// 組合「使用者下拉選單」與「該使用者的喜好清單」兩塊資料。
    /// </summary>
    public class PreferenceIndexViewModel
    {
        /// <summary>使用者下拉選單來源。</summary>
        public IEnumerable<SelectListItem> UserOptions { get; set; } = Enumerable.Empty<SelectListItem>();

        /// <summary>目前選定的使用者 ID（來自 Session）。</summary>
        public string? CurrentUserId { get; set; }

        /// <summary>目前使用者的喜好清單。</summary>
        public IEnumerable<PreferenceListItemDto> Items { get; set; } = Enumerable.Empty<PreferenceListItemDto>();
    }
}
