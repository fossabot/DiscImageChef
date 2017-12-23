// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : Filters.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Filters.
//
// --[ Description ] ----------------------------------------------------------
//
//     Enumerates all filters and instantiates the correct one.
//
// --[ License ] --------------------------------------------------------------
//
//     This library is free software; you can redistribute it and/or modify
//     it under the terms of the GNU Lesser General Public License as
//     published by the Free Software Foundation; either version 2.1 of the
//     License, or (at your option) any later version.
//
//     This library is distributed in the hope that it will be useful, but
//     WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//     Lesser General Public License for more details.
//
//     You should have received a copy of the GNU Lesser General Public
//     License along with this library; if not, see <http://www.gnu.org/licenses/>.
//
// ----------------------------------------------------------------------------
// Copyright © 2011-2018 Natalia Portillo
// ****************************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using DiscImageChef.Console;

namespace DiscImageChef.Filters
{
    public class FiltersList
    {
        public SortedDictionary<string, Filter> Filters;

        /// <summary>
        /// Fills the list of all known filters
        /// </summary>
        public FiltersList()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(Filter));
            Filters = new SortedDictionary<string, Filter>();

            foreach(Type type in assembly.GetTypes())
                try
                {
                    if(!type.IsSubclassOf(typeof(Filter))) continue;

                    Filter filter = (Filter)type.GetConstructor(Type.EmptyTypes)?.Invoke(new object[] { });
                    if(filter != null && !Filters.ContainsKey(filter.Name.ToLower())) Filters.Add(filter.Name.ToLower(), filter);
                }
                catch(Exception exception) { DicConsole.ErrorWriteLine("Exception {0}", exception); }
        }

        /// <summary>
        /// Gets the filter that allows to read the specified path
        /// </summary>
        /// <param name="path">Path</param>
        /// <returns>The filter that allows reading the specified path</returns>
        public Filter GetFilter(string path)
        {
            Filter noFilter = null;

            foreach(Filter filter in Filters.Values)
                if(filter.UUID != new Guid("12345678-AAAA-BBBB-CCCC-123456789000"))
                {
                    if(!filter.Identify(path)) continue;

                    Filter foundFilter =
                        (Filter)filter.GetType().GetConstructor(Type.EmptyTypes)?.Invoke(new object[] { });

                    foundFilter?.Open(path);

                    if(foundFilter?.IsOpened() == true) return foundFilter;
                }
                else noFilter = filter;

            if(!noFilter?.Identify(path) == true) return noFilter;

            noFilter?.Open(path);

            return noFilter;
        }

        /// <summary>
        /// Gets all known filters
        /// </summary>
        /// <returns>Known filters</returns>
        public SortedDictionary<string, Filter> GetFiltersList()
        {
            return Filters;
        }
    }
}