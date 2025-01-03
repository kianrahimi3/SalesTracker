using eBay.ApiClient.Auth.OAuth2;
using eBay.ApiClient.Auth.OAuth2.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using SalesTrackerMVC.Models;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using static eBay.ApiClient.Auth.OAuth2.CredentialUtil;
using static System.Net.WebRequestMethods;


using EbaySharp;
using EbaySharp.Controllers;
using EbaySharp.Entities.Developer.KeyManagement.SigningKey;
using EbaySharp.Entities.Identity;
using Microsoft.IdentityModel.Tokens;
using Selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Collections.Specialized;

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

        private string clientId = "KianRahi-KianApp-PRD-dfa89c492-0c9137b8";
        private string clientSecret = "PRD-fa89c4924aeb-3dce-4795-9bbd-35a2";
        private string ruName = "Kian_Rahimi-KianRahi-KianAp-mxopcrtk";
        //private string scope = "https://api.ebay.com/oauth/api_scope https://api.ebay.com/oauth/api_scope/buy.order.readonly https://api.ebay.com/oauth/api_scope/buy.guest.order https://api.ebay.com/oauth/api_scope/sell.marketing.readonly https://api.ebay.com/oauth/api_scope/sell.marketing https://api.ebay.com/oauth/api_scope/sell.inventory.readonly https://api.ebay.com/oauth/api_scope/sell.inventory https://api.ebay.com/oauth/api_scope/sell.account.readonly https://api.ebay.com/oauth/api_scope/sell.account https://api.ebay.com/oauth/api_scope/sell.fulfillment.readonly https://api.ebay.com/oauth/api_scope/sell.fulfillment https://api.ebay.com/oauth/api_scope/sell.analytics.readonly https://api.ebay.com/oauth/api_scope/sell.marketplace.insights.readonly https://api.ebay.com/oauth/api_scope/commerce.catalog.readonly https://api.ebay.com/oauth/api_scope/buy.shopping.cart https://api.ebay.com/oauth/api_scope/buy.offer.auction";
        private string scope = "https://api.ebay.com/oauth/api_scope https://api.ebay.com/oauth/api_scope/sell.marketing.readonly https://api.ebay.com/oauth/api_scope/sell.marketing https://api.ebay.com/oauth/api_scope/sell.inventory.readonly https://api.ebay.com/oauth/api_scope/sell.inventory https://api.ebay.com/oauth/api_scope/sell.account.readonly https://api.ebay.com/oauth/api_scope/sell.account https://api.ebay.com/oauth/api_scope/sell.fulfillment.readonly https://api.ebay.com/oauth/api_scope/sell.fulfillment https://api.ebay.com/oauth/api_scope/sell.analytics.readonly https://api.ebay.com/oauth/api_scope/sell.finances https://api.ebay.com/oauth/api_scope/sell.payment.dispute https://api.ebay.com/oauth/api_scope/commerce.identity.readonly https://api.ebay.com/oauth/api_scope/sell.reputation https://api.ebay.com/oauth/api_scope/sell.reputation.readonly https://api.ebay.com/oauth/api_scope/commerce.notification.subscription https://api.ebay.com/oauth/api_scope/commerce.notification.subscription.readonly https://api.ebay.com/oauth/api_scope/sell.stores https://api.ebay.com/oauth/api_scope/sell.stores.readonly https://api.ebay.com/oauth/scope/sell.edelivery";
        //private string secureUrl = "https://auth.ebay.com/oauth2/authorize?client_id=KianRahi-KianApp-PRD-dfa89c492-0c9137b8&response_type=code&redirect_uri=Kian_Rahimi-KianRahi-KianAp-mxopcrtk&scope=https://api.ebay.com/oauth/api_scope https://api.ebay.com/oauth/api_scope/sell.marketing.readonly https://api.ebay.com/oauth/api_scope/sell.marketing https://api.ebay.com/oauth/api_scope/sell.inventory.readonly https://api.ebay.com/oauth/api_scope/sell.inventory https://api.ebay.com/oauth/api_scope/sell.account.readonly https://api.ebay.com/oauth/api_scope/sell.account https://api.ebay.com/oauth/api_scope/sell.fulfillment.readonly https://api.ebay.com/oauth/api_scope/sell.fulfillment https://api.ebay.com/oauth/api_scope/sell.analytics.readonly https://api.ebay.com/oauth/api_scope/sell.finances https://api.ebay.com/oauth/api_scope/sell.payment.dispute https://api.ebay.com/oauth/api_scope/commerce.identity.readonly https://api.ebay.com/oauth/api_scope/sell.reputation https://api.ebay.com/oauth/api_scope/sell.reputation.readonly https://api.ebay.com/oauth/api_scope/commerce.notification.subscription https://api.ebay.com/oauth/api_scope/commerce.notification.subscription.readonly https://api.ebay.com/oauth/api_scope/sell.stores https://api.ebay.com/oauth/api_scope/sell.stores.readonly https://api.ebay.com/oauth/scope/sell.edelivery";
        private string secureUrl = "https://auth.ebay.com/oauth2/ThirdPartyAuthSucessFailure?isAuthSuccessful=true&code=v%5E1.1%23i%5E1%23I%5E3%23f%5E0%23r%5E1%23p%5E3%23t%5EUl41XzU6REZFODJBMjRCMzUxNTExMDk2RjU3NTAwNEE0MDZBN0NfMF8xI0VeMjYw&expires_in=299";

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
            await SyncWithEbayAccount();

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
            await ConnectToEbay();

            Transactions transactions = await GetEbayTransactions();

            return View(pageViewModel);
        }

        public async Task ConnectToEbay()
        {
            

            string clientString = $"{clientId}:{clientSecret}";
            byte[] clientStringEncoded = Encoding.UTF8.GetBytes(clientString);

            Chilkat.HttpRequest req = new Chilkat.HttpRequest();
            req.HttpVerb = "POST";
            req.Path = "/identity/v1/oauth2/token";
            req.ContentType = "application/x-www-form-urlencoded";
            
            req.AddParam("grant_type", "authorization_code");
            req.AddParam("scopes", scope);

            /*
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

            ebayToken.access_token = jResp.StringOf("access_token");
            ebayToken.expires_in = jResp.IntOf("expires_in");
            ebayToken.token_type = jResp.StringOf("token_type");
            */


            var parameters = new Dictionary<string, string>
                {
                    { "client_id", clientId },
                    { "redirect_uri", ruName },
                    { "response_type", "code" },
                    { "scope", scope }
                    // state
                };

            string url = "https://auth.ebay.com/oauth2/authorize?" +
                    $"client_id={parameters["client_id"]}&" +
                    $"redirect_uri={parameters["redirect_uri"]}&" +
                    $"response_type={parameters["response_type"]}&" +
                    $"scope={parameters["scope"]}";

            var webRequest = (HttpWebRequest)HttpWebRequest.Create(url);

            using (HttpClient authorizeClient = new HttpClient())
            {
                parameters = new Dictionary<string, string>
                {
                    { "client_id", clientId },
                    { "redirect_uri", ruName },
                    { "response_type", "code" },
                    { "prompt", "login" },
                    { "scope", scope }
                    // state
                };

                //authorizeClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                url = "https://auth.ebay.com/oauth2/authorize?" +
                    $"client_id={parameters["client_id"]}&" +
                    $"redirect_uri={parameters["redirect_uri"]}&" +
                    $"response_type={parameters["response_type"]}&" +
                    $"prompt={parameters["prompt"]}&" +
                    $"scope={parameters["scope"]}";

                var content = new FormUrlEncodedContent(parameters);
                //var response = await authorizeClient.GetAsync("https://auth.sandbox.ebay.com/oauth2/authorize", content);
                var response = await authorizeClient.GetAsync(url);

                var body = await response.Content.ReadAsStringAsync();


                IWebDriver driver = new FirefoxDriver();
               // url = "https://auth.ebay.com/oauth2/authorize?" +
               //     "client_id=KianRahi-KianApp-PRD-dfa89c492-0c9137b8&" +
               //     "response_type=code&" +
               //     "redirect_uri=Kian_Rahimi-KianRahi-KianAp-mxopcrtk&" +
               //     "scope=https://api.ebay.com/oauth/api_scope https://api.ebay.com/oauth/api_scope/sell.marketing.readonly https://api.ebay.com/oauth/api_scope/sell.marketing https://api.ebay.com/oauth/api_scope/sell.inventory.readonly https://api.ebay.com/oauth/api_scope/sell.inventory https://api.ebay.com/oauth/api_scope/sell.account.readonly https://api.ebay.com/oauth/api_scope/sell.account https://api.ebay.com/oauth/api_scope/sell.fulfillment.readonly https://api.ebay.com/oauth/api_scope/sell.fulfillment https://api.ebay.com/oauth/api_scope/sell.analytics.readonly https://api.ebay.com/oauth/api_scope/sell.finances https://api.ebay.com/oauth/api_scope/sell.payment.dispute https://api.ebay.com/oauth/api_scope/commerce.identity.readonly https://api.ebay.com/oauth/api_scope/sell.reputation https://api.ebay.com/oauth/api_scope/sell.reputation.readonly https://api.ebay.com/oauth/api_scope/commerce.notification.subscription https://api.ebay.com/oauth/api_scope/commerce.notification.subscription.readonly https://api.ebay.com/oauth/api_scope/sell.stores https://api.ebay.com/oauth/api_scope/sell.stores.readonly https://api.ebay.com/oauth/scope/sell.edelivery";
                driver.Url = url;


                string checkUrl = "https://auth.ebay.com/oauth2/ThirdPartyAuthSucessFailure?isAuthSuccessful=true";
                while (!driver.Url.Contains(checkUrl))
                {

                }
                NameValueCollection queryParams = HttpUtility.ParseQueryString(driver.Url);
                string authCode = queryParams.Get("code");
                driver.Quit();
            }

            using (HttpClient client = new HttpClient())
            {
                parameters = new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                   // { "code", authorizationCode },
                    { "redirect_uri", ruName }
                };

                var content = new FormUrlEncodedContent(parameters);
                var response = await client.PostAsync("https://api.ebay.com/identity/v1/oauth2/token", content);

                string body = await response.Content.ReadAsStringAsync();
            }



        }

        public async Task<Transactions> GetEbayTransactions()
        {
            /*
            IdentityController identityController = new IdentityController();
            string refresh_token = await identityController.GetRefreshToken(clientId, clientSecret, secureUrl, ruName);
            ClientCredentials cc = await identityController.GetClientCredentials(clientId, clientSecret, refresh_token, scope);
            

            EbayController ebayController = new EbayController(ebayToken.access_token);
            var t = await ebayController.GetTransactions();
            */

            
            string url = "https://apiz.ebay.com/sell/finances/v1/transaction/";

            HttpClient client = new HttpClient();
            
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("Authorization", ebayToken.access_token);


            var response = await client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            
            Transactions transactions = new Transactions();
            transactions = JsonConvert.DeserializeObject<Transactions>(await  response.Content.ReadAsStringAsync());


            
            return transactions;
        }
        /*
        public async Task<string> GetRefreshToken()
        {
            IdentityController identityController = new EbaySharp.Controllers.IdentityController();
            string refreshToken = await identityController.GetRefreshToken(clientId, clientSecret, secureUrl, ruName);

            return refreshToken;
        }
        */
    }
}
