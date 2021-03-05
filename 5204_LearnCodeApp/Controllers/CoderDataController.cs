using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using _5204_LearnCodeApp.Models;
using System.Diagnostics;

namespace _5204_LearnCodeApp.Controllers
{
    public class CoderDataController : ApiController
    {
        private LearnCodeAppDbContext db = new LearnCodeAppDbContext();


        /// <summary>
        /// Gets a list or Coders in the database alongside a status code (200 OK).
        /// </summary>
        /// <returns>A list of Coders including their ID, name, and URL.</returns>
        /// <example>
        /// GET: api/CoderData/GetCoders
        /// </example>
        [ResponseType(typeof(IEnumerable<CoderDto>))]
        public IHttpActionResult GetCoders()
        {
            List<Coder> Coders = db.Coders.ToList();
            List<CoderDto> CoderDtos = new List<CoderDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Coder in Coders)
            {
                CoderDto NewCoder = new CoderDto
                {
                    CoderID = Coder.CoderID,
                    FirstName = Coder.FirstName,
                    LastName = Coder.LastName,
                    UserName = Coder.UserName,
                    CoderBio = Coder.CoderBio,
                    CoderHasPic = Coder.CoderHasPic,
                    ProfileImage = Coder.ProfileImage //add new column to database table : coder
                    
                };
                CoderDtos.Add(NewCoder);
            }

            return Ok(CoderDtos);
        }

        /// <summary>
        /// Gets a list of Comments in the database alongside a status code (200 OK).
        /// </summary>
        /// <param name="id">The input Coderid</param>
        /// <returns>A list of Comments associated with the Coder</returns>
        /// <example>
        /// GET: api/CoderData/GetCommentsForCoder
        /// </example>
        [ResponseType(typeof(IEnumerable<CommentDto>))]
        public IHttpActionResult GetCommentsForCoder(int id)
        {
            //select * from opalyers where Comments.Coderid=@id
            //finding Comments that match up 
            List<Comment> Comments = Comments = db
                .Comments
                .Where(p => p.CoderID == id)
                .ToList();
            List<CommentDto> CommentDtos = new List<CommentDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Comment in Comments)
            {
                CommentDto NewComment = new CommentDto
                {
                    CommentID = Comment.CommentID,
                    CommentContent = Comment.CommentContent,
                    CoderID = Comment.CoderID  //refers to WHO wrote the comment Coder FK
                };
                CommentDtos.Add(NewComment);
            }

