using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conda_AI.Model;

namespace Conda_AI.Database.Repositories
{
    public class MessagesRepository(DatabaseConnection databaseConnection)
        : AbstractRepository<Model.Message>(databaseConnection)
    {

        public async Task<List<Message>> GetByDiscussionIdAsync(int discussionId)
        {
            await InitAsync();
            return await Connection.Table<Message>()
                .Where(m => m.DiscussionId == discussionId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }
        
        public new async Task<int> CreateAsync(Message message)
        {
            await InitAsync();
            // Update the last updated timestamp on the associated discussion
            var discussion = await Connection.Table<Discussion>().Where(d => d.DiscussionId == message.DiscussionId).FirstOrDefaultAsync();
            if (discussion != null)
            {
                discussion.LastUpdatedAt = DateTime.Now;
                await Connection.UpdateAsync(discussion);
            }

            return await Connection.InsertAsync(message);
        }
    }
}
