using Entity;
using Entity.Models;
using Gestor.Extensions;
using Gestor.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Gestor.Services
{
    public class LoginService : ILoginService
    {
        private readonly SignInManager<Usuario> signInManager;
        private readonly UserManager<Usuario> userManager;
        private readonly ApplicationDbContext db;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment env;
        private readonly IEmailService emailService;
        private readonly IUsuarioService usuarioService;

        public LoginService(SignInManager<Usuario> _signInManager, UserManager<Usuario> _userManager, ApplicationDbContext _db, IConfiguration _configuration, IWebHostEnvironment _env, IEmailService _emailService, IUsuarioService _usuarioService) {
            signInManager = _signInManager;
            userManager = _userManager;
            db = _db;
            configuration = _configuration;
            env = _env;
            emailService = _emailService;
            usuarioService = _usuarioService;
        }

        public async Task Login(string usuario, string senha, bool lembrarSe) {
            if (!usuario.IsEmail()) {
                throw new CustomException("E-mail inválido.");
            }
            var user = await userManager.FindByNameAsync(usuario);

            if (user == null)
                throw new CustomException("E-mail não cadastrado.");

            if (!user.Ativo)
                throw new CustomException("Usuário inativo.");

            var result = await signInManager.PasswordSignInAsync(usuario, senha, lembrarSe, false);
            if (!result.Succeeded) {
                throw new CustomException("Usuário ou senha inválidos.");
            }

            await usuarioService.AtualizaClaims(user);
        }

        public async Task Logout() {
            await signInManager.SignOutAsync();
        }

        public async Task<string> EsqueciMinhaSenha(string email) {
            var usuario = await userManager.FindByNameAsync(email);
            if (usuario == null)
                throw new CustomException("E-mail não cadastrado.");
            
            var validadeEmDias = configuration.GetValue<int>("ValidadeTokenSenha");
            var token = new TokenSenha(usuario.Email, validadeEmDias);

            db.TokensDeSenha.Add(token);
            await db.SaveChangesAsync();

            await EnviarEmailSenha(email, token.Token, usuario.Nome);

            return token.Token;
        }

        /// <summary>
        /// Envia um Email com token para o destinatário trocar a senha.
        /// </summary>
        /// <param name="email">Destinatário.</param>
        /// <param name="token">Token válido gerado.</param>
        /// <param name="nome">Nome do destinatário.</param>
        private async Task EnviarEmailSenha(string email, string token, string nome) {
            var directory = $"{env.WebRootPath}";
            emailService.Body = File.ReadAllText($"{directory}/Conteudo/ModelosEmail/AcessoEmail.html");
            emailService.IsHTML = true;
            emailService.AddBodyProperty("##nome##", nome);
            emailService.AddBodyProperty("##email##", email);
            emailService.AddBodyProperty("##token##", token);
            await emailService.Send(email, "Confirmação de cadastro");
        }

        public async Task TrocaSenha(string token, string novaSenha) {
            var tokenNormalizado = token.ToLower();
            var tokenSenha = await db.TokensDeSenha.Where(t => t.Token == tokenNormalizado).FirstOrDefaultAsync();

            if (!tokenSenha.EstaValido())
                throw new CustomException("Token inválido.");

            var usuario = await userManager.FindByNameAsync(tokenSenha.UsuarioEmail);
            if (usuario == null)
                throw new CustomException("Email inválido.");

            await userManager.RemovePasswordAsync(usuario);
            await userManager.AddPasswordAsync(usuario, novaSenha);
        }
        
        public async Task<bool> ValidarToken(string token) {
            var tokenNormalizado = token.ToLower();
            var tokenSenha = await db.TokensDeSenha.Where(t => t.Token == tokenNormalizado).FirstOrDefaultAsync();

            return tokenSenha.EstaValido();
        }
    }
}
