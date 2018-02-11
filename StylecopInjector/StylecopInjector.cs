// <copyright file="StylecopInjector.cs" company="OI">
// Copyright (c) OI. All rights reserved.
// </copyright>

namespace StylecopInjector
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    /// <summary>
    /// The Stylecop Injector
    /// </summary>
    public class StylecopInjector
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StylecopInjector"/> class.
        /// </summary>
        /// <param name="folder">The folder.</param>
        public StylecopInjector(string folder)
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
        /// Gets the project.
        /// </summary>
        /// <value>
        /// The project.
        /// </value>
        public string Project { get; }

        /// <summary>
        /// Injects this instance.
        /// </summary>
        /// <exception cref="System.IO.FileNotFoundException">Thrown when stylecop.json isn't found</exception>
        public void Inject()
        {
            if (!File.Exists(Path.Combine(this.Folder, Constants.StylecopJson)))
            {
                throw new FileNotFoundException($"Stylecop.json was not found in {this.Folder}");
            }

            var projectfiles = Directory.GetFiles(this.Folder, Constants.SearchPattern, SearchOption.AllDirectories);

            foreach (var projectFile in projectfiles)
            {
                var depth = projectFile.Replace(this.Folder, string.Empty).Count(c => c == '\\');

                var path = string.Empty;
                for (int i = 0; i < depth; i++)
                {
                    path += @"..\";
                }

                var fullPath = path + "stylecop.json";

                var projectXml = XDocument.Load(projectFile);
                XNamespace ns = @"http://schemas.microsoft.com/developer/msbuild/2003";

                var hasStyleCop = projectXml.Element(ns + "Project")
                                  .Elements(ns + "ItemGroup")
                                  .Elements(ns + "AdditionalFiles")
                                  .Attributes("Include")
                                  .Any(a => a.Value.Contains(Constants.StylecopJson));

                var changed = false;
                if (hasStyleCop == false)
                {
                    projectXml.Element(ns + "Project").Add(new XElement(ns + "ItemGroup", new XElement(ns + "AdditionalFiles", new XAttribute("Include", fullPath))));
                    Console.WriteLine($"Added {Constants.StylecopJson} to project: {projectFile}");
                    changed = true;
                }
                else
                {
                    if (projectXml.Element(ns + "Project").Elements(ns + "ItemGroup").Elements(ns + "AdditionalFiles").Attributes("Include").First().Value != fullPath)
                    {
                        projectXml.Element(ns + "Project").Elements(ns + "ItemGroup").Elements(ns + "AdditionalFiles").Attributes("Include").First().Value = fullPath;
                        Console.WriteLine($"Updated project: {projectFile}");
                        changed = true;
                    }
                }

                if (changed)
                {
                    var xmlWriter = XmlWriter.Create(projectFile, new XmlWriterSettings { Indent = true });
                    projectXml.WriteTo(xmlWriter);
                    xmlWriter.Close();
                }
            }
        }
    }
}
