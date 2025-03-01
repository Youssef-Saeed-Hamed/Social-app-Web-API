using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Data_Transfer_Object
{
    public class CommentsDto
    {
       

        public string CommentId { get; set; }
        public string CommentText { get; set; }
        public UserPostDto User { get; set; }
        public int RepliesSize { get; set; }
        public int NumberOfLikes { get; set; }
        public bool IsLiked { get; set; }
        public DateTime DateTime { get; set; }
        //public IEnumerable<CommentsDto>? Replies { get; set; }


    
    }
}
