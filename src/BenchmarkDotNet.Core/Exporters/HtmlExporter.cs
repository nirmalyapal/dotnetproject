﻿using BenchmarkDotNet.Extensions;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

namespace BenchmarkDotNet.Exporters
{
    public class HtmlExporter : ExporterBase
    {
        internal const string CssDefinition = @"
<style type=""text/css"">
	table { border-collapse: collapse; display: block; width: 100%; overflow: auto; }
	td, th { padding: 6px 13px; border: 1px solid #ddd; }
	tr { background-color: #fff; border-top: 1px solid #ccc; }
	tr:nth-child(even) { background: #f8f8f8; }
</style>";

        protected override string FileExtension => "html";

        public static readonly IExporter Default = new HtmlExporter();

        public override void ExportToLog(Summary summary, ILogger logger)
        {
            logger.WriteLine("<!DOCTYPE html>");
            logger.WriteLine("<html lang='en'>");
            logger.WriteLine("<head>");
            logger.WriteLine("<meta charset='utf-8' />");
            logger.WriteLine("<title>" + summary.Title + "</title>");
            logger.WriteLine(CssDefinition);
            logger.WriteLine("</head>");

            logger.WriteLine("<body>");
            logger.Write("<pre><code>");
            logger.WriteLine();
            foreach (string infoLine in summary.HostEnvironmentInfo.ToFormattedString())
            {
                logger.WriteLine(infoLine);
            }
            logger.WriteLine(summary.AllRuntimes);
            logger.Write("</code></pre>");
            logger.WriteLine();

            PrintTable(summary.Table, logger);

            logger.WriteLine("</body>");
            logger.WriteLine("</html>");
        }

        private void PrintTable(SummaryTable table, ILogger logger)
        {
            if (table.FullContent.Length == 0)
            {
                logger.WriteLineError("<pre>There are no benchmarks found</pre>");
                return;
            }

            logger.Write("<pre><code>");
            table.PrintCommonColumns(logger);
            logger.WriteLine("</code></pre>");
            logger.WriteLine();

            logger.WriteLine("<table>");

            logger.Write("<thead>");
            logger.Write("<tr>");
            table.PrintLine(table.FullHeader, logger, "<th>", "</th>");
            logger.WriteLine("</tr>");
            logger.Write("</thead>");

            logger.Write("<tbody>");
            foreach (var line in table.FullContent)
            {
                logger.Write("<tr>");
                PrintLine(table, line, logger, "<td>", "</td>");
                logger.Write("</tr>");
            }
            logger.Write("</tbody>");

            logger.WriteLine("</table>");
        }

        private void PrintLine(SummaryTable table, string[] line, ILogger logger, string leftDel, string rightDel)
        {
            for (int columnIndex = 0; columnIndex < table.ColumnCount; columnIndex++)
            {
                if (table.Columns[columnIndex].NeedToShow)
                {
                    logger.WriteStatistic(leftDel + line[columnIndex].HtmlEncode() + rightDel);
                }
            }

            logger.WriteLine();
        }
    }
}