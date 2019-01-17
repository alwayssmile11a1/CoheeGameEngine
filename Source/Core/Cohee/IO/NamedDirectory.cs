﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cohee.IO
{
    /// <summary>
	/// Enumerates special directories on the current system.
	/// </summary>
	public enum NamedDirectory
    {
        /// <summary>
        /// The directory in which the running Duality application is located.
        /// </summary>
        Current,
        /// <summary>
        /// The directory where applications store their data.
        /// </summary>
        ApplicationData,
        /// <summary>
        /// The current users "My Documents" folder.
        /// </summary>
        MyDocuments,
        /// <summary>
        /// The current users "My Pictures" folder.
        /// </summary>
        MyPictures,
        /// <summary>
        /// The current users "My Music" folder.
        /// </summary>
        MyMusic
    }
}
