using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProjectTaskManager.DTOs;
using ProjectTaskManager.Jobs;
using ProjectTaskManager.Services.Interfaces;

namespace ProjectTaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        private readonly IExcelExportService _excelExportService;
        private readonly IPdfExportService _pdfExportService;
        private readonly ILogger<CompaniesController> _logger;

        // Single constructor with all dependencies
        public CompaniesController(
            ICompanyService companyService,
            IExcelExportService excelExportService,
            IPdfExportService pdfExportService,
            ILogger<CompaniesController> logger)
        {
            _companyService = companyService;
            _excelExportService = excelExportService;
            _pdfExportService = pdfExportService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var companies = await _companyService.GetAllCompaniesAsync();
            return Ok(companies);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var company = await _companyService.GetCompanyByIdAsync(id);
                return Ok(company);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CompanyDto dto)
        {
            var created = await _companyService.CreateCompanyAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CompanyDto dto)
        {
            try
            {
                await _companyService.UpdateCompanyAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _companyService.DeleteCompanyAsync(id);
            return NoContent();
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportCompanies([FromQuery] string format = "excel")
        {
            try
            {
                var companies = await _companyService.GetAllForExportAsync();
                byte[] fileBytes;
                string contentType;
                string fileName;

                if (format.ToLower() == "pdf")
                {
                    fileBytes = await _pdfExportService.GenerateCompanyReportAsync(companies);
                    contentType = "application/pdf";
                    fileName = $"Companies_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                }
                else // default to excel
                {
                    fileBytes = await _excelExportService.GenerateCompanyReportAsync(companies);
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    fileName = $"Companies_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                }

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting companies");
                return StatusCode(500, "An error occurred while generating the export.");
            }
        }

        [HttpGet("export/background")]
        public IActionResult ExportCompaniesBackground([FromQuery] string email)
        {
            try
            {
                // Enqueue a fire-and-forget background job
                var jobId = BackgroundJob.Enqueue<ExportJobs>(x => x.GenerateAndEmailExport(email));

                return Accepted(new
                {
                    jobId,
                    message = "Export started. You will receive an email when ready.",
                    dashboardUrl = "/hangfire"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting background export");
                return StatusCode(500, "Failed to start export job.");
            }
        }
    }
}