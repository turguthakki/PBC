/* ----------------------------------------------------------------------- *

    * Compiler.cs

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

namespace th {

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
public class Compiler
{
  public string outputDir {get; protected set;} = null;
  public string name {get; protected set;} = null;
  public string thumb {get; protected set;} = null;

  public List<SourceFile> sourceFiles {get; protected set;} = new List<SourceFile>();
  public List<SourceFile.Line> lines {get; protected set;} = new List<SourceFile.Line>();

  // -----------------------------------------------------------------------
  public Compiler(Queue<string> args) 
  {
    while(args.Count > 0) {
      string arg = args.Dequeue();
      switch(arg) {
        case "-o" :
        case "--output-dir" :  {
          outputDir = args.nextArg();
        } break;
        case "-n" :
        case "--name" : {
          name = args.nextArg();
        } break;
        case "-t" :
        case "--thumb" : {
          thumb = args.nextArg();
        } break;
        default : {
          sourceFiles.Add(new SourceFile(Path.GetFullPath(arg))); 
        } break;
      }
    }

    if ((outputDir.empty() && name.empty()) || (!outputDir.empty() && !name.empty())) {
      throw new ArgumentException("Either script name or output directory must be specified.");
    }

    if (!name.empty()) {
      string scriptsDir = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\SpaceEngineers\\IngameScripts\\local");
      if (!Directory.Exists(scriptsDir)) {
        throw new DirectoryNotFoundException("Scripts directory not found. Are you sure the game is installed?");
      }
      outputDir = Path.GetFullPath(scriptsDir + Path.DirectorySeparatorChar + name);
    }

    if (sourceFiles.Count <= 0)
      throw new ArgumentException("No source files given");
  }

  // -------------------------------------------------------------------------
  public void readSourceFiles()
  {
    foreach(SourceFile file in sourceFiles) {
      Console.WriteLine("Reading source : " + file.pathToFile);
      file.read();
    }
  }

  // -------------------------------------------------------------------------
  public void compile()
  {
    Console.WriteLine("Compiling script");
    foreach(SourceFile file in sourceFiles) {
      lines.AddRange(file.lines);
    }
  }

  // -------------------------------------------------------------------------
  public void writeOutput()
  {
    Console.WriteLine("Writing files");
    string []lines = new string[this.lines.Count];

    for (int i = 0; i < lines.Length; i++)
      lines[i] = this.lines[i].content;

    Directory.CreateDirectory(outputDir);
    File.WriteAllText(outputDir + Path.DirectorySeparatorChar + "script.cs", string.Join(Environment.NewLine, lines));

    if (!thumb.empty()) {
      File.Copy(thumb, outputDir + Path.DirectorySeparatorChar + "thumb.png", true);
    }
    Console.WriteLine("Done");
  }
}

} // End of namepsace th