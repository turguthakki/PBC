/* ----------------------------------------------------------------------- *

    * Utils.cs

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
public static class Utils
{
  // -------------------------------------------------------------------------
  public static bool empty(this string s)
  {
    return string.IsNullOrEmpty(s);
  }

  // -------------------------------------------------------------------------
  public static bool isStartOfRegion(this string line, string region)
  {
    line = line.Trim();
    if (line.StartsWith("#region")) {
      string[] tokens = line.Split(' ');
      return tokens.Length == 2 && tokens[1] == region;
    }
    return false;
  }

  // -------------------------------------------------------------------------
  public static bool isEndOfRegion(this string line, string region)
  {
    line = line.Trim();
    if (line.StartsWith("#endregion")) {
      string[] tokens = line.Split(' ');
      return tokens.Length == 2 && tokens[1] == region;
    }
    return false;
  }
  // -------------------------------------------------------------------------
  public static string sanitizeFileName(this string fileName)
  {
    return string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
  }

  // -------------------------------------------------------------------------
  public static string nextArg(this Queue<string> args)
  {
    if (args.Count <= 0)
      return null;
    return args.Dequeue();
  }

}

} // End of namepsace th

