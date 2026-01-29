using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Conda_AI.Model
{
    public class MessageGroup
    {
        public string Date { get; set; }
        public DateTime DateObj { get; set; }
        public ObservableCollection<Message> Messages { get; set; } = [];
    }
}
