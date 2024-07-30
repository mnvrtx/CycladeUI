using CycladeBindings.Models;

namespace CycladeBindingsEditor
{
    public class BindingFieldInfo
    {
        public readonly string Type;
        public readonly string Name;
        public readonly BindMode BindMode;
        public readonly bool IsButton;

        public BindingFieldInfo(string type, string name, BindMode bindMode, bool isButton)
        {
            Type = type;
            Name = name;
            BindMode = bindMode;
            IsButton = isButton;
        }
    }
}