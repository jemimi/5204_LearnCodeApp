using System;
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
    public class ResourceDataController : ApiController
    {

        //This variable is our database access point
        private LearnCodeAppDbContext db = new LearnCodeAppDbContext();

        //This code is mostly scaffolded from the base models and database context
        //New > WebAPIController with Entity Framework Read/Write Actions
        //Choose model "Resource"
        //Choose context "LearnApp Data Context"
        //Note: The base scaffolded code needs many improvements for a fully
        //functioning MVP.

        /// <summary>
        /// Gets a list or Resource in the database alongside a status code (200 OK).
        /// </summary>
        /// <returns>A list of Resource including their ID, name, and URL.</returns>
        /// <example>
        /// GET: api/ResourceData/GetResource
        /// </example>

        // GET: api/ResourceData/5
        [ResponseType(typeof(IEnumerable<Resource>))]
        public IHttpActionResult GetResources()
        {
            List<Resource> Resources = db.Resources.ToList();
            List<ResourceDto> ResourceDtos = new List<ResourceDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Resource in Resources)
            {
                ResourceDto NewResource = new ResourceDto
                {
                    ResourceID = Resource.ResourceID,
                    ResourceTitle = Resource.ResourceTitle,
                    MediaType = Resource.MediaType,
                    ResourceType = Resource.ResourceType,
                    ResourceLanguage = Resource.ResourceLanguage,
                    ResourceDescription = Resource.ResourceDescription,
                    ResourceImage = Resource.ResourceImage,
                    ResourceURL = Resource.ResourceURL
                };
                ResourceDtos.Add(NewResource);
            }

            return Ok(ResourceDtos);
        }

        /// <summary>
        /// Gets a list of Comments in the database alongside a status code (200 OK).
        /// </summary>
        /// <param name="id">The input Resourceid</param>
        /// <returns>A list of Comments associated with the Resource</returns>
        /// <example>
        /// GET: api/ResourceData/GetCommentsForResource
        /// </example>
        [ResponseType(typeof(IEnumerable<CommentDto>))]
        public IHttpActionResult GetCommentsForResource(int id)
        {
            //select * from comments where Comments.Resourceid=@id
            //finding Comments that match up 
            List<Comment> Comments = Comments = db
                .Comments
                .Where(p => p.ResourceID == id)
                .ToList();
            List<CommentDto> CommentDtos = new List<CommentDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Comment in Comments)
            {
                CommentDto NewComment = new CommentDto
                {
                    CommentID = Comment.CommentID,
                    CommentContent = Comment.CommentContent,
                    ResourceID = Comment.ResourceID  //THIS IS THE FOREIGN KEY IN THE COMMENTS TABLE
                };
                CommentDtos.Add(NewComment);
            }

            return Ok(CommentDtos);
        }

        /// <summary>
        /// Gets a list of Coders in the database alongside a status code (200 OK).
        /// </summary>
        /// <param name="id">The input Resourceid</param>
        /// <returns>A list of Coders including their ID, name, and URL.</returns>
        /// <example>
        /// GET: api/CoderData/GetCodersForResource
        /// </example>
        [ResponseType(typeof(IEnumerable<CoderDto>))]
        public IHttpActionResult GetCodersForResource(int id)
        {
            //MANY TO MANY RELATIONSHIP: finding the records that are related and showing in the result set
            //select * from Coders
            //inner join Coder Resources on CoderResources.Coderid = Coders.Coderid
            //inner join Resources on CoderResources.Resourceid = Resources.Resourceid
            //WHERE Resources.Resourceid=@id
            //go to CoderDatacontroller.cs to see the reverse 
            //can create another bridging table MODEL 

            List<Coder> Coders = db.Coders
                //returns true or false and ends up in a result set
                //lambda expression
                //if the ResourceID matches the int id
                .Where(s => s.Resources.Any(t => t.ResourceID == id))
                .ToList();

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
                    ProfileImage = Coder.ProfileImage,
                    CoderEmail = Coder.CoderEmail,
                    CoderURL = Coder.CoderURL,
                    CoderBio = Coder.CoderBio
                };
                CoderDtos.Add(NewCoder);
            }

            return Ok(CoderDtos);
        }


        /// <summary>
        /// Finds a particular Resource in the database with a 200 status code. If the Resource is not found, return 404.
        /// </summary>
        /// <param name="id">The Resource id</param>
        /// <returns>Information about the Resource, including Resource ID, title, media type, resource type, language  Resourceid</returns>
        // <example>
        // GET: api/ResourceData/FindResource/5
        // </example>
        [HttpGet]
        [ResponseType(typeof(ResourceDto))]
        public IHttpActionResult FindResource(int id)
        {
            //Find the data
            Resource Resource = db.Resources.Find(id);
            //if not found, return 404 status code.
            if (Resource == null)
            {
                return NotFound();
            }

            //put into a 'friendly object format'
            ResourceDto ResourceDto = new ResourceDto
            {
                ResourceID = Resource.ResourceID,
                ResourceTitle = Resource.ResourceTitle,
                MediaType = Resource.MediaType,
                ResourceType = Resource.ResourceType,
                ResourceLanguage = Resource.ResourceLanguage,
                ResourceDescription = Resource.ResourceDescription,
                ResourceImage = Resource.ResourceImage,
                ResourceURL = Resource.ResourceURL
            };


            //pass along data as 200 status code OK response
            return Ok(ResourceDto);
        }


        /// <summary>
        /// Updates a Resource in the database given information about the Resource.
        /// </summary>
        /// <param name="id">The Resource id</param>
        /// <param name="Resource">A Resource object. Received as POST data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/ResourceData/UpdateResource/5
        /// FORM DATA: Resource JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateResource(int id, [FromBody] Resource Resource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Resource.ResourceID)
            {
                return BadRequest();
            }

            db.Entry(Resource).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResourceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Adds a Resource to the database.
        /// </summary>
        /// <param name="Resource">A Resource object. Sent as POST form data.</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/ResourceData/AddResource
        ///  FORM DATA: Resource JSON Object
        /// </example>
        [ResponseType(typeof(Resource))]
        [HttpPost]
        public IHttpActionResult AddResource([FromBody] Resource Resource)
        {
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Resources.Add(Resource);
            db.SaveChanges();

            return Ok(Resource.ResourceID);
        }


        /// <summary>
        /// Deletes a Resource in the database
        /// </summary>
        /// <param name="id">The id of the Resource to delete.</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/ResourceData/DeleteResource/5
        /// </example>
        [HttpPost]
        public IHttpActionResult DeleteResource(int id)
        {
            Resource Resource = db.Resources.Find(id);
            if (Resource == null)
            {
                return NotFound();
            }

            db.Resources.Remove(Resource);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        /// <summary>
        /// Finds a Resource in the system. Internal use only.
        /// </summary>
        /// <param name="id">The Resource id</param>
        /// <returns>TRUE if the Resource exists, false otherwise.</returns>

        private bool ResourceExists(int id)
        {
            return db.Resources.Count(e => e.ResourceID == id) > 0;
        }
    }
}

// after setting up a WEBAPI controller: 
//****NEXT: go to App_Start folder > webapiconfig.cs file and add the action attribute to the route template
// routeTemplate: "api/{controller}/{action}/{id}",