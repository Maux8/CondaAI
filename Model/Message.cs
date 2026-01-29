using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conda_AI.Model.Enums;
using SQLite;

namespace Conda_AI.Model
{
    [Table("Message")]
    public class Message
    {
        [PrimaryKey, AutoIncrement]
        public int MessageId { get; set; }
        [NotNull]
        public string Content { get; set; }
        [NotNull]
        public MessageAuthor Author { get; set; }
        
        [NotNull]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign key to Discussion
        [NotNull]
        public int DiscussionId { get; set; }

        // Navigation property to Discussion (not stored in DB)
        [Ignore]
        public Discussion Discussion { get; set; }
    }
}
