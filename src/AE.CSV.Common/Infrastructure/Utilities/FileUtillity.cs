using AE.CSV.Common.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace AE.CSV.Common.Infrastructure.Utilities;

public class FileUtillity : IFileUtillity
{
    #region Contructor

    public FileUtillity(ILogger<FileUtillity> logger)
    {
        _logger = logger;
    }

    #endregion

    #region Instance Variables

    private readonly ILogger<FileUtillity> _logger;

    #endregion

    #region Properties

    #endregion

    #region Initialize and Shutdown methods

    #endregion

    #region Private Methods

    #endregion

    #region Public Methods

    #region // bool FileExists(string FileFullPath) //
    /// <summary>
    /// 파일이 존재하는지 여부를 반환한다.
    /// </summary>
    /// <param name="FileFullPath">파일 전체 경로</param>
    /// <returns>파일 존재 여부</returns>
    public bool FileExists(string FileFullPath)
    {
        bool bFileExists = false;

        FileInfo fi = new FileInfo(FileFullPath);
        if (fi.Exists)
            bFileExists = true;

        return bFileExists;
    }
    #endregion

    #region // bool FolderExistsNCreate(string FolderPath) //
    /// <summary>
    /// 폴더가 있는지 확인하고 없으면 만든다.
    /// </summary>
    /// <param name="FolderPath"></param>
    /// <returns></returns>
    public bool FolderExistsNCreate(string FolderPath)
    {
        bool bResult = false;

        try
        {
            DirectoryInfo dir = new DirectoryInfo(FolderPath);
            if (!dir.Exists)
                dir.Create();

            bResult = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "FolderExistsNCreate Error: {Message}", ex.Message);
        }

        return bResult;
    }
    #endregion

    #endregion

    #region Event Handlers

    #endregion
}
