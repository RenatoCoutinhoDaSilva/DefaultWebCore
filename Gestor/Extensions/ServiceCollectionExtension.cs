using Entity;
using Entity.Models;
using Gestor.Interfaces;
using Gestor.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Gestor.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static void InjetarDependencias(this IServiceCollection services) {
            services.AddTransient<UserManager<Usuario>>();
            services.AddTransient<ApplicationDbContext>();
            services.AddTransient<SignInManager<Usuario>>();

            services.AddTransient<IUsuarioService, UsuariosService>();
            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<IEmailService, EmailService>();
        }
    }
}
