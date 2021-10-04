using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Utilities
{
    public class VersionConfiguration
    {
        public int versionNumber { get; set; }
        public int subVersionNumber { get; set; }
    }
    public class VersionUtil
    {
        private readonly VersionConfiguration _versionConfig;
        public VersionUtil(VersionConfiguration versionConfig)
        {
            _versionConfig = versionConfig;
        }

        public string GetDisplayVersion()
        {
            return "v" + _versionConfig.versionNumber + "." + _versionConfig.subVersionNumber;
        }
    }
}
