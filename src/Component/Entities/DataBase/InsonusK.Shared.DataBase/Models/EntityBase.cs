using System.ComponentModel.DataAnnotations;
using InsonusK.Shared.Models.Common;

namespace InsonusK.Shared.DataBase.Models;

public abstract class EntityBase : IDbModel
{
    [Key]
    public int Id { get; set; }

}