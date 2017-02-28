// --------------------------------------------------------------------------------------
// FAKE build script
// --------------------------------------------------------------------------------------

#r @"packages/build/FAKE/tools/FakeLib.dll"
#r "System.IO.Compression.FileSystem"

open Fake
open Fake.Git
open Fake.AssemblyInfoFile
open Fake.ReleaseNotesHelper
open System
open System.IO
open Fake.Testing.Expecto


let project = "HelloWorld sample"

let summary = "HelloWorld sample"

let description = summary

let configuration = "Release"

let clientPath = "./src/Client" |> FullName

let dotnetcliVersion = "1.0.0-rc4-004771"

let dotnetSDKPath = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) </> "dotnetcore" |> FullName

let dotnetExePath =
    dotnetSDKPath </> (if isWindows then "dotnet.exe" else "dotnet")
    |> FullName

let deployDir = "./deploy"


// Pattern specifying assemblies to be tested using expecto
let testExecutables = "test/**/bin/Release/*Tests*.exe"

// --------------------------------------------------------------------------------------
// END TODO: The rest of the file includes standard build steps
// --------------------------------------------------------------------------------------

let run' timeout cmd args dir =
    if execProcess (fun info ->
        info.FileName <- cmd
        if not (String.IsNullOrWhiteSpace dir) then
            info.WorkingDirectory <- dir
        info.Arguments <- args
    ) timeout |> not then
        failwithf "Error while running '%s' with args: %s" cmd args

let run = run' System.TimeSpan.MaxValue

let platformTool tool winTool =
    let tool = if isUnix then tool else winTool
    tool
    |> ProcessHelper.tryFindFileOnPath
    |> function Some t -> t | _ -> failwithf "%s not found" tool 

let nodePath = platformTool "node" "node.exe"

let npmTool = platformTool "npm" "npm.cmd"

// Read additional information from the release notes document
let release = LoadReleaseNotes "RELEASE_NOTES.md"
let packageVersion = SemVerHelper.parse release.NugetVersion

// Helper active pattern for project types
let (|Fsproj|Csproj|Vbproj|Shproj|) (projFileName:string) =
    match projFileName with
    | f when f.EndsWith("fsproj") -> Fsproj
    | f when f.EndsWith("csproj") -> Csproj
    | f when f.EndsWith("vbproj") -> Vbproj
    | f when f.EndsWith("shproj") -> Shproj
    | _                           -> failwith (sprintf "Project file %s not supported. Unknown project type." projFileName)

// Generate assembly info files with the right version & up-to-date information
Target "AssemblyInfo" (fun _ ->
    let getAssemblyInfoAttributes projectName =
        [ Attribute.Title (projectName)
          Attribute.Product project
          Attribute.Description summary
          Attribute.Version release.AssemblyVersion
          Attribute.FileVersion release.AssemblyVersion
          Attribute.Configuration configuration ]

    let getProjectDetails projectPath =
        let projectName = System.IO.Path.GetFileNameWithoutExtension(projectPath)
        ( projectPath,
          projectName,
          System.IO.Path.GetDirectoryName(projectPath),
          (getAssemblyInfoAttributes projectName)
        )

    !! "src/**/*.??proj"
    |> Seq.map getProjectDetails
    |> Seq.iter (fun (projFileName, projectName, folderName, attributes) ->
        match projFileName with
        | Fsproj -> CreateFSharpAssemblyInfo (folderName </> "AssemblyInfo.fs") attributes
        | Csproj -> CreateCSharpAssemblyInfo ((folderName </> "Properties") </> "AssemblyInfo.cs") attributes
        | Vbproj -> CreateVisualBasicAssemblyInfo ((folderName </> "My Project") </> "AssemblyInfo.vb") attributes
        | Shproj -> ()
        )
)

// --------------------------------------------------------------------------------------
// Clean build results

Target "Clean" (fun _ ->
    CleanDirs ["bin"; "temp"; "docs/output"; deployDir; Path.Combine(clientPath,"public/bundle")]
)


