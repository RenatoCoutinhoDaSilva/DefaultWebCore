using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Entity.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void RemovePluralizingTableNameConvention(this ModelBuilder modelBuilder) {
            foreach (IMutableEntityType entity in modelBuilder.Model.GetEntityTypes()) {
                if (entity.DisplayName() != "Usuario")
                    entity.SetTableName(entity.DisplayName());
            }
        }
    }
}
