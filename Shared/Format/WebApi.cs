using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Format
{
    public class WebApi
    {
        public static string Format(string repositoryId, string documentId)
        {
            return repositoryId + "_" + documentId;
        }
    }
}
