/* ----------------------------------------------------------------------- *

    * ProjectGenerator.cs

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
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using Microsoft.Win32;

namespace th {

// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
public class ProjectGenerator
{
  const string resourcePrefix = "th.templates.";
  static Dictionary<string, string> assemblies = new Dictionary<string, string>() {
    {"Sandbox.Common", "Sandbox.Common.dll"},
    {"Sandbox.Game", "Sandbox.Game.dll"},
    {"Sandbox.Graphics", "Sandbox.Graphics.dll"},
    {"SpaceEngineers.Game", "SpaceEngineers.Game.dll"},
    {"SpaceEngineers.ObjectBuilders", "SpaceEngineers.ObjectBuilders.dll"},
    {"VRage", "VRage.dll"},
    {"VRage.Audio", "VRage.Audio.dll"},
    {"VRage.Game", "VRage.Game.dll"},
    {"VRage.Input", "VRage.Input.dll"},
    {"VRage.Library", "VRage.Library.dll"},
    {"VRage.Math", "VRage.Math.dll"},
    {"VRage.Render", "VRage.Render.dll"},
    {"VRage.Render11", "VRage.Render11.dll"},
    {"VRage.Scripting", "VRage.Scripting.dll"},
  };

  Assembly assembly;
  public string userName {get; protected set;} = null;
  public string steamExe {get; protected set;} = null;
  public string steamPath {get; protected set;} = null;
  public string gamePath {get; protected set;} = null;
  public string seBinPath {get; protected set;} = null;
  public string thumbnailPath {get; protected set;} = null;

  public string programName {get; protected set;} = null;
  public string outputDir {get; protected set;} = null;
  public string sanitizedProgramName {get; protected set;} = null;

  public List<string> sourceFiles {get; protected set;} = new List<string>();
  public List<string> additionalFiles {get; protected set;} = new List<string>();

  public bool toggleGenerateThumbnail = true;

  // -------------------------------------------------------------------------
  static XElement newElement(XNamespace ns, string str, params object[] arguments)
  {
    XElement rv = XElement.Parse(String.Format(str, arguments));
    foreach(XNode node in rv.DescendantNodesAndSelf()) {
      if (node is XElement) {
        XElement element = node as XElement;
        element.Name = ns + element.Name.LocalName;
      }
    }
    return rv;
  }

  // -------------------------------------------------------------------------
  static void writeFormattedXMLDocument(string path, XDocument document)
  {
    XmlWriterSettings settings = new XmlWriterSettings();

    settings.Indent = true;
    settings.IndentChars = "  ";
    XmlWriter writer = XmlWriter.Create(path, settings);
    document.WriteTo(writer);  
    writer.Flush();
    writer.Close();
  }

  // -------------------------------------------------------------------------
  public ProjectGenerator(Queue<string> args)
  {
    assembly = Assembly.GetExecutingAssembly();

    while(args.Count > 0) {
      string arg = args.Dequeue();
      switch(arg) {
        case "-o" :
        case "--output-dir" :  {
          outputDir = args.nextArg() + Path.DirectorySeparatorChar;
        } break;
        case "-n" :
        case "--program-name" :  {
          programName = args.nextArg();
        } break;
        case "--no-thumb" : {
          toggleGenerateThumbnail = false;
        } break;
        case "--game-path" : {
          gamePath = args.nextArg();
        } break;
        default : {
        } break;
      }
    }

    userName = Environment.UserName.sanitizeFileName();

    foreach(string keyRoot in new string[] {"HKEY_CURRENT_USER", "HKEY_LOCAL_MACHINE"}) {
      steamExe = Path.GetFullPath((string) Registry.GetValue(keyRoot + @"\Software\Valve\Steam", "SteamExe", ""));
      steamPath = Path.GetFullPath((string) Registry.GetValue(keyRoot + @"\Software\Valve\Steam", "SteamPath", ""));
      if (!steamExe.empty() && !steamPath.empty())
        break;
    }

    Console.WriteLine("Steam found at : " + steamPath);

    if (gamePath == null && steamPath == null) {
      throw new DirectoryNotFoundException("Cannot determine where the game is installed. user --game-path to set it's location");
    }
    else if (gamePath == null) {
      gamePath = Path.GetFullPath(steamPath + @"\SteamApps\common\SpaceEngineers");
      Console.WriteLine("Assuming game location : " + gamePath);
    }
    else {
      gamePath = Path.GetFullPath(gamePath);
      Console.WriteLine("Using given game location : " + gamePath);
    }

    seBinPath = Path.GetFullPath(gamePath + @"Bin64\");
    thumbnailPath = Path.GetFullPath(gamePath + @"\Content\Textures\GUI\Icons\IngameProgrammingIcon.png");

    if (programName.empty()) {
      throw new ArgumentException("Program name is not given");
    }

    sanitizedProgramName = programName.sanitizeFileName();
    sourceFiles.Add(sanitizedProgramName + ".cs");
  }

  // -------------------------------------------------------------------------
  void generateProjectFile()
  {
    string filePath = Path.GetFullPath(Path.Combine(outputDir, sanitizedProgramName + ".csproj"));
    Console.WriteLine("Generating project file : " + filePath);

    XDocument doc = XDocument.Load(assembly.GetManifestResourceStream(resourcePrefix + "Program.csproj.inc"));
    XNamespace ns = doc.Root.GetDefaultNamespace();
    XmlNamespaceManager nsm = new XmlNamespaceManager(new NameTable());
    nsm.AddNamespace("mb", "http://schemas.microsoft.com/developer/msbuild/2003");

    XElement localProps = doc.XPathSelectElement(@"/mb:Project/mb:Import[1]", nsm);

    XElement assemblyName = doc.XPathSelectElement(@"/mb:Project/mb:PropertyGroup/mb:AssemblyName", nsm);
    assemblyName.Add(programName);

    XElement sourceFilesGroup = doc.XPathSelectElement(@"/mb:Project/mb:ItemGroup[1]", nsm);
    foreach(string sourceFile in sourceFiles) 
      sourceFilesGroup.Add(newElement(ns, @"<Compile Include=""{0}""/>", sourceFile));

    XElement additionalFilesGroup = doc.XPathSelectElement(@"/mb:Project/mb:ItemGroup[2]", nsm);

    if (toggleGenerateThumbnail) 
      additionalFilesGroup.Add(newElement(ns, @"<AdditionalFiles Include=""{0}""><PBC>Thumbnail</PBC></AdditionalFiles>", "thumb.png"));

    foreach(string additionalFile in additionalFiles) 
      additionalFilesGroup.Add(newElement(ns, @"<AdditionalFiles Include=""{0}""/>", additionalFile));

    writeFormattedXMLDocument(filePath, doc);
  }

  // -------------------------------------------------------------------------
  void generateUserProperties()
  {
    string filePath = Path.GetFullPath(Path.Combine(outputDir, "User.props"));
    Console.WriteLine("Generating user properties file : " + filePath);

    XDocument doc = XDocument.Load(assembly.GetManifestResourceStream(resourcePrefix + "User.props"));
    XNamespace ns = doc.Root.GetDefaultNamespace();
    XmlNamespaceManager nsm = new XmlNamespaceManager(new NameTable());
    nsm.AddNamespace("mb", "http://schemas.microsoft.com/developer/msbuild/2003");
  
    XElement assemblySearchPaths = doc.XPathSelectElement(@"/mb:Project/mb:PropertyGroup/mb:AssemblySearchPaths", nsm);
    assemblySearchPaths.Add(seBinPath + ";$(AssemblySearchPaths)");

    XElement path = doc.XPathSelectElement(@"/mb:Project/mb:PropertyGroup/mb:Path", nsm);
    path.Add(AppDomain.CurrentDomain.BaseDirectory + ";$(PATH)");

    writeFormattedXMLDocument(filePath, doc);
  }

  // -------------------------------------------------------------------------
  public void copyThumbnail()
  {
    string filePath = Path.GetFullPath(Path.Combine(outputDir, "thumb.png"));
    Console.WriteLine("Copying thumbnail to " + filePath);
    File.Copy(thumbnailPath, outputDir + Path.DirectorySeparatorChar + "thumb.png", true);
  }

  // -------------------------------------------------------------------------
  string readResouceTextFile(string name)
  {
    return new StreamReader(assembly.GetManifestResourceStream(resourcePrefix + name)).ReadToEnd();
  }

  // -------------------------------------------------------------------------
  void writeResourceToFile(string name, string output)
  {
    Console.WriteLine("Writing " + Path.GetFullPath(output));
    Stream istream = assembly.GetManifestResourceStream(resourcePrefix + name);
    Stream ostream = File.Create(output);
    istream.CopyTo(ostream);   
    ostream.Close();   
    istream.Close();
  }

  // -------------------------------------------------------------------------
  public void generate()
  {
    Directory.CreateDirectory(outputDir);
    writeResourceToFile("Program.cs", outputDir + Path.DirectorySeparatorChar + sanitizedProgramName + ".cs");
    generateProjectFile();
    generateUserProperties();
    if (toggleGenerateThumbnail) {
      copyThumbnail();
    }
    Console.WriteLine("Done");
  }
}

} // End of namepsace th

