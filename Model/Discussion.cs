using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Conda_AI.Model
{
    [Table("Discussion")]
    public class Discussion
    {
        [PrimaryKey, AutoIncrement]
        public int DiscussionId { get; set; }

        [NotNull]
        public string Title { get; set; }

        [NotNull]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime LastUpdatedAt { get; set; } = DateTime.Now;

        // Context information that can help define the scope/purpose of this discussion
        public string Context { get; set; }

        // Indicates if this is the currently active discussion
        [Ignore]
        public bool IsActive { get; set; }
        
        public string Description { get; set; }

        [Ignore]
        public ObservableCollection<Message> Messages { get; set; } = [];
    }
}
