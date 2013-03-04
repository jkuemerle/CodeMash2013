using System;
using System.Data.SQLite;
using System.IO;
using System.Web.Mvc;
using SRPTalk.Models;

namespace SRPTalk.Controllers
{
    public class PriceController : Controller
    {
        readonly PriceService _priceService;

        public PriceController()
        {
            var sqlLiteDbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "prices.s3db");
            var priceDb = new PriceDb("Data Source=" + sqlLiteDbPath);
            _priceService = new PriceService(new ModelStateAdapter(ModelState), priceDb);
        }

        public ViewResult Index()
        {
            return View(new PriceLookup());
        }

        [HttpPost]
        public ViewResult Index(PriceLookup priceLookup)
        {
            if(ModelState.IsValid)
                priceLookup.Price = _priceService.GetPrice(priceLookup);

            return View(priceLookup);
        }
    }

    public interface IValidationAdapter
    {
        void AddModelError(string key, string errorMessage);
    }

    public class ModelStateAdapter : IValidationAdapter
    {
        readonly ModelStateDictionary _modelState;

        public ModelStateAdapter(ModelStateDictionary modelState)
        {
            _modelState = modelState;
        }

        public void AddModelError(string key, string errorMessage)
        {
            _modelState.AddModelError(key, errorMessage);
        }
    }

    public class PriceService //: IPriceService
    {
        readonly IValidationAdapter _modelState;
        readonly PriceDb _priceDb;

        public PriceService(IValidationAdapter modelState, PriceDb priceDb)
        {
            _modelState = modelState;
            _priceDb = priceDb;
        }

        public decimal? GetPrice(PriceLookup priceLookup)
        {
            if (!string.IsNullOrEmpty(priceLookup.Upc))
            {
                if (_priceDb.DoesUpcExist(priceLookup.Upc))
                    return _priceDb.GetPriceByUpc(priceLookup.Upc);
                _modelState.AddModelError("NotFound", "UPC not found");
            }
            else if (!string.IsNullOrEmpty(priceLookup.Isbn))
            {
                if (_priceDb.DoesIsbnExist(priceLookup.Isbn))
                    return _priceDb.GetPriceByIsbn(priceLookup.Isbn);
                _modelState.AddModelError("NotFound", "ISBN not found");
            }
            else
                _modelState.AddModelError("Required", "You must enter a UPC or an ISBN");
            return null;
        }
    }

    public class PriceDb
    {
        readonly string _connectionString;

        public PriceDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        public decimal GetPriceByUpc(string upc)
        {
            var sql = "SELECT Price FROM Prices WHERE Upc = @upc";
            var cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("upc", upc);
            return ExecuteCommand<decimal>(cmd);
        }

        public decimal GetPriceByIsbn(string isbn)
        {
            var sql = "SELECT Price FROM Prices WHERE Isbn = @isbn";
            var cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("isbn", isbn);
            return ExecuteCommand<decimal>(cmd);
        }

        public bool DoesUpcExist(string upc)
        {
            var sql = "SELECT COUNT(1) FROM Prices WHERE Upc = @upc";
            var cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("upc", upc);
            return ExecuteCommand<long>(cmd) > 0;
        }

        public bool DoesIsbnExist(string isbn)
        {
            var sql = "SELECT COUNT(1) FROM Prices WHERE Isbn = @isbn";
            var cmd = new SQLiteCommand(sql);
            cmd.Parameters.AddWithValue("isbn", isbn);
            return ExecuteCommand<int>(cmd) > 0;
        }

        T ExecuteCommand<T>(SQLiteCommand cmd)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                cmd.Connection = conn;
                var result = (T) cmd.ExecuteScalar();
                conn.Close();
                return result;
            }
        }
    }
}