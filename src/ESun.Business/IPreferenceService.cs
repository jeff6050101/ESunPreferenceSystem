using ESun.Common.Dtos;
using ESun.Common.Entities;

namespace ESun.Business
{
    /// <summary>
    /// 喜好金融商品業務服務。
    /// 負責商業規則與計算（手續費、總金額），並協調資料層完成交易。
    /// 展示層只依賴此介面，不直接接觸資料層。
    /// </summary>
    public interface IPreferenceService
    {
        /// <summary>取得所有使用者（供下拉選單）。</summary>
        Task<IEnumerable<User>> GetUsersAsync();

        /// <summary>查詢指定使用者的喜好清單【需求 2】。</summary>
        Task<IEnumerable<PreferenceListItemDto>> GetListAsync(string userID);

        /// <summary>取得單筆（供編輯畫面帶值）。查無或不屬於此使用者時回傳 null。</summary>
        Task<PreferenceListItemDto?> GetByIdAsync(int sn, string userID);

        /// <summary>
        /// 新增喜好【需求 1】。
        /// 由業務層計算 TotalFee / TotalAmount 後再交給資料層寫入。
        /// </summary>
        /// <returns>新建立的 SN。</returns>
        Task<int> AddAsync(PreferenceInputDto input);

        /// <summary>
        /// 更改喜好【需求 4】。
        /// </summary>
        /// <returns>影響筆數：1 = 成功，0 = 查無該筆或不屬於此使用者。</returns>
        Task<int> UpdateAsync(PreferenceInputDto input);

        /// <summary>刪除喜好【需求 3】。</summary>
        /// <returns>影響筆數：1 = 成功，0 = 查無該筆或不屬於此使用者。</returns>
        Task<int> DeleteAsync(int sn, string userID);
    }
}
