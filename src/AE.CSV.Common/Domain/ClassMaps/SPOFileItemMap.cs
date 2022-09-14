using AE.CSV.Common.Domain.Entities;
using CsvHelper.Configuration;

namespace AE.CSV.Common.Domain.ClassMaps;

public sealed class SPOFileItemMap : ClassMap<SPOFileItem>
{
    public SPOFileItemMap()
    {
        Map(m => m.SiteName).Name("Site name").Optional();
        Map(m => m.SiteUrl).Name("Site Url");
        Map(m => m.FileUniqueId).Name("UniqueId");
        Map(m => m.FileName).Name("File name");
        Map(m => m.FileUrl).Name("File url");
        Map(m => m.Created).Name("Created");
        Map(m => m.Modified).Name("Modified");
        Map(m => m.FileSize).Name("File size(MB)");
        Map(m => m.FileVersionCnt).Name("Version count");
        Map(m => m.FileVersionUsed).Name("Version used(MB)");
        Map(m => m.FileVersionUrl).Name("Version link");
    }
}
