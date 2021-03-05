using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel; //need for reference to primary and foreign keys 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _5204_LearnCodeApp.Models
{
    public class Resource  //coding resource
    {

        public int ResourceID { get; set; }

        public string ResourceTitle { get; set; }

        public string MediaType { get; set; } //type of media: video, pdf, book etc
        public string ResourceType { get; set; } //project, tutorial, code snippet
        public string ResourceLanguage { get; set; }  //javascript, c#, HTML
        public string ResourceDescription { get; set; }  
        public string ResourceURL { get; set; }
        public string ResourceImage { get; set; }

        //This is where the database relationships are set!!!~~~~

        //A Resource can have many Comments
        //This is a ONE to Many relationship: Resource FK in Comments TABLE
        public ICollection<Comment> Comments { get; set; }

        //A Resource can have many Coders
        //this is in reference to the Many to Many relationship that requires a briding table
        //many to many relationship Icollection for BOTH sides of the relationship. 
        public ICollection<Coder> Coders{ get; set; }
    }

    public class ResourceDto //information that will be make public 
    {
        public int ResourceID { get; set; }

        [DisplayName("Resource")]
        public string ResourceTitle { get; set; }

        [DisplayName("Format")]
        public string MediaType { get; set; }

        [DisplayName("Type")]
        public string ResourceType { get; set; }

        [DisplayName("Language")]
        public string ResourceLanguage { get; set; }

        [DisplayName("Description")]
        public string ResourceDescription { get; set; }

        [DisplayName("Image")]
        public string ResourceImage { get; set; }

        [DisplayName("URL")]
        public string ResourceURL { get; set; }
    }
}