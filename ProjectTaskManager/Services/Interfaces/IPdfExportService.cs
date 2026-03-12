using ProjectTaskManager.DTOs;

namespace ProjectTaskManager.Services.Interfaces
{
    public interface IPdfExportService
    {
        Task<byte[]> GenerateCompanyReportAsync(IEnumerable<CompanyExportDto> companies);
    }
}