﻿<?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="15.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">

  <Import Project="$(MSCommonProps)" Condition="Exists('$(MSCommonProps)')"/>
  <Import Project="User.props" Condition="Exists('User.props')" />

  <PropertyGroup>
    <MSCommonProps>$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props</MSCommonProps>
    <Platform>AMD64</Platform>
    <RootNamespace>SpaceEngineers</RootNamespace>
    <AssemblyName></AssemblyName>
    <TargetFrameworkVersion>v4.6.1</TargetFrameworkVersion>
    <OutputPath>bin</OutputPath>
  </PropertyGroup>

  <ItemGroup>
  </ItemGroup>

  <ItemGroup>
  </ItemGroup>

  <ItemGroup>
    <Reference Include="System"/>
    <Reference Include="System.Core"/>
    <Reference Include="System.Xml.Linq"/>
    <Reference Include="System.Data.DataSetExtensions" />
    <Reference Include="Microsoft.CSharp"/>
    <Reference Include="System.Data"/>
    <Reference Include="System.Net.Http"/>
    <Reference Include="System.Xml"/>

    <Reference Include="Sandbox.Common"/>
    <Reference Include="Sandbox.Game"/>
    <Reference Include="Sandbox.Graphics"/>
    <Reference Include="SpaceEngineers.Game"/>
    <Reference Include="SpaceEngineers.ObjectBuilders"/>
    <Reference Include="VRage"/>
    <Reference Include="VRage.Audio"/>
    <Reference Include="VRage.Game"/>
    <Reference Include="VRage.Input"/>
    <Reference Include="VRage.Library"/>
    <Reference Include="VRage.Math"/>
    <Reference Include="VRage.Render"/>
    <Reference Include="VRage.Render11"/>
    <Reference Include="VRage.Scripting"/>
  </ItemGroup>

  <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets"/>
  <PropertyGroup>
  <PBCBuild>
    <![CDATA[
      set PATH=$(Path);
      PBC.exe -b -f $(MSBuildThisFile)
    ]]>
  </PBCBuild>
  </PropertyGroup>

  <Target Name="Build">
    <Exec Command="$(PBCBuild)" />
  </Target>

</Project>
