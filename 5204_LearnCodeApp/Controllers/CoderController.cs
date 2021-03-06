using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using _5204_LearnCodeApp.Models;
using _5204_LearnCodeApp.Models.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace _5204_LearnCodeApp.Controllers
{
    public class CoderController : Controller
    {
        //Http Client is the proper way to connect to a webapi
        //https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0

        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;


        static CoderController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };

            //getting the information from a database - 
            client = new HttpClient(handler);
            //*****change this to match your own local port number
            client.BaseAddress = new Uri("https://localhost:44366/api/"); //changed the port number to my pc but doesn't work 
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));


            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ACCESS_TOKEN);

        }

        /*
        // GET: Coder/List?{PageNum}
        // If the page number is not included, set it to 0
        public ActionResult List(int PageNum = 0)
        {
            // Grab all coders
            string url = "coderdata/getcoders";
            // Send off an HTTP request
            // GET : /api/coderdata/getcoders
            // Retrieve response
            HttpResponseMessage response = client.GetAsync(url).Result;
            // If the response is a success, proceed
            if (response.IsSuccessStatusCode)
            {
                // Fetch the response content into IEnumerable<coderDto>
                IEnumerable<CoderDto> SelectedCoders = response.Content.ReadAsAsync<IEnumerable<CoderDto>>().Result;

                // -- Start of Pagination Algorithm --

                // Find the total number of coders
                int CoderCount = SelectedCoders.Count();
                // Number of coders to display per page
                int PerPage = 4;
                // Determines the maximum number of pages (rounded up), assuming a page 0 start.
                int MaxPage = (int)Math.Ceiling((decimal)CoderCount / PerPage) - 1;

                // Lower boundary for Max Page
                if (MaxPage < 0) MaxPage = 0;
                // Lower boundary for Page Number
                if (PageNum < 0) PageNum = 0;
                // Upper Bound for Page Number
                if (PageNum > MaxPage) PageNum = MaxPage;

                // The Record Index of our Page Start
                int StartIndex = PerPage * PageNum;

                //Helps us generate the HTML which shows "Page 1 of ..." on the list view
                ViewData["PageNum"] = PageNum;
                ViewData["PageSummary"] = " " + (PageNum + 1) + " of " + (MaxPage + 1) + " ";

                // -- End of Pagination Algorithm --


                // Send back another request to get coders, this time according to our paginated logic rules
                // GET api/coderdata/getcoderspage/{startindex}/{perpage}
                url = "coderdata/getcoderspage/" + StartIndex + "/" + PerPage;
                response = client.GetAsync(url).Result;

                // Retrieve the response of the HTTP Request
                IEnumerable<CoderDto> SelectedCodersPage = response.Content.ReadAsAsync<IEnumerable<CoderDto>>().Result;

                //Return the paginated of coders instead of the entire list
                return View(SelectedCodersPage);

            }
            else
            {
                // If we reach here something went wrong with our list algorithm
                return RedirectToAction("Error");
            }


        }

       */
        // GET: Coder/List
        public ActionResult List()
        {
            string url = "coderdata/getcoders";
            HttpResponseMessage response = client.GetAsync(url).Result;
            Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<CoderDto> SelectedCoders = response.Content.ReadAsAsync<IEnumerable<CoderDto>>().Result;
                return View(SelectedCoders); 
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        

        // GET: Coder/Details/5
        public ActionResult Details(int id)
        {
            ShowCoder ViewModel = new ShowCoder();
            string url = "coderdata/findcoder/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Coder data transfer object
                CoderDto SelectedCoder = response.Content.ReadAsAsync<CoderDto>().Result;
                ViewModel.Coder = SelectedCoder;

                //We don't need to throw any errors if this is null
                //A Coder not having any comments is not an issue.
                url = "coderdata/getcommentsforcoder/" + id;
                response = client.GetAsync(url).Result;
                //Can catch the status code (200 OK, 301 REDIRECT), etc.
                //Debug.WriteLine(response.StatusCode);
                IEnumerable<CommentDto> SelectedComments = response.Content.ReadAsAsync<IEnumerable<CommentDto>>().Result;
                ViewModel.CoderComments = SelectedComments;
                

                url = "coderdata/getresourcesforcoder/" + id;
                response = client.GetAsync(url).Result;
                //Can catch the status code (200 OK, 301 REDIRECT), etc.
                //Debug.WriteLine(response.StatusCode);
                //Put data into Coder data transfer object
                IEnumerable<ResourceDto> SelectedResources = response.Content.ReadAsAsync<IEnumerable<ResourceDto>>().Result;
                ViewModel.CoderResources = SelectedResources;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Coder/Create
        public ActionResult Create()
        {
            UpdateCoder ViewModel = new UpdateCoder();
            //get information about resources this coder may have
            string url = "resourcedata/getresource";
            HttpResponseMessage response = client.GetAsync(url).Result;
            //IEnumerable<ResourceDto> PotentialResources = response.Content.ReadAsAsync < IEnumerable<ResourceDto>>().Result;
            
           //ViewModel.Allresources = PotentialResources;


            return View(ViewModel);
        }

        // POST: Coder/Create
        [HttpPost]
        [ValidateAntiForgeryToken()]
        
        public ActionResult Create(Coder CoderInfo) //coderinfo is a parameter. not part of DB
        {
            Debug.WriteLine(CoderInfo.UserName); //Coder first name or User name ?
            string url = "coderdata/addcoder";
            Debug.WriteLine(jss.Serialize(CoderInfo));
            HttpContent content = new StringContent(jss.Serialize(CoderInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {

                int Coderid = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id = Coderid });
            }
            else
            {
                return RedirectToAction("Error");
            }


        }

        [HttpGet]
        // GET: Coder/Edit/5
        public ActionResult Edit(int id)
        {
            UpdateCoder ViewModel = new UpdateCoder();

            string url = "coderdata/findcoder/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Coder data transfer object
                CoderDto SelectedCoder = response.Content.ReadAsAsync<CoderDto>().Result;
                ViewModel.Coder = SelectedCoder;

                //get inforamtion about resources this coder could use: 
                url = "resourcedata/getresources";
                response = client.GetAsync(url).Result;

                //able to edit the list of resources 
                IEnumerable<ResourceDto> PotentialResources = response.Content.ReadAsAsync<IEnumerable<ResourceDto>>().Result;
                ViewModel.Allresources = PotentialResources;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Coder/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(int id, Coder CoderBio, HttpPostedFileBase CoderPic)
        {
            //Debug.WriteLine(CoderBio.UserName);
            string url = "coderdata/updateCoder/" + id;
            //Debug.WriteLine(jss.Serialize(CoderBio));
            HttpContent content = new StringContent(jss.Serialize(CoderBio));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {

                if(CoderPic != null) { 

                Debug.WriteLine("Calling Update Image method.");
                //Send over image data for coder
                url = "coderdata/updatecoderpic/" + id;
                Debug.WriteLine("Received coder picture" + CoderPic.FileName);

                //provides a container for different multimedia file types
                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(CoderPic.InputStream);
                requestcontent.Add(imagecontent, "CoderPic", CoderPic.FileName);
                response = client.PostAsync(url, requestcontent).Result;
            }
                return RedirectToAction("Details", new {id = id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Coder/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "coderdata/findcoder/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Coder data transfer object
                CoderDto SelectedCoder = response.Content.ReadAsAsync<CoderDto>().Result;
                return View(SelectedCoder);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Coder/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(int id)
        {
            string url = "coderdata/deletecoder/" + id;
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
