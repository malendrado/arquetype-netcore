using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Backend.Data.Helpers
{
    public static class AttributeReader
    {
        //Get DB Table Name
        public static string GetTableName(DbContext context, string tableName)
        {
            // We need dbcontext to access the models
            var models = context.Model;

            // Get all the entity types information
            var entityTypes = models.GetEntityTypes();

            // T is Name of class
            var entityTypeOfT = entityTypes.First(t => t.ClrType.Name == tableName);

            var tableNameAnnotation = entityTypeOfT.GetAnnotation("Relational:TableName");
            var TableName = tableNameAnnotation.Value.ToString();
            return TableName;
        }
    }
}