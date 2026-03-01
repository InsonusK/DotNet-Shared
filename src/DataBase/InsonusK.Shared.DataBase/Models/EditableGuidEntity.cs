
using System.ComponentModel.DataAnnotations;
using InsonusK.Shared.Models.Common;

namespace InsonusK.Shared.DataBase.Models;

public abstract class EditableGuidEntity : ConstantGuidEntity, IUpdateInfoModel, IVersionatedModel
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
