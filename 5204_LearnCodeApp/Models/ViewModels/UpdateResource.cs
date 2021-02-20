using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5204_LearnCodeApp.Models.ViewModels
{
    public class UpdateResource
    {
        //base information about the Resource
        public ResourceDto Resource { get; set; }
        //display all coders that have this Resource 
        public IEnumerable<CoderDto> ResourceCoders { get; set; }
        //display coders which could be Resource in a dropdownlist
        public IEnumerable<CommentDto> ResourceComments { get; set; }
    }
}