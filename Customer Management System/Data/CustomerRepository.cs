using System.Data;
using Customer_Management_System.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;


namespace Customer_Management_System.Data
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerContext _context;

        public CustomerRepository(CustomerContext context)
        {
            _context = context;
        }

        // Get a page of customers including Address
        public async Task<(IEnumerable<Customer> Customers, int TotalCount)> GetPageCustomersAsync(int pageNumber, int pageSize, string sortColumn = "Name", string sortDirection = "asc", string nameFilter = null, string vatFilter = null)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : pageSize;

            IQueryable<Customer> query = _context.Customers.Include(c => c.Address);

            // Apply filters
            if (!string.IsNullOrEmpty(nameFilter))
            {
                query = query.Where(c => c.Name.Contains(nameFilter));
            }

            if (!string.IsNullOrEmpty(vatFilter))
            {
                query = query.Where(c => c.VATNumber.Contains(vatFilter));
            }

            // Apply sort based on known column names
            query = (sortColumn, sortDirection.ToLower()) switch
            {
                ("Name", "desc") => query.OrderByDescending(c => c.Name),
                ("Name", _) => query.OrderBy(c => c.Name),

                ("PhoneNumber", "desc") => query.OrderByDescending(c => c.PhoneNumber),
                ("PhoneNumber", _) => query.OrderBy(c => c.PhoneNumber),

                ("ContactName", "desc") => query.OrderByDescending(c => c.ContactName),
                ("ContactName", _) => query.OrderBy(c => c.ContactName),

                ("ContactEmail", "desc") => query.OrderByDescending(c => c.ContactEmail),
                ("ContactEmail", _) => query.OrderBy(c => c.ContactEmail),

                ("VATNumber", "desc") => query.OrderByDescending(c => c.VATNumber),
                ("VATNumber", _) => query.OrderBy(c => c.VATNumber),

                ("Address", "desc") => query.OrderByDescending(c =>
                    c.Address.StreetName + " " + c.Address.Suburb + " " + c.Address.City + " " + c.Address.Province + " " + c.Address.PostalCode),
                ("Address", _) => query.OrderBy(c =>
                    c.Address.StreetName + " " + c.Address.Suburb + " " + c.Address.City + " " + c.Address.Province + " " + c.Address.PostalCode),

                _ => query.OrderBy(c => c.CustomerID)
            };

            var totalCount = await query.CountAsync();

            var customers = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (customers, totalCount);

        }



        // Get customer by ID including Address
        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.CustomerID == id);
        }

        // Add a new customer 
        public async Task AddAsync(Customer customer)
        {
            // Check for duplicate VAT
            if(customer.VATNumber != null)
            {
                bool exists = await _context.Customers
                .AnyAsync(c => c.VATNumber == customer.VATNumber);

                if (exists)
                    throw new InvalidOperationException("A customer with this VAT number already exists.");
            }
            
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        // Update an existing customer
        public async Task UpdateAsync(Customer customer)
        {
            var existingCustomer = await _context.Customers
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.CustomerID == customer.CustomerID);

            if (existingCustomer == null)
                throw new KeyNotFoundException("Customer not found.");

            // Update basic fields
            existingCustomer.Name = customer.Name;
            existingCustomer.PhoneNumber = customer.PhoneNumber;
            existingCustomer.ContactName = customer.ContactName;
            existingCustomer.ContactEmail = customer.ContactEmail;
            existingCustomer.VATNumber = customer.VATNumber;

            // Update Address fields
            if (customer.Address != null)
            {
                if (existingCustomer.Address == null)
                {
                    existingCustomer.Address = customer.Address;
                }
                else
                {
                    existingCustomer.Address.StreetName = customer.Address.StreetName;
                    existingCustomer.Address.Suburb = customer.Address.Suburb;
                    existingCustomer.Address.City = customer.Address.City;
                    existingCustomer.Address.Province = customer.Address.Province;
                    existingCustomer.Address.PostalCode = customer.Address.PostalCode;
                }
            }

            await _context.SaveChangesAsync();
        }

        // Delete customer by ID
        public async Task DeleteAsync(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.CustomerID == id);

            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }

        // Check if a customer exists
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Customers.AnyAsync(c => c.CustomerID == id);
        }

        public async Task<Customer?> GetByVerificationTokenAsync(string token)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.VerificationToken == token);
        }
    }
}




