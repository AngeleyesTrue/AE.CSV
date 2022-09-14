using AE.CSV.Common.Application.Common.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace AE.CSV.Common.Infrastructure.Helper;

public class CSVFileHelper : ICSVFileHelper
{
    #region Contructor

    public CSVFileHelper(ILogger<CSVFileHelper> logger, IFileUtillity fileUtillity)
    {
        _logger = logger;
        _fileUtillity = fileUtillity;
    }

    #endregion

    #region Instance Variables

    private readonly ILogger<CSVFileHelper> _logger;
    private readonly IFileUtillity _fileUtillity;

    #endregion

    #region Properties
    #endregion

    #region Initialize and Shutdown methods

    #endregion

    #region Private Methods

    #endregion

    #region Public Methods

    #region // IEnumerable<string> GetReadFile(string strFilePath) //
    /// <summary>
    /// CSV 파일을 읽어 Record를 반환한다.
    /// </summary>
    /// <param name="strFilePath"></param>
    /// <returns></returns>
    //public IEnumerable<CSVDataViewModel> GetReadFile(string strFilePath)
    //{
    //    IEnumerable<CSVDataViewModel> records = null;

    //    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    //    {
    //        HasHeaderRecord = false,
    //        LeaveOpen = false,
    //    };

    //    using (StreamReader textReader = new StreamReader(strFilePath))
    //    using (var csvr = new CsvReader(textReader, config))
    //    {
    //        records = csvr.GetRecords<CSVDataViewModel>().ToList();
    //    }

    //    return records;
    //}
    #endregion

    #region // List<T> GetReadFileByContents<T, TK>(string? strContent) where T : class where TK : ClassMap<T> //
    /// <summary>
    /// CSV 파일을 읽어 Record를 반환한다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mapType"></param>
    /// <param name="strContent"></param>
    /// <returns></returns>
    public List<T> GetReadFileByContents<T, TK>(string? strContent) where T : class where TK : ClassMap<T>
    {
        List<T> records = null;
        using (TextReader textReader = new StreamReader(strContent))
        using (var csvr = new CsvReader(textReader, CultureInfo.CurrentUICulture, true))
        {
            csvr.Context.RegisterClassMap<TK>();

            records = csvr.GetRecords<T>().ToList();
        }

        return records;
    }
    #endregion

    #region // List<T> GetReadFileByFilePath<T, TK>(string? strCsvPath) where T : class where TK : ClassMap<T> //
    /// <summary>
    /// CSV 파일을 읽어 Record를 반환한다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mapType"></param>
    /// <param name="strCsvPath"></param>
    /// <returns></returns>
    public List<T> GetReadFileByFilePath<T, TK>(string? strCsvPath) where T : class where TK : ClassMap<T>
    {
        if (String.IsNullOrWhiteSpace(strCsvPath))
        {
            throw new ArgumentNullException(nameof(strCsvPath));
        }

        List<T> records = null;
        using (var stream = File.Open(strCsvPath, FileMode.Open))
        using (var reader = new StreamReader(stream, Encoding.UTF8))
        using (var csvr = new CsvReader(reader, CultureInfo.CurrentUICulture, true))
        {
            csvr.Context.RegisterClassMap<TK>();

            records = csvr.GetRecords<T>().ToList();
        }

        return records;
    }
    #endregion

