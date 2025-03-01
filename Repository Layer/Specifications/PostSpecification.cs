using Core_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository_Layer.Specifications
{
    public class PostSpecification : BaseSpecification<Post>
    {
        public PostSpecification(string Id) : base(post => post.Id == Id)
        {
            Includes.Add(post => post.User);
            Includes.Add(post => post.Comments);

        }

        public PostSpecification(PostSpecificationParameters specs) :
            base( post => (string.IsNullOrWhiteSpace(specs.UserId)) || (post.UserId == specs.UserId)
            )
        {
            Includes.Add(post => post.User);
            Includes.Add(post => post.Comments);
        }
    }
}
