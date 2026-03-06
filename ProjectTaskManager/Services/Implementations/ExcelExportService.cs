using ClosedXML.Excel;
using ClosedXML.Excel.Drawings;
using ProjectTaskManager.DTOs;
using ProjectTaskManager.Services.Interfaces;
using System.IO;

namespace ProjectTaskManager.Services.Implementations
{
    public class ExcelExportService : IExcelExportService
    {
        private readonly byte[]? _companyLogo; // Made nullable

        public ExcelExportService()
        {
            // Example: Load logo from file (adjust path as needed)
            if (File.Exists("wwwroot/logo.png"))
            {
                _companyLogo = File.ReadAllBytes("wwwroot/logo.png");
            }
            else
            {
                _companyLogo = null; // Explicitly set to null if file doesn't exist
            }
        }

        public async Task<byte[]> GenerateCompanyReportAsync(IEnumerable<CompanyExportDto> companies)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Companies");

            // Optional: Insert logo at top
            int headerRow = 1; // Default header row
            if (_companyLogo != null)
            {
                using var imageStream = new MemoryStream(_companyLogo);
                worksheet.AddPicture(imageStream, XLPictureFormat.Png, "Logo")
                    .MoveTo(worksheet.Cell("A1"))
                    .Scale(0.5); // adjust scale as needed

                // Shift headers down to make room for logo
                worksheet.Row(1).InsertRowsBelow(1);
                headerRow = 3; // Logo takes rows 1-2, headers start at row 3
            }

            // Headers
            worksheet.Cell(headerRow, 1).Value = "ID";
            worksheet.Cell(headerRow, 2).Value = "Name";
            worksheet.Cell(headerRow, 3).Value = "Email";
            worksheet.Cell(headerRow, 4).Value = "Location";
            worksheet.Cell(headerRow, 5).Value = "Projects";
            worksheet.Cell(headerRow, 6).Value = "Users";
            worksheet.Cell(headerRow, 7).Value = "Created";

            // Style header row
            var headerRange = worksheet.Range(headerRow, 1, headerRow, 7);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Data rows
            int dataRow = headerRow + 1;
            foreach (var company in companies)
            {
                worksheet.Cell(dataRow, 1).Value = company.Id;
                worksheet.Cell(dataRow, 2).Value = company.Name;
                worksheet.Cell(dataRow, 3).Value = company.Email;
                worksheet.Cell(dataRow, 4).Value = company.Location;
                worksheet.Cell(dataRow, 5).Value = company.ProjectCount;
                worksheet.Cell(dataRow, 6).Value = company.UserCount;
                worksheet.Cell(dataRow, 7).Value = company.CreatedAt.ToString("yyyy-MM-dd");
                dataRow++;
            }

            // Convert the data range to an Excel Table (ListObject)
            if (dataRow > headerRow + 1) // Only create table if there's data
            {
                var rangeWithData = worksheet.Range(headerRow, 1, dataRow - 1, 7);
                var table = rangeWithData.CreateTable();
                table.Theme = XLTableTheme.TableStyleLight9; // Choose a style
                table.ShowAutoFilter = true;
                table.ShowRowStripes = true;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}