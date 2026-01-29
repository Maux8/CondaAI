using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conda_AI.Model
{
    public class User(string name)
    {
        public string Name { get; set; } = name;
        public Dictionary<string, object> Settings { get; set; } = new();
    }
}
