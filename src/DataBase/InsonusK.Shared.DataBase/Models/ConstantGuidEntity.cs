using System.ComponentModel.DataAnnotations;
using InsonusK.Shared.Models.Common;

namespace InsonusK.Shared.DataBase.Models;

public abstract class ConstantGuidEntity : ConstantNoGuidEntity, IGuidModel
{
    private Guid _guid;

    [Required]
    public virtual Guid Guid
    {
        get => _guid; set
        {
            if (Id != 0 && Guid != value)
                throw new ApplicationException("Cann't set up Guid for exist entity");
            _guid = value;
        }
    }

    public bool IsEqualStringId(string stringId) => Guid.ToString() == stringId || Id.ToString() == stringId;

}