// include Fake libs
#r "src/packages/FAKE/tools/FakeLib.dll"

open Fake
open Fake.Paket
open Fake.FileUtils
open Fake.Testing.XUnit2
open System

// Directories
let rootDir = currentDirectory
let buildDir  = currentDirectory + "/bin/"
let nugetDir = currentDirectory + "/nuget/"
let testOutputDir = currentDirectory + "/testResults/"

let nugetApiKey = environVar "BAMBOO_nugetApiKey"
let nugetPublishUrl = environVar "BAMBOO_nugetPublishUrl"
let nugetVersion = getBuildParamOrDefault "nugetVersion" null

// Filesets
let appReferences  =
    !! "src/**/*.csproj"

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; nugetDir; testOutputDir]
    MSBuildRelease buildDir "Clean" appReferences
        |> Log "Clean-Output: "
)

Target "BuildApp" (fun _ ->
    MSBuildRelease buildDir "Build" appReferences
        |> Log "BuildApp-Output: "
)

Target "Test" (fun _ ->
    
    let testAssemblies =     
        !! (buildDir @@ "/*Tests.dll")
    
    if testAssemblies |> Seq.isEmpty then
        traceError "No test assemblies found"
        ()
    else
        testAssemblies |> xUnit2 (fun p -> 
                    { p with 
                        NUnitXmlOutputPath = Some (testOutputDir + "testresults.xml");
                        ToolPath = @"src/packages/xunit.runner.console/tools/xunit.console.exe"
                    })
)


Target "Pack" (fun _ ->
    Pack (fun p ->
        let p' = {p with OutputPath = nugetDir; WorkingDir = rootDir + "/src" }
        if nugetVersion = null then p' else {p' with Version = nugetVersion }
        )
)

Target "Push" (fun _ ->
    Push (fun p ->
        {p with
            ApiKey = nugetApiKey
            PublishUrl = nugetPublishUrl
            WorkingDir = nugetDir
        })
)


// Build order
"Clean"
  ==> "BuildApp"
  ==> "Test"
  ==> "Pack"
  ==> "Push"

// start build
RunTargetOrDefault "Build"
