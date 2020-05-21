using StephenZeng.Prototypes.DisasterRecovery.Domain;

namespace StephenZeng.Prototypes.DisasterRecovery.Dal.Interfaces
{
    public interface IRepository
    {
        void Add(EntityBase entity);
        void Update(EntityBase entity, params string[] updatedPropertyNames);
    }
}