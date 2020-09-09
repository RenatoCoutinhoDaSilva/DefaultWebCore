using Entity.Enums;
using Entity.Models;
using Gestor.ViewModel.Usuario;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using X.PagedList;

namespace Gestor.Interfaces
{
    public interface IUsuarioService
    {
        /// <summary>
        /// Busca uma lista paginada de usuários com o perfil de gestor filtrando pelo termo digitado.
        /// </summary>
        /// <param name="pagina">Páginação atual da lista. Mínimo: 1.</param>
        /// <param name="tamanho">Tamanho da lista paginada. Padrão: 30.</param>
        /// <param name="termoBusca">Termo usado no filtro da lista. Vazio trás todos os usuários.</param>
        Task<IPagedList<ItemListaUsuarioVM>> BuscaPaginadaGestores(int pagina, int tamanho, string termoBusca, int perfil);

        /// <summary>
        /// Busca uma lista comum de usuários filtrada pelo termo digitado.
        /// </summary>
        /// <param name="tamanho">Tamanho da lista paginada. Padrão: 30.</param>
        /// <param name="termoBusca">Termo usado no filtro da lista. Vazio trás todos os usuários.</param>
        Task<List<ItemListaUsuarioVM>> BuscaLista(int tamanho, string termoBusca, PerfilUsuario? perfil = null);

        /// <summary>
        /// Troca o status de Ativo do usuário.
        /// </summary>
        /// <param name="id">Identificador único do usuário.</param>
        /// <param name="status">'true' para desbloquear e 'false' para bloquear.</param>
        Task TrocaStatus(long id, bool status);

        /// <summary>
        /// Deleta o usuário da base de dados.
        /// </summary>
        /// <param name="id">Identificador único do usuário.</param>
        Task ExcluirUsuario(long id);

        /// <summary>
        /// Busca o usuário pelo Id no Banco de Dados.
        /// </summary>
        /// <param name="id">Identificador único do usuário.</param>
        Task<Usuario> BuscaPorId(long id);

        /// <summary>
        /// Busca o usuário pelo Id no Banco de Dados.
        /// </summary>
        /// <param name="email">Email do usuário.</param>
        Task<Usuario> BuscaPorEmail(string email);

        /// <summary>
        /// Altera nome e perfil do usuário.
        /// </summary>
        /// <param name="id">Identificador Único do usuário.</param>
        /// <param name="nome">Novo nome para o usuário.</param>
        /// <param name="perfil">Novo perfil para o usuário.</param>
        Task Editar(long id, string nome, string documento, string novaSenha, string telefone, IFormFile foto, bool status);

        /// <summary>
        /// Cadastra um novo usuário no banco de dados.
        /// </summary>
        /// <param name="email">Email do usuário. Deve ser único no banco de dados.</param>
        /// <param name="nome">Nome do usuário para fácil identificação.</param>
        /// <param name="perfil">Perfil de acessos do usuário.</param>
        Task<long> Cadastrar(string email, string nome, string documento, string telefone, IFormFile foto, PerfilUsuario perfil, string senha, bool ativo = true);

        /// <summary>
        /// Atualiza os dados do usuário na memória e o Cookie.
        /// </summary>
        /// <param name="id">Identificador único do usuário.</param>
        Task RefreshSignIn(long id);

        /// <summary>
        /// Atualiza as Claims do usuário no Banco de Dados.
        /// </summary>
        /// <param name="user">Usuário que será atualizado.</param>
        Task AtualizaClaims(Usuario user);

        /// <summary>
        /// Adiciona uma função específica no usuário caso ele ainda não tenha.
        /// </summary>
        /// <param name="user">Usuário a ser alterado.</param>
        /// <param name="perfil">Novo perfil do usuário.</param>
        Task AdicionaRole(Usuario user, PerfilUsuario perfil);

        /// <summary>
        /// Remove uma função específica no usuário caso ele tenha.
        /// </summary>
        /// <param name="user">Usuário a ser alterado.</param>
        /// <param name="perfil">Perfil a ser removido do usuário.</param>
        Task RemoveRole(Usuario user, PerfilUsuario perfil);

        /// <summary>
        /// Troca a senha do usuário utilizando a senha atual.
        /// </summary>
        /// <param name="username">Nome de usuário.</param>
        /// <param name="senhaAntiga">Senha atual.</param>
        /// <param name="novaSenha">Senha nova.</param>
        Task<bool> TrocaSenha(string username, string senhaAtual, string novaSenha);
    }
}
