using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entity.Enums;
using Entity.Extensions;
using Entity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Entity
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser<long>, IdentityRole<long>, long, IdentityUserClaim<long>,
        IdentityUserRole<long>, IdentityUserLogin<long>, IdentityRoleClaim<long>, IdentityUserToken<long>>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<TokenSenha> TokensDeSenha { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.RemovePluralizingTableNameConvention();

            const long USER_ID = 1;
            const long ROLE_ID = 1;

            modelBuilder.Entity<IdentityRole<long>>().HasData(
                new IdentityRole<long> { 
                    Id = ROLE_ID,
                    Name = Enums.Roles.Gestor, 
                    NormalizedName = Enums.Roles.Gestor.ToUpper() 
                }
            );

            var hasher = new PasswordHasher<Usuario>();
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario {
                    Id = USER_ID,
                    DataCadastro = DateTime.Now,
                    Email = "admin@admin.com.br",
                    UserName = "admin@admin.com.br",
                    EmailConfirmed = true,
                    Nome = "Admin",
                    NormalizedEmail = "ADMIN@ADMIN.COM.BR",
                    NormalizedUserName = "ADMIN@ADMIN.COM.BR",
                    Ativo = true,
                    Perfil = PerfilUsuario.Gestor,
                    PasswordHash = hasher.HashPassword(null, "123@Password"),
                    SecurityStamp = string.Empty
                }
            );

            modelBuilder.Entity<IdentityUserRole<long>>().HasData(
                new IdentityUserRole<long> {
                    RoleId = ROLE_ID,
                    UserId = USER_ID
                }
            );


            base.OnModelCreating(modelBuilder);
        }

        public async override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken)) {
            foreach (var entry in ChangeTracker.Entries().Where(entry => entry.Entity.GetType().GetProperty("DataCadastro") != null)) {
                if (entry.State == EntityState.Added) {
                    entry.Property("DataCadastro").CurrentValue = DateTime.Now;
                }

                if (entry.State == EntityState.Modified) {
                    entry.Property("DataCadastro").IsModified = false;

                    if (entry.Properties.Select(x => x.Metadata.Name).Contains("DataAtualizacao"))
                        entry.Property("DataAtualizacao").CurrentValue = DateTime.Now;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Efetua a exclusão lógica do usuário no banco de dados.
        /// </summary>
        /// <param name="entry">Entidade a ser deletada. Ex.: Use 'db.Entry(model)' para pegar a entidade.</param>
        public async Task SoftDeleteAsync(EntityEntry entry) {
            string tableName = GetTableName(entry);

            string sql =
                string.Format(
                    "UPDATE {0} SET Excluido = 1, DataAtualizacao = '{1}' WHERE Id = ",
                        tableName, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            var connection = Database.GetDbConnection();
            connection.Open();

            using (var command = connection.CreateCommand()) {
                command.CommandText = sql + entry.OriginalValues["Id"];
                await command.ExecuteNonQueryAsync();
            }

            connection.Close();
        }

        /// <summary>
        /// Remove de fez a entidade do banco de dados, não há recuperação.
        /// </summary>
        /// <param name="entry">Entidade a ser deletada. Ex.: Use 'db.Entry(model)' para pegar a entidade.</param>
        public async Task HardDeleteAsync(EntityEntry entry) {
            entry.State = EntityState.Deleted;
            await base.SaveChangesAsync();
        }

        private string GetTableName(EntityEntry ent) {
            Type entityType = ent.Entity.GetType();

            if (entityType.BaseType != null && (entityType.Namespace == "System.Data.Entity.DynamicProxies" || entityType.Namespace == "Castle.Proxies"))
                entityType = entityType.BaseType;

            return entityType.Name;
        }
    }
}
