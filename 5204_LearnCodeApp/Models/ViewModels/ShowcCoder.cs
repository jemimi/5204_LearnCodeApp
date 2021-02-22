using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5204_LearnCodeApp.Models.ViewModels
{
    public class ShowCoder
    {
        //Information about the Resource

        public CoderDto Coder { get; set; }

        //Information about all comments made by a coder
        public IEnumerable<CommentDto> CoderComments{ get; set; } //see CoderController.cs

        //Showing single resource
        public ResourceDto Resource { get; set; }

        //Information about all resources by a coder
        public IEnumerable<ResourceDto> CoderResources { get; set; }  //see CoderController.cs
    }
}