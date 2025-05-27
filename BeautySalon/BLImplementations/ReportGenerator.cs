using BeautySalon.Entities;
using BeautySalon.Enums;
using System.Text;

namespace BeautySalon.BLImplementations
{
    public class ReportGenerator
    {
        public async Task<string> GenerateStaffReport(List<Staff> staffList)
        {
            StringBuilder reportContent = new StringBuilder("REPORT : Staff\n");
            reportContent.Append($"> Quantity: {staffList.Count}\n");

            foreach (var staff in staffList)
            {
                reportContent.Append($"Name: {staff.FIO}, Post Type: {staff.postType}, Birth Date: {staff.BirthDate.ToShortDateString()}\n");
            }

            string filePath = "C:\\Reports\\staff_report.txt";
            await File.WriteAllTextAsync(filePath, reportContent.ToString());
            return filePath;
        }

        public async Task<string> GenerateProductReport(decimal minPrice, decimal maxPrice, List<Product> productList)
        {
            StringBuilder reportContent = new StringBuilder("REPORT : Products\n");

            // Total quantity of each product
            var productQuantities = productList.GroupBy(p => p.Name).Select(g => new { ProductName = g.Key, TotalQuantity = g.Sum(p => p.StockQuantity) });
            reportContent.AppendLine("Total Quantity by Product:");
            foreach (var product in productQuantities)
            {
                reportContent.AppendLine($"> Name: {product.ProductName}, Total Quantity: {product.TotalQuantity}");
            }

            // Products with price less than or equal to maxPrice
            reportContent.AppendLine("\nProducts with Price <= " + maxPrice.ToString("C"));
            var cheapProducts = productList.Where(p => p.PricePerOne <= maxPrice);
            foreach (var product in cheapProducts)
            {
                reportContent.AppendLine($"> Name: {product.Name}, Quantity: {product.StockQuantity}, Price: {product.PricePerOne:C}");
            }

            // Products with price greater than or equal to minPrice
            reportContent.AppendLine("\nProducts with Price >= " + minPrice.ToString("C"));
            var expensiveProducts = productList.Where(p => p.PricePerOne >= minPrice);
            foreach (var product in expensiveProducts)
            {
                reportContent.AppendLine($"> Name: {product.Name}, Quantity: {product.StockQuantity}, Price: {product.PricePerOne:C}");
            }

            // Products by type
            reportContent.AppendLine("\nProducts by Type:");
            var productsByType = productList.GroupBy(p => p.Type);
            foreach (var group in productsByType)
            {
                reportContent.AppendLine($"\nType: {group.Key}");
                foreach (var product in group)
                {
                    reportContent.AppendLine($"> Name: {product.Name}, Quantity: {product.StockQuantity}, Price: {product.PricePerOne:C}");
                }
            }

            string filePath = "C:\\Reports\\product_report.txt";
            await File.WriteAllTextAsync(filePath, reportContent.ToString());
            return filePath;
        }

        public async Task<string> GenerateServiceReport(List<Service> serviceList)
        {
            StringBuilder reportContent = new StringBuilder("REPORT : Services\n");
            reportContent.Append($"> Quantity: {serviceList.Count}\n");

            var groupedServices = serviceList.GroupBy(s => s.DurationMinutes);

            foreach (var group in groupedServices)
            {
                reportContent.Append($"Duration: {group.Key} minutes, Quantity: {group.Count()}, Total Price: {group.Sum(s => s.BasePrice):C}\n");
                foreach (var service in group)
                {
                    reportContent.Append($"> Name: {service.Name}, Price: {service.BasePrice:C}\n");
                }
            }

            string filePath = "C:\\Reports\\service_report.txt";
            await File.WriteAllTextAsync(filePath, reportContent.ToString());
            return filePath;
        }

        public async Task<string> GenerateVisitReport(string staffId, DateTime startDate, DateTime endDate, List<Visit> visitList)
        {
            StringBuilder reportContent = new StringBuilder("REPORT : Visits\n");

            // Visits by the specified staff member
            var staffVisits = visitList.Where(v => v.StaffID == staffId);
            reportContent.Append($"> Quantity: {staffVisits.Count()}\n");
            foreach (var visit in staffVisits)
            {
                reportContent.Append($"Date: {visit.DateTimeOfVisit.ToShortDateString()}, Customer: {visit.Customer.FIO}, Services: {string.Join(", ", visit.Services?.Select(s => s.Service.Name) ?? new List<string>())}\n");
            }

            // Visits by customer
            var customerVisits = visitList.GroupBy(v => v.CustomerID);
            reportContent.AppendLine("\nCustomer Visit Counts:");
            foreach (var group in customerVisits)
            {
                reportContent.AppendLine($"> Customer ID: {group.Key}, Visit Count: {group.Count()}");
            }

            string filePath = "C:\\Reports\\visit_report.txt";
            await File.WriteAllTextAsync(filePath, reportContent.ToString());
            return filePath;
        }

        public async Task<string> GenerateRequestReport(string customerId, List<Request> requestList)
        {
            StringBuilder reportContent = new StringBuilder("REPORT : Requests\n");
            reportContent.Append($"> Quantity: {requestList.Count}\n");

            // Customer's requests
            var customerRequests = requestList.Where(r => r.CustomerID == customerId);
            foreach (var request in customerRequests)
            {
                reportContent.Append($"Request ID: {request.ID}, Status: {request.Status}, Total Price: {request.TotalPrice:C}\n");
                reportContent.Append("Products:\n");
                foreach (var product in request.Products)
                {
                    reportContent.Append($"> Name: {product.Product.Name}, Quantity: {product.Amount}, Price: {product.Product.PricePerOne:C}\n");
                }
                reportContent.Append("Services:\n");
                foreach (var service in request.Services)
                {
                    reportContent.Append($"> Name: {service.Service.Name}, Quantity: {service.QuantityOrSessions}, Price: {service.TotalItemPrice:C}\n");
                }
            }

            // Approved but not finished requests
            var pendingRequests = requestList.Where(r => r.Status == OrderStatus.Completed && !r.IsDeleted);
            reportContent.AppendLine("\nPending Requests:");
            foreach (var request in pendingRequests)
            {
                reportContent.AppendLine($"> Request ID: {request.ID}, Customer: {request.Customer.FIO}, Total Price: {request.TotalPrice:C}");
            }

            string filePath = "C:\\Reports\\request_report.txt";
            await File.WriteAllTextAsync(filePath, reportContent.ToString());
            return filePath;
        }
    }
}