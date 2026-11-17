using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using FADETools.Shared;

namespace FADETools.ProjectEditor
{
    public static class ProjectGenerator
    {
        public static string FADESdkAttrValue => $"FADE.NET.Sdk/{GeneratedFADENupkgsVersions.FADENETSdk}";

        public static string FADEMinimumRequiredTfm => "net10.0";

        public static ProjectRootElement GenGameProject(string name)
        {
            if (name.Length == 0)
                throw new ArgumentException("Project name is empty.", nameof(name));

            var root = ProjectRootElement.Create(NewProjectFileOptions.None);

            root.Sdk = FADESdkAttrValue;

            var mainGroup = root.AddPropertyGroup();
            mainGroup.AddProperty("TargetFramework", FADEMinimumRequiredTfm);

            // Non-gradle builds require .NET 10 to match the jar libraries included in the export template.
            var net10 = mainGroup.AddProperty("TargetFramework", "net10.0");
            net10.Condition = " '$(FADETargetPlatform)' == 'android' ";

            mainGroup.AddProperty("EnableDynamicLoading", "true");

            string sanitizedName = IdentifierUtils.SanitizeQualifiedIdentifier(name, allowEmptyIdentifiers: true);

            // If the name is not a valid namespace, manually set RootNamespace to a sanitized one.
            if (sanitizedName != name)
                mainGroup.AddProperty("RootNamespace", sanitizedName);

            return root;
        }

        public static string GenAndSaveGameProject(string dir, string name)
        {
            if (name.Length == 0)
                throw new ArgumentException("Project name is empty.", nameof(name));

            string path = Path.Combine(dir, name + ".csproj");

            var root = GenGameProject(name);

            // Save (without BOM)
            root.Save(path, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            return Guid.NewGuid().ToString().ToUpperInvariant();
        }
    }
}
