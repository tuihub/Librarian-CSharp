using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.Porter.Constants
{
    public static class WellKnownAppInfoSource
    {
        public const string Unspecified = "";
        public const string Steam = "steam";
        public const string Vndb = "vndb";
        public const string Bangumi = "bangumi";

        public static TuiHub.Protos.Librarian.V1.WellKnownAppInfoSource ToProto(this string source)
        {
            return source switch
            {
                Steam => TuiHub.Protos.Librarian.V1.WellKnownAppInfoSource.Steam,
                Vndb => TuiHub.Protos.Librarian.V1.WellKnownAppInfoSource.Vndb,
                Bangumi => TuiHub.Protos.Librarian.V1.WellKnownAppInfoSource.Bangumi,
                _ => TuiHub.Protos.Librarian.V1.WellKnownAppInfoSource.Unspecified
            };
        }
    }
}
