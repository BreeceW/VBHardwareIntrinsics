Imports System.Runtime.Intrinsics
Imports System.Runtime.Intrinsics.X86

Module Program
    Const _count = 1048576
    Const _max As Integer = Integer.MaxValue / _count
    Const _iterations = 10000
    ReadOnly _rand As Random = New Random()

    Sub Main(args As String())
        If Not Sse2.IsSupported Then
            Console.WriteLine("SSE2 not supported")
            Exit Sub
        End If

        Dim numbers(_count - 1) As Integer
        For i = 0 To numbers.GetUpperBound(0)
            numbers(i) = _rand.Next(_max)
        Next

        Dim watch As Stopwatch

        Dim sum1 As Integer
        watch = Stopwatch.StartNew()
        For i = 1 To _iterations
            sum1 = SimpleSum(numbers)
        Next
        watch.Stop()
        Dim d1 = watch.ElapsedMilliseconds / _count

        Dim sum2 As Integer
        watch = Stopwatch.StartNew()
        For i = 1 To _iterations
            sum2 = SumVectorizedSse2(numbers)
        Next
        watch.Stop()
        Dim d2 = watch.ElapsedMilliseconds / _count

        Console.ForegroundColor = If(d1 <= d2, ConsoleColor.Green, ConsoleColor.Red)
        Console.WriteLine("Simple: {0}, took {1} ms", sum1, d1)
        Console.ForegroundColor = If(d2 <= d1, ConsoleColor.Green, ConsoleColor.Red)
        Console.WriteLine("  SSE2: {0}, took {1} ms", sum2, d2)
        Console.ResetColor()
    End Sub

    Function SimpleSum(source As Integer()) As Integer
        Dim result = 0
        For i = 0 To source.GetUpperBound(0)
            result += source(i)
        Next
        SimpleSum = result
    End Function

    Function SumVectorizedSse2(source As Integer()) As Integer
        Dim result As Integer

        Dim vresult = Vector128(Of Integer).Zero
        Dim i = 0
        Dim lastBlockIndex = source.Length - (source.Length Mod 4)

        While i < lastBlockIndex
            vresult = Sse2.Add(vresult, Vector128.Create(source(i), source(i + 1), source(i + 2), source(i + 3)))
            i += 4
        End While

        If Ssse3.IsSupported Then
            vresult = Ssse3.HorizontalAdd(vresult, vresult)
            vresult = Ssse3.HorizontalAdd(vresult, vresult)
        Else
            vresult = Sse2.Add(vresult, Sse2.Shuffle(vresult, &H4E))
            vresult = Sse2.Add(vresult, Sse2.Shuffle(vresult, &HB1))
        End If
        result = vresult.ToScalar()

        While i < source.Length
            result += source(i)
            i += 1
        End While

        SumVectorizedSse2 = result
    End Function
End Module
