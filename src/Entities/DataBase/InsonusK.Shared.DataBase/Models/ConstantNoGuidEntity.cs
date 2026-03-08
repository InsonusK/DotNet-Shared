using System.ComponentModel.DataAnnotations;
using InsonusK.Shared.Models.Common;

namespace InsonusK.Shared.DataBase.Models;

/// <summary>
/// Базовая сущность с константным идентификатором
/// Подходит для ConstantEntity и ImmutableCompositeEntity
/// см ADR 1
/// </summary>
public abstract class ConstantNoGuidEntity : EntityBase, ICreationInfoModel
{

    private DateTimeOffset _serverCreatedDateTime;
    [Required]
    public DateTimeOffset ServerCreatedDateTime
    {
        get => _serverCreatedDateTime; set
        {
            if (Id != 0)
                throw new ApplicationException("Cann't set up CreatedDateTime for exist entity");
            _serverCreatedDateTime = value;
        }
    }
    private DateTimeOffset _userCreatedDateTime;
    [Required]
    public DateTimeOffset UserCreatedDateTime
    {
        get => _userCreatedDateTime; set
        {
            if (Id != 0)
                throw new ApplicationException("Cann't set up UserDateTime for exist entity");
            _userCreatedDateTime = value;
        }
    }


}
