using ESun.Common.Dtos;
using ESun.Common.Entities;
using ESun.Data;

namespace ESun.Business
{
    /// <summary>
    /// 喜好金融商品業務服務實作。
    /// </summary>
    public class PreferenceService : IPreferenceService
    {
        private readonly IPreferenceRepository _repository;

        public PreferenceService(IPreferenceRepository repository)
        {
            _repository = repository;
        }

        // ===================================================================
        // 查詢類：無商業邏輯，直接轉發給資料層
        // ===================================================================
        public Task<IEnumerable<User>> GetUsersAsync()
            => _repository.GetUsersAsync();

        public Task<IEnumerable<PreferenceListItemDto>> GetListAsync(string userID)
            => _repository.GetListAsync(userID);

        public Task<PreferenceListItemDto?> GetByIdAsync(int sn, string userID)
            => _repository.GetByIdAsync(sn, userID);

        public Task<int> DeleteAsync(int sn, string userID)
            => _repository.DeleteAsync(sn, userID);


        // ===================================================================
        // 計算：純函式，不碰資料庫 → 可獨立單元測試
        // ===================================================================
        /// <summary>
        /// 計算總手續費與預計扣款總金額。
        ///   TotalFee    = Price × Quantity × FeeRate
        ///   TotalAmount = Price × Quantity + TotalFee
        /// 手續費四捨五入至小數 4 位（對應 DECIMAL(18,4)）。
        /// </summary>
        public static (decimal totalFee, decimal totalAmount) Calculate(
            decimal price, int quantity, decimal feeRate)
        {
            // TODO（你的部分）：
            // 1. 算出小計 subtotal
            // 2. 算出 totalFee，用 Math.Round(..., 4, MidpointRounding.AwayFromZero)
            // 3. 算出 totalAmount
            // 4. return (totalFee, totalAmount);
            decimal subtotal = price * quantity;
            decimal totalFee = Math.Round(subtotal * feeRate, 4, MidpointRounding.AwayFromZero);
            decimal totalAmount = subtotal + totalFee;
            return (totalFee, totalAmount);
        }


        // ===================================================================
        // 異動類：先計算，再交給資料層（資料層負責 Transaction）
        // ===================================================================
        public Task<int> AddAsync(PreferenceInputDto input)
        {
            // TODO（你的部分）：
            // 1. var (fee, amount) = Calculate(input.Price, input.PurchaseQuantity, input.FeeRate);
            // 2. return _repository.AddAsync(input, fee, amount);
            var (fee, amount) = Calculate(input.Price, input.PurchaseQuantity, input.FeeRate);
            return _repository.AddAsync(input, fee, amount);
        }

        public Task<int> UpdateAsync(PreferenceInputDto input)
        {
            // TODO（你的部分）：同 AddAsync，但呼叫 _repository.UpdateAsync
            var (fee, amount) = Calculate(input.Price, input.PurchaseQuantity, input.FeeRate);
            return _repository.UpdateAsync(input, fee, amount);
        }
    }
}
