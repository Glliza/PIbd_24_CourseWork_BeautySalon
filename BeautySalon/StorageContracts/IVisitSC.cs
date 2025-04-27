using BeautySalon.DataModels;

namespace BeautySalon.StorageContracts;

public interface IVisitSC
{
    // Get a list of visits with optional filtering
    // Filters align with potential Visit properties (Staff, Customer, Date/Time range, Status)
    Task<List<VisitDM>> GetList(
        bool onlyActive = true, // Filter by IsDeleted flag
        string? visitID = null, // Filter by specific Visit ID
        string? customerID = null,
        string? staffID = null,
        bool? status = null, // Filter by boolean status (e.g., completed/not completed)
        DateTime? fromDateTimeOfVisit = null,
        DateTime? toDateTimeOfVisit = null);

    Task<VisitDM?> GetElementByID(string id);

    Task AddElement(VisitDM visitDataModel);

    Task UpdElement(VisitDM visitDataModel);

    Task DelElement(string id);

    // Restore a soft-deleted visit by ID (optional but good practice)
    Task RestoreElement(string id);

    // Optional: Methods to get visits including related details (e.g., linked Receipt, or items if lists were modeled differently)
    // Task<VisitDM?> GetVisitWithDetailsAsync(string id);
}
