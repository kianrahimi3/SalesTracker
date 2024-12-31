using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SalesTrackerMVC.Models
{
    public class Sales
    {
        [Key]
        public int Sale_Id { get; set; }
        public int Item_ID { get; set; }
        public decimal Price_Sold { get; set; }
        public DateTime Date_Sold { get; set; }
        public int Category { get; set; }
        public int Location_Sold { get; set; }       
    }

    public class Items
    {
        [Key]
        public int Item_ID { get; set; }
        public string Item { get; set; }
        public DateTime Purchase_Date { get; set; }
        public decimal Purchase_Price { get; set; }
        public int Category_ID { get; set; }
    }


    public class SalesDBContext : DbContext
    {
        public SalesDBContext(DbContextOptions<SalesDBContext> options) 
            : base(options) { }
        public DbSet<Sales> Sales { get; set; }
        public DbSet<Items> Items { get; set; }
    }

    
}
