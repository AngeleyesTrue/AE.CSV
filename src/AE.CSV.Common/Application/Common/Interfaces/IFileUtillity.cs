namespace AE.CSV.Common.Application.Common.Interfaces;

public interface IFileUtillity
{
    bool FileExists(string FileFullPath);
    bool FolderExistsNCreate(string FolderPath);
}