            return Ok(CommentDtos);
        }

        /// <summary>
        /// Gets a list or Resources in the database alongside a status code (200 OK).
        /// </summary>
        /// <param name="id">The input Coderid</param>
        /// <returns>A list of Resources including their ID, name, and URL.</returns>
        /// <example>
        /// GET: api/ResourceData/GetResourcesForCoder
        /// </example>
        [ResponseType(typeof(IEnumerable<ResourceDto>))]
        public IHttpActionResult GetResourcesForCoder(int id)
        {
            //MANY TO MANY RELATIONSHIP: finding the records that are related and showing in the result set
            //select * from Resources
            //inner join sponspor Coders on ResourceCoders.Resourceid = Resources.Resourceid
            //inner join Coders on ResourceCoders.Coderid = Coders.Coderid
            //WHERE Coders.Coderid=@id
            //go to ResourceDatacontroller.cs to see the reverse 
            //can create another bridging table MODEL 

            List<Resource> Resources = db.Resources
                //returns true or false and ends up in a result set
                //lambda expression
                //if the CoderID matches the int id
                .Where(s => s.Coders.Any(t => t.CoderID == id))
                .ToList();
            List<ResourceDto> ResourceDtos = new List<ResourceDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Resource in Resources)
            {
                ResourceDto NewResource = new ResourceDto
                {
                    ResourceID = Resource.ResourceID,
                    ResourceTitle = Resource.ResourceTitle,
                    ResourceURL = Resource.ResourceURL
                };
                ResourceDtos.Add(NewResource);
            }

            return Ok(ResourceDtos);
        }

        /// <summary>
        /// Finds a particular Coder in the database with a 200 status code. If the Coder is not found, return 404.
        /// </summary>
        /// <param name="id">The Coder id</param>
        /// <returns>Information about the Coder, including Coder id, bio, first and last name, and Coderid</returns>
        // <example>
        // GET: api/CoderData/FindCoder/5
        // </example>
        [HttpGet]
        [ResponseType(typeof(CoderDto))]
        public IHttpActionResult FindCoder(int id)
        {
            //Find the data
            Coder Coder = db.Coders.Find(id);
            //if not found, return 404 status code.
            if (Coder == null)
            {
                return NotFound();
            }

            //put into a 'friendly object format'
            CoderDto CoderDto = new CoderDto
            {
                CoderID = Coder.CoderID,
                FirstName = Coder.FirstName,
                LastName = Coder.LastName,
                UserName = Coder.UserName,
                CoderBio = Coder.CoderBio,
                CoderHasPic = Coder.CoderHasPic,
                ProfileImage = Coder.ProfileImage,
                //ResourceID = Coder.ResourceID
            };

            //pass along data as 200 status code OK response
            return Ok(CoderDto);
        }

        /// <summary>
        /// Updates a Coder in the database given information about the Coder.
        /// </summary>
        /// <param name="id">The Coder id</param>
        /// <param name="Coder">A Coder object. Received as POST data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/CoderData/UpdateCoder/5
        /// FORM DATA: Coder JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateCoder(int id, [FromBody] Coder Coder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Coder.CoderID)
            {
                return BadRequest();
            }

            db.Entry(Coder).State = EntityState.Modified;
            // profile photo update is handled by another method'
            db.Entry(Coder).Property(p => p.CoderHasPic).IsModified = false;
            db.Entry(Coder).Property(p => p.PicExtension).IsModified = false;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CoderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;  //throws an exception
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        /// <summary>
        /// Receives Coder picture data, uploads it to the webserver and updates the Coder's HasPic option
        /// </summary>
        /// <param name="id">the Coder id</param>
        /// <returns>status code 200 if successful.</returns>
        /// <example> **********************************
        /// curl -F Coderpic=@file.jpg "https://localhost:xx/api/Coderdata/updateCoderpic/2"
        /// POST: api/CoderData/UpdateCoderPic/3
        /// HEADER: enctype=multipart/form-data
        /// FORM-DATA: image
        /// </example>
        /// https://stackoverflow.com/questions/28369529/how-to-set-up-a-web-api-controller-for-multipart-form-data

        [HttpPost]
        public IHttpActionResult UpdateProfileImage(int id) //only update the IMAGE of the Coder
                                                         //separate the parts of the Coder so to debug more effectively
        {

            bool haspic = false;  //default is no profile photo 
            string picextension;
            if (Request.Content.IsMimeMultipartContent())
            {
                Debug.WriteLine("Received multipart form data."); //confirm that you received the info

                int numfiles = HttpContext.Current.Request.Files.Count; //provide info on the  files 
                Debug.WriteLine("Files Received: " + numfiles);

                //Check if a file is posted
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var ProfileImage = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (ProfileImage.ContentLength > 0) //0 bytes
                    {
                        var valtypes = new[] { "jpeg", "jpg", "png", "gif" };  //valid types of image files 
                        var extension = Path.GetExtension(ProfileImage.FileName).Substring(1);
                        //Check the extension of the file
                        if (valtypes.Contains(extension)) //if file extension is valid
                        {
                            try
                            {
                                //file name is the id of the image
                                string fn = id + "." + extension;  //ex: 3.png

                                //get a direct file path to ~/Content/Coders/{id}.{extension}
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Coders/"), fn);

                                //save the file
                                ProfileImage.SaveAs(path);

                                //if these are all successful then we can set these fields
                                haspic = true;
                                picextension = extension;

                                //Update the Coder haspic and picextension fields in the database
                                Coder SelectedCoder = db.Coders.Find(id);
                                SelectedCoder.CoderHasPic = haspic;  //is there a picture for this Coder? boolean
                                SelectedCoder.ProfileImage = extension; //if there is pic, what is the extension? 
                                db.Entry(SelectedCoder).State = EntityState.Modified;

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Coder Image was not saved successfully.");
                                Debug.WriteLine("Exception:" + ex);
                            }
                        }
                    }

                }
            }

            return Ok();
        }



        /// <summary>
        /// Adds a Coder to the database.
        /// </summary>
        /// <param name="Coder">A Coder object. Sent as POST form data.</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/CoderData/AddCoder
        ///  FORM DATA: Coder JSON Object
        /// </example>
        [ResponseType(typeof(CoderDto))]
        [HttpPost]
        public IHttpActionResult AddCoder([FromBody] Coder Coder)
        {
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Coders.Add(Coder);
            db.SaveChanges();

            return Ok(Coder.CoderID);
        }

        /// <summary>
        /// Deletes a Coder in the database
        /// </summary>
        /// <param name="id">The id of the Coder to delete.</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/CoderData/DeleteCoder/5
        /// </example>
        [HttpPost]
        public IHttpActionResult DeleteCoder(int id)
        {
            Coder Coder = db.Coders.Find(id);
            if (Coder == null)
            {
                return NotFound();
            }
            if (Coder.CoderHasPic && Coder.PicExtension != "")
            {
                //AND DELETE IMAGE from path
                string path = HttpContext.Current.Server.MapPath("~/Content/Coders/" + id + "." + Coder.PicExtension);
                Debug.WriteLine(Coder.PicExtension);

                if (System.IO.File.Exists(path))
                {
                    Debug.WriteLine("File exists.. preparing to delete!");
                    System.IO.File.Delete(path);
                }
            }
            
            db.Coders.Remove(Coder);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing) //releases managed and unmanaged resources
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Finds a Coder in the system. Internal use only.
        /// </summary>
        /// <param name="id">The Coder id</param>
        /// <returns>TRUE if the Coder exists, false otherwise.</returns>
        private bool CoderExists(int id)
        {
            return db.Coders.Count(e => e.CoderID == id) > 0;
        }
    }
}