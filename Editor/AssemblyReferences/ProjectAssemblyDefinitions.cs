using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditorInternal;
using UnityEngine;

namespace ArcaneOnyx.GameEventGenerator
{
    /// <summary>
    /// Reads the assembly definition files of the project as they are on disk right now.
    /// </summary>
    public static class ProjectAssemblyDefinitions
    {
        private const string GuidReferencePrefix = "GUID:";

        public static string ReadName(AssemblyDefinitionAsset assemblyDefinition)
        {
            return ReadContent(assemblyDefinition).name;
        }

        /// <summary>
        /// Returns the names of the assemblies the given assembly references. An assembly without an
        /// assembly definition file in the project (a precompiled one, or one not generated yet) has none.
        /// </summary>
        public static IReadOnlyList<string> ReadNamesOfReferences(string assemblyName)
        {
            var namesOfReferences = new List<string>();

            AssemblyDefinitionAsset assemblyDefinition = Load(CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(assemblyName));
            if (assemblyDefinition == null)
            {
                return namesOfReferences;
            }

            foreach (string reference in ReadContent(assemblyDefinition).references)
            {
                string nameOfReference = FindNameOfReference(reference);
                if (nameOfReference != null)
                {
                    namesOfReferences.Add(nameOfReference);
                }
            }

            return namesOfReferences;
        }

        private static string FindNameOfReference(string reference)
        {
            bool isWrittenAsName = !reference.StartsWith(GuidReferencePrefix, StringComparison.Ordinal);
            if (isWrittenAsName)
            {
                return reference;
            }

            AssemblyDefinitionAsset referencedAssemblyDefinition = Load(CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyReference(reference));
            return referencedAssemblyDefinition == null ? null : ReadName(referencedAssemblyDefinition);
        }

        private static AssemblyDefinitionAsset Load(string assemblyDefinitionPath)
        {
            return string.IsNullOrEmpty(assemblyDefinitionPath)
                ? null
                : AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(assemblyDefinitionPath);
        }

        private static AssemblyDefinitionContent ReadContent(AssemblyDefinitionAsset assemblyDefinition)
        {
            return JsonUtility.FromJson<AssemblyDefinitionContent>(assemblyDefinition.text);
        }

        [Serializable]
        private class AssemblyDefinitionContent
        {
            public string name = string.Empty;
            public string[] references = Array.Empty<string>();
        }
    }
}
