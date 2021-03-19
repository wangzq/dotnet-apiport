// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Fx.Portability.Analyzer
{
    public class DotNetFrameworkFilter : IDependencyFilter
    {
        /// <summary>
        /// These keys are a collection of public key tokens derived from all the reference assemblies in
        /// "%ProgramFiles%\Reference Assemblies\Microsoft" on a Windows 10 machine with VS 2015 installed.
        /// </summary>
        private static readonly HashSet<PublicKeyToken> MicrosoftKeys = new HashSet<PublicKeyToken>(new PublicKeyToken[]
        {
        });

        private static readonly string[] FrameworkAssemblyNamePrefixes = new[]
        {
            "System.",
            "Microsoft.AspNet.",
            "Microsoft.AspNetCore.",
            "Microsoft.CSharp.",
            "Microsoft.EntityFrameworkCore.",
            "Microsoft.Win32.",
            "Microsoft.VisualBasic.",
            "Windows."
        };

        public virtual bool IsFrameworkMember(string name, PublicKeyToken publicKeyToken)
            => IsFrameworkAssembly(name, publicKeyToken);

        public virtual bool IsFrameworkAssembly(string name, PublicKeyToken publicKeyToken)
            => IsKnownPublicKeyToken(publicKeyToken) || IsKnownName(name);

        private static bool IsKnownPublicKeyToken(PublicKeyToken publicKeyToken)
            => MicrosoftKeys.Contains(publicKeyToken);

        private static bool IsKnownName(string name)
        {
            // Name is null, default to submitting the API
            if (name is null)
            {
                return true;
            }

            if (string.Equals(name, "mscorlib", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (FrameworkAssemblyNamePrefixes.Any(p => name.StartsWith(p, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            return false;
        }
    }
}
