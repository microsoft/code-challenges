using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_core_mssql_app.Controllers
{
    public class HomeController : Controller
    {
        public Repository.IPersonRepository PersonRepo { get; set; }

        public HomeController(Repository.IPersonRepository _repo)
        {
            PersonRepo = _repo;
        }

        public IActionResult Index()
        {
            List<Models.Person> personCollection = PersonRepo.GetAll().ToList();
            return View(personCollection);
        }

        [HttpPost]
        public IActionResult Index(Microsoft.AspNetCore.Http.FormCollection formCollection)
        {
            try
            {
                Models.Person person = new Models.Person()
                {
                    Name = Request.Form["personname"].ToString(),
                    Address = Request.Form["address"].ToString(),
                    ContactNo = Request.Form["contactno"].ToString(),
                    Picture = string.Empty
                };

                PersonRepo.Add(person);
            }
            catch (Exception) { ViewBag.ClusterIPError = "Unable to add records. Please verify your connection."; }

            List<Models.Person> personCollection = PersonRepo.GetAll().ToList();
            return View(personCollection);
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
