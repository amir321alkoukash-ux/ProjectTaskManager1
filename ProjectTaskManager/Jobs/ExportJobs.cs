using ProjectTaskManager.Services.Interfaces;

namespace ProjectTaskManager.Jobs
{
    public class ExportJobs
    {
        private readonly ICompanyService _companyService;
        private readonly IExcelExportService _excelExportService;
        private readonly IEmailService _emailService;
        private readonly ILogger<ExportJobs> _logger;

        public ExportJobs(
            ICompanyService companyService,
            IExcelExportService excelExportService,
            IEmailService emailService,
            ILogger<ExportJobs> logger)
        {
            _companyService = companyService;
            _excelExportService = excelExportService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task GenerateAndEmailExport(string recipientEmail)
        {
            try
            {
                _logger.LogInformation("Starting background export for {Email}", recipientEmail);

                // Get company data
                var companies = await _companyService.GetAllForExportAsync();

                // Generate Excel file
                var fileBytes = await _excelExportService.GenerateCompanyReportAsync(companies);

                // Generate filename with timestamp
                var fileName = $"Companies_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                // Send email with attachment
                await _emailService.SendEmailWithAttachmentAsync(
                    recipientEmail,
                    "Your Company Export is Ready",
                    "Please find attached the company export file.",
                    fileBytes,
                    fileName);

                _logger.LogInformation("Export completed successfully for {Email}", recipientEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Export failed for {Email}", recipientEmail);

                // Send failure notification
                await _emailService.SendEmailAsync(
                    recipientEmail,
                    "Export Failed",
                    $"Your company export failed. Error: {ex.Message}");

                throw; // Hangfire will retry
            }
        }
    }
}