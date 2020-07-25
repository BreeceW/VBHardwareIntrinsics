# Hardware Intrinsics in VB.NET
This repository serves to prove the point that VB.NET does, in fact, support hardware intrinsics. There are benchmarks to see how it compares to C#, though it looks like VB.NET does not support ReadOnlySpan and certainly not unsafe code. VB.NET is roughly on-par with C# when both use `Unsafe.As` to convert from the integer array to Vector128. VB.NET is unable to use `Sse2.LoadVector128`, so it is likely impossible to achieve comparable performance.

Adapted from https://devblogs.microsoft.com/dotnet/hardware-intrinsics-in-net-core/
