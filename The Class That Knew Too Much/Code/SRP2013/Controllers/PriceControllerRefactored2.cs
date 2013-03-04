using System;
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
            var priceSvc = new PriceService(ModelState, priceDb);
            if (ModelState.IsValid)
                price.Price = priceSvc.GetPrice(price);

            return View(price);
        }

    }

    public class PriceService
    {
        private ModelStateDictionary _modelState;
        private PriceDb _priceDb;

        public PriceService(ModelStateDictionary modelState, PriceDb priceDb)
        {
            this._modelState = modelState;
            this._priceDb = priceDb;
        }

        public decimal? GetPrice(PriceLookup price)
        {
            if (!string.IsNullOrEmpty(price.Upc))
            {
                var priceValue = _priceDb.GetPriceByUpc(price.Upc);
                if (priceValue == null)
                    _modelState.AddModelError("NotFound", "UPC not found");
                return priceValue;
            }
            else if (!string.IsNullOrEmpty(price.Isbn))
            {
                var priceValue = _priceDb.GetPriceByIsbn(price.Isbn);
                if (price.Price == null)
                    _modelState.AddModelError("NotFound", "ISBN not found");
                return priceValue;
            }
            else
                _modelState.AddModelError("Required", "You must enter a UPC or an ISBN");
            return null;
        }
    }

    public class PriceDb
    {
        private string _connectionString;

        public PriceDb(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public decimal? GetPriceByUpc(string upc)
        {
            var sql = "SELECT Price FROM Prices WHERE Upc = @upc";
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var sqlCommand = new SQLiteCommand(sql, conn);
                sqlCommand.Parameters.AddWithValue("upc", upc);
                var price = (decimal?)sqlCommand.ExecuteScalar();
                conn.Close();
                return price;
            }
        }

        public decimal? GetPriceByIsbn(string isbn)
        {
            var sql = "SELECT Price FROM Prices WHERE Isbn = @isbn";
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                var sqlCommand = new SQLiteCommand(sql, conn);
                sqlCommand.Parameters.AddWithValue("isbn", isbn);
                var price = (decimal?)sqlCommand.ExecuteScalar();
                conn.Close();
                return price;
            }
        }
    }
}