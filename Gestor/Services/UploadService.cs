using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Gestor.Services
{
    public class UploadService
    {
        private readonly IWebHostEnvironment env;

        public UploadService(IWebHostEnvironment _env) {
            env = _env;
        }

        /// <summary>
        /// Faz o upload de um arquivo e salva no servidor retornando o caminho completo com o nome gerado para o arquivo.
        /// </summary>
        /// <param name="arquivo">Arquivo enviado pela rede.</param>
        /// <param name="destino">Pasta onde o arquivo será salvo.</param>
        public async Task<string> UploadArquivo(IFormFile arquivo, string destino) {
            if (arquivo == null)
                throw new CustomException("Arquivo inválido para upload.");

            var extensao = Path.GetExtension(arquivo.FileName);
            var nomeArquivo = Guid.NewGuid().ToString() + DateTime.Now.ToString("ddMMyyyy_HHmmss") + extensao;

            if (destino[0] != '/')
                destino = $"/{destino}";

            var directory = $"{env.WebRootPath}{destino}";
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (var fileStream = new FileStream($"{directory}/{nomeArquivo}", FileMode.Create))
                await arquivo.CopyToAsync(fileStream);

            return nomeArquivo;
        }
    }
}
