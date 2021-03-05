using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace _5204_LearnCodeApp.Models.ViewModels
{
    public class ResourcesCoders
    {
        [Key]
        public int ResourceCodersID { get; set; }

        [ForeignKey("Resource")]
        public int ResourceID { get; set; }
        public virtual Resource Resource { get; set; }

        [ForeignKey("Coder")]
        public int CoderID { get; set; }
        public virtual Coder Coder { get; set; }

        //Extra field added to bridging table
        public bool Primary { get; set; }
    }
}