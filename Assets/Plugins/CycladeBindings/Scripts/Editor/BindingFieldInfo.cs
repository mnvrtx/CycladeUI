using CycladeBindings.Models;

namespace CycladeBindingsEditor.Editor
{
    public class BindingFieldInfo
    {
        public readonly string Type;
        public readonly string Name;
        public readonly BindMode BindMode;

        public BindingFieldInfo(string type, string name, BindMode bindMode)
        {
            Type = type;
            Name = name;
            BindMode = bindMode;
        }
    }
}