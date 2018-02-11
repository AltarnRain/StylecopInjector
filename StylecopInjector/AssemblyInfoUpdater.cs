﻿// <copyright file="AssemblyInfoUpdater.cs" company="OI">
// Copyright (c) OI. All rights reserved.
// </copyright>

namespace SylecopInjector
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using StylecopInjector;

    /// <summary>
    /// Updated the AssemblyInfo file with "auto-generated" so it is ignored by stylecop.
    /// </summary>
    public class AssemblyInfoUpdater
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyInfoUpdater"/> class.
        /// </summary>
        /// <param name="folder">The folder.</param>
        public AssemblyInfoUpdater(string folder)
        {
            this.Folder = folder;
        }

        /// <summary>
        /// Gets the folder.
        /// </summary>
        /// <value>
        /// The folder.
        /// </value>
        public string Folder { get; }

        /// <summary>
        /// Updates the assembly information.
        /// </summary>
        public void UpdateAssemblyInfo()
        {
            var files = Directory.GetFiles(this.Folder, "AssemblyInfo.cs", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var lines = File.ReadAllLines(file);

                var lineList = new List<string>(lines);
                var containsAutoGenerated = false;
                foreach (var line in lineList)
                {
                    if (line == Constants.Autogenerated)
                    {
                        containsAutoGenerated = true;
                        break;
                    }
                }

                if (containsAutoGenerated == false)
                {
                    lineList.Insert(0, Constants.Autogenerated);
                    using (var streamWriter = new StreamWriter(file))
                    {
                        foreach (var line in lineList)
                        {
                            streamWriter.WriteLine(line);
                        }
                    }

                    Console.WriteLine($"Added {Constants.Autogenerated} to file {file}");
                }
            }
        }
    }
}
