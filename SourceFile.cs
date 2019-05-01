/* ----------------------------------------------------------------------- *

    * SourceFile.cs

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
public class SourceFile
{
  // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
  public class Line
  {
    public SourceFile file {get; private set;}
    public string content {get; private set;}
    public int lineNumber {get; private set;}
    public bool empty => string.IsNullOrEmpty(content);

    // -----------------------------------------------------------------------
    internal Line(SourceFile file, string content, int lineNumber)
    {
      this.file = file;
      this.content = content;
      this.lineNumber = lineNumber;
    }
  }

  const string program = "Program";
  const string library = "Library";

  public string pathToFile {get; private set;}
  public List<Line> lines {get; private set;} = new List<Line>();

  // -------------------------------------------------------------------------
  public SourceFile(string pathToFile)
  {
    this.pathToFile = pathToFile;
    if (!File.Exists(pathToFile)) {
      throw new FileNotFoundException("File not found : " + pathToFile);
    }
  }
  
  // -------------------------------------------------------------------------
  public void read()
  {
    if (!File.Exists(pathToFile)) {
      throw new FileNotFoundException("File not found : " + pathToFile);
    }

    string [] fileContent = File.ReadAllLines(pathToFile);

    this.pathToFile = pathToFile;

    bool code = false;
    for (int l = 0; l < fileContent.Length; l++) {
      string line = fileContent[l];
      if (line.isEndOfRegion(program) || line.isEndOfRegion(library))
        code = false;

      if (code) {
        lines.Add(new Line(this, line, l));
      }

      if (line.isStartOfRegion(program) || line.isStartOfRegion(library))
        code = true;
    }
  }
}

} // End of namepsace th

