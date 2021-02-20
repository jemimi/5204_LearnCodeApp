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
            client.BaseAddress = new Uri("https://localhost:44366/api/"); //hanged the port number to my pc but doesn't work 
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));


            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ACCESS_TOKEN);

        }



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
            string url = "resourcedate/getresource";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<ResourceDto> PotentialResources = response.Content.ReadAsAsync < IEnumerable<ResourceDto>>().Result;
            ViewModel.Allresources = PotentialResources;


            return View(ViewModel);
        }

        // POST: Coder/Create
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Coder CoderInfo)
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
        public ActionResult Edit(int id, Coder CoderInfo, HttpPostedFileBase CoderPic)
        {
            Debug.WriteLine(CoderInfo.UserName);
            string url = "Coderdata/updateCoder/" + id;
            Debug.WriteLine(jss.Serialize(CoderInfo));
            HttpContent content = new StringContent(jss.Serialize(CoderInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Send over image data for player
                url = "coderdata/updatecoderpic/" + id;
                Debug.WriteLine("Received coder picture" +CoderPic.FileName);

                //provides a container for different multimedia file types
                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(CoderPic.InputStream);
                requestcontent.Add(imagecontent, "CoderPic", CoderPic.FileName);
                response = client.PostAsync(url, requestcontent).Result;

                return RedirectToAction("Details", new { id = id });
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
