using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel; //need to add this reference so that the primary and foreign keys work
using System.ComponentModel.DataAnnotations; //need to add this reference
using System.ComponentModel.DataAnnotations.Schema; //need to add this reference

namespace _5204_LearnCodeApp.Models
{

    //This class describes a Comment entity.
    //It is used for actually generating a database table
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }

        public string CommentContent { get; set; }

        

        //Foreign keys in Entity Framework
        /// https://www.entityframeworktutorial.net/code-first/foreignkey-dataannotations-attribute-in-code-first.aspx

        //A Comment is made for 1 resource 
        //Resource foreign key place in the Comments table
        [ForeignKey("Resource")]
        public int ResourceID { get; set; }
        public virtual Resource Resource{ get; set; }  //there will be an initial error here if the resource.cs file has not been set up

        //A comment is written by 1 coder
        //Resource foreign key place in the Coder table
        [ForeignKey("Coder")]
        public int CoderID { get; set; }
        public virtual Coder Coder { get; set; }  //there will be an initial error here if the resource.cs file has not been set up


    }

    //This class can be used to transfer information about a Comment.
    //also known as a "Data Transfer Object"
    //https://docs.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5
    //You can think of this class as the 'Model' that was used in 5101.
    //It is simply a vessel of communication

    public class CommentDto
    {
        public int CommentID { get; set; }

        [DisplayName("Comment")]
        public string CommentContent { get; set; }
        
        //this is in reference to the foreign key in the Comments table
        public int ResourceID { get; set; }
        public int CoderID { get; set; }
    }
}