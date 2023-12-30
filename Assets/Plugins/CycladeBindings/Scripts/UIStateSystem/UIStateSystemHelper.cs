using System;
using System.Collections.Generic;
using System.Linq;
using CycladeBindings.UIStateSystem.Base;

namespace CycladeBindings.UIStateSystem
{
    public static class UIStateSystemHelper
    {
        public static void SetState(Enum[] states, List<BaseStatefulElement> statefulElements)
        {
            foreach (var element in statefulElements)
            {
                if (element is BaseGroupableState baseSelectableState)
                {
                    var group = baseSelectableState.stateGroup;
                    var en = states.FirstOrDefault(q => q.GetType().Name == group);
                    if (en != null)
                    {
                        var state = en.ToString();
                        baseSelectableState.Select(state);
                    }
                }
            }
        }
    }
}