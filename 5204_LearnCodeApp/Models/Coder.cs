using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations; //need for reference to primary and foreign keys 
using System.ComponentModel.DataAnnotations.Schema;

namespace _5204_LearnCodeApp.Models
{
    public class Coder
    {
        [Key]
        public int CoderID { get; set; } //primary key

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string ProfileImage { get; set; }
        public bool CoderHasPic { get; set; }

        //If the user has an image, record the extension of the image (.png, .gif, .jpg, etc.)
        public string PicExtension { get; set; }
        public string CoderEmail { get; set; }

        public string CoderURL { get; set; }
        public string CoderBio { get; set; }

        //Foreign keys in Entity Framework
        /// https://www.entityframeworktutorial.net/code-first/foreignkey-dataannotations-attribute-in-code-first.aspx

        //A Coder can make many Comments
        //This is a ONE to Many relationship: Coder FK in Comments TABLE
        public ICollection<Comment> Comments { get; set; }



        //Utilizes the inverse property to specify the "Many"
        //https://www.entityframeworktutorial.net/code-first/inverseproperty-dataannotations-attribute-in-code-first.aspx
        //InverseProperty attribute is used when two entities have more than one relationship.

        //One Coder has Many Resources
        //this refers to the many to many relationship 
        public ICollection<Resource> Resources { get; set; }
    }

    public class CoderDto
    {
        public int CoderID { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DisplayName("UserName")]
        public string UserName { get; set; }

        [DisplayName("User Photo")]
        public string ProfileImage { get; set; }

        [DisplayName("Email")]
        public string CoderEmail { get; set; }

        [DisplayName("Your Website")]
        public string CoderURL { get; set; }

        [DisplayName("Short Bio")]
        public string CoderBio { get; set; }

        public bool CoderHasPic { get; set; }
        public string PicExtension { get; set; }

        [DisplayName("Resource")]
        public int ResourceID { get; set; }
    }
}