namespace AE.CSV.Common.Domain.Entities;

public class SPOFileItem
{
    public string? SiteName { get; set; }
    public string? SiteUrl { get; set; }
    public string? FileUniqueId { get; set; }
    public string? FileName { get; set; }
    public string? FileUrl { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public double? FileSize { get; set; }
    public double FileVersionCnt { get; set; }
    public double? FileVersionUsed { get; set; }
    public string? FileVersionUrl { get; set; }
}
