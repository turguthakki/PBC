/* ----------------------------------------------------------------------- *

    * PBC.cs

    ----------------------------------------------------------------------

    Copyright (C) 2016, Turgut Hakki Ozdemir
    All Rights Reserved.

    THIS SOFTWARE IS PROVIDED 'AS-IS', WITHOUT ANY EXPRESS
    OR IMPLIED WARRANTY. IN NO EVENT WILL THE AUTHOR(S) BE HELD LIABLE FOR
    ANY DAMAGES ARISING FROM THE USE OR DISTRIBITION OF THIS SOFTWARE

    Turgut Hakkı Özdemir <turgut.hakki@gmail.com>

* ------------------------------------------------------------------------ */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace th {

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
public static class PBC
{
  static bool verbose = false;

  // -------------------------------------------------------------------------
  static void printHelpString()
  {
    Console.WriteLine(
      String.Join(Environment.NewLine, 
        " This here",
        " is the help string"
      )
    );
  }

  // -------------------------------------------------------------------------
  static void processArguments(Queue<string> args)
  {
    while(args.Count > 0) {
      string arg = args.Dequeue();

      switch(arg) {
        case "-v" : {
          verbose = true;
        } break;


        case "-h" : 
        case "--help" :
        case "/h" :
        case "/?" :
        case "/help" : {
          printHelpString(); 
        } return;

        case "-g" :
        case "--generate" : {
          new ProjectGenerator(args).generate(); 
        } return;

        case "-b" : 
        case "--build" : {
          ProjectBuilder projectBuilder = new ProjectBuilder(args);

          projectBuilder.build();

        } return;
        
        case "-c" :
        case "--compile" : {
          Compiler compiler = new Compiler(args); 

          compiler.readSourceFiles();
          compiler.compile();
          compiler.writeOutput();

        } return;
      }
    }
  }

  // -------------------------------------------------------------------------
  static void Main(string[] args)
  {
    try {
      if (args.Length == 0) {
        throw new ArgumentException("No arguments given");
      }
      processArguments(new Queue<string>(args));
    }
    catch(System.Exception e) {
      if (verbose) {
        Console.Error.WriteLine("Error : " + e.Message + Environment.NewLine + e.Message + e.StackTrace + Environment.NewLine + "Type /? for help.");
      }
      else {
        Console.Error.WriteLine("Error : " + Environment.NewLine+ e.Message + Environment.NewLine + "\nType /? for help.");
      }
    }   
  }

}
} // End of namepsace th