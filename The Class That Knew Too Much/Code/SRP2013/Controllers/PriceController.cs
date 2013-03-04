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
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(price.Upc))
                {
                    var sql = "SELECT Price FROM Prices WHERE Upc = @upc";
                    using (var conn = new SQLiteConnection(connectionString))
                    {
                        conn.Open();
                        var sqlCommand = new SQLiteCommand(sql, conn);
                        sqlCommand.Parameters.AddWithValue("upc", price.Upc);
                        var result = sqlCommand.ExecuteScalar();
                        if (result == null)
                            ModelState.AddModelError("NotFound", "UPC not found");
                        else
                            price.Price = (decimal) sqlCommand.ExecuteScalar();
                        conn.Close();
                    }
                }
                else if (!string.IsNullOrEmpty(price.Isbn))
                {
                    var sql = "SELECT Price FROM Prices WHERE Isbn = @isbn";
                    using (var conn = new SQLiteConnection(connectionString))
                    {
                        conn.Open();
                        var sqlCommand = new SQLiteCommand(sql, conn);
                        sqlCommand.Parameters.AddWithValue("isbn", price.Isbn);
                        var result = sqlCommand.ExecuteScalar();
                        if (result == null)
                            ModelState.AddModelError("NotFound", "ISBN not found");
                        else
                            price.Price = (decimal) sqlCommand.ExecuteScalar();
                        conn.Close();
                    }
                }
                else
                    ModelState.AddModelError("Required", "You must enter a UPC or an ISBN");
            }

            return View(price);
        }
    }
}