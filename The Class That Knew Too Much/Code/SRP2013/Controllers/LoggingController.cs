using System;
using System.Collections.Generic;
using System.Web.Mvc;
using SRPTalk.Models;

namespace SRPTalk.Controllers
{
    public class LoggingController : Controller
    {
        public ViewResult Index()
        {
            var model = new CarsHomePageModel();

            model.Makes = DataLayerCars.GetAllMakes();
            model.BodyTypes = DataLayerCars.GetAllBodyTypes();
            model.Dealerships = DataLayerDealerships.GetAllDealers();

            model.Log = Logger.EntireLog;

            return View(model);
        }
    }

    public class DataLayerCars
    {
        public static List<string> GetAllMakes()
        {
            Logger.Log("Entering DataLayerCars.GetAllMakes"); 
            try
            {
                return new List<string> { "Honda", "Toyota", "Kia", "Ford", "Hyundai", "Ferrari" };
            }
            finally
            {
                Logger.Log("Exiting DataLayerCars.GetAllMakes");
            }
        }

        public static List<string> GetAllBodyTypes()
        {
            Logger.Log("Entering DataLayerCars.GetAllBodyTypes"); 
            try
            {
                return new List<string> { "Sedan", "Coupe", "SUV", "Minivan", };
            }
            finally
            {
                Logger.Log("Exiting DataLayerCars.GetAllBodyTypes");
            }
        }
    }

    public class DataLayerDealerships
    {
        public static List<string> GetAllDealers()
        {
            Logger.Log("Entering DataLayerDealerships.GetAllDealers"); 
            try
            {
                return new List<string> {"Ricart", "Germain", "Byers", "Lindsay", "Paul's Cars"};
            }
            finally
            {
                Logger.Log("Exiting DataLayerDealerships.GetAllDealers");
            }
        }
    }

    public static class Logger
    {
        static readonly List<string> _logMessages = new List<string>();
        public static void Log(string message)
        {
            _logMessages.Add(string.Format("[{0}] {1}\n", DateTime.Now, message));
        }
        public static IEnumerable<string> EntireLog
        {
            get { return _logMessages; }
        }
    }
}