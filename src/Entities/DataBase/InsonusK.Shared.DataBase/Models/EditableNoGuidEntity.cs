
using System.ComponentModel.DataAnnotations;
using InsonusK.Shared.Models.Common;

namespace InsonusK.Shared.DataBase.Models;

/// <summary>
/// Базовая сущность с константным идентификатором
/// Подходит для EditableCompositeEntity и EditableNoGuidEntity
/// см ADR 1
/// </summary>
public abstract class EditableNoGuidEntity : ConstantNoGuidEntity, IUpdateInfoModel, IVersionatedModel
{


  private DateTimeOffset serverUpdatedDateTime;
  [Required]
  public DateTimeOffset ServerUpdatedDateTime
  {
    get => serverUpdatedDateTime; set
    {
      IUpdateInfoModel.ValidateUpdateValue(this, value);
      serverUpdatedDateTime = value;
    }
  }
  [Required]
  public DateTimeOffset UserUpdatedDateTime { get; set; }

  public uint Version { get; set; }

}
