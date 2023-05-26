using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAD_Plugin
{
    /// <summary>
    ///   <br />
    /// </summary>
    public class ProviderDataObject
    {
        public string Name { get; set; }

        public bool UsingPkqsesPkq { get; set; }
        public bool UsingRecoveryMail { get; set; }
        public bool UsingRecoveryPhone { get; set; }
        public bool UsingBackupCode { get; set; }

        public bool UsesRBA { get; set; }
        public bool Uses2StepVerificationFA { get; set; }
        public bool ProvidesServicesForMultipleDevices { get; set; }

    }
}
