using System;
using System.Collections.Generic;
using System.Text;
using ZL.CommandCore.Abs;

namespace ZL.CommandCore.Authorization
{
    public class AuthorizationParameter : IParameter
    {
        public virtual void Initialize(IServiceProvider provider, string constraint, string[] authorizationCode)
        {
            Constraint = constraint;
        }
        public virtual void SetConstraint(string constraint)
        {
            Constraint = constraint;
        }

        public virtual string GetConstraint()
        {
            return Constraint;
        }
        

        protected string Constraint { get; set; }
    }
}
