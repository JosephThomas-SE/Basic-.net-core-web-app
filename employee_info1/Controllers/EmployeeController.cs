using FireSharp.Config;
using FireSharp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using employee_info1.Models;
using FireSharp.Response;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis;

namespace employee_info1.Controllers
{
    public class EmployeeController : Controller
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = {Firebase Solution Dashboard -> Project overview -> Project settings -> Service Accounts -> Database Secrets},
            BasePath = { Firebase Solution Dashboard -> RealTime Database -> You can see the database link}
        };


        IFirebaseClient client;


        //Get data from Employee info Database
        public IActionResult Index()
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Employees");
            dynamic data = JsonConvert.DeserializeObject<dynamic>(response.Body);
            var list = new List<Employee>();
            if (data != null)
            {
                foreach (var item in data)
                {
                    list.Add(JsonConvert.DeserializeObject<Employee>(((JProperty)item).Value.ToString()));
                }
            }
            return View(list);
        }


        //Create operation of employee info
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Employee employee)
        {
            try
            {
                client = new FireSharp.FirebaseClient(config);
                var data = employee;
                PushResponse response = client.Push("Employees/", data);
                data.Id = response.Result.name;
                SetResponse setResponse = client.Set("Employees/" + data.Id, data);

                if (setResponse.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ModelState.AddModelError(string.Empty, "Added Succesfully");
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Something went wrong!!");
                    return View();
                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
            }

            return View();
        }

        //Update of employee info
        [HttpGet]
        public ActionResult Edit(string Id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Get("Employees/" + Id);
            Employee data = JsonConvert.DeserializeObject<Employee>(response.Body);
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(Employee employee)
        {
            client = new FireSharp.FirebaseClient(config);
            SetResponse response = client.Set("Employees/" + employee.Id, employee);
            return RedirectToAction("Index");
        }

        //Deletion of Employee info
        [HttpDelete]
        public ActionResult Delete(string Id)
        {
            client = new FireSharp.FirebaseClient(config);
            FirebaseResponse response = client.Delete("Employees/" + Id);
            return RedirectToAction("Index");
        }
    }
}
