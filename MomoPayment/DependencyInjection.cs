using Repositories;
using Repositories.Implements;
using Repositories.Interfaces;
using Repositories.Models;
using Services.Interfaces;
using Services.Services;
using WebAPI.Middlewares;

namespace WebAPI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(ExagenContext));
            services.AddHttpContextAccessor();

            #region Repositories
            services.AddScoped<ITransactionRepo, TransactionRepo>();
            services.AddScoped<IUserRepo, UserRepo>();
            #endregion

            #region Services
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITransactionServices, TransactionServices>();
            services.AddScoped<IMomoServices, MomoServices>();
            #endregion
           
            services.AddSingleton<GlobalExceptionMiddleware>();
            services.AddRouting(opt => opt.LowercaseUrls = true);

            return services;

        }
    }
}
