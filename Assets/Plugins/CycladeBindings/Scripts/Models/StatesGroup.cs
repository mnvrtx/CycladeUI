using System;
using System.Collections.Generic;

namespace CycladeBindings.Models
{
    [Serializable]
    public class StatesGroup
    {
        public string name;
        public List<string> states = new();

        public StatesGroup(string name)
        {
            this.name = name;
        }
    }
}