using System.Web.Mvc;
using SRPTalk.Models;
using System.Data.SQLite;

namespace SRPTalk.Controllers
{
    public class PriceController : Controller
    {
        public ViewResult Index()
        {
            return View(new PriceLookup());
        }

        [HttpPost]
        public ViewResult Index(PriceLookup price)
        {
            var connectionString = @"Data Source=" + Server.MapPath("App_Data/prices.s3db");
            var priceDb = new PriceDb(connectionString);
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(price.Upc))
                {
                    priceDb.GetPriceByUpc(price);
                    if(price.Price == null)
                        ModelState.AddModelError("NotFound", "UPC not found");
                }
                else if (!string.IsNullOrEmpty(price.Isbn))
                {
                    priceDb.GetPriceByIsbn(price);
                    if (price.Price == null)
                        ModelState.AddModelError("NotFound", "ISBN not found");
                }
                else
                    ModelState.AddModelError("Required", "You must enter a UPC or an ISBN");
            }

            return View(price);
        }

    }

    public class PriceDb
    {
        private string _connectionString;

        public PriceDb(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public void GetPriceByUpc(PriceLookup price)
        {
            var sql = "SELECT Price FROM Prices WHERE Upc = @upc";
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var sqlCommand = new SQLiteCommand(sql, conn);
                sqlCommand.Parameters.AddWithValue("upc", price.Upc);
                var result = sqlCommand.ExecuteScalar();
                if (result != null)
                    price.Price = (decimal)result;
                conn.Close();
            }
        }

        public void GetPriceByIsbn(PriceLookup price)
        {
            var sql = "SELECT Price FROM Prices WHERE Isbn = @isbn";
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var sqlCommand = new SQLiteCommand(sql, conn);
                sqlCommand.Parameters.AddWithValue("isbn", price.Isbn);
                var result = sqlCommand.ExecuteScalar();
                if (result != null)
                    price.Price = (decimal)result;
                conn.Close();
            }
        }
    }
}