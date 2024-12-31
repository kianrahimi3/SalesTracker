using Microsoft.AspNetCore.Mvc;
using SalesTrackerMVC.Models;
using System.Diagnostics;

using SalesTrackerMVC.Models;

namespace SalesTrackerMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SalesDBContext salesDBContext;

        private PageViewModel pageViewModel;

        public HomeController(ILogger<HomeController> logger, SalesDBContext context)
        {
            _logger = logger;
            salesDBContext = context;
            pageViewModel = new PageViewModel();

            pageViewModel.counts = new Dictionary<string, decimal>();
            pageViewModel.counts.Add("TotalSold", 0);
            pageViewModel.counts.Add("TotalPurchased", 0);

            pageViewModel.Sales = salesDBContext.Sales;
            pageViewModel.Items = salesDBContext.Items;
        }

        public IActionResult Index()
        {
            var totalSold = salesDBContext.Sales.ToList().Sum(x => x.Price_Sold);
            pageViewModel.counts["TotalSold"] = totalSold;

            var totalPurchased = salesDBContext.Sales
                .Join(salesDBContext.Items,
                    sales => sales.Item_ID,
                    items => items.Item_ID,
                    (sales, items) => new { Sales = sales, Items = items })
                .ToList().Sum(x => x.Items.Purchase_Price);
            pageViewModel.counts["TotalPurchased"] = totalPurchased;

            return View(pageViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        public IActionResult GetUser()
        {
            /*
            IncrementalHash sha256 = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            sha256.AppendData(Encoding.UTF8.GetBytes(challengeCode));
            sha256.AppendData(Encoding.UTF8.GetBytes(verificationToken));
            sha256.AppendData(Encoding.UTF8.GetBytes(endpoint));
            byte[] bytes = sha256.GetHashAndReset();
            Console.WriteLine(BitConverter.ToString(bytes).Replace("-", string.Empty).ToLower());
            */

            return Redirect(nameof(Index));
        }
    }
}
