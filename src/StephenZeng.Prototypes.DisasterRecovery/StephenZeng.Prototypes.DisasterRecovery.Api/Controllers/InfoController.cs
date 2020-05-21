using System;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace StephenZeng.Prototypes.DisasterRecovery.Api.Controllers
{
    [ApiController]
    [Route("")]
    [Route("api/[controller]")]
    public class InfoController : ControllerBase
    {
        private static readonly string Version = typeof(InfoController).Assembly.GetName().Version.ToString();
        private readonly string _timeZone;

        public InfoController()
        {
            _timeZone = TimeZoneInfo.Local.StandardName;
        }

        public dynamic Get()
        {
            var framework = Assembly
                .GetEntryAssembly()?
                .GetCustomAttribute<TargetFrameworkAttribute>()?
                .FrameworkName;

            return new
            {
                AppVersion = Version,
                TimeZone = _timeZone,
                os = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
                DotNetversion = framework
            };
        }
    }
}
