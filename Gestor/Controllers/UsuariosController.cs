using System;
using System.Threading.Tasks;
using Entity.Enums;
using Gestor.Extensions;
using Gestor.Interfaces;
using Gestor.ViewModel.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Gestor.Controllers
{
    [Authorize(Roles = Roles.Gestor)]
    public class UsuariosController : Controller
    {
        private readonly ILogger<UsuariosController> logger;
        private readonly IUsuarioService usuarioService;

        public UsuariosController(ILogger<UsuariosController> _logger, IUsuarioService _usuarioService) {
            logger = _logger;
            usuarioService = _usuarioService;
        }

        /// <summary>
        /// Lista de usuários filtrada e paginada
        /// </summary>
        /// <param name="pagina">Páginação atual. Mínimo 1</param>
        /// <param name="termoBusca">Termo que será buscado para trazer a lista. Vazio trás todos os usuários.</param>
        public async Task<IActionResult> Index(int pagina = 1, string termoBusca = "", int perfil = 0)
        {
            try {
                var lista = await usuarioService.BuscaPaginadaGestores(pagina, 30, termoBusca, perfil);
                return View(lista);
            } catch (Exception erro) {
                logger.LogError(erro.ToString());
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Abre formulário para cadastrar um novo usuário no sistema
        /// </summary>
        public async Task<IActionResult> Criar() {
            return View();
        }

        /// <summary>
        /// Recebe as informações do novo usuário e valida para salvar no banco de dados
        /// </summary>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Criar(CadastroUsuarioVM viewmodel) {
            if (ModelState.IsValid) {
                try {
                    await usuarioService.Cadastrar(viewmodel.Email, viewmodel.Nome, viewmodel.Documento, viewmodel.Telefone, viewmodel.Foto, PerfilUsuario.Gestor, viewmodel.Senha);
                    ViewBag.Ok = true;
                    ModelState.Clear();
                    return View();
                } catch (CustomException erro) {
                    ViewBag.Erro = erro.Message;
                } catch (Exception erro) {
                    logger.LogError(erro.ToString());
                    ViewBag.Erro = "Erro interno no servidor.";
                }
            }
            return View(viewmodel);
        }

        /// <summary>
        /// Abre formulário para editar um usuário existente no sistema
        /// </summary>
        /// <param name="id">Identificador único do usuário</param>
        public async Task<IActionResult> Editar(long id) {
            if (id > 0) {
                try {
                    var user = await usuarioService.BuscaPorId(id);
                    if (user == null)
                        return NotFound();
                    var viewmodel = new EditarUsuarioVM(user);
                    return View(viewmodel);
                } catch (Exception erro) {
                    logger.LogError(erro.ToString());
                    return RedirectToAction("Error", "Home");
                }
            }
            else {
                return BadRequest();
            }
        }

        /// <summary>
        /// Recebe as informações atualizadas do usuário e valida para salvar no banco de dados
        /// </summary>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Editar(EditarUsuarioVM viewmodel) {
            if (ModelState.IsValid) {
                try {
                    await usuarioService.Editar(viewmodel.Id, viewmodel.Nome, viewmodel.Documento, viewmodel.Senha, viewmodel.Telefone, viewmodel.Foto, viewmodel.Ativo);
                    if (viewmodel.Id == User.Identity.Id())
                        await usuarioService.RefreshSignIn(User.Identity.Id());

                    return RedirectToAction("Index");
                } catch (CustomException erro) {
                    ViewBag.Erro = erro.Message;
                } catch (Exception erro) {
                    logger.LogError(erro.ToString());
                    ViewBag.Erro = "Erro interno no servidor.";
                }
            }
            return View(viewmodel);
        }

    }
}