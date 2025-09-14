using Customer_Management_System.Models;

namespace Customer_Management_System.Data
{
    public interface ICustomerRepository
    {
        // Gets a list of customers in a page.
        Task<(IEnumerable<Customer> Customers, int TotalCount)> GetPageCustomersAsync(int pageNumber, int pageSize, string sortColumn = "Name", string sortDirection = "asc", string nameFilter = null, string vatFilter = null);

        // Gets a customer by their unique ID.
        Task<Customer?> GetByIdAsync(int id);

        // Adds a new customer.
        Task AddAsync(Customer customer);

        // Updates an existing customer.
        Task UpdateAsync(Customer customer);

        // Deletes a customer by ID.
        Task DeleteAsync(int id);
        // Checks a customer exists by ID
        Task<bool> ExistsAsync(int id);

        // existing methods…
        Task<Customer?> GetByVerificationTokenAsync(string token);
    }

}
