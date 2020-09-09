using Entity;
using Entity.Enums;
using Entity.Models;
using Gestor.Extensions;
using Gestor.Interfaces;
using Gestor.ViewModel.Usuario;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using X.PagedList;

namespace Gestor.Services
{
    public class UsuariosService : IUsuarioService
    {
        private readonly ApplicationDbContext db;
        private readonly SignInManager<Usuario> signinManager;
        private readonly UserManager<Usuario> userManager;
        private readonly IEmailService emailService;
        private readonly IWebHostEnvironment env;

        public UsuariosService(ApplicationDbContext _db, UserManager<Usuario> _userManager, IEmailService _emailService, IWebHostEnvironment _env, SignInManager<Usuario> _signinManager) {
            db = _db;
            userManager = _userManager;
            emailService = _emailService;
            env = _env;
            signinManager = _signinManager;
        }

        public async Task<IPagedList<ItemListaUsuarioVM>> BuscaPaginadaGestores(int pagina, int tamanho, string termoBusca, int perfil) {
            if (pagina < 1)
                pagina = 1;
            if (tamanho < 1)
                tamanho = 30;

            var query = db.Usuarios.AsQueryable();

            if (perfil > 0) {
                query = query.Where(u => u.Perfil == (PerfilUsuario)perfil);
            }

            if (!string.IsNullOrEmpty(termoBusca)) {
                var termoNormalizado = termoBusca.ToLower();
                query = query.Where(u => u.Nome.ToLower().Contains(termoNormalizado) || u.Email.Contains(termoNormalizado)).AsQueryable();
            }

            return await query.Select(u => new ItemListaUsuarioVM(u)).ToPagedListAsync(pagina, tamanho);
        }

        public async Task<List<ItemListaUsuarioVM>> BuscaLista(int tamanho, string termoBusca, PerfilUsuario? perfil = null) {
            if (tamanho < 1)
                tamanho = 30;

            var query = db.Usuarios.AsQueryable(); ;

            if (perfil.HasValue && perfil.Value > 0) {
                query = query.Where(u => u.Perfil == perfil.Value).AsQueryable();
            }

            if (!string.IsNullOrEmpty(termoBusca)) {
                var termoNormalizado = termoBusca.ToLower();
                query = query.Where(u => u.Nome.ToLower().Contains(termoNormalizado) || u.Email.Contains(termoNormalizado)).AsQueryable();
            }

            return await query.Select(u => new ItemListaUsuarioVM(u)).Take(tamanho).ToListAsync();
        }

        public async Task TrocaStatus(long id, bool status) {
            var user = await db.Usuarios.Where(u => u.Id == id).FirstOrDefaultAsync();
            user.Ativo = status;
            db.Entry(user).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }

        public async Task ExcluirUsuario(long id) {
            var user = await userManager.FindByIdAsync(id.ToString());
            await userManager.DeleteAsync(user);
        }

