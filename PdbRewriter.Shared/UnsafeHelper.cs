using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PdbRewriter.Core
{
    public static class UnsafeHelper
    {
        public static unsafe long IndexOf(long startOffset, byte* haystack, long haystackLength, byte* needle, long needleLength)
        {
            var hNext = haystack + startOffset;
            var hEnd = haystack + haystackLength + 1 - needleLength;
            var nEnd = needle + needleLength;

            for (; hNext < hEnd; hNext++)
            {
                var hInc = hNext;
                var nInc = needle;
                for (; *nInc == *hInc; hInc++)
                {
                    if (++nInc == nEnd)
                    {
                        return hNext - haystack;
                    }
                }
            }

            return -1;
        }

        public static unsafe void Override(long startOffset, byte* bytesPointer, byte* rewrittenBytes, long rewrittenBytesLength)
        {
            var rInc = rewrittenBytes;
            var bInc = bytesPointer + startOffset;

            var rEnd = bInc + rewrittenBytesLength;

            for (; bInc < rEnd; bInc++, rInc++)
            {
                *bInc = *rInc;
            }
        }
    }
}
