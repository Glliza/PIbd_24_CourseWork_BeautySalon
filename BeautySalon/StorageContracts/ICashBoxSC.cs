using BeautySalon.DataModels;

namespace BeautySalon.StorageContracts;

public interface ICashBoxSC
{
    // Get a list of cash boxes (optional filter by active status)
    Task<List<CashBoxDM>> GetList(bool onlyActive = true); // Filter by IsDeleted flag

    // Get a single cash box by its unique ID
    Task<CashBoxDM?> GetElementByID(string id);

    // Add a new cash box (less common operation)
    Task AddElement(CashBoxDM cashBoxDataModel);

    // Update an existing cash box (e.g., change its current capacity - though this is often done indirectly via transactions)
    // Update method should ideally focus on metadata changes, capacity changes handled by specific methods.
    Task UpdElement(CashBoxDM cashBoxDataModel);

    // Soft delete a cash box by ID (e.g., decommission it)
    Task DelElement(string id);

    // Restore a soft-deleted cash box by ID (optional but good practice)
    Task RestoreElement(string id);

    // Optional: Specific methods for managing cash flow directly at the data level (alternative to BL calculation)
    // Task DepositCashAsync(string cashBoxId, decimal amount);
    // Task WithdrawCashAsync(string cashBoxId, decimal amount);
    // Task SetCurrentCapacityAsync(string cashBoxId, decimal newCapacity); // Direct capacity change
}
