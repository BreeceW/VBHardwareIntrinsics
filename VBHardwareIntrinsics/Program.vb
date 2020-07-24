Imports System.Runtime.Intrinsics
Imports System.Runtime.Intrinsics.X86

Module Program
    Const _count = 10000
    Const _max As Integer = Integer.MaxValue / _count
    ReadOnly _rand As Random = New Random()

    Sub Main(args As String())
        Dim numbers(_count - 1) As Integer
        For i = 0 To numbers.GetUpperBound(0)
            numbers(i) = _rand.Next(_max)
        Next

        Console.WriteLine("Simple: {0}", SumVectorizedSse2(numbers))
        If (Sse2.IsSupported) Then
            Console.WriteLine("  Sse2: {0}", SumVectorizedSse2(numbers))
        Else
            Console.WriteLine("  Sse2: Unsupported")
        End If
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
