﻿// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : Identify.cs
// Author(s)      : Natalia Portillo <claunia@claunia.com>
//
// Component      : Disk image plugins.
//
// --[ Description ] ----------------------------------------------------------
//
//     Identifies XGS emulator disk images.
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
using System.IO;
using DiscImageChef.CommonTypes.Interfaces;

namespace DiscImageChef.DiscImages
{
    public partial class Apple2Mg
    {
        public bool Identify(IFilter imageFilter)
        {
            Stream stream = imageFilter.GetDataForkStream();
            stream.Seek(0, SeekOrigin.Begin);

            if(stream.Length < 65) return false;

            byte[] header = new byte[64];
            stream.Read(header, 0, 64);

            uint magic = BitConverter.ToUInt32(header, 0x00);
            if(magic != MAGIC) return false;

            uint dataoff = BitConverter.ToUInt32(header, 0x18);
            if(dataoff > stream.Length) return false;

            uint datasize = BitConverter.ToUInt32(header, 0x1C);
            // There seems to be incorrect endian in some images on the wild
            if(datasize           == 0x00800C00) datasize = 0x000C8000;
            if(dataoff + datasize > stream.Length) return false;

            uint commentoff = BitConverter.ToUInt32(header, 0x20);
            if(commentoff > stream.Length) return false;

            uint commentsize = BitConverter.ToUInt32(header, 0x24);
            if(commentoff + commentsize > stream.Length) return false;

            uint creatoroff = BitConverter.ToUInt32(header, 0x28);
            if(creatoroff > stream.Length) return false;

            uint creatorsize = BitConverter.ToUInt32(header, 0x2C);
            return creatoroff + creatorsize <= stream.Length;
        }
    }
}