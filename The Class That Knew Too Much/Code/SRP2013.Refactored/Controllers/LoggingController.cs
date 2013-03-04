using System;
using System.Collections.Generic;
using System.Web.Mvc;
using PostSharp.Aspects;
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

    [Serializable]
    public class LoggingAspect : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
            Logger.Log(string.Format("Entering {0}.{1}",
                args.Method.DeclaringType.Name, args.Method.Name));
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            Logger.Log(string.Format("Exiting {0}.{1}",
                args.Method.DeclaringType.Name, args.Method.Name));
        }
    }

    [LoggingAspect]
    public class DataLayerCars
    {
        public static List<string> GetAllMakes()
        {
            return new List<string> { "Honda", "Toyota", "Kia", "Ford", "Hyundai", "Ferrari" };
        }

        public static List<string> GetAllBodyTypes()
        {
            return new List<string> { "Sedan", "Coupe", "SUV", "Minivan", };
        }
    }

    [LoggingAspect]
    public class DataLayerDealerships
    {
        public static List<string> GetAllDealers()
        {
            return new List<string> {"Ricart", "Germain", "Byers", "Lindsay", "Paul's Cars"};
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