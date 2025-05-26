using BeautySalon.Entities;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BeautySalon.BLImplementations;

public class ReportGenerator
{
    public async Task<string> GenerateStaffReport(List<Staff> staffList)
    {
        string reportContent = "REPORT : Staff\n";
        reportContent += "> Quantity: " + staffList.Count + "\n";

        string filePath = "C:\\Reports\\staff_report.txt";
        await File.WriteAllTextAsync(filePath, reportContent);
        return filePath;
    }
}