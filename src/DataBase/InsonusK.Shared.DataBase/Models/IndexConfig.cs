using Ardalis.GuardClauses;
using InsonusK.Shared.DataBase.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsonusK.Shared.DataBase.Models;


public class IndexConfig<TEntity> : IIndexConfig where TEntity : class
{
    public string TableName => typeof(TEntity).Name;
    private string[] _fields = null!;
    public required string[] Fields
    {
        get => _fields;
        init
        {
            Guard.Against.NullOrEmpty(value, nameof(Fields));
            _fields = value;
        }
    }
    public bool IsUnique { get; init; } = true;

    public string IndexName
    {
        get
        {
            var prefix = IsUnique ? "UX" : "IX";
            return $"{prefix}_{TableName}_{string.Join('_', Fields.OrderBy(f => f))}";
        }
    }
    public void AddToEntityTypeBuilder(EntityTypeBuilder<TEntity> builder)
    {
        var indexBuilder = builder.HasIndex(Fields).HasDatabaseName(IndexName);
        if (IsUnique)
            indexBuilder.IsUnique();
    }
}

public static class IndexConfigExtensions
{
    public static void Apply<TEntity>(this IndexConfig<TEntity>[] indexConfig, EntityTypeBuilder<TEntity> builder) where TEntity : class
    {
        foreach (var config in indexConfig)
        {
            config.AddToEntityTypeBuilder(builder);
        }
    }
}