        public async Task<Usuario> BuscaPorId(long id) {
            return await db.Usuarios.Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Usuario> BuscaPorEmail(string email) {
            var emailNormalized = email.ToLower();
            return await db.Usuarios.Where(u => u.UserName == emailNormalized).FirstOrDefaultAsync();
        }

        public async Task Editar(long id, string nome, string documento, string novaSenha, string telefone, IFormFile foto, bool status) {
            var user = await userManager.FindByIdAsync(id.ToString());
            if (user == null)
                throw new CustomException("Usuário não encontrado.");

            user.Nome = nome;
            user.Documento = documento;
            user.Telefone = telefone;
            user.Ativo = status;

            if (foto != null) {
                var uploadService = new UploadService(env);
                user.Foto = await uploadService.UploadArquivo(foto, Usuario.CaminhoArquivo);
            }

            await userManager.UpdateAsync(user);

            if (!string.IsNullOrEmpty(novaSenha)) {
                await userManager.RemovePasswordAsync(user);
                await userManager.AddPasswordAsync(user, novaSenha);
            }

            await AtualizaClaims(user);
        }

        public async Task<long> Cadastrar(string email, string nome, string documento, string telefone, IFormFile foto, PerfilUsuario perfil, string senha, bool ativo = true) {

            var user = new Usuario {
                DataCadastro = DateTime.Now,
                Email = email.ToLower(),
                UserName = email.ToLower(),
                EmailConfirmed = true,
                Nome = nome,
                NormalizedEmail = email.ToUpper(),
                NormalizedUserName = email.ToUpper(),
                Ativo = ativo,
                Perfil = perfil,
                Documento = documento,
                Telefone = telefone
            };

            if (foto != null) {
                var uploadService = new UploadService(env);
                user.Foto = await uploadService.UploadArquivo(foto, Usuario.CaminhoArquivo);
            }

            var result = await userManager.CreateAsync(user, senha);

            if (result.Errors.Any() && result.Errors.FirstOrDefault().Code == "DuplicateUserName")
                throw new CustomException("E-mail já cadastrado no sistema.");

            if (!result.Succeeded)
                throw new CustomException(result.Errors.Select(e => e.Description).FirstOrDefault());

            await AdicionaRole(user, perfil);
            await EnviarEmail(user.Email, nome);

            await CriarClaims(user);

            return user.Id;
        }

        public async Task RefreshSignIn(long id) {
            var user = await userManager.FindByIdAsync(id.ToString());
            await signinManager.RefreshSignInAsync(user);
        }

        private async Task CriarClaims(Usuario user) {
            await userManager.AddClaimAsync(user, new Claim(ClaimTypes.Sid, user.Id.ToString()));
            await userManager.AddClaimAsync(user, new Claim("Nome", user.Nome));
            await userManager.AddClaimAsync(user, new Claim("Perfil", user.Perfil.GetDisplayName()));
            
            if (!string.IsNullOrEmpty(user.Foto))
                await userManager.AddClaimAsync(user, new Claim("Foto", user.Foto));
        }

        public async Task AtualizaClaims(Usuario user) {
            var claims = await userManager.GetClaimsAsync(user);

            var id = claims.Where(cl => cl.Type == ClaimTypes.Sid).FirstOrDefault();
            if (id != null && id.Value != user.Id.ToString())
                await userManager.ReplaceClaimAsync(user, claims.FirstOrDefault(cl => cl.Type == ClaimTypes.Sid), new Claim(ClaimTypes.Sid, user.Id.ToString()));

            var perfil = claims.Where(cl => cl.Type == "Perfil").FirstOrDefault();
            if (perfil != null && perfil.Value != user.Perfil.GetDisplayName())
                await userManager.ReplaceClaimAsync(user, claims.FirstOrDefault(cl => cl.Type == "Perfil"), new Claim("Perfil", user.Perfil.GetDisplayName()));

            var nome = claims.Where(cl => cl.Type == "Nome").FirstOrDefault();
            if (nome != null && nome.Value != user.Nome)
                await userManager.ReplaceClaimAsync(user, claims.FirstOrDefault(cl => cl.Type == "Nome"), new Claim("Nome", user.Nome));

            var foto = claims.Where(cl => cl.Type == "Foto").FirstOrDefault();
            if ((foto != null && foto.Value != user.Foto) && !string.IsNullOrEmpty(user.Foto))
                await userManager.ReplaceClaimAsync(user, claims.FirstOrDefault(cl => cl.Type == "Foto"), new Claim("Foto", user.Foto));
        }

        private async Task EnviarEmail(string email, string nome) {
            var directory = $"{env.WebRootPath}";
            emailService.Body = File.ReadAllText($"{directory}/ModelosEmail/InicioEmailNovoUsuario.html");
            emailService.IsHTML = true;
            emailService.AddBodyProperty("##nome##", nome);
            emailService.AddBodyProperty("##email##", email);
            await emailService.Send(email, "Confirmação de cadastro");
        }

        public async Task AdicionaRole(Usuario user, PerfilUsuario perfil) {
            switch (perfil) {
                case PerfilUsuario.Gestor:
                    await userManager.AddToRoleAsync(user, Roles.Gestor);
                    break;
            };
        }

        public async Task RemoveRole(Usuario user, PerfilUsuario perfil) {
            switch (perfil) {
                case PerfilUsuario.Gestor:
                    await userManager.AddToRoleAsync(user, Roles.Gestor);
                    break;
            };
        }

        public async Task<bool> TrocaSenha(string username, string senhaAtual, string novaSenha) {
            var user = await userManager.FindByNameAsync(username);

            if (!string.IsNullOrEmpty(novaSenha)) {
                var result = await userManager.ChangePasswordAsync(user, senhaAtual, novaSenha);
                return result.Succeeded;
            }
            return false;
        } 
    }
}