    #region // async Task<bool> SplitFile<T, TK>(string? strCsvPath, int rowCount, string strSvcPathFormat) where T : class where TK : ClassMap<T> //
    /// <summary>
    /// CSV 파일을 읽어 row 수 만큼 행을 추가하고 파일을 나눈다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mapType"></param>
    /// <param name="strCsvPath"></param>
    /// <returns></returns>
    public async Task<bool> SplitFile<T, TK>(string? strCsvPath, int rowCount, string? strSvcPathFormat) where T : class where TK : ClassMap<T>
    {
        if (String.IsNullOrWhiteSpace(strCsvPath) || String.IsNullOrWhiteSpace(strSvcPathFormat))
        {
            throw new ArgumentNullException(nameof(strCsvPath));
        }

        int splitNum = 0;

        using (var stream = File.Open(strCsvPath, FileMode.Open))
        using (var reader = new StreamReader(stream, Encoding.UTF8))
        using (var csvr = new CsvReader(reader, CultureInfo.CurrentUICulture, true))
        {
            csvr.Context.RegisterClassMap<TK>();

            List<T> newItem = new();
            foreach (var item in csvr.GetRecords<T>())
            {
                if (item != null)
                {
                    PropertyInfo? propInfo = item.GetType().GetProperty("FileVersionUrl"); //this returns null
                    if (propInfo != null)
                    {
                        propInfo.SetValue(item, "");
                    }

                    newItem.Add(item);

                    if (newItem.Count > rowCount)
                    {
                        await CreateCSV(newItem, string.Format(strSvcPathFormat, splitNum.ToString()));
                        _logger.LogInformation("파일 생성: {name}", string.Format(strSvcPathFormat, splitNum.ToString().PadLeft(3, '0')));
                        newItem.Clear();
                        splitNum++;
                    }
                }
            }

            if (newItem.Any())
            {
                await CreateCSV(newItem, string.Format(strSvcPathFormat, splitNum.ToString()));
                _logger.LogInformation("파일 생성: {name}", string.Format(strSvcPathFormat, splitNum.ToString().PadLeft(3, '0')));
                newItem.Clear();
            }
        }

        return splitNum > 0;
    }
    #endregion

    #region // async Task<bool> CreateCSV<T>(List<T> data, string strCreatePath) //
    /// <summary>
    /// CSV 파일을 생성한다.
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="data">입력 데이터</param>
    /// <param name="strCreatePath">파일 경로</param>
    /// <returns>성공여부</returns>
    public async Task<bool> CreateCSV<T>(List<T> data, string strCreatePath)
    {
        bool bResult = false;

        try
        {
            if (_fileUtillity.FolderExistsNCreate(strCreatePath.Substring(0, strCreatePath.LastIndexOf(@"\"))))
            {
                using (var writer = new StreamWriter(strCreatePath, false, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.CurrentCulture))
                {
                    await csv.WriteRecordsAsync(data);
                    bResult = true;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"CreateCSV Error: {ex.Message}", null);
        }

        return bResult;
    }
    #endregion

    #region // async Task<bool> AppendCSV<T>(List<T> data, string strCsvPath) //
    /// <summary>
    /// CSV 파일에 데이터를 추가한다.
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="data">입력 데이터</param>
    /// <param name="strCsvPath">파일 경로</param>
    /// <returns>성공여부</returns>
    public async Task<bool> AppendCSV<T>(List<T> data, string strCsvPath)
    {
        bool bResult = false;

        try
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = false,
            };

            using (var stream = File.Open(strCsvPath, FileMode.Append))
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            using (var csv = new CsvWriter(writer, config))
            {
                await csv.WriteRecordsAsync(data);
                bResult = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AppendCSV Error: {Message}", ex.Message);
        }

        return bResult;
    }
    #endregion

    #region // async Task<bool> CreateOrAppendCSV<T>(List<T> data, string strCreatePath) //
    /// <summary>
    /// CSV 파일이 없으면 생성해서 데이터를 입력하고 있으면 데이터를 추가한다.
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="data">입력 데이터</param>
    /// <param name="strCreatePath">파일 경로</param>
    /// <returns>성공여부</returns>
    public async Task<bool> CreateOrAppendCSV<T>(List<T> data, string strCreatePath)
    {
        bool bResult = false;

        try
        {
            if (_fileUtillity.FolderExistsNCreate(strCreatePath.Substring(0, strCreatePath.LastIndexOf(@"\"))))
            {
                if (_fileUtillity.FileExists(strCreatePath))
                {
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = false,
                    };

                    using (var stream = File.Open(strCreatePath, FileMode.Append))
                    using (var writer = new StreamWriter(stream, Encoding.UTF8))
                    using (var csv = new CsvWriter(writer, config))
                    {
                        await csv.WriteRecordsAsync(data);
                        bResult = true;
                    }
                }
                else
                {
                    using (var writer = new StreamWriter(strCreatePath, false, Encoding.UTF8))
                    using (var csv = new CsvWriter(writer, CultureInfo.CurrentCulture))
                    {
                        await csv.WriteRecordsAsync(data);
                        bResult = true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateOrAppendCSV Error: {Message}", ex.Message);
        }

        return bResult;
    }
    #endregion

    #endregion

    #region Event Handlers

    #endregion
}
