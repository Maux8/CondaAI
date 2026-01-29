using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conda_AI.Model
{
    [Table("TaskCategory")]
    public class TaskCategory
    {
        [PrimaryKey, AutoIncrement]
        public int TaskCategory_Id { get; set; }
        [NotNull]
        public string Name { get; set; }
        [NotNull]
        public string ColorCode { get; set; } = "#FFFFFF"; // Default to white if not specified

        [Ignore]
        public List<TaskModel> Tasks { get; set; } = new List<TaskModel>();
    }
}
