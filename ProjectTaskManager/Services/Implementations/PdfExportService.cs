using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ProjectTaskManager.DTOs;
using ProjectTaskManager.Services.Interfaces;

namespace ProjectTaskManager.Services.Implementations
{
    public class PdfExportService : IPdfExportService
    {
        public Task<byte[]> GenerateCompanyReportAsync(IEnumerable<CompanyExportDto> companies)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    // page.Margin(2, Unit.Cm);
                    page.Margin(2, Unit.Centimetre);
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(composer => ComposeContent(composer, companies));
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
                });
            });

            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            return Task.FromResult(stream.ToArray());
        }

        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("Company Report").FontSize(20).SemiBold();
                    column.Item().Text($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}").FontSize(10).FontColor(Colors.Grey.Medium);
                });
                // Optional: Add logo
                // row.ConstantItem(100).Image("wwwroot/logo.png");
            });
        }

        private void ComposeContent(IContainer container, IEnumerable<CompanyExportDto> companies)
        {
            container.Table(table =>
            {
                // Define columns
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(30);  // ID
                    columns.RelativeColumn(2);   // Name
                    columns.RelativeColumn(3);   // Email
                    columns.RelativeColumn(2);   // Location
                    columns.ConstantColumn(50);  // Projects
                    columns.ConstantColumn(50);  // Users
                    columns.ConstantColumn(70);  // Created
                });

                // Header row
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("ID");
                    header.Cell().Element(CellStyle).Text("Name");
                    header.Cell().Element(CellStyle).Text("Email");
                    header.Cell().Element(CellStyle).Text("Location");
                    header.Cell().Element(CellStyle).Text("Projects");
                    header.Cell().Element(CellStyle).Text("Users");
                    header.Cell().Element(CellStyle).Text("Created");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold())
                                         .PaddingVertical(5)
                                         .BorderBottom(1)
                                         .BorderColor(Colors.Black);
                    }
                });

                // Data rows
                foreach (var company in companies)
                {
                    table.Cell().Element(CellStyle).Text(company.Id.ToString());
                    table.Cell().Element(CellStyle).Text(company.Name);
                    table.Cell().Element(CellStyle).Text(company.Email);
                    table.Cell().Element(CellStyle).Text(company.Location);
                    table.Cell().Element(CellStyle).Text(company.ProjectCount.ToString());
                    table.Cell().Element(CellStyle).Text(company.UserCount.ToString());
                    table.Cell().Element(CellStyle).Text(company.CreatedAt.ToString("yyyy-MM-dd"));

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1)
                                         .BorderColor(Colors.Grey.Lighten2)
                                         .PaddingVertical(5);
                    }
                }
            });
        }
    }
}