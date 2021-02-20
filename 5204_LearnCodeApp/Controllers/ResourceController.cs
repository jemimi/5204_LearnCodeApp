using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _5204_LearnCodeApp.Models;
using _5204_LearnCodeApp.Models.ViewModels;  //after need to add ViewModels folder
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace _5204_LearnCodeApp.Controllers
{
    public class ResourceController : Controller
    {

        //After configuring a WebAPI controller, 
        //configure a controller to access the WebAPI and serve Views
        //Set up: Add Controller> MVC controller with read/write actions (not entity)


        //Http Client is the proper way to connect to a webapi
        //https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0

        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;


        static ResourceController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };

            //getting the information from a database - 
            client = new HttpClient(handler);
            //change this to match your own local port number
            client.BaseAddress = new Uri("https://localhost:44366/api/");
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));


            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ACCESS_TOKEN);

        }

        // GET: Resource/List
        public ActionResult List()
        {
            string url = "resourcedata/getresources";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<ResourceDto> SelectedResources = response.Content.ReadAsAsync<IEnumerable<ResourceDto>>().Result;
                return View(SelectedResources);
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        // GET: Resource/Details/5
        public ActionResult Details(int id)
        {
            ShowResource ViewModel = new ShowResource();  
            string url = "resourcedata/findresource/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Resource data transfer object
                ResourceDto SelectedResource = response.Content.ReadAsAsync<ResourceDto>().Result;
                ViewModel.Resource = SelectedResource;

                //We don't need to throw any errors if this is null
                //A Resource not having any Comments is not an issue.
                url = "Resourcedata/getcommentsforresource/" + id;
                response = client.GetAsync(url).Result;
                //Can catch the status code (200 OK, 301 REDIRECT), etc.
                //Debug.WriteLine(response.StatusCode);
                IEnumerable<CommentDto> SelectedComments = response.Content.ReadAsAsync<IEnumerable<CommentDto>>().Result;
                ViewModel.ResourceComments = SelectedComments;


                url = "Resourcedata/getcodersforresource/" + id;
                response = client.GetAsync(url).Result;
                //Can catch the status code (200 OK, 301 REDIRECT), etc.
                //Debug.WriteLine(response.StatusCode);
                //Put data into Resource data transfer object
                IEnumerable<CoderDto> SelectedCoders = response.Content.ReadAsAsync<IEnumerable<CoderDto>>().Result;
                ViewModel.ResourceCoders = SelectedCoders;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Resource/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Resource/Create
        [HttpPost]

        //ValidateAntiForgeryToken: prevent cross-site request forgery attacks 
        // a harmful script element, malicious command, or code is sent 
        //from the browser of a trusted user
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Resource ResourceInfo)
        {
            Debug.WriteLine(ResourceInfo.ResourceTitle);
            string url = "resourcedata/addResource";
            Debug.WriteLine(jss.Serialize(ResourceInfo));
            HttpContent content = new StringContent(jss.Serialize(ResourceInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {

                int Resourceid = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id = Resourceid });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Resource/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "resourcedata/findresource/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Resource data transfer object
                ResourceDto SelectedResource = response.Content.ReadAsAsync<ResourceDto>().Result;
                return View(SelectedResource);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Resource/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(int id, Resource ResourceInfo)
        {
            Debug.WriteLine(ResourceInfo.ResourceTitle);
            string url = "resourcedata/updateresource/" + id;
            Debug.WriteLine(jss.Serialize(ResourceInfo));
            HttpContent content = new StringContent(jss.Serialize(ResourceInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        // original part of the scaffolded page: 

        // GET: Resource/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "resourcedata/findresource/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Resource data transfer object
                ResourceDto SelectedResource = response.Content.ReadAsAsync<ResourceDto>().Result;
                return View(SelectedResource);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }



        // POST: Resource/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(int id)
        {
            string url = "resourcedata/deleteResource/" + id;
            //post body is empty
            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public ActionResult Error()
        {
            return View();
        }


    }
}

// ************ NEXT *****************************
//After setting up the MVC controller with read/write actions, 
//***set up: 
// (1) VIEW MODEL folder
// (2) add a viewmodel class to display resource information with coder and comment info too