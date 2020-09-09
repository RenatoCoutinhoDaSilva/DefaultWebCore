namespace Gestor.ViewModel.Usuario
{
    public class ItemListaUsuarioVM
    {
        public ItemListaUsuarioVM(Entity.Models.Usuario model) {
            Id = model.Id;
            Nome = model.Nome;
            Email = model.Email;
            Perfil = model.Perfil.ToString();
            Ativo = model.Ativo;
            AtivoString = model.Ativo ? "Sim" : "Não";
            AtivoClass = model.Ativo ? "badge-success" : "badge-danger";
        }

        public long Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Perfil { get; set; }
        public string AtivoString { get; set; }
        public bool Ativo { get; set; }
        public string AtivoClass { get; set; }
    }
}
