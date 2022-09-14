using CsvHelper.Configuration;

namespace AE.CSV.Common.Application.Common.Interfaces;

public interface ICSVFileHelper
{
    //IEnumerable<CSVDataViewModel> GetReadFile(string strFilePath);
    List<T> GetReadFileByContents<T, TK>(string? strContent) where T : class where TK : ClassMap<T>;
    List<T> GetReadFileByFilePath<T, TK>(string? strCsvPath) where T : class where TK : ClassMap<T>;
    Task<bool> SplitFile<T, TK>(string? strCsvPath, int rowCount, string? strSvcPathFormat) where T : class where TK : ClassMap<T>;
    Task<bool> CreateCSV<T>(List<T> data, string strCreatePath);
    Task<bool> AppendCSV<T>(List<T> data, string strCsvPath);
    Task<bool> CreateOrAppendCSV<T>(List<T> data, string strCreatePath);
}
