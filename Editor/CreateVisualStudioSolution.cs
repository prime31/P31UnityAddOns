using UnityEditor;
using UnityEngine;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;

public class CreateVisualStudioSolution
{
	private static void WriteFilesRecursively(DirectoryInfo dir, StreamWriter swr, string relPath) {
		DirectoryInfo[] dis = dir.GetDirectories();
        
        foreach (DirectoryInfo din in dis) {
       	   	//add a line for each .cs file found.
        	FileInfo[] fis = din.GetFiles("*.cs");
        	foreach (FileInfo fi in fis)
        	{
        	    string relative = relPath + fi.FullName.Substring(dir.FullName.Length+1);
        	    relative = relative.Replace("/", "\\");
        	    swr.WriteLine("     <Compile Include=\"AssetsLink\\"+relative+"\" />");
        	}
        	
        	//add a line for each shader found.
        	fis = din.GetFiles("*.shader");
        	foreach (FileInfo fi in fis)
        	{
            	string relative = relPath + fi.FullName.Substring(dir.FullName.Length + 1);
            	relative = relative.Replace("/", "\\");
            	swr.WriteLine("     <None Include=\"AssetsLink\\"+relative+"\" />");
        	}
        	
        	WriteFilesRecursively(din, swr, relPath + "\\" + din.Name + "\\");
        }
	}
	
    [MenuItem("CustomTools/CreateVisualStudioSolution")]
    public static void CreateIt()
    {
        string vs_root = Path.Combine(Directory.GetCurrentDirectory() , "VisualStudio");
        Directory.CreateDirectory(vs_root);

        //create symlink to assets folder.
        ProcessStartInfo t = new System.Diagnostics.ProcessStartInfo("ln", "-s ../Assets VisualStudio/AssetsLink");
        Process p = System.Diagnostics.Process.Start(t);
        p.WaitForExit();

        //write solution file out to disk.
        StreamWriter sw = new StreamWriter(Path.Combine(vs_root,GetProjectName() + ".sln"));
        sw.Write(GetSolutionText());
        sw.Close();

        //write first part of our project file.
        sw = new StreamWriter(Path.Combine(vs_root,GetProjectName() + ".csproj"));
        sw.Write(GetProjectFileHead());

        DirectoryInfo di = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(),"Assets"));
        WriteFilesRecursively(di, sw, "");
    
        //and write the tail of our projectfile.
        sw.Write(GetProjectFileTail());
        sw.Close();

        //Copy the required assemblies.
        System.Reflection.Assembly unityeditor = System.Reflection.Assembly.GetAssembly(typeof(EditorUtility));
        System.IO.File.Copy(unityeditor.Location, Path.Combine(vs_root,"UnityEditor.dll"), true);

        System.Reflection.Assembly unityengine = System.Reflection.Assembly.GetAssembly(typeof(GameObject));
        System.IO.File.Copy(unityengine.Location, Path.Combine(vs_root ,"UnityEngine.dll"), true);

        //Copy the .xml documentation of these assemblies
        string assetsfolder = Path.Combine(Directory.GetCurrentDirectory(),"Assets/Editor");
        di = new DirectoryInfo(assetsfolder);
        FileInfo[] fis;
        fis = di.GetFiles("UnityEngine.xml");
        if (fis.Length > 0) File.Copy(fis[0].FullName, Path.Combine(vs_root, "UnityEngine.xml"),true);
        fis = di.GetFiles("UnityEditor.xml");
        if (fis.Length > 0) File.Copy(fis[0].FullName, Path.Combine(vs_root, "UnityEditor.xml"),true);

    }

    static string GetProjectName()
    {
        DirectoryInfo d = new DirectoryInfo(Directory.GetCurrentDirectory());
        string projectname = d.Name;
        return projectname;
    }
    static string MyHash(string input)
    {
        byte[] bs = MD5.Create().ComputeHash(Encoding.Default.GetBytes(input));
        StringBuilder sb = new StringBuilder();
        foreach (byte b in bs)
            sb.Append(b.ToString("x2"));
        string s = sb.ToString();
        s = s.Substring(0, 8) + "-" + s.Substring(8, 4) + "-" + s.Substring(12, 4) + "-" + s.Substring(16, 4) + "-" + s.Substring(20, 12);
        return s.ToUpper();
    }
    static string GetProjectGUID()
    {
        return MyHash(GetProjectName() + "salt");
    }
    static string GetSolutionGUID()
    {
        return MyHash(GetProjectName());
    }


    static string GetSolutionText()
    {
        string t = @"Microsoft Visual Studio Solution File, Format Version 10.00
# Visual Studio 2008
Project(~{" + GetSolutionGUID() + @"}~) = ~" + GetProjectName() + @"~, ~" + GetProjectName() + @".csproj~, ~{" + GetProjectGUID() + @"}~
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{" + GetProjectGUID() + @"}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{" + GetProjectGUID() + @"}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{" + GetProjectGUID() + @"}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{" + GetProjectGUID() + @"}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal
";
        return t.Replace("~", "\"");
    }



    static string GetProjectFileHead()
    {
        string t = @"<?xml version=~1.0~ encoding=~utf-8~?>
<Project ToolsVersion=~3.5~ DefaultTargets=~Build~ xmlns=~http://schemas.microsoft.com/developer/msbuild/2003~>
  <PropertyGroup>
    <Configuration Condition=~ '$(Configuration)' == '' ~>Debug</Configuration>
    <Platform Condition=~ '$(Platform)' == '' ~>AnyCPU</Platform>
    <ProductVersion>9.0.21022</ProductVersion>
    <SchemaVersion>2.0</SchemaVersion>
    <ProjectGuid>{" + GetProjectGUID() + @"}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>Irrelevant</RootNamespace>
    <AssemblyName>Irrelvant</AssemblyName>
    <TargetFrameworkVersion>v3.5</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
  </PropertyGroup>
  <PropertyGroup Condition=~ '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ~>
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition=~ '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ~>
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=~System~ />
    <Reference Include=~System.Core~ />
    <Reference Include=~UnityEngine~>
      <HintPath>.\UnityEngine.dll</HintPath>
    </Reference>
    <Reference Include=~UnityEditor~>
      <HintPath>.\UnityEditor.dll</HintPath>
    </Reference>
  </ItemGroup>
  <ItemGroup>
";
        return t.Replace("~", "\"");
    }

    static string GetProjectFileTail()
    {
        string t = @"  </ItemGroup>
  <Import Project=~$(MSBuildToolsPath)\Microsoft.CSharp.targets~ />
  <!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
       Other similar extension points exist, see Microsoft.Common.targets.
  <Target Name=~BeforeBuild~>
  </Target>
  <Target Name=~AfterBuild~>
  </Target>
  -->
</Project>
    ";
        return t.Replace("~", "\"");
    }
}