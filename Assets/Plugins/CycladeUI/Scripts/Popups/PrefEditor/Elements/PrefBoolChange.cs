using System;
using CycladeUI.Utils.Switchers;

namespace CycladeUI.Popups.PrefEditor.Elements
{
    public class PrefBoolChange : PrefBaseChange
    {
        public SelectableImage enabledImage;

        [NonSerialized] public bool InitialValue;

        public void SetFlag(bool val)
        {
            InitialValue = val;
            enabledImage.Select(val);
        }

        public void U_OnChange()
        {
            var f = enabledImage.IsEnabled;
            FieldInfo.SetValue(Obj, f);
        }

        public override void ResetProperty()
        {
            enabledImage.Select(InitialValue);
            U_OnChange();
        }
    }
}