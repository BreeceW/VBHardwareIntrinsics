Imports BenchmarkDotNet.Attributes
Imports System.Runtime.CompilerServices
Imports System.Runtime.Intrinsics
Imports System.Runtime.Intrinsics.X86

Public Class VBSums
    Private Const _count = 1048576
    Private Const _max As Integer = Integer.MaxValue / _count
    Private ReadOnly _rand As Random = New Random()
    Private ReadOnly _source(_count - 1) As Integer

    Public Sub New()
        For i = 0 To _source.GetUpperBound(0)
            _source(i) = _rand.Next(_max)
        Next
    End Sub

    <Benchmark>
    Public Function VBSimpleSum() As Integer
        Dim result = 0
        For i = 0 To _source.GetUpperBound(0)
            result += _source(i)
        Next
        VBSimpleSum = result
    End Function

    <Benchmark>
    Public Function VBSumVectorizedSse2Vector128Create() As Integer
        Dim result As Integer

        Dim vresult = Vector128(Of Integer).Zero
        Dim i = 0
        Dim lastBlockIndex = _source.Length - (_source.Length Mod 4)

        While i < lastBlockIndex
            vresult = Sse2.Add(vresult, Vector128.Create(_source(i), _source(i + 1), _source(i + 2), _source(i + 3)))
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

        While i < _source.Length
            result += _source(i)
            i += 1
        End While

        VBSumVectorizedSse2Vector128Create = result
    End Function

    <Benchmark>
    Public Function VBSumVectorizedSse2UnsafeAs() As Integer
        Dim result As Integer

        Dim vresult = Vector128(Of Integer).Zero
        Dim i = 0
        Dim lastBlockIndex = _source.Length - (_source.Length Mod 4)

        While i < lastBlockIndex
            vresult = Sse2.Add(vresult, Unsafe.As(Of Integer, Vector128(Of Integer))(_source(i)))
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

        While i < _source.Length
            result += _source(i)
            i += 1
        End While

        VBSumVectorizedSse2UnsafeAs = result
    End Function
End Class
