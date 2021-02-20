using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5204_LearnCodeApp.Models.ViewModels
{
    //model called show Comment:
    public class ShowComment
    {
        //Information about the Comment

        public CommentDto Comment { get; set; }

        //Information about resources with a comment
        public IEnumerable<ResourceDto> CommentResources { get; set; }

        //Information about all coders that have a comment
        public IEnumerable<CoderDto> CommentCoders { get; set; }
    }
}

//================NEXT =====================
// Set up in the VIEW folder: 
// (1) Comment Folder
// (2) View files for Comment to display data from the viewmodel 
// Remember! separation of concerns !!~~