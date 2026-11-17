using System;

#nullable enable

namespace FADE
{
    /// <summary>
    /// Exposes the target class as a global script class to FADE Engine.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class GlobalClassAttribute : Attribute { }
}
