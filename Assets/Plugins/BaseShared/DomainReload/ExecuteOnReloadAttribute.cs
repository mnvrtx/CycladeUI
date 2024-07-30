using System;

// ReSharper disable ClassNeverInstantiated.Global

namespace BaseShared.DomainReload
{
    /// <summary>
    /// Attribute used by the <see cref="Codice.Client.BaseCommands.KnownCommandOptions.Common.Utils.DomainReloadHandler"/> to execute a method on reload
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ExecuteOnReloadAttribute : Attribute
    {
        /// <summary> On reload, execute method </summary>
        public ExecuteOnReloadAttribute() {}
    }
}
