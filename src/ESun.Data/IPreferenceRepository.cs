using ESun.Common.Dtos;
using ESun.Common.Entities;

namespace ESun.Data
{
    /// <summary>
    /// 喜好金融商品資料存取介面。
    /// 一律透過 Stored Procedure 存取資料庫，本層不得出現任何 inline SQL。
    ///
    /// 【資料範圍限制】
    /// 每個方法都必須傳入 userID，SP 的 WHERE 條件一併比對 UserID，
    /// 使查詢與異動在【資料層】即被限制於該使用者範圍內。
    /// 如此即使呼叫端傳入他人的 SN，也只會得到空結果或 0 筆影響，
    /// 而非仰賴應用層事後判斷（避免遺漏某個入口造成 IDOR 越權）。
    /// </summary>
    public interface IPreferenceRepository
    {
        /// <summary>取得所有使用者（供下拉選單）。SP：usp_User_GetList</summary>
        Task<IEnumerable<User>> GetUsersAsync();

        /// <summary>
        /// 查詢指定使用者的喜好清單【需求 2】。SP：usp_Preference_GetList
        /// </summary>
        Task<IEnumerable<PreferenceListItemDto>> GetListAsync(string userID);

        /// <summary>取得單筆（供編輯畫面帶值）。SP：usp_Preference_GetById</summary>
        /// <returns>查無資料，或該筆不屬於此使用者時，回傳 null。</returns>
        Task<PreferenceListItemDto?> GetByIdAsync(int sn, string userID);

        /// <summary>
        /// 新增喜好【需求 1】。SP：usp_Preference_Add
        /// 於單一交易內同時寫入 Product 與 LikeList。
        /// </summary>
        /// <returns>新建立的 LikeList.SN。</returns>
        Task<int> AddAsync(PreferenceInputDto input, decimal totalFee, decimal totalAmount);

        /// <summary>
        /// 更改喜好【需求 4】。SP：usp_Preference_Update
        /// 於單一交易內同時更新 Product 與 LikeList。
        /// </summary>
        /// <returns>影響筆數：1 = 成功，0 = 查無該筆或該筆不屬於此使用者。</returns>
        Task<int> UpdateAsync(PreferenceInputDto input, decimal totalFee, decimal totalAmount);

        /// <summary>
        /// 刪除喜好【需求 3】。SP：usp_Preference_Delete
        /// 於單一交易內同時刪除 LikeList 與 Product。
        /// </summary>
        /// <returns>影響筆數：1 = 成功，0 = 查無該筆或該筆不屬於此使用者。</returns>
        Task<int> DeleteAsync(int sn, string userID);
    }
}
