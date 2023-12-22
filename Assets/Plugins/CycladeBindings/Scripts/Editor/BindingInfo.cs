using System;

namespace CycladeBindingsEditor
{
    public class BindingInfo
    {
        public Type Type;
        public bool NeedToFullName;
        public string OptionalBindingShortName;

        public BindingInfo(Type type, string optionalBindingShortName, bool needToFullName = false)
        {
            Type = type;
            OptionalBindingShortName = optionalBindingShortName;
            NeedToFullName = needToFullName;
        }
    }
}