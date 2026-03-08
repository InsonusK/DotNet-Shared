namespace InsonusK.Shared.DataBase.Models.Interfaces;

public interface IIndexConfig
{
    public string TableName { get; }
    public string IndexName { get; }
    public string[] Fields { get; }
}
