using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _5204_LearnCodeApp.Models.ViewModels
{
    //The View Model required to update a Coder 
    public class UpdateCoder
    {

        //Information about the player
        public CoderDto Coder { get; set; }
        //Needed for a dropdownlist which presents the player with a choice of teams to play for
        public IEnumerable<ResourceDto> Allresources { get; set; }
        public IEnumerable<CommentDto> Allcomments { get; set; }
    }
}