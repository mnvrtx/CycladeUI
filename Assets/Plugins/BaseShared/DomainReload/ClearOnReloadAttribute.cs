using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace BaseShared.DomainReload
{
    /// <summary>
    /// Attribute used by the <see cref="Codice.Client.BaseCommands.KnownCommandOptions.Common.Utils.DomainReloadHandler"/> to clear an event, a field or a property on reload
    /// </summary>
    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Property)]
    public class ClearOnReloadAttribute : Attribute
    {
        /// <summary> Value set to the target field or property on reload </summary>
        public readonly object ValueOnReload;
        /// <summary> Flag used to create or not a new instance of the target field or property (does not work on events) </summary>
        public readonly bool CreateNewInstance;

        /// <summary> On reload, clear event, field or property </summary>
        public ClearOnReloadAttribute()
        {
            ValueOnReload = null;
            CreateNewInstance = false;
        }

        /// <summary> On reload, set the given value to a target field or property (does not work for events) </summary>
        /// <param name="resetValue"> Value set to target field or property. Value type needs to match the field/property type. Does not work on events. </param>
        public ClearOnReloadAttribute(object resetValue)
        {
            ValueOnReload = resetValue;
            CreateNewInstance = false;
        }

        /// <summary> On reload, clear or re-initialize target field or property </summary>
        /// <param name="newInstance"> Create a new instance of the target field or property, if TRUE. Does not work on events. </param>
        public ClearOnReloadAttribute(bool newInstance)
        {
            ValueOnReload = null;
            CreateNewInstance = newInstance;
        }
    }
}
