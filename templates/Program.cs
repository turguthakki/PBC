﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using VRage;
using VRageMath;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;

using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;

namespace SpaceEngineers {
  partial class Program : MyGridProgram 
  {
    #region Program
    
    // -----------------------------------------------------------------------
    public Program()
    {
      // The constructor, called only once every session and
      // always before any other method is called. Use it to
      // initialize your script. 
      //     
      // The constructor is optional and can be removed if not
      // needed.
      // 
      // It's recommended to set RuntimeInfo.UpdateFrequency 
      // here, which will allow your script to run itself without a 
      // timer block.
    }

    // -----------------------------------------------------------------------
    public void Save()
    {
      // Called when the program needs to save its state. Use
      // this method to save your state to the Storage field
      // or some other means. 
      // 
      // This method is optional and can be removed if not
      // needed.
    }

    // -----------------------------------------------------------------------
    public void Main(string argument, UpdateType updateSource)
    {
      // The main entry point of the script, invoked every time
      // one of the programmable block's Run actions are invoked,
      // or the script updates itself. The updateSource argument
      // describes where the update came from.
      // 
      // The method itself is required, but the arguments above
      // can be removed if not needed.
    }

    #endregion Program
  }
} // End of namespace SpaceEngineers