Target "InstallDotNetCore" (fun _ ->
    let correctVersionInstalled = 
        try
            if FileInfo(dotnetExePath |> Path.GetFullPath).Exists then
                let processResult = 
                    ExecProcessAndReturnMessages (fun info ->  
                    info.FileName <- dotnetExePath
                    info.WorkingDirectory <- Environment.CurrentDirectory
                    info.Arguments <- "--version") (TimeSpan.FromMinutes 30.)

                processResult.Messages |> separated "" = dotnetcliVersion
                
            else
                false
        with 
        | _ -> false

    if correctVersionInstalled then
        tracefn "dotnetcli %s already installed" dotnetcliVersion
    else
        CleanDir dotnetSDKPath
        let archiveFileName = 
            if isWindows then
                sprintf "dotnet-dev-win-x64.%s.zip" dotnetcliVersion
            elif isLinux then
                sprintf "dotnet-dev-ubuntu-x64.%s.tar.gz" dotnetcliVersion
            else
                sprintf "dotnet-dev-osx-x64.%s.tar.gz" dotnetcliVersion
        let downloadPath = sprintf "https://dotnetcli.azureedge.net/dotnet/Sdk/%s/%s" dotnetcliVersion archiveFileName
        let localPath = Path.Combine(dotnetSDKPath, archiveFileName)

        tracefn "Installing '%s' to '%s'" downloadPath localPath
        
        let proxy = Net.WebRequest.DefaultWebProxy
        proxy.Credentials <- Net.CredentialCache.DefaultCredentials
        use webclient = new Net.WebClient(Proxy = proxy)
        webclient.DownloadFile(downloadPath, localPath)

        if not isWindows then
            let assertExitCodeZero x =
                if x = 0 then () else
                failwithf "Command failed with exit code %i" x

            Shell.Exec("tar", sprintf """-xvf "%s" -C "%s" """ localPath dotnetSDKPath)
            |> assertExitCodeZero
        else  
            System.IO.Compression.ZipFile.ExtractToDirectory(localPath, dotnetSDKPath)
        
        tracefn "dotnet cli path - %s" dotnetSDKPath
        System.IO.Directory.EnumerateFiles dotnetSDKPath
        |> Seq.iter (fun path -> tracefn " - %s" path)
        System.IO.Directory.EnumerateDirectories dotnetSDKPath
        |> Seq.iter (fun path -> tracefn " - %s%c" path System.IO.Path.DirectorySeparatorChar)

    let oldPath = System.Environment.GetEnvironmentVariable("PATH")
    System.Environment.SetEnvironmentVariable("PATH", sprintf "%s%s%s" dotnetSDKPath (System.IO.Path.PathSeparator.ToString()) oldPath)
)

// --------------------------------------------------------------------------------------
// Build library & test project

Target "InstallClient" (fun _ ->
    run npmTool "install" ""
)

Target "BuildClient" (fun _ ->
    run npmTool "run build" clientPath
)


let vsProjProps = 
#if MONO
    [ ("DefineConstants","MONO"); ("Configuration", configuration) ]
#else
    [ ("Configuration", configuration); ("Platform", "Any CPU") ]
#endif

Target "BuildTests" (fun _ ->
    !! "./Tests.sln"
    |> MSBuildReleaseExt "" vsProjProps "Rebuild"
    |> ignore
)

// --------------------------------------------------------------------------------------
// Rename driver for macOS or Linux

Target "RenameDrivers" (fun _ ->
    if not isWindows then
        run npmTool "install phantomjs" ""
    if isMacOS && not <| File.Exists "test/UITests/bin/Release/chromedriver" then
        Fake.FileHelper.Rename "test/UITests/bin/Release/chromedriver" "test/UITests/bin/Release/chromedriver_macOS"
    elif isLinux then
        Fake.FileHelper.Rename "test/UITests/bin/Release/chromedriver" "test/UITests/bin/Release/chromedriver_linux64"    
)


// --------------------------------------------------------------------------------------
// Run the Website

let ipAddress = "localhost"
let port = 8080


// --------------------------------------------------------------------------------------
// Release Scripts



// --------------------------------------------------------------------------------------
// Run all targets by default. Invoke 'build <Target>' to override

Target "All" DoNothing

"Clean"
  ==> "InstallDotNetCore"
  ==> "InstallClient"
  ==> "AssemblyInfo"
  ==> "BuildClient"
  ==> "BuildTests"
  ==> "RenameDrivers"
  ==> "All"

RunTargetOrDefault "All"
