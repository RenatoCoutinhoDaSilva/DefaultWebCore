using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gestor.Interfaces
{
    public interface IEmailService
    {
        string Body { get; set; }
        bool IsHTML { get; set; }
        bool EnableDebug { get; set; }
        string SenderName { get; set; }
        string SenderEmail { get; set; }

        ///<summary>
        /// Faz a substituição de string no meio do corpo especificado.
        ///</summary>
        /// <param name="key">Representa o valor que será substituído.</param>
        /// <param name="value">Representa o valor que será inserido.</param>
        void AddBodyProperty(string key, string value);

        ///<summary>
        /// Adiciona um anexo no email.
        ///</summary>
        /// <param name="path">Caminho do arquivo no servidor.</param>
        void AddAttachment(string path);

        /// <summary>
        /// Envia o e-mail para uma lista de destinatários um a um.
        /// </summary>
        /// <param name="destinyList">Lista de destinatários.</param>
        /// <param name="senderEmail">Endereço de e-mail do remetente.</param>
        /// <param name="senderName">Nome do remetente.</param>
        /// <param name="subject">Assunto do e-mail.</param>
        Task SendToList(List<string> destinyList, string subject);

        /// <summary>
        /// Envia o e-mail para um destinatário apenas.
        /// </summary>
        /// <param name="destiny">Endereço de e-mail do destinatário.</param>
        /// <param name="senderEmail">Endereço de e-mail do remetente.</param>
        /// <param name="senderName">Nome do remetente.</param>
        /// <param name="subject">Assunto do e-mail.</param>
        Task Send(string destiny, string subject);
    }
}
