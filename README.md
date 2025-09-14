# Customer Management System Roadmap

## Project Requirements

### Customer Model
**Entities**
- **CustomerID**
- **Name** *(required)*
- **AddressID (FK)** – links customer to address  
- **TelephoneNumber**
- **ContactName**
- **ContactEmail** *(must follow email structure)*
- **VATNumber** *(South African structure: starts with `4`, 10 digits, unique)*
- **IsVerified** *(boolean, default = false)*
- **VerificationToken** *(string, for email confirmation)*
- **TokenExpiry** *(time limit for token validity)*

### Address Model
**Entities**
- **AddressID**
- **StreetName**
- **Suburb**
- **City**
- **Province**
- **PostalCode**
- **Notes**

> Stored in a separate table to keep the customer table lean.  
> Displayed as a single string for compact usage.

---

## Features & Implementations

### Customer Create & Edit Page
- Unified into one Razor Page.
- Controlled via boolean: edit vs create.
- Same form reused with either prefilled or empty fields.
- Separate logic for processing.

### Pagination
- Loads records page by page (not the full dataset).
- Displays:
  - Current page
  - Total pages
  - Total records
- Uses stored procedure for retrieval.  

**Issue:** Loading all records slows down performance.  
**Improvement:** Preload ~5 pages of data to reduce DB queries while keeping performance balanced.

### Email Verification
- Uses **FluentEmail** library.
- Sends email with verification link.
- Token-based system *(expires after set time)*.
- Unverified emails displayed in red.

**Issue:** Avoiding reliance on external APIs (data security concern).  
**Improvement:** Implement custom API with:
- Domain verification
- Syntax validation
- MX lookup
- SMTP validation
- Disposable email detection

### Stored Procedures
**Implementation**
- Used for paginated customer retrieval.
- Returns both paged data and total count.

### Sorting
- Supports ascending/descending by text, numbers, or null values.
- Displays direction indicator (arrow).

**Improvements**
- Modernized sort indicators.
- Make entire header clickable.
- Multi-field sorting with priority.

### Filtering
- Search bar for VAT and Name.
- Supports partial and full text matches.

**Improvements**
- Hide filters behind a button for a modern, uncluttered design.

### Deleting Users
- Deletion requires confirmation (**SweetAlert dialog**).

**Improvements**
- Add undo delete option.

---

## Entity Framework Usage
- Models created via EF.
- Migrations used for schema updates.
- LINQ methods used for querying.
- Repository pattern for DB interactions.

---

## Tech Stack
- **Backend:** ASP.NET MVC (C#)
- **Database:** Azure SQL Database / SQL Server (local)
- **Hosting:** Azure Web App Service
- **Frontend:** JavaScript, Bootstrap 5, CSS  
- **Libraries & Tools:**
  - FluentEmail (email verification)
  - SweetAlert2 (dialogs)
  - jQuery (events & selectors)
  - Progressive Web App (downloadable app)

---

## Stored Procedure
```sql
CREATE PROCEDURE [dbo].[GetCustomersPaged]
    @PageNumber INT,
    @PageSize   INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Result set: customers with address
    SELECT 
        c.CustomerID,
        c.Name,
        c.PhoneNumber,
        c.ContactName,
        c.ContactEmail,
        c.VATNumber,
        a.StreetName,
        a.Suburb,
        a.City,
        a.Province,
        a.PostalCode
    FROM Customers c
    INNER JOIN Address a ON c.AddressId = a.Id
    ORDER BY c.CustomerID
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;

    -- Total count
    SELECT COUNT(*) AS TotalCount
    FROM Customers;
END
