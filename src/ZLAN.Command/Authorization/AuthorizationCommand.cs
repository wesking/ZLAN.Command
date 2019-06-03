using System;
using System.Collections.Generic;
using System.Text;
using ZLAN.Command.Abs;

namespace ZLAN.Command.Authorization
{
    public abstract class AuthorizationCommand<T> : Command<T>
    {
        protected string Constraint { get; set; }

        protected override IResult<T> PrepareExecute(IParameter commandParameter)
        {
            AuthorizationParameter parameter = commandParameter as AuthorizationParameter;

            if (parameter.GetConstraint() == null)
            {
                return ErrorResult<T>.NoAuthorization;
            }

            this.Constraint = parameter.GetConstraint();

            return base.PrepareExecute(commandParameter);
        }
        
    }
}
