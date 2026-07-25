using ESun.Data;
using Microsoft.Extensions.DependencyInjection;

namespace ESun.Business
{
    /// <summary>
    /// 業務層的 DI 註冊。內部一併帶入資料層，
    /// 使展示層（Web）只需呼叫 AddBusinessLayer()，
    /// 從頭到尾不需直接參考 ESun.Data —— 維持分層邊界。
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
        {
            services.AddDataLayer();                                    // 帶入資料層
            services.AddScoped<IPreferenceService, PreferenceService>();
            return services;
        }
    }
}
