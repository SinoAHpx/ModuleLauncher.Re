using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleLauncher.NET.Models.Authentication
{
    public class DeviceCodeInfo
    {
        /// <summary>
        /// Basically this is not supposed to be used by end users.
        /// </summary>
        public string DeviceCode { get; set; }

        /// <summary>
        /// User needs to input this in the browser.
        /// </summary>
        public string UserCode { get; set; }

        /// <summary>
        /// You may open this url for users.
        /// </summary>
        public string VerificationUrl { get; set; }
    }
}
