using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conda_AI.Model;
using SQLite;

namespace Conda_AI.Database.Repositories
{
    public class DiscussionsRepository(DatabaseConnection databaseConnection) : AbstractRepository<Discussion>(databaseConnection)
    {
        public new async Task<List<Discussion>> GetAllAsync()
        {
            await InitAsync();
            return await Connection.Table<Discussion>().OrderByDescending(d => d.LastUpdatedAt).ToListAsync();
        }

        public new async Task<Discussion?> GetByIdAsync(int id)
        {
            await InitAsync();
            return await Connection.Table<Discussion>().Where(d => d.DiscussionId == id).FirstOrDefaultAsync();
        }

        public new async Task<int> UpdateAsync(Discussion discussion)
        {
            await InitAsync();
            discussion.LastUpdatedAt = DateTime.Now;
            return await Connection.UpdateAsync(discussion);
        }

        public async Task<List<Message>> GetDiscussionMessagesAsync(int discussionId)
        {
            await InitAsync();
            return await Connection.Table<Message>()
                .Where(m => m.DiscussionId == discussionId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }
    }
}