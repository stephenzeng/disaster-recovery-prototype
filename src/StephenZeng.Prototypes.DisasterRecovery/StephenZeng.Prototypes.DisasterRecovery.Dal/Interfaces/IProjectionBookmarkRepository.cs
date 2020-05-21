using System.Threading.Tasks;

namespace StephenZeng.Prototypes.DisasterRecovery.Dal.Interfaces
{
    public interface IProjectionBookmarkRepository
    {
        Task Update(string name, long sequenceId);
        Task<long> GetLastSequenceId(string name);
        Task<long> GetLastSequenceId();
    }
}