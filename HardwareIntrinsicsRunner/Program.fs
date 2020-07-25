open BenchmarkDotNet.Running
open CsHardwareIntrinsicsBenchmark
open VBHardwareIntrinsicsBenchmark
open System.Runtime.Intrinsics.X86

[<EntryPoint>]
let main _ =
    match Sse2.IsSupported with
    | true -> 
        BenchmarkRunner.Run<CsSums>() |> ignore
        BenchmarkRunner.Run<VBSums>() |> ignore
    | false -> printfn "SSE2 not supported"

    0
