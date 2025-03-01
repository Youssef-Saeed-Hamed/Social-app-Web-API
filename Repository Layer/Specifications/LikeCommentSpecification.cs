using Core_Layer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository_Layer.Specifications
{
    public class LikeCommentSpecification : BaseSpecification<LikeComment>
    {
        public LikeCommentSpecification() : base(x => x.Id == x.Id)
        {
            Includes.Add(x => x.User);
        }
    }
}
