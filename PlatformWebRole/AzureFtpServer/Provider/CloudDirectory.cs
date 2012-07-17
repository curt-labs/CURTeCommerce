using System;
using System.Collections.Generic;

namespace AzureFtpServer.Provider
{
    public class CloudDirectory
    {
        public String Path { get; set; }
    }

    public class CloudDirectoryCollection : List<CloudDirectory>
    {
    }
}