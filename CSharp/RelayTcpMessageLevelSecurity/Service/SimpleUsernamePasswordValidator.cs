//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace Microsoft.ServiceBus.Samples
{
    using System;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;

    public class SimpleUsernamePasswordValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            if (null == userName || null == password)
            {
                throw new ArgumentNullException();
            }

            if (!(userName == "test1" && password == "1tset") && 
                !(userName == "test2" && password == "2tset"))
            {
                throw new SecurityTokenException("Incorrect username, password, or both.");
            }
        }
    }
}
