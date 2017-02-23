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
using System.Runtime.Serialization;
using System.Security;
using System.Security.Principal;
using Microsoft.HealthVault.Person;

namespace Microsoft.HealthVault.Web.Mvc
{
    /// <summary>
    /// An IIdentity object representing a <see cref="PersonInfo"/> object.
    /// </summary>
    [Serializable]
    public class HealthVaultIdentity : IIdentity, ISerializable
    {
        /// <summary>
        /// Creates a new <see cref="HealthVaultIdentity"/> object for the given <see cref="PersonInfo"/>
        /// </summary>
        /// <param name="personInfo">The <see cref="PersonInfo"/> object to represent</param>
        public HealthVaultIdentity(PersonInfo personInfo)
        {
            PersonInfo = personInfo;
        }

        /// <summary>
        /// Development only. This constructor is used by the Visual Studio development server.
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected HealthVaultIdentity(SerializationInfo info, StreamingContext context)
        {
        }

        /// <summary>
        /// Gets the <see cref="PersonInfo"/> object represented by this <see cref="HealthVaultIdentity"/>
        /// </summary>
        public PersonInfo PersonInfo { get; internal set; }

        /// <summary>
        /// Gets the type of authentication
        /// </summary>
        public string AuthenticationType => "HealthVault";

        /// <summary>
        /// Gets whether the user is authenticated. This is true if a <see cref="PersonInfo"/> exists.
        /// </summary>
        public bool IsAuthenticated => PersonInfo != null;

        /// <summary>
        /// Gets the name of the current user, as specificed in the <see cref="PersonInfo"/> object.
        /// </summary>
        public string Name => PersonInfo != null ? PersonInfo.Name : string.Empty;

        /// <summary>
        /// Development only. This method is used by the Visual Studio development server.
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        [SecurityCritical]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (context.State == StreamingContextStates.CrossAppDomain)
            {
                info.SetType(typeof(GenericIdentity));
                info.AddValue("m_name", Name);
                info.AddValue("m_type", AuthenticationType);
            }
        }
    }
}
