using Core_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository_Layer.Specifications
{
    public class CommentSpecification : BaseSpecification<Comment>
    {
        public CommentSpecification(string Id) : base(post => post.Id == Id)
        {
            Includes.Add(comment => comment.User);
        }

        public CommentSpecification(CommentSpecificationParameter parameter) :
            base(comment => (string.IsNullOrWhiteSpace(parameter.PostId)) || ((comment.PostId == parameter.PostId) && (comment.commentParentId == parameter.CommentId)))
            
        {
            Includes.Add(comment => comment.User);
            
            Includes.Add(comment => comment.Recives);
        }


    }
}
