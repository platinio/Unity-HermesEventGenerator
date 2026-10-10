using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ArcaneOnyx.GameEventGenerator
{
    /// <summary>
    /// Rewrites the "references" list of an assembly definition. Lines end with "\n"; the generator
    /// unifies the line endings of the whole file afterwards.
    /// </summary>
    public static class AssemblyDefinitionReferences
    {
        private const string ReferenceIndentation = "        ";
        private const string ClosingBracketIndentation = "    ";

        private static readonly Regex ReferencesList = new Regex(@"""references""\s*:\s*\[(?<references>[^\]]*)\]");
        private static readonly Regex QuotedReference = new Regex(@"""(?<reference>[^""]+)""");

        /// <summary>
        /// Returns the assembly definition with the extra references added to the ones it already lists,
        /// sorted and without duplicates.
        /// </summary>
        public static string WithExtraReferences(string assemblyDefinition, IEnumerable<string> extraReferences)
        {
            Match referencesList = ReferencesList.Match(assemblyDefinition);
            if (!referencesList.Success)
            {
                throw new ArgumentException("The assembly definition has no \"references\" list to add the extra references to.");
            }

            SortedSet<string> allReferences = ReadReferences(referencesList);
            allReferences.UnionWith(extraReferences);

            string textBeforeTheList = assemblyDefinition.Substring(0, referencesList.Index);
            string textAfterTheList = assemblyDefinition.Substring(referencesList.Index + referencesList.Length);

            return textBeforeTheList + WriteReferencesList(allReferences) + textAfterTheList;
        }

        private static SortedSet<string> ReadReferences(Match referencesList)
        {
            var references = new SortedSet<string>(StringComparer.Ordinal);

            foreach (Match quotedReference in QuotedReference.Matches(referencesList.Groups["references"].Value))
            {
                references.Add(quotedReference.Groups["reference"].Value);
            }

            return references;
        }

        private static string WriteReferencesList(IReadOnlyCollection<string> references)
        {
            if (references.Count == 0)
            {
                return "\"references\": []";
            }

            var referenceLines = new List<string>();

            foreach (string reference in references)
            {
                referenceLines.Add($"{ReferenceIndentation}\"{reference}\"");
            }

            return "\"references\": [\n" + string.Join(",\n", referenceLines) + $"\n{ClosingBracketIndentation}]";
        }
    }
}
