using BenchmarkDotNet.Attributes;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace CsHardwareIntrinsicsBenchmark
{
    public class CsSums
    {
        private const int _count = 1048576;
        private const double _max = int.MaxValue / (double)_count;
        private readonly static Random _rand = new Random();
        private readonly int[] _source = new int[_count];

        public CsSums()
        {
            for (var i = 0; i < _source.Length; i++)
            {
                _source[i] = _rand.Next((int)_max);
            }
        }

        [Benchmark]
        public int CsSimpleSum()
        {
            var result = 0;
            for (var i = 0; i < _source.Length; i++)
            {
                result += _source[i];
            }
            return result;
        }

        [Benchmark]
        public unsafe int CsSumVectorizedSse2LoadVector128()
        {
            int result;

            fixed (int* pSource = _source)
            {
                Vector128<int> vresult = Vector128<int>.Zero;

                int i = 0;
                int lastBlockIndex = _source.Length - (_source.Length % 4);

                while (i < lastBlockIndex)
                {
                    vresult = Sse2.Add(vresult, Sse2.LoadVector128(pSource + i));
                    i += 4;
                }

                if (Ssse3.IsSupported)
                {
                    vresult = Ssse3.HorizontalAdd(vresult, vresult);
                    vresult = Ssse3.HorizontalAdd(vresult, vresult);
                }
                else
                {
                    vresult = Sse2.Add(vresult, Sse2.Shuffle(vresult, 0x4E));
                    vresult = Sse2.Add(vresult, Sse2.Shuffle(vresult, 0xB1));
                }
                result = vresult.ToScalar();

                while (i < _source.Length)
                {
                    result += pSource[i];
                    i += 1;
                }
            }

            return result;
        }

        [Benchmark]
        public unsafe int CsSumVectorizedSse2UnsafeAs()
        {
            int result;

            Vector128<int> vresult = Vector128<int>.Zero;

            int i = 0;
            int lastBlockIndex = _source.Length - (_source.Length % 4);

            while (i < lastBlockIndex)
            {
                vresult = Sse2.Add(vresult, Unsafe.As<int, Vector128<int>>(ref _source[i]));
                i += 4;
            }

            if (Ssse3.IsSupported)
            {
                vresult = Ssse3.HorizontalAdd(vresult, vresult);
                vresult = Ssse3.HorizontalAdd(vresult, vresult);
            }
            else
            {
                vresult = Sse2.Add(vresult, Sse2.Shuffle(vresult, 0x4E));
                vresult = Sse2.Add(vresult, Sse2.Shuffle(vresult, 0xB1));
            }
            result = vresult.ToScalar();

            while (i < _source.Length)
            {
                result += _source[i];
                i += 1;
            }

            return result;
        }
    }
}
