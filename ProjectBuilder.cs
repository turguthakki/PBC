/* ----------------------------------------------------------------------- *

    * ProjectBuilder.cs

    ----------------------------------------------------------------------

    Copyright (C) 2019, Turgut Hakki Ozdemir
    All Rights Reserved.

    THIS SOFTWARE IS PROVIDED 'AS-IS', WITHOUT ANY EXPRESS
    OR IMPLIED WARRANTY. IN NO EVENT WILL THE AUTHOR(S) BE HELD LIABLE FOR
    ANY DAMAGES ARISING FROM THE USE OR DISTRIBITION OF THIS SOFTWARE

    Permission is granted to anyone to use this software for any purpose,
    including commercial applications, and to alter it and redistribute it
    freely, subject to the following restrictions:

    1. The origin of this software must not be misrepresented; you must not
       claim that you wrote the original software.
    2. Altered source versions must be plainly marked as such, and must not be
       misrepresented as being the original software.
    4. Distributions in binary form must reproduce the above copyright notice
       and an acknowledgment of use of this software either in documentation
       or binary form itself.
    3. This notice may not be removed or altered from any source distribution.

    Turgut Hakkı Özdemir <turgut.hakki@gmail.com>

* ------------------------------------------------------------------------ */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Construction;

namespace th {

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
public class ProjectBuilder
{
  public string projectFile {get; private set;}
  public string name {get; private set;}
  public string outputDir {get; protected set;} = null;
  public string thumb {get; protected set;} = null;

  Compiler compiler;
  List<string> files = new List<string>();

  // -------------------------------------------------------------------------
  public ProjectBuilder(Queue<string> args)
  {
    while(args.Count > 0) {
      string arg = args.Dequeue();
      switch(arg) {
        case "-f" :
        case "--project-file" :  {
          projectFile = args.nextArg();
        } break;
        case "-n" :
        case "--name" : {
          name = args.nextArg();
        } break;
        case "-o" :
        case "--output-dir" :  {
          outputDir = args.nextArg();
        } break;
        case "-t" :
        case "--thumb" : {
          thumb = args.nextArg();
        } break;
      }
    }

    if (projectFile.empty()) {
      throw new ArgumentException("Project file must be specified.");
    }

    readProject(projectFile);

    List<string> compilerParams = new List<String>();

    if (outputDir.empty()) {
      compilerParams.Add("-n");
      compilerParams.Add(name);
    }
    else {
      compilerParams.Add("-o");
      compilerParams.Add(outputDir);
    }

    if (!thumb.empty()) {
      compilerParams.Add("-t");
      compilerParams.Add(thumb);
    }

    compilerParams.AddRange(files);
    compiler = new Compiler(new Queue<string>(compilerParams));
  }

  // -------------------------------------------------------------------------
  public void readProject(string projectFile, bool mainProject = true)
  {
    Project project;
    project = new Project(projectFile);
    project.ReevaluateIfNecessary();

    if (mainProject) {
      if (name.empty()) {
        foreach(ProjectProperty property in project.Properties) {
          if (property.Name == "AssemblyName")
            name = property.EvaluatedValue;
        }
      }

      if (thumb.empty()) {
        foreach(ProjectProperty property in project.Properties) {
          if (property.Name == "AssemblyName")
            name = property.EvaluatedValue;
        }
      }
    }
   
    foreach(ProjectItem item in project.Items) {
      if (item.ItemType == "Compile") {
        string path = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(item.Xml.ContainingProject.FullPath), item.EvaluatedInclude));
        if (path.empty() || Path.GetExtension(path).ToLowerInvariant() != ".cs")
          continue;
        files.Add(path);  
      }
      else if (item.ItemType == "ProjectReference") {
        string path = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(item.Xml.ContainingProject.FullPath), item.EvaluatedInclude));
        Console.WriteLine("Project file : " + path);
        readProject(path, false);
      }
      else if (thumb.empty() && item.ItemType == "AdditionalFiles" && item.HasMetadata("PBC") && item.GetMetadataValue("PBC").ToLowerInvariant() == "thumbnail") {
        thumb = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(item.Xml.ContainingProject.FullPath), item.EvaluatedInclude));
      }
    }
  }

  // -------------------------------------------------------------------------
  public void build()
  {
    compiler.readSourceFiles();
    compiler.compile();    
    compiler.writeOutput();
  }
}

} // End of namepsace th

