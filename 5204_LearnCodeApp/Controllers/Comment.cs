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
    public class CommentController : Controller
    {

        //After configuring a WebAPI controller, 
        //configure a controller to access the WebAPI and serve Views
        //Set up: Add Controller> MVC controller with read/write actions (not entity)


        //Http Client is the proper way to connect to a webapi
        //https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0

        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;


        static CommentController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false
            };

            //getting the information from a database - 
            client = new HttpClient(handler);
            //change this to match your own local port number
            //https://docs.microsoft.com/en-us/dotnet/api/system.uri?view=net-5.0
            //BUG *******************************
            client.BaseAddress = new Uri("https://localhost:44366/api/");
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));


            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ACCESS_TOKEN);

        }

        // GET: Comment/List
        public ActionResult List()
        {
            string url = "commentdata/getcomments";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<CommentDto> SelectedComments = response.Content.ReadAsAsync<IEnumerable<CommentDto>>().Result;
                return View(SelectedComments);
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        // GET: Comment/Details/5
        public ActionResult Details(int id)
        {
            ShowComment ViewModel = new ShowComment();  
            string url = "Commentdata/findComment/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Comment data transfer object
                CommentDto SelectedComment = response.Content.ReadAsAsync<CommentDto>().Result;
                ViewModel.Comment = SelectedComment;

                //We don't need to throw any errors if this is null
                //A Comment not having any Comments is not an issue.
                url = "resourcedata/getcommentsforresource/" + id;
                response = client.GetAsync(url).Result;
                //Can catch the status code (200 OK, 301 REDIRECT), etc.
                //Debug.WriteLine(response.StatusCode);
                IEnumerable<ResourceDto> SelectedComments = response.Content.ReadAsAsync<IEnumerable<ResourceDto>>().Result;
                ViewModel.CommentResources = SelectedComments;


                url = "Commentdata/getcommentsforcoder/" + id;
                response = client.GetAsync(url).Result;
                //Can catch the status code (200 OK, 301 REDIRECT), etc.
                //Debug.WriteLine(response.StatusCode);
                //Put data into Comment data transfer object
                IEnumerable<CoderDto> SelectedCoders = response.Content.ReadAsAsync<IEnumerable<CoderDto>>().Result;
                ViewModel.CommentCoders = SelectedCoders;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Comment/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Comment/Create
        [HttpPost]

        //ValidateAntiForgeryToken: prevent cross-site request forgery attacks 
        // a harmful script element, malicious command, or code is sent 
        //from the browser of a trusted user
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Comment CommentInfo)
        {
            Debug.WriteLine(CommentInfo.CommentContent);
            string url = "Commentdata/addComment";
            Debug.WriteLine(jss.Serialize(CommentInfo));
            HttpContent content = new StringContent(jss.Serialize(CommentInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {

                int Commentid = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id = Commentid });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Comment/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "Commentdata/findComment/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Comment data transfer object
                CommentDto SelectedComment = response.Content.ReadAsAsync<CommentDto>().Result;
                return View(SelectedComment);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Comment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(int id, Comment CommentInfo)
        {
            Debug.WriteLine(CommentInfo.CommentContent);
            string url = "Commentdata/updateComment/" + id;
            Debug.WriteLine(jss.Serialize(CommentInfo));
            HttpContent content = new StringContent(jss.Serialize(CommentInfo));
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

        // GET: Comment/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "Commentdata/findComment/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Comment data transfer object
                CommentDto SelectedComment = response.Content.ReadAsAsync<CommentDto>().Result;
                return View(SelectedComment);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }



        // POST: Comment/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(int id)
        {
            string url = "Commentdata/deleteComment/" + id;
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
// (2) add a viewmodel class to display Comment information with coder and comment info too