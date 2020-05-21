using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StephenZeng.Prototypes.DisasterRecovery.Common;
using StephenZeng.Prototypes.DisasterRecovery.Dal.Interfaces;
using StephenZeng.Prototypes.DisasterRecovery.Domain;

namespace StephenZeng.Prototypes.DisasterRecovery.Dal.Repositories
{
    public class Repository : IRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger _logger;

        public Repository(ApplicationDbContext dbContext, ILoggerFactory loggerFactory)
        {
            _dbContext = dbContext;
            _logger = loggerFactory.CreateLogger(GetType().Name);
        }

        public virtual void Add(EntityBase entity)
        {
            var entityName = entity.GetType().Name;
            var properties = new List<PropertyInfo>();
            var ownedProperties = new List<PropertyInfo>();

            foreach (var property in entity.GetType().GetProperties())
            {
                if (IsSimpleType(property.PropertyType))
                {
                    properties.Add(property);
                }
                else
                {
                    if (property.PropertyType.GetCustomAttributes<OwnedAttribute>().Any())
                    {
                        ownedProperties.Add(property);
                    }
                }
            }

            AddPrincipalEntity(entityName, properties, entity);

            foreach (var p in ownedProperties)
            {
                var propertyValue = p.GetValue(entity);
                if (propertyValue == null) continue;

                AddOwnedEntity($"{entityName}{p.Name}",
                    $"{entityName}Id",
                    entity.Id,
                    p.PropertyType,
                    propertyValue);
            }
        }

        public virtual void Update(EntityBase entity, params string[] updatedPropertyNames)
        {
            var entityName = entity.GetType().Name;
            var properties = new List<PropertyInfo>();
            var ownedProperties = new List<PropertyInfo>();

            foreach (var property in entity.GetType().GetProperties().Where(p => updatedPropertyNames.Any(n => n == p.Name)))
            {
                if (IsSimpleType(property.PropertyType))
                {
                    properties.Add(property);
                }
                else
                {
                    if (property.PropertyType.GetCustomAttributes<OwnedAttribute>().Any())
                    {
                        ownedProperties.Add(property);
                    }
                }
            }

            UpdatePrincipalEntity(entityName, properties, entity);

            foreach (var p in ownedProperties)
            {
                var propertyValue = p.GetValue(entity);
                if (propertyValue == null) continue;

                UpdateOwnedEntity($"{entityName}{p.Name}",
                    $"{entityName}Id",
                    entity.Id,
                    p.PropertyType,
                    propertyValue);
            }
        }

        private static bool IsSimpleType(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return IsSimpleType(typeInfo.GetGenericArguments()[0]);
            }

            return typeInfo.IsPrimitive
                   || typeInfo.IsEnum
                   || type == typeof(string)
                   || type == typeof(Guid)
                   || type == typeof(decimal)
                   || type == typeof(DateTime)
                   || type == typeof(DateTimeOffset);
        }

        private void AddPrincipalEntity(string tableName, IEnumerable<PropertyInfo> properties, EntityBase entity)
        {
            var parameters = new List<SqlParameter>();

            var sql = new StringBuilder();
            var fields = new StringBuilder();
            var values = new StringBuilder();

            sql.Append($"INSERT INTO {tableName}s ( ");
            foreach (var p in properties)
            {
                fields.Append($"{p.Name},");
                values.Append($"@{p.Name},");
                parameters.Add($"@{p.Name}".ToParameter(p.GetValue(entity)));
            }

            fields.Remove(fields.Length - 1, 1);
            values.Remove(values.Length - 1, 1);

            sql.Append(fields);
            sql.Append(") VALUES (");
            sql.Append(values);
            sql.Append(");");

            _logger.LogTrace($"{sql}. {parameters.Select(p => new { p.ParameterName, p.Value }).ToJson()}");
            _dbContext.Database.ExecuteSqlRaw(sql.ToString(), parameters);
        }

        private void UpdatePrincipalEntity(string tableName, IEnumerable<PropertyInfo> properties, EntityBase entity)
        {
            var parameters = new List<SqlParameter>();
            var sql = new StringBuilder();

            sql.Append($"UPDATE {tableName}s SET ");
            foreach (var p in properties)
            {
                sql.Append($"{p.Name} = @{p.Name},");
                parameters.Add($"@{p.Name}".ToParameter(p.GetValue(entity)));
            }

            sql.Remove(sql.Length - 1, 1);
            sql.Append($" WHERE Id = @Id");
            parameters.Add("@Id".ToParameter(entity.Id));

            _logger.LogTrace($"{sql}. {parameters.Select(p => new { p.ParameterName, p.Value }).ToJson()}");
            var count = _dbContext.Database.ExecuteSqlRaw(sql.ToString(), parameters);
            if (count == 0) throw new InvalidOperationException($"{sql} did not update any records");
        }

        private void AddOwnedEntity(string tableName, string keyName, Guid keyValue, Type propertyType, object propertyValue)
        {
            var properties = propertyType.GetProperties();
            var parameters = new List<SqlParameter> { $"@{keyName}".ToParameter(keyValue) };

            var sql = new StringBuilder();
            var fields = new StringBuilder();
            var values = new StringBuilder();

            sql.Append($"INSERT INTO {tableName} ( {keyName}");
            foreach (var p in properties)
            {
                fields.Append($",{p.Name}");
                values.Append($",@{p.Name}");
                parameters.Add($"@{p.Name}".ToParameter(p.GetValue(propertyValue)));
            }

            sql.Append(fields);
            sql.Append($") VALUES (@{keyName}");
            sql.Append(values);
            sql.Append(");");

            _logger.LogTrace($"{sql}. {parameters.Select(p => new { p.ParameterName, p.Value }).ToJson()}");
            _dbContext.Database.ExecuteSqlRaw(sql.ToString(), parameters);
        }

        private void UpdateOwnedEntity(string tableName, string keyName, Guid keyValue, Type propertyType, object propertyValue)
        {
            var properties = propertyType.GetProperties();
            var parameters = new List<SqlParameter> { $"@{keyName}".ToParameter(keyValue) };

            var sql = new StringBuilder();

            sql.Append($"UPDATE {tableName} SET ");
            foreach (var p in properties)
            {
                sql.Append($"{p.Name} = @{p.Name},");
                parameters.Add($"@{p.Name}".ToParameter(p.GetValue(propertyValue)));
            }

            sql.Remove(sql.Length - 1, 1);
            sql.Append($" WHERE {keyName} = @{keyName}");

            _logger.LogTrace($"{sql}. {parameters.Select(p => new { p.ParameterName, p.Value }).ToJson()}");
            _dbContext.Database.ExecuteSqlRaw(sql.ToString(), parameters);
        }
    }

    public static class Extensions
    {
        public static SqlParameter ToParameter(this string name, object value)
        {
            return new SqlParameter(name, value ?? DBNull.Value);
        }
    }
}
