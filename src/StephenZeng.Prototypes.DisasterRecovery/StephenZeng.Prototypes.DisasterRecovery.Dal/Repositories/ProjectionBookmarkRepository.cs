using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StephenZeng.Prototypes.DisasterRecovery.Dal.Interfaces;
using StephenZeng.Prototypes.DisasterRecovery.Domain;

namespace StephenZeng.Prototypes.DisasterRecovery.Dal.Repositories
{
    public class ProjectionBookmarkRepository : IProjectionBookmarkRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProjectionBookmarkRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Update(string name, long sequenceId)
        {
            var bookmark = await _dbContext.ProjectionBookmarks.FirstOrDefaultAsync(b => b.Name == name);
            if (bookmark == null)
            {
                bookmark = new ProjectionBookmark
                {
                    Name = name,
                    LastEventSequenceId = sequenceId,
                    LastChangedDate = DateTimeOffset.Now
                };
                _dbContext.ProjectionBookmarks.Add(bookmark);
            }
            else
            {
                bookmark.LastEventSequenceId = sequenceId;
                bookmark.LastChangedDate = DateTimeOffset.Now;
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task<long> GetLastSequenceId(string name)
        {
            var bookmark = await _dbContext.ProjectionBookmarks
                .Where(b => b.Name == name)
                .Select(b => new { b.LastEventSequenceId })
                .FirstOrDefaultAsync();

            return bookmark?.LastEventSequenceId ?? 0;
        }

        public async Task<long> GetLastSequenceId()
        {
            var bookmark = await _dbContext.ProjectionBookmarks
                .OrderByDescending(b => b.LastEventSequenceId)
                .Select(b => new { b.LastEventSequenceId })
                .FirstOrDefaultAsync();

            return bookmark?.LastEventSequenceId ?? 0;
        }
    }
}