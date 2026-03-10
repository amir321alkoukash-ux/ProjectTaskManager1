using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ProjectTaskManager.DTOs;
using ProjectTaskManager.Services.Interfaces;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectTaskManager.Services.Implementations
{
    public class PdfExportService : IPdfExportService
    {
        private readonly byte[]? _companyLogo;

        public PdfExportService()
        {
            try
            {
                if (File.Exists("wwwroot/logo.png"))
                {
                    _companyLogo = File.ReadAllBytes("wwwroot/logo.png");
                    Console.WriteLine("✅ PDF Logo loaded successfully.");
                }
                else
                {
                    _companyLogo = null;
                    Console.WriteLine("❌ PDF Logo file not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading PDF logo: {ex.Message}");
                _companyLogo = null;
            }
        }

        public async Task<byte[]> GenerateCompanyReportAsync(IEnumerable<CompanyExportDto> companies)
        {
            try
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4.Landscape());
                        page.Margin(2, Unit.Centimetre);
                        page.Header().Element(container => ComposeHeader(container, _companyLogo));
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
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine("=== PDF Export Error ===");
                Console.WriteLine(ex.ToString());
                throw;
            }
        }

        private void ComposeHeader(IContainer container, byte[]? logoBytes)
        {
            container.Row(row =>
            {
                // Logo column (if logo exists)
                if (logoBytes != null)
                {
                    row.ConstantItem(100).Column(column =>
                    {
                        using var stream = new MemoryStream(logoBytes);
                        column.Item().Image(stream).FitWidth();
                    });
                }

                // Title column
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text("Company Report").FontSize(20).SemiBold();
                    column.Item().Text($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm}").FontSize(10).FontColor(Colors.Grey.Medium);
                });
            });
        }

        private void ComposeContent(IContainer container, IEnumerable<CompanyExportDto> companies)
        {
            container.Table(table =>
            {
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