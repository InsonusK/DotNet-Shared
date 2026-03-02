using Ardalis.GuardClauses;
using InsonusK.Shared.DataBase.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InsonusK.Shared.DataBase.Models;


public class UXConfig<TEntity>: IIndexConfig where TEntity : class
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
