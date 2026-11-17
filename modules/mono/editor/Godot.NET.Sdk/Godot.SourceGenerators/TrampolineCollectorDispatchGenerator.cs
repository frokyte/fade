using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace FADE.SourceGenerators;

[Generator]
public class TrampolineCollectorDispatchGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
        INamedTypeSymbol[] godotClasses = context
            .Compilation.SyntaxTrees
            .SelectMany(tree =>
                tree.GetRoot().DescendantNodes()
                    .OfType<ClassDeclarationSyntax>()
                    .SelectFADEScriptClasses(context.Compilation)
                    // Report and skip non-partial classes
                    .Where(x =>
                    {
                        if (x.cds.IsPartial())
                        {
                            if (x.cds.IsNested() && !x.cds.AreAllOuterTypesPartial(out _))
                            {
                                return false;
                            }

                            return true;
                        }

                        return false;
                    })
                    .Select(x => x.symbol)
            )
            .Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default)
            .ToArray();

        if (godotClasses.Length > 0)
        {
            foreach (var godotClass in godotClasses)
            {
                VisitFADEScriptClass(context, godotClass);
            }
        }
    }

    private static void VisitFADEScriptClass(
        GeneratorExecutionContext context,
        INamedTypeSymbol symbol
    )
    {
        INamespaceSymbol namespaceSymbol = symbol.ContainingNamespace;
        string classNs = namespaceSymbol != null && !namespaceSymbol.IsGlobalNamespace
            ? namespaceSymbol.FullQualifiedNameOmitGlobal()
            : string.Empty;
        bool hasNamespace = classNs.Length != 0;

        bool isInnerClass = symbol.ContainingType != null;

        string uniqueHint = symbol.FullQualifiedNameOmitGlobal().SanitizeQualifiedNameForUniqueHint()
                            + "_TrampolineCollectorDispatch.generated";

        var source = new StringBuilder();

        source.Append("using FADE;\n");
        source.Append("using FADE.NativeInterop;\n");
        source.Append("\n");

        if (hasNamespace)
        {
            source.Append("namespace ");
            source.Append(classNs);
            source.Append(";\n\n");
        }

        if (isInnerClass)
        {
            AppendPartialContainingTypeDeclarations(symbol.ContainingType);

            void AppendPartialContainingTypeDeclarations(INamedTypeSymbol? containingType)
            {
                if (containingType == null)
                    return;

                AppendPartialContainingTypeDeclarations(containingType.ContainingType);

                source.Append("partial ");
                source.Append(containingType.GetDeclarationKeyword());
                source.Append(" ");
                source.Append(containingType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
                source.Append("\n{\n");
            }
        }

        source.Append("partial class ");
        source.Append(symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
        source.Append("\n{\n");

        source.Append("#pragma warning disable CS0628 // Disable warning about redundant 'new' keyword\n");
        source.Append("#pragma warning disable CS0109")
            .Append(" // Disable warning about new protected member declared in sealed type\n");

        source.Append("    ").Append(symbol.IsSealed ? "" : "protected ")
            .Append("internal new static partial class FADEInternal\n    {\n");

        const string CollectorType = "global::FADE.Bridge.TrampolineCollectors";
        const string OptionsType = "global::FADE.Bridge.TrampolineCollectionOptions";

        source.Append("        /// <summary>\n")
            .Append("        /// Collects the trampolines for all the members visible to FADE.\n")
            .Append(
                "        /// This method is used by FADE to collect the trampolines to call this class's members from the native side.\n")
            .Append("        /// Do not call this method.\n")
            .Append("        /// </summary>\n");

        source
            .Append("        public static void GetFADEClassTrampolines(")
            .Append(CollectorType).Append(" collectors, ").Append(OptionsType).Append(" options)\n        {\n");

        if (!context.IsFADESourceGeneratorDisabled("ScriptMethods"))
        {
            source.Append("            ").Append(symbol.FullQualifiedNameIncludeGlobal()).Append(".FADEInternal")
                .Append(".GetFADEMethodTrampolines(collectors.MethodTrampolineCollector);\n");
        }

        if (!context.IsFADESourceGeneratorDisabled("ScriptProperties"))
        {
            source.Append("            ").Append(symbol.FullQualifiedNameIncludeGlobal()).Append(".FADEInternal")
                .Append(".GetFADEPropertyTrampolines(collectors.PropertyTrampolineCollector);\n");
        }

        if (!context.IsFADESourceGeneratorDisabled("ScriptSignals"))
        {
            source.Append("            ").Append(symbol.FullQualifiedNameIncludeGlobal()).Append(".FADEInternal")
                .Append(".GetFADERaiseSignalTrampolines(collectors.RaiseSignalTrampolineCollector);\n");
        }

        var usableBaseType = symbol.GetClosestBaseTypeDeclaringFADEInternalMethod("GetFADEClassTrampolines");

        if (usableBaseType != null)
        {
            source
                .Append("            if (options.IncludeAncestors) {\n")
                .Append("                ").Append(usableBaseType.FullQualifiedNameIncludeGlobal())
                .Append(".FADEInternal.GetFADEClassTrampolines(collectors, options);\n")
                .Append("            }\n");
        }


        source.Append("        }\n");

        source.Append("    }\n"); // partial class FADEInternal

        // Generate GetFADEClassFADEInternalType
        // When FADESharp calls `GetNestedType("FADEInternal")`, that method returns a generic type definition,
        // even though we're calling it on a constructed generic type. This is an AOT-compatible workaround.
        // This workaround should no longer be needed once assemblies and generated code are trim-compatible.
        if (symbol.IsGenericType)
        {
            source.Append("#pragma warning disable CS0109 // Disable warning about redundant 'new' keyword\n");
            source.Append("    private new static global::System.Type GetFADEClassFADEInternalType()\n    {\n");
            source.Append("        return typeof(FADEInternal);\n");
            source.Append("    }\n");
            source.Append("#pragma warning restore CS0109\n");
        }

        source.Append("#pragma warning restore CS0109\n");
        source.Append("#pragma warning restore CS0628\n");

        source.Append("}\n"); // partial class

        if (isInnerClass)
        {
            var containingType = symbol.ContainingType;

            while (containingType != null)
            {
                source.Append("}\n"); // outer class

                containingType = containingType.ContainingType;
            }
        }

        context.AddSource(uniqueHint, SourceText.From(source.ToString(), Encoding.UTF8));
    }
}
