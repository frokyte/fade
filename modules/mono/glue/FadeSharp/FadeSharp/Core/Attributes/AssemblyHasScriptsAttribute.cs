using System;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace FADE
{
    /// <summary>
    /// Attribute that determines that the assembly contains FADE scripts and, optionally, the
    /// collection of types that implement scripts; otherwise, retrieving the types requires lookup.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class AssemblyHasScriptsAttribute : Attribute
    {
        /// <summary>
        /// If the FADE scripts contained in the assembly require lookup
        /// and can't rely on <see cref="ScriptTypes"/>.
        /// </summary>
        [MemberNotNullWhen(false, nameof(ScriptTypes))]
        public bool RequiresLookup { get; }

        /// <summary>
        /// The collection of types that implement a FADE script.
        /// </summary>
        public Type[]? ScriptTypes { get; }

        /// <summary>
        /// Constructs a new AssemblyHasScriptsAttribute instance
        /// that requires lookup to get the FADE scripts.
        /// </summary>
        public AssemblyHasScriptsAttribute()
        {
            RequiresLookup = true;
            ScriptTypes = null;
        }

        /// <summary>
        /// Constructs a new AssemblyHasScriptsAttribute instance
        /// that includes the FADE script types and requires no lookup.
        /// </summary>
        /// <param name="scriptTypes">The collection of types that implement a FADE script.</param>
        public AssemblyHasScriptsAttribute(Type[] scriptTypes)
        {
            RequiresLookup = false;
            ScriptTypes = scriptTypes;
        }
    }
}

#nullable restore
