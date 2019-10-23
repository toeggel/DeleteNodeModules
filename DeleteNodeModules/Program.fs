open System
open System.IO
open Microsoft.VisualBasic.FileIO

let rec getAllFolder dir pattern =
    seq {
        let matches = Directory.EnumerateDirectories(dir, pattern)
        yield! matches
        let subFolders = 
            Directory.EnumerateDirectories(dir) 
            |> Seq.filter (fun n -> matches |> Seq.contains n |> not) |> Seq.toList
        for subFolder in subFolders do
            yield! getAllFolder subFolder pattern }

[<EntryPoint>]
let main argv =
    Console.WriteLine("Search node_modules in any subfolder of " + Environment.CurrentDirectory)
    let nodeModules = getAllFolder Environment.CurrentDirectory "*node_modules"

    nodeModules |> Seq.iter (fun dir -> printfn "%s" dir)
    Console.WriteLine(String.Format("{0} node_modules found", (nodeModules |> Seq.length)))

    if nodeModules |> Seq.length <= 0 then
        Console.WriteLine("No node_modules found")
    else
        Console.WriteLine("Delete all folders listed above? (y/n)")
        let input = Console.ReadLine()

        match input with
            | "y" | "Y" -> 
                nodeModules |> Seq.iter (fun dir -> FileSystem.DeleteDirectory(dir, UIOption.AllDialogs, RecycleOption.SendToRecycleBin))
                printfn "Listed folders deleted"
            | _ -> 
                printfn "Nothing deleted"

    Console.WriteLine("Hit any key to exit")
    Console.ReadLine() |> ignore

    0 // return an integer exit code