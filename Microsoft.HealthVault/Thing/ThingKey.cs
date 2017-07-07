// Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT License
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;

namespace Microsoft.HealthVault.Thing
{
    /// <summary>
    /// Uniquely identifies a thing in the system.
    /// </summary>
    ///
    public class ThingKey
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ThingBase"/>
        /// class with the specified globally unique ID for the
        /// <see cref="ThingBase"/> and globally unique version stamp.
        /// </summary>
        ///
        /// <param name="id">
        /// A globally unique identifier for the <see cref="ThingBase"/>
        /// in the system.
        /// </param>
        ///
        /// <param name="versionStamp">
        /// A globally unique identifier for the version of the <see cref="ArgumentException"/>
        /// in the system.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// The <paramref name="versionStamp"/> or <paramref name="versionStamp"/>
        /// parameter is Guid.Empty.
        /// </exception>
        ///
        public ThingKey(Guid id, Guid versionStamp)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException(Resources.ThingIdInvalid, nameof(id));
            }

            if (versionStamp == Guid.Empty)
            {
                throw new ArgumentException(Resources.ThingVersionInvalid, nameof(versionStamp));
            }

            _thingId = id;
            _versionStamp = versionStamp;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ThingBase"/>
        /// class with the specified globally unique ID for the
        /// <see cref="ThingBase"/>.
        /// </summary>
        ///
        /// <param name="id">
        /// A globally unique identifier for the <see cref="ArgumentException"/>
        /// in the system.
        /// </param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="id"/> is Guid.Empty.
        /// </exception>
        ///
        public ThingKey(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException(Resources.ThingIdInvalid, nameof(id));
            }

            _thingId = id;
        }

        /// <summary>
        /// Gets the unique identifier of the <see cref="ThingBase"/>.
        /// </summary>
        ///
        /// <value>
        /// A globally unique identifier for the <see cref="ThingBase"/>,
        /// issued when the item is created.
        /// </value>
        ///
        public Guid Id
        {
            get { return _thingId; }
            set { _thingId = value; }
        }

        private Guid _thingId;

        /// <summary>
        /// Gets the unique version stamp of the <see cref="ThingBase"/>.
        /// </summary>
        ///
        /// <value>
        /// A globally unique identifier that represents the version of the
        /// <see cref="ThingBase"/>. A new version stamp is issued each
        /// time the item is changed.
        /// </value>
        ///
        /// <remarks>
        /// The version stamp of the current version of a <see cref="ThingBase"/>
        /// is always equal to the <see cref="Id"/> of that item.
        /// </remarks>
        ///
        public Guid VersionStamp
        {
            get { return _versionStamp; }
            set
            {
                if (value != Guid.Empty)
                {
                    _versionStamp = value;
                }
            }
        }

        private Guid _versionStamp;

        /// <summary>
        /// Gets a string representation of the key.
        /// </summary>
        ///
        /// <returns>
        /// <see cref="VersionStamp"/>.ToString().
        /// </returns>
        ///
        public override string ToString()
        {
            if (_versionStamp != Guid.Empty)
            {
                return _thingId + "," + _versionStamp;
            }

            return _thingId.ToString();
        }

        /// <summary>
        /// Compares one <see cref="ThingKey"/> to another.
        /// </summary>
        ///
        /// <param name="obj">
        /// The <see cref="ThingKey"/> to compare against this.
        /// </param>
        ///
        /// <returns>
        /// <b>true</b> if both the thing keys have
        /// the same ID and version stamp; otherwise, <b>false</b>.
        /// </returns>
        ///
        public override bool Equals(object obj)
        {
            bool result = false;
            ThingKey rVal = obj as ThingKey;

            if (rVal != null)
            {
                result = (_versionStamp == rVal.VersionStamp)
                       && (_thingId == rVal.Id);
            }

            return result;
        }

        /// <summary>
        /// Gets the hashcode value for the object.
        /// </summary>
        ///
        /// <returns>
        /// <see cref="VersionStamp"/>.GetHashCode().
        /// </returns>
        ///
        public override int GetHashCode()
        {
            return _versionStamp.GetHashCode();
        }
    }
}
