namespace Gestor.ViewModel.Usuario
{
    public class DetalhesUsuarioVM
    {
        public DetalhesUsuarioVM(Entity.Models.Usuario model) {
            Id = model.Id.ToString();
            Nome = model.Nome;
            Email = model.Email;
            CPF = model.Documento;
            Perfil = model.Perfil.ToString();
            DataCadastro = model.DataCadastro.ToString("dd/MM/yyyy");
            AtivoString = model.Ativo ? "Sim" : "Não";
            AtivoClass = model.Ativo ? "badge-success" : "badge-danger";
        }

        public string Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string CPF { get; set; }
        public string Perfil { get; set; }
        public string DataCadastro { get; set; }
        public string AtivoString { get; set; }
        public string AtivoClass { get; set; }
    }
}
