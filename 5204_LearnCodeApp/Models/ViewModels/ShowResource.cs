using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5204_LearnCodeApp.Models.ViewModels
{
    //model called show Resource:
    public class ShowResource
    {
        //Information about the Resource

        public ResourceDto Resource { get; set; }

        //Information about all comments on a resource
        public IEnumerable<CommentDto> ResourceComments { get; set; }

        //Information about all coders for that resource
        public IEnumerable<CoderDto> ResourceCoders { get; set; }
    }
}

//================NEXT =====================
// Set up in the VIEW folder: 
// (1) Resource Folder
// (2) View files for Resource to display data from the viewmodel 
// Remember! separation of concerns !!~~