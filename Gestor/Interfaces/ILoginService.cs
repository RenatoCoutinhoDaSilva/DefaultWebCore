using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gestor.Interfaces
{
    public interface ILoginService
    {
        /// <summary>
        /// Efetua o login do usuário no Identity, ou seja, cria o Cookie e a sessão do usuário.
        /// </summary>
        /// <param name="usuario">E-mail do usuário.</param>
        /// <param name="lembrarSe">Aumenta a vida útil do Cookie para que o usuário fique logado direto.</param>
        Task Login(string usuario, string senha, bool lembrarSe);

        /// <summary>
        /// Remove o Cookie do usuário e apaga a sessão, ou seja, desloga ele.
        /// </summary>
        Task Logout();

        /// <summary>
        /// Gera um token e envia por e-mail para o usuário alterar a senha.
        /// </summary>
        /// <param name="email">E-mail usado para efetuar login no sistema.</param>
        Task<string> EsqueciMinhaSenha(string email);

        /// <summary>
        /// Utiliza um token previamente criado para alterar a senha do usuário por uma que ele queira.
        /// </summary>
        /// <param name="token">Token gerado no método EsqueciMinhaSenha.</param>
        Task TrocaSenha(string token, string novaSenha);

        /// <summary>
        /// Verifica se o Token é válido ou não.
        /// </summary>
        Task<bool> ValidarToken(string token);
    }
}
