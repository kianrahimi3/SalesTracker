using eBay.ApiClient.Auth.OAuth2;
using eBay.ApiClient.Auth.OAuth2.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SalesTrackerMVC.Models;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using static eBay.ApiClient.Auth.OAuth2.CredentialUtil;
using static System.Net.WebRequestMethods;

namespace SalesTrackerMVC.Controllers
{
    public class SalesController : Controller
    {
        class EbayToken
        {
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
        }


        private readonly SalesDBContext salesDBContext;
        
        private Dictionary<string, decimal> counts;
        private PageViewModel pageViewModel;

        private EbayToken ebayToken;

        public SalesController(SalesDBContext salesDBContext)
        {
            this.salesDBContext = salesDBContext;
            pageViewModel = new PageViewModel();

            pageViewModel.Sales = salesDBContext.Sales;
            pageViewModel.Items = salesDBContext.Items;

            ebayToken = new EbayToken();
        }

        public async Task<IActionResult> Index()
        {
            await SyncWithEbayAccount(null);

            return View(pageViewModel);
        }



        // Create Partial View
        public IActionResult OnGetPartial() =>
            PartialView("_AuthorPartialRP");



        // Check if Sale_ID exists in Sales Table
        private bool SalesExists(int id)
        {
            return salesDBContext.Sales.Any(x => x.Sale_Id == id);
        }

        /* EDIT */

        // GET: Sales/Edit/{id}
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sales = await salesDBContext.Sales.FindAsync(id);
            if (sales == null)
            {
                return NotFound();
            }

            pageViewModel.salesTable = sales;
            return View(pageViewModel.salesTable);
        }


        // POST : Sales/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Sale_Id, Item_ID, Price_Sold, Date_Sold, Category, Location_Sold")] Sales sales)
        //public async Task<IActionResult> Edit(int id, PageViewModel pageViewModel)
        {
            pageViewModel.salesTable = sales;

            if (id != pageViewModel.salesTable.Sale_Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    salesDBContext.Update(pageViewModel.salesTable);
                    await salesDBContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SalesExists(pageViewModel.salesTable.Sale_Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            pageViewModel.salesTable = pageViewModel.salesTable;
            return View(pageViewModel.salesTable);
        }

        /* DELETE */

        // GET: Sales/Delete/{id}
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sales = await salesDBContext.Sales.FirstOrDefaultAsync(x => x.Sale_Id == id);
            if (sales == null)
            {
                return NotFound();
            }

            pageViewModel.salesTable = sales;
            return View(pageViewModel);
        }

        // POST: Sales/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sales = await salesDBContext.Sales.FindAsync(id);
            if (sales != null)
            {
                salesDBContext.Remove(sales);
            }

            await salesDBContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /* DETAILS */

        // GET: Sales/Details/{id}
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sales = salesDBContext.Sales.FirstOrDefault(x => x.Sale_Id == id);
            if (sales == null)
            {
                return NotFound();
            }

            pageViewModel.salesTable = sales;
            return View(pageViewModel);
        }



        /* Other Functions */
        public async Task<IActionResult> SyncWithEbayAccount()
        {
 
            string clientId = "KianRahi-KianApp-PRD-dfa89c492-0c9137b8";
            string clientSecret = "PRD-fa89c4924aeb-3dce-4795-9bbd-35a2";
            string ruName = "Kian_Rahimi-KianRahi-KianAp-mxopcrtk";

            string clientString = $"{clientId}:{clientSecret}";
            byte[] clientStringEncoded = Encoding.UTF8.GetBytes(clientString);

            Chilkat.HttpRequest req = new Chilkat.HttpRequest();
            req.HttpVerb = "POST";
            req.Path = "/identity/v1/oauth2/token";
            req.ContentType = "application/x-www-form-urlencoded";
            req.AddParam("grant_type", "client_credentials");

            string scope = "https://api.ebay.com/oauth/api_scope https://api.ebay.com/oauth/api_scope/buy.order.readonly https://api.ebay.com/oauth/api_scope/buy.guest.order https://api.ebay.com/oauth/api_scope/sell.marketing.readonly https://api.ebay.com/oauth/api_scope/sell.marketing https://api.ebay.com/oauth/api_scope/sell.inventory.readonly https://api.ebay.com/oauth/api_scope/sell.inventory https://api.ebay.com/oauth/api_scope/sell.account.readonly https://api.ebay.com/oauth/api_scope/sell.account https://api.ebay.com/oauth/api_scope/sell.fulfillment.readonly https://api.ebay.com/oauth/api_scope/sell.fulfillment https://api.ebay.com/oauth/api_scope/sell.analytics.readonly https://api.ebay.com/oauth/api_scope/sell.marketplace.insights.readonly https://api.ebay.com/oauth/api_scope/commerce.catalog.readonly https://api.ebay.com/oauth/api_scope/buy.shopping.cart https://api.ebay.com/oauth/api_scope/buy.offer.auction";
            req.AddParam("scopes", scope);


            Chilkat.Http http = new Chilkat.Http();
            http.Login = clientId;
            http.Password = clientSecret;
            http.BasicAuth = true;

            Chilkat.HttpResponse res = http.PostUrlEncoded("https://api.ebay.com/identity/v1/oauth2/token", req);

            Chilkat.StringBuilder sbResponseBody = new Chilkat.StringBuilder();
            res.GetBodySb(sbResponseBody);
            Chilkat.JsonObject jResp = new Chilkat.JsonObject();
            jResp.LoadSb(sbResponseBody);
            jResp.EmitCompact = false;


            return View(pageViewModel);
        }
    }
}
