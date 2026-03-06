using ProjectTaskManager.DTOs;

namespace ProjectTaskManager.Services.Interfaces
{
    public interface IExcelExportService
    {
        Task<byte[]> GenerateCompanyReportAsync(IEnumerable<CompanyExportDto> companies);
    }
}