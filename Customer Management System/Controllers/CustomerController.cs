using System;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Customer_Management_System.Data;
using Customer_Management_System.Models;
using FluentEmail.Core;
using FluentEmail.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Customer_Management_System.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepo;
        private readonly IFluentEmail fluentEmail;
        ILogger<CustomerController> _logger;

        public CustomerController(ICustomerRepository customerRepo, IFluentEmail fluentEmail, ILogger<CustomerController> logger)
        {
            _customerRepo = customerRepo;
            this.fluentEmail = fluentEmail;
            this._logger = logger;

        }

        // GET: Customer
        public async Task<IActionResult> Index(
            int page = 1,
            int pageSize = 10,
            string sortColumn = "Name",
            string sortDirection = "asc",
            string nameFilter = null,
            string vatFilter = null)
        {
            try
            {
                var (customers, totalCount) = await _customerRepo
                    .GetPageCustomersAsync(page, pageSize, sortColumn, sortDirection, nameFilter, vatFilter);

                var model = new PageCustomerViewModel
                {
                    Customers = customers,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    SortColumn = sortColumn,
                    SortDirection = sortDirection
                };

                return View(model);
            }
            catch (SqlException)
            {
                // SQL Server not reachable / network issue
                return View("DatabaseError");
            }
            catch (Exception ex)
            {
                // other unexpected errors
                return View("Error", ex);
            }
        }

        // GET: Customer/Create
        public IActionResult Create()
        {
            var customer = new Customer
            {
                Address = new Address()
            };
            ViewData["isEdit"] = false;
            return View("CustomerDetails", customer);
        }


        // POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                ViewData["isEdit"] = false;
                return View("CustomerDetails", customer);
            }

            try
            {
                //genereated verification token and sets and starts the expiration timer
                customer.VerificationToken = GenerateVerificationToken();
                customer.TokenExpiry = DateTime.UtcNow.AddHours(3);

                string emailToVerify = customer.ContactEmail;
                await _customerRepo.AddAsync(customer);

                _logger.LogInformation("Creating customer with email: {Email}", customer.ContactEmail);

                if (string.IsNullOrWhiteSpace(emailToVerify))
                {
                    _logger.LogWarning("Customer email is empty, skipping verification email.");
                }
                else
                {
                    //Creates Customer
                    var verificationLink = Url.Action("VerifyEmail", "Customer",
                        new { token = WebUtility.UrlEncode(customer.VerificationToken) }, Request.Scheme);

                    await fluentEmail
                       .To(customer.ContactEmail)
                       .Subject("Verify your email")
                       .Body($"Hello {customer.Name},\n\nPlease verify your email by clicking this link:\n{verificationLink}")
                       .SendAsync();
                }


                return RedirectToAction(nameof(Index));
            }
            catch (SqlException)
            {
                // Database-related issue
                return View("DatabaseError");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                // Unexpected error
                ModelState.AddModelError("", "An unexpected error occurred. Please try again later.");
                return View("CustomerDetails", customer);
            }
        }


        //Verifies the email by comparing the tokens from the database and the token that the contact is using from their email
        [HttpGet]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            token = WebUtility.UrlDecode(token);

            if (string.IsNullOrEmpty(token))
                return BadRequest("Invalid verification token.");

            var customer = await _customerRepo.GetByVerificationTokenAsync(token);

            if (customer == null || customer.TokenExpiry < DateTime.UtcNow)
                return View("VerificationFailed");

            customer.IsVerified = true;
            customer.VerificationToken = null;
            customer.TokenExpiry = null;
            await _customerRepo.UpdateAsync(customer);

            return View("VerificationSuccess");

        }

        //creates a token using random characters
        private string GenerateVerificationToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(tokenBytes)
                         .Replace("+", "-")
                         .Replace("/", "_")
                         .Replace("=", ""); 
        }


        // GET: Customer/Edit/5 populate input boxes with customer information by ID
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerRepo.GetByIdAsync(id.Value);
            if (customer == null)
            {
                return NotFound();
            }

            ViewData["isEdit"] = true;
            return View("CustomerDetails", customer);
        }

        // POST: Customer/Edit/5 Updates customer info by ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerID,Name,PhoneNumber,ContactName,ContactEmail,VATNumber,Address")] Customer customer)
        {
            if (id != customer.CustomerID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewData["isEdit"] = true;
                return View("CustomerDetails", customer);
            }

            try
            {
                await _customerRepo.UpdateAsync(customer);
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException)
            {
                return View("DatabaseError");
            }
            catch (Exception)
            {
                if (!await _customerRepo.ExistsAsync(customer.CustomerID))
                {
                    return NotFound();
                }

                ModelState.AddModelError("", "An error occurred while updating. Please try again later.");
                ViewData["isEdit"] = true;
                return View("CustomerDetails", customer);
            }
        }

        // GET: Customer/Delete/5 displays customer information by ID
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _customerRepo.GetByIdAsync(id.Value);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customer/Delete/5 deletes customer information by ID
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _customerRepo.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (SqlException)
            {
                return View("DatabaseError");
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

    }
}
