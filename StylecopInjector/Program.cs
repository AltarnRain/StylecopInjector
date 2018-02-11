// <copyright file="Program.cs" company="OI">
// Copyright (c) OI. All rights reserved.
// </copyright>

namespace StylecopInjector
{
    using System;
    using System.IO;
    using SylecopInjector;

    /// <summary>
    /// Main program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            var folder = Directory.GetCurrentDirectory();

            foreach (var arg in args)
            {
                var key = string.Empty;
                var value = string.Empty;
                try
                {
                    key = arg.Split('=')[0].ToLower();
                    value = arg.Split('=')[1];
                }
                catch
                {
                    Console.WriteLine($"Parameter {arg} is not valid. Specify parameters as a=b");
                    return;
                }

                switch (key)
                {
                    case "folder":
                        folder = value;
                        break;
                }
            }

            if (!folder.EndsWith("\\"))
            {
                folder += "\\";
            }

            var styleCopInjector = new StylecopInjector(folder);
            styleCopInjector.Inject();

            var assemblyInfoUpdater = new AssemblyInfoUpdater(folder);
            assemblyInfoUpdater.UpdateAssemblyInfo();
        }
    }
}
