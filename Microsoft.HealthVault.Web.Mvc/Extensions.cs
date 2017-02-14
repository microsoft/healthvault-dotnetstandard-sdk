// ********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the Microsoft Public License.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
// ********************************************************

using System;
using System.Security.Principal;
using System.Web;

namespace Microsoft.HealthVault.Web.Mvc
{
    /// <summary>
    /// Useful extension methods for code readability and brevity.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns the <see cref="PersonInfo"/> object of the current HealthVault user.
        /// </summary>
        /// <param name="user">The current user</param>
        /// <returns>The <see cref="PersonInfo"/> object of the current user</returns>
        public static PersonInfo PersonInfo(this IPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            HealthVaultIdentity identity = user.Identity as HealthVaultIdentity;

            return identity?.PersonInfo;
        }

        /// <summary>
        /// Returns a <see cref="Mvc.PersonInfoManager"/> to handle PersonInfo 
        /// actions for the current request.
        /// </summary>
        /// <param name="context">The current HttpContext</param>
        /// <returns>A <see cref="Mvc.PersonInfoManager"/> object</returns>
        public static PersonInfoManager PersonInfoManager(this System.Web.HttpContext context)
        {
            return new HttpContextWrapper(context).PersonInfoManager();
        }

        /// <summary>
        /// Returns a <see cref="Mvc.PersonInfoManager"/> to handle PersonInfo 
        /// actions for the current request.
        /// </summary>
        /// <param name="context">The current HttpContext</param>
        /// <returns>A <see cref="Mvc.PersonInfoManager"/> object</returns>
        public static PersonInfoManager PersonInfoManager(this HttpContextBase context)
        {
            return new PersonInfoManager(context);
        }
    }
}
