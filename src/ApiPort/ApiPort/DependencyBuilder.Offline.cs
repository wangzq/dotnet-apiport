// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Autofac;
using Microsoft.Fx.Portability;
using Microsoft.Fx.Portability.Reporting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ApiPort
{
    internal static partial class DependencyBuilder
    {
        static partial void RegisterOfflineModule(ContainerBuilder builder)
        {
            builder.RegisterModule(new OfflineDataModule(DefaultOutputFormatInstanceName));
            LoadReportWriters(builder);
        }

        private static void LoadReportWriters(ContainerBuilder builder)
        {
            foreach (var path in GetReportPlugins(GetApplicationDirectory()))
            {
                try
                {
                    var name = new AssemblyName(Path.GetFileNameWithoutExtension(path));
                    var assembly = Assembly.Load(name);

                    builder.RegisterAssemblyTypes(assembly)
                        .AssignableTo<IReportWriter>()
                        .As<IReportWriter>()
                        .SingleInstance();
                }
                catch (Exception)
                {
                }
            }
        }

        private static IEnumerable<string> GetReportPlugins(string directory)
        {
            const string prefix = "Microsoft.Fx.Portability.Reports.";
            var prefixLen = prefix.Length;
            var files = Directory.EnumerateFiles(directory, prefix + "*.dll").ToArray();
            return files.Where(f => !IsRazorViewsAssembly(f, files));
        }

        private static bool IsRazorViewsAssembly(string file, string[] files)
        {
            if (file.EndsWith(".views.dll", StringComparison.OrdinalIgnoreCase))
            {
                var prefix = file.Substring(0, file.Length - ".views.dll".Length);
                return files.Any(f => string.Equals(f, prefix + ".dll", StringComparison.OrdinalIgnoreCase));
            }

            return false;
        }

        private static string GetApplicationDirectory()
        {
            return Path.GetDirectoryName(typeof(DependencyBuilder).GetTypeInfo().Assembly.Location);
        }
    }
}
