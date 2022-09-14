using AE.CSV.Common.Application.Common.Interfaces;
using AE.CSV.Common.Domain.ClassMaps;
using AE.CSV.Common.Domain.Entities;
using AE.CSV.CSApp.Configs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AE.CSV.CSApp.Services;

public class ConsoleApp : IConsoleApp
{
    private readonly ILogger<ConsoleApp> _logger;
    private readonly ICSVFileHelper _csvHelper;
    private readonly ConsoleAppConfigSettings _configSettings;

    public ConsoleApp(ILogger<ConsoleApp> logger, ICSVFileHelper csvHelper, IOptions<ConsoleAppConfigSettings> options)
    {
        _logger = logger;
        _csvHelper = csvHelper;
        _configSettings = options.Value;
    }

    public async Task Run(string[] args)
    {
        try
        {
            var bResult = await _csvHelper.SplitFile<SPOFileItem, SPOFileItemMap>(_configSettings.CSVFilePath, 400000, _configSettings.CSVFilePath?.Replace(".csv", "_{0}.csv"));
            if (bResult)
            {
                _logger.LogInformation("성공");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Run Error: {Message}", ex.Message);
        }
    }
}
