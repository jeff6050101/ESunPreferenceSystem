using System.ComponentModel.DataAnnotations;

namespace ESun.Web.Models
{
    /// <summary>
    /// 新增／編輯喜好金融商品的表單檢視模型。
    /// 驗證規則以 DataAnnotations 宣告，MVC 會在 Model Binding 後自動套用，
    /// 結果放進 ModelState（伺服器端驗證，無法被前端繞過）。
    /// </summary>
    public class PreferenceFormViewModel
    {
        /// <summary>流水序號。新增時為 null；編輯時有值（隱藏欄位帶入）。</summary>
        public int? SN { get; set; }

        /// <summary>使用者 ID。來自 Session 的目前使用者（隱藏欄位帶入）。</summary>
        [Required]
        public string UserID { get; set; } = string.Empty;

        // ===== 範例（已完成）：照這個模式替下面的欄位加上驗證 =====
        [Required(ErrorMessage = "請輸入產品名稱")]
        [StringLength(100, ErrorMessage = "產品名稱不可超過 100 字")]
        [Display(Name = "產品名稱")]
        public string ProductName { get; set; } = string.Empty;

        [Required]
        [Range(0, 100000000, ErrorMessage = "產品價格須介於 0 與 1 億之間")]
        [Display(Name = "產品價格")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 1, ErrorMessage = "手續費率必須在 0 ~ 1 之間（0.03 = 3%）")]
        [Display(Name = "手續費率")]
        public decimal FeeRate { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "購買數量至少為 1")]
        [Display(Name = "購買數量")]
        public int PurchaseQuantity { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "帳號不可超過 20 字")]
        [Display(Name = "扣款帳號")]
        public string Account { get; set; } = string.Empty;
    }
}
