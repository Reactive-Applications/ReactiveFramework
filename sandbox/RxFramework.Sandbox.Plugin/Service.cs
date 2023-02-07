using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxFramework.Sandbox.TestPlugin;
internal class Service : IService
{
    private readonly ILogger<IService> _logger;

    public Service(ILogger<IService> logger)
    {
        _logger = logger;
    }

    public void WriteToConsole(string message)
    {
        _logger.LogInformation(message);
    }
}
