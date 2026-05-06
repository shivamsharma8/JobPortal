using System.Text;
using AnalyticsService.Application.DTOs;
using BuildingBlocks.Common.Results;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AnalyticsService.Application.Services;

public class ReportGenerator
{
    public ReportGenerator()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public Result<byte[]> GeneratePlatformStatsPdf(IReadOnlyList<PlatformStatResponse> stats)
    {
        try
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(x => ComposeContent(x, stats));
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
                });
            });

            var pdfBytes = document.GeneratePdf();
            return Result.Success(pdfBytes);
        }
        catch (Exception ex)
        {
            return Result.Failure<byte[]>(Error.Custom("Report.PdfFailed", "Failed to generate PDF."));
        }
    }

    public Result<byte[]> GeneratePlatformStatsCsv(IReadOnlyList<PlatformStatResponse> stats)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Date,Total Jobs,Total Applications,Total Interviews,Total Offers");
        
        foreach (var stat in stats.OrderByDescending(s => s.Date))
        {
            sb.AppendLine($"{stat.Date:yyyy-MM-dd},{stat.TotalJobs},{stat.TotalApplications},{stat.TotalInterviews},{stat.TotalOffers}");
        }

        return Result.Success(Encoding.UTF8.GetBytes(sb.ToString()));
    }

    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("HireConnect Platform Analytics").FontSize(20).SemiBold().FontColor(Colors.Blue.Darken2);
                column.Item().Text($"Generated on: {DateTime.Now:g}").FontSize(10);
            });
        });
    }

    private void ComposeContent(IContainer container, IReadOnlyList<PlatformStatResponse> stats)
    {
        container.PaddingVertical(1, Unit.Centimetre).Column(column =>
        {
            column.Spacing(5);
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("Date");
                    header.Cell().Element(CellStyle).AlignRight().Text("Jobs");
                    header.Cell().Element(CellStyle).AlignRight().Text("Applications");
                    header.Cell().Element(CellStyle).AlignRight().Text("Interviews");
                    header.Cell().Element(CellStyle).AlignRight().Text("Offers");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var stat in stats.OrderByDescending(x => x.Date))
                {
                    table.Cell().Element(CellStyle).Text(stat.Date.ToString("yyyy-MM-dd"));
                    table.Cell().Element(CellStyle).AlignRight().Text(stat.TotalJobs.ToString());
                    table.Cell().Element(CellStyle).AlignRight().Text(stat.TotalApplications.ToString());
                    table.Cell().Element(CellStyle).AlignRight().Text(stat.TotalInterviews.ToString());
                    table.Cell().Element(CellStyle).AlignRight().Text(stat.TotalOffers.ToString());

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                    }
                }
            });
        });
    }
}
