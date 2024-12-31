using Microsoft.EntityFrameworkCore;

namespace SalesTrackerMVC.Models
{
    public class PageViewModel
    {
        //public List<Sales> salesTable { get; set; }
        public DbSet<Sales> Sales { get; set; }
        //public List<Items> itemsTable { get; set; }
        public DbSet<Items> Items { get; set; }
        public Dictionary<string, decimal> counts { get; set; }

        public Sales salesTable { get; set; }
        public Items itemsTable { get; set; }
    }
}
