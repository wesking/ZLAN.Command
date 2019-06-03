using System;
using System.Collections.Generic;
using System.Text;
using ZLAN.Command.Abs;

namespace ZLAN.Command.Authorization
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
