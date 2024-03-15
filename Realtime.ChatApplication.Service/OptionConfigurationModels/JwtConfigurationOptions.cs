using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Realtime.ChatApplication.Service.OptionConfigurationModels
{
    public class JwtConfigurationOptions
    {
        public const string JwtKey = "Jwt";
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryInMinutes { get; set; } = 0;
}
}
