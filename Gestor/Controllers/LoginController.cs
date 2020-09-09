using System;
using System.Threading.Tasks;
using Gestor.Interfaces;
using Gestor.ViewModel.Login;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gestor.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginService loginService;
        private readonly ILogger<UsuariosController> logger;

        public LoginController(ILogger<UsuariosController> _logger, ILoginService _loginService) {
            logger = _logger;
            loginService = _loginService;
        }

        /// <summary>
        /// Get para a tela de Login.
        /// </summary>
        public async Task<IActionResult> Index() {
            return View();
        }

        /// <summary>
        /// Post para efetuar Login no sistema.
        /// </summary>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Index(LoginVM viewmodel) {
            try {
                await loginService.Login(viewmodel.Email, viewmodel.Senha, viewmodel.LembrarSe);
                return RedirectToAction("Index", "Home");
            } catch (CustomException e) {
                ViewBag.Erro = e.Message;
            } catch (Exception e) {
                logger.LogError(e.ToString());
                ViewBag.Erro = "Erro interno no servidor.";
            }
            viewmodel.Senha = "";
            return View(viewmodel);
        }

        /// <summary>
        /// Efetua Logout no sistema para um usuário logado.
        /// </summary>
        public async Task<IActionResult> Logout() {
            try {
                await loginService.Logout();
                return RedirectToAction("Index", "Login");
            } catch (Exception e) {
                logger.LogError(e.ToString());
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Get para a página de esqueci minha senha.
        /// </summary>
        public async Task<IActionResult> EsqueciMinhaSenha() {
            return View();
        }

        /// <summary>
        /// Post para gerar o token de nova senha e enviar por e-mail para o usuário.
        /// </summary>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> EsqueciMinhaSenha(EsqueciSenhaVM viewmodel) {
            if (ModelState.IsValid) {
                try {
                    await loginService.EsqueciMinhaSenha(viewmodel.Email);
                    return RedirectToAction("EnvioTokenSenha");
                } catch (CustomException e) {
                    ViewBag.Erro = e.Message;
                } catch (Exception e) {
                    logger.LogError(e.ToString());
                    ViewBag.Erro = "Erro interno no servidor.";
                }
            }
            return View(viewmodel);
        }

        /// <summary>
        /// Tela apenas informativa, para mostrar que o e-mail foi enviado para o usuário trocar a senha.
        /// </summary>
        public async Task<IActionResult> EnvioTokenSenha() {
            return View();
        }

        /// <summary>
        /// Tela que o usuário vai parar quando usa o link no e-mail de resetar senha.
        /// </summary>
        public async Task<IActionResult> ResetarSenha(string token) {
            try {
                var tokenValido = await loginService.ValidarToken(token);
                if (tokenValido) {
                    var viewmodel = new ResetarSenhaVM() { token = token };
                    return View(viewmodel);
                } else {
                    return RedirectToAction("TokenInvalido");
                }
            } catch (Exception e) {
                logger.LogError(e.ToString());
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Tela apenas informativa, para dizer ao usuário que o token perdeu a validade.
        /// </summary>
        public async Task<IActionResult> TokenInvalido() {
            return View();
        }

        /// <summary>
        /// Post para realmente resetar a senha do usuário utilizando o token enviado para o e-mail.
        /// </summary>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> ResetarSenha(ResetarSenhaVM viewmodel) {
            if (ModelState.IsValid) {
                try {
                    await loginService.TrocaSenha(viewmodel.token, viewmodel.Senha);
                    return RedirectToAction("SenhaResetada");
                } catch (CustomException e) {
                    ModelState.AddModelError("Erro", e.Message);
                } catch (Exception e) {
                    logger.LogError(e.ToString());
                    ViewBag.Erro = "Erro interno no servidor.";
                }
            }
            viewmodel.Senha = "";
            viewmodel.ConfirmaSenha = "";

            return View(viewmodel);
        }

        /// <summary>
        /// Tela apenas informativa para dizer ao usuário que a senha foi resetada e ele pode tentar efetuar o login novamente.
        /// </summary>
        public async Task<IActionResult> SenhaResetada() {
            return View();
        }
    }
}