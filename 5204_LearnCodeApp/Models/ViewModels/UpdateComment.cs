using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5204_LearnCodeApp.Models.ViewModels
{
    public class UpdateComment
    {
        //base information about the Comment
        public CommentDto Comment { get; set; }

        //display all coders that have this Comment 
        //public IEnumerable<CoderDto>  { get; set; }

        //display resources that can have Comment in a dropdownlist
        //public IEnumerable<ResourceDto> CommentResources { get; set; }
    }
}