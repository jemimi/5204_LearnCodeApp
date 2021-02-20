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
    public class CommentDataController : ApiController
    {

        //This variable is our database access point
        private LearnCodeAppDbContext db = new LearnCodeAppDbContext();

        //This code is mostly scaffolded from the base models and database context
        //New > WebAPIController with Entity Framework Read/Write Actions
        //Choose model "Comment"
        //Choose context "LearnApp Data Context"
        //Note: The base scaffolded code needs many improvements for a fully
        //functioning MVP.

        /// <summary>
        /// Gets a list or Comment in the database alongside a status code (200 OK).
        /// </summary>
        /// <returns>A list of Comment including their ID and content</returns>
        /// <example>
        /// GET: api/CommentData/GetComment
        /// </example>

        // GET: api/CommentContent/5
        [ResponseType(typeof(IEnumerable<CommentDto>))]
        public IHttpActionResult GetComments()
        {
            List<Comment> Comments = db.Comments.ToList();
            List<CommentDto> CommentDtos = new List<CommentDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Comment in Comments)
            {
                CommentDto NewComment = new CommentDto
                {
                    CommentID = Comment.CommentID,
                    CommentContent = Comment.CommentContent,
                    
                };
                CommentDtos.Add(NewComment);
            }

            return Ok(CommentDtos);
        }

        
        


        /// <summary>
        /// Finds a particular Comment in the database with a 200 status code. If the Comment is not found, return 404.
        /// </summary>
        /// <param name="id">The Comment id</param>
        /// <returns>Information about the Comment, including Comment ID, title, media type, Comment type, language  Commentid</returns>
        // <example>
        // GET: api/CommentData/FindComment/5
        // </example>
        [HttpGet]
        [ResponseType(typeof(CommentDto))]
        public IHttpActionResult FindComment(int id)
        {
            //Find the data
            Comment Comment = db.Comments.Find(id);
            //if not found, return 404 status code.
            if (Comment == null)
            {
                return NotFound();
            }

            //put into a 'friendly object format'
            CommentDto CommentDto = new CommentDto
            {
                CommentID = Comment.CommentID,
                CommentContent = Comment.CommentContent,
           
            };


            //pass along data as 200 status code OK response
            return Ok(CommentDto);
        }


        /// <summary>
        /// Updates a Comment in the database given information about the Comment.
        /// </summary>
        /// <param name="id">The Comment id</param>
        /// <param name="Comment">A Comment object. Received as POST data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/CommentData/UpdateComment/5
        /// FORM DATA: Comment JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateComment(int id, [FromBody] Comment Comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Comment.CommentID)
            {
                return BadRequest();
            }

            db.Entry(Comment).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
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
        /// Adds a Comment to the database.
        /// </summary>
        /// <param name="Comment">A Comment object. Sent as POST form data.</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/CommentData/AddComment
        ///  FORM DATA: Comment JSON Object
        /// </example>
        [ResponseType(typeof(Comment))]
        [HttpPost]
        public IHttpActionResult AddComment([FromBody] Comment Comment)
        {
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Comments.Add(Comment);
            db.SaveChanges();

            return Ok(Comment.CommentID);
        }


        /// <summary>
        /// Deletes a Comment in the database
        /// </summary>
        /// <param name="id">The id of the Comment to delete.</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/CommentData/DeleteComment/5
        /// </example>
        [HttpPost]
        public IHttpActionResult DeleteComment(int id)
        {
            Comment Comment = db.Comments.Find(id);
            if (Comment == null)
            {
                return NotFound();
            }

            db.Comments.Remove(Comment);
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
        /// Finds a Comment in the system. Internal use only.
        /// </summary>
        /// <param name="id">The Comment id</param>
        /// <returns>TRUE if the Comment exists, false otherwise.</returns>

        private bool CommentExists(int id)
        {
            return db.Comments.Count(e => e.CommentID == id) > 0;
        }
    }
}

// after setting up a WEBAPI controller: 
//****NEXT: go to App_Start folder > webapiconfig.cs file and add the action attribute to the route template
// routeTemplate: "api/{controller}/{action}/{id}",