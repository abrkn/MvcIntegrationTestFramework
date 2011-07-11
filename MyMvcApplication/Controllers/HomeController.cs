using System.Configuration;
using System.Web;
using System.Web.Mvc;

namespace MyMvcApplication.Controllers
{
    using System;

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            ViewBag.CustomMessage = ConfigurationManager.AppSettings["TestMessage"];

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult DoStuffWithSessionAndCookies()
        {
            Session["myIncrementingSessionItem"] = ((int?)(Session["myIncrementingSessionItem"] ?? 0)) + 1;
            Response.Cookies.Add(new HttpCookie("mycookie", "myval"));
            return Content("OK");
        }

        public ActionResult MakeAnimalSound(string id)
        {
            ViewBag.Animal = id;
            ViewBag.Sound = this.AnimalSoundProvider.GetSoundFromAnimalType(id);

            return this.View("AnimalSound");
        }

        [Authorize]
        public ActionResult SecretAction()
        {
            return Content("Hello, you're logged in as " + User.Identity.Name);
        }

        public IAnimalSoundProvider AnimalSoundProvider { get; set; }

        public HomeController()
        {
            this.AnimalSoundProvider = new SiteAnimalSoundProvider();
        }
    }

    public interface IAnimalSoundProvider
    {
        string GetSoundFromAnimalType(string animalType);
    }

    public class SiteAnimalSoundProvider : IAnimalSoundProvider
    {
        public string GetSoundFromAnimalType(string animalType)
        {
            switch (animalType)
            {
                case "Dog":
                    return "Woof!";
                case "Cat":
                    return "Meooow...";
                default:
                    return "Caplonk!";
            }
        }
    }
}
