using System.Collections.Generic;
using CycladeBindings.Models;
using UnityEngine;

namespace CycladeBindings
{
    public class BindingGenerator : MonoBehaviour
    {
        public string additionalFolderPath = $"";
        public string bindPostfix = $"Binding";
        public string bindTitle = "[Bind]";
        public string bindArrTitle = "[BindArr]";
        public string bindViewInstanceTitle = "[BindViewInstance]";
        public string lastGeneratedPath = "";
        public List<StatesGroup> stateGroups;
    }
}