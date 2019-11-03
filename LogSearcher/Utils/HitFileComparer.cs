using LogSearcher.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace LogSearcher.Utils
{
    public class HitFileComparer : IEqualityComparer<HitFile>
    {
        public bool Equals([AllowNull] HitFile x, [AllowNull] HitFile y)
        {
            return x.FilePathAndName == y.FilePathAndName;
        }

        public int GetHashCode([DisallowNull] HitFile obj)
        {
            return obj.FilePathAndName.GetHashCode();
        }
    }

}
