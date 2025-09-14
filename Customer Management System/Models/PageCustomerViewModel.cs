namespace Customer_Management_System.Models
{
    public class PageCustomerViewModel
    {
        public IEnumerable<Customer> Customers { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public string SortColumn { get; set; }
        public string SortDirection { get; set; }
        public string? NameFilter { get; set; }
        public string? VatFilter { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

}
