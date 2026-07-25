using Microsoft.Extensions.DependencyInjection;

namespace ESun.Data
{
    /// <summary>
    /// 資料層的 DI 註冊。由本層自行提供，讓上層不需認識具體實作類別。
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services)
        {
            // Scoped = 每個 HTTP 請求一個實例
            services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();
            services.AddScoped<IPreferenceRepository, PreferenceRepository>();
            return services;
        }
    }
}
