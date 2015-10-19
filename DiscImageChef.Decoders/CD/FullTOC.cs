﻿// /***************************************************************************
// The Disc Image Chef
// ----------------------------------------------------------------------------
//
// Filename       : FullTOC.cs
// Version        : 1.0
// Author(s)      : Natalia Portillo
//
// Component      : Component
//
// Revision       : $Revision$
// Last change by : $Author$
// Date           : $Date$
//
// --[ Description ] ----------------------------------------------------------
//
// Description
//
// --[ License ] --------------------------------------------------------------
//
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as
//     published by the Free Software Foundation, either version 3 of the
//     License, or (at your option) any later version.
//
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
//
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// ----------------------------------------------------------------------------
// Copyright (C) 2011-2015 Claunia.com
// ****************************************************************************/
// //$Id$
using System;
using DiscImageChef.Console;
using System.Text;

namespace DiscImageChef.Decoders.CD
{
    /// <summary>
    /// Information from the following standards:
    /// ANSI X3.304-1997
    /// T10/1048-D revision 9.0
    /// T10/1048-D revision 10a
    /// T10/1228-D revision 7.0c
    /// T10/1228-D revision 11a
    /// T10/1363-D revision 10g
    /// T10/1545-D revision 1d
    /// T10/1545-D revision 5
    /// T10/1545-D revision 5a
    /// T10/1675-D revision 2c
    /// T10/1675-D revision 4
    /// T10/1836-D revision 2g
    /// </summary>
    public static class FullTOC
    {
        public struct CDFullTOC
        {
            /// <summary>
            /// Total size of returned session information minus this field
            /// </summary>
            public UInt16 DataLength;
            /// <summary>
            /// First complete session number in hex
            /// </summary>
            public byte FirstCompleteSession;
            /// <summary>
            /// Last complete session number in hex
            /// </summary>
            public byte LastCompleteSession;
            /// <summary>
            /// Track descriptors
            /// </summary>
            public TrackDataDescriptor[] TrackDescriptors;
        }

        public struct TrackDataDescriptor
        {
            /// <summary>
            /// Byte 0
            /// Session number in hex
            /// </summary>
            public byte SessionNumber;
            /// <summary>
            /// Byte 1, bits 7 to 4
            /// Type of information in Q subchannel of block where this TOC entry was found
            /// </summary>
            public byte ADR;
            /// <summary>
            /// Byte 1, bits 3 to 0
            /// Track attributes
            /// </summary>
            public byte CONTROL;
            /// <summary>
            /// Byte 2
            /// </summary>
            public byte TNO;
            /// <summary>
            /// Byte 3
            /// </summary>
            public byte POINT;
            /// <summary>
            /// Byte 4
            /// </summary>
            public byte Min;
            /// <summary>
            /// Byte 5
            /// </summary>
            public byte Sec;
            /// <summary>
            /// Byte 6
            /// </summary>
            public byte Frame;
            /// <summary>
            /// Byte 7, CD only
            /// </summary>
            public byte Zero;
            /// <summary>
            /// Byte 7, bits 7 to 4, DDCD only
            /// </summary>
            public byte HOUR;
            /// <summary>
            /// Byte 7, bits 3 to 0, DDCD only
            /// </summary>
            public byte PHOUR;
            /// <summary>
            /// Byte 8
            /// </summary>
            public byte PMIN;
            /// <summary>
            /// Byte 9
            /// </summary>
            public byte PSEC;
            /// <summary>
            /// Byte 10
            /// </summary>
            public byte PFRAME;
        }

        public static CDFullTOC? Decode(byte[] CDFullTOCResponse)
        {
            if (CDFullTOCResponse == null)
                return null;

            CDFullTOC decoded = new CDFullTOC();

            BigEndianBitConverter.IsLittleEndian = BitConverter.IsLittleEndian;

            decoded.DataLength = BigEndianBitConverter.ToUInt16(CDFullTOCResponse, 0);
            decoded.FirstCompleteSession = CDFullTOCResponse[2];
            decoded.LastCompleteSession = CDFullTOCResponse[3];
            decoded.TrackDescriptors = new TrackDataDescriptor[(decoded.DataLength - 2) / 11];

            if (decoded.DataLength + 2 != CDFullTOCResponse.Length)
            {
                DicConsole.DebugWriteLine("CD full TOC decoder", "Expected CDFullTOC size ({0} bytes) is not received size ({1} bytes), not decoding", decoded.DataLength + 2, CDFullTOCResponse.Length);
                return null;
            }

            for (int i = 0; i < ((decoded.DataLength - 2) / 11); i++)
            {
                decoded.TrackDescriptors[i].SessionNumber = CDFullTOCResponse[0 + i * 11 + 4];
                decoded.TrackDescriptors[i].ADR = (byte)((CDFullTOCResponse[1 + i * 11 + 4] & 0xF0) >> 4);
                decoded.TrackDescriptors[i].CONTROL = (byte)(CDFullTOCResponse[1 + i * 11 + 4] & 0x0F);
                decoded.TrackDescriptors[i].TNO = CDFullTOCResponse[2 + i * 11 + 4];
                decoded.TrackDescriptors[i].POINT = CDFullTOCResponse[3 + i * 11 + 4];
                decoded.TrackDescriptors[i].Min = CDFullTOCResponse[4 + i * 11 + 4];
                decoded.TrackDescriptors[i].Sec = CDFullTOCResponse[5 + i * 11 + 4];
                decoded.TrackDescriptors[i].Frame = CDFullTOCResponse[6 + i * 11 + 4];
                decoded.TrackDescriptors[i].Zero = CDFullTOCResponse[7 + i * 11 + 4];
                decoded.TrackDescriptors[i].HOUR = (byte)((CDFullTOCResponse[7 + i * 11 + 4] & 0xF0) >> 4);
                decoded.TrackDescriptors[i].PHOUR = (byte)(CDFullTOCResponse[7 + i * 11 + 4] & 0x0F);
                decoded.TrackDescriptors[i].PMIN = CDFullTOCResponse[8 + i * 11 + 4];
                decoded.TrackDescriptors[i].PSEC = CDFullTOCResponse[9 + i * 11 + 4];
                decoded.TrackDescriptors[i].PFRAME = CDFullTOCResponse[10 + i * 11 + 4];
            }

            return decoded;
        }

        public static string Prettify(CDFullTOC? CDFullTOCResponse)
        {
            if (CDFullTOCResponse == null)
                return null;

            CDFullTOC response = CDFullTOCResponse.Value;

            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("First complete session number: {0}", response.FirstCompleteSession).AppendLine();
            sb.AppendFormat("Last complete session number: {0}", response.LastCompleteSession).AppendLine();
            foreach (TrackDataDescriptor descriptor in response.TrackDescriptors)
            {
                if ((descriptor.CONTROL != 4 && descriptor.CONTROL != 6) ||
                    (descriptor.ADR != 1 && descriptor.ADR != 5) ||
                    descriptor.TNO != 0)
                {
                    sb.AppendLine("Unknown TOC entry format, printing values as-is");
                    sb.AppendFormat("SessionNumber = {0}", descriptor.SessionNumber).AppendLine();
                    sb.AppendFormat("ADR = {0}", descriptor.ADR).AppendLine();
                    sb.AppendFormat("CONTROL = {0}", descriptor.CONTROL).AppendLine();
                    sb.AppendFormat("TNO = {0}", descriptor.TNO).AppendLine();
                    sb.AppendFormat("POINT = {0}", descriptor.POINT).AppendLine();
                    sb.AppendFormat("Min = {0}", descriptor.Min).AppendLine();
                    sb.AppendFormat("Sec = {0}", descriptor.Sec).AppendLine();
                    sb.AppendFormat("Frame = {0}", descriptor.Frame).AppendLine();
                    sb.AppendFormat("HOUR = {0}", descriptor.HOUR).AppendLine();
                    sb.AppendFormat("PHOUR = {0}", descriptor.PHOUR).AppendLine();
                    sb.AppendFormat("PMIN = {0}", descriptor.PMIN).AppendLine();
                    sb.AppendFormat("PSEC = {0}", descriptor.PSEC).AppendLine();
                    sb.AppendFormat("PFRAME = {0}", descriptor.PFRAME).AppendLine();
                }
                else
                {
                    sb.AppendFormat("Session {0}", descriptor.SessionNumber).AppendLine();
                    switch (descriptor.ADR)
                    {
                        case 1:
                            {
                                switch (descriptor.POINT)
                                {
                                    case 0xA0:
                                        {
                                            sb.AppendFormat("First track number: {0}", descriptor.PMIN).AppendLine();
                                            sb.AppendFormat("Disc type: {0}", descriptor.PSEC).AppendLine();
                                            sb.AppendFormat("Absolute time: {3:X2}:{0:X2}:{1:X2}:{2:X2}", descriptor.Min, descriptor.Sec, descriptor.Frame, descriptor.HOUR).AppendLine();
                                            break;
                                        }
                                    case 0xA1:
                                        {
                                            sb.AppendFormat("Last track number: {0}", descriptor.PMIN).AppendLine();
                                            sb.AppendFormat("Absolute time: {3:X2}:{0:X2}:{1:X2}:{2:X2}", descriptor.Min, descriptor.Sec, descriptor.Frame, descriptor.HOUR).AppendLine();
                                            break;
                                        }
                                    case 0xA2:
                                        {
                                            sb.AppendFormat("Lead-out start position: {3:X2}:{0:X2}:{1:X2}:{2:X2}", descriptor.PMIN, descriptor.PSEC, descriptor.PFRAME, descriptor.PHOUR).AppendLine();
                                            sb.AppendFormat("Absolute time: {3:X2}:{0:X2}:{1:X2}:{2:X2}", descriptor.Min, descriptor.Sec, descriptor.Frame, descriptor.HOUR).AppendLine();
                                            break;
                                        }
                                    case 0xF0:
                                        {
                                            sb.AppendFormat("Book type: 0x{0:X2}", descriptor.PMIN);
                                            sb.AppendFormat("Material type: 0x{0:X2}", descriptor.PSEC);
                                            sb.AppendFormat("Moment of inertia: 0x{0:X2}", descriptor.PFRAME);
                                            sb.AppendFormat("Absolute time: {3:X2}:{0:X2}:{1:X2}:{2:X2}", descriptor.Min, descriptor.Sec, descriptor.Frame, descriptor.HOUR).AppendLine();
                                            break;
                                        }
                                    default:
                                        {
                                            if (descriptor.POINT >= 0x01 && descriptor.POINT <= 0x63)
                                            {
                                                sb.AppendFormat("Track start position for track {3}: {4:X2}:{0:X2}:{1:X2}:{2:X2}", descriptor.PMIN, descriptor.PSEC, descriptor.PFRAME, descriptor.POINT, descriptor.PHOUR).AppendLine();
                                                sb.AppendFormat("Absolute time: {3:X2}:{0:X2}:{1:X2}:{2:X2}", descriptor.Min, descriptor.Sec, descriptor.Frame, descriptor.HOUR).AppendLine();
                                            }
                                            else
                                            {
                                                sb.AppendFormat("ADR = {0}", descriptor.ADR).AppendLine();
                                                sb.AppendFormat("CONTROL = {0}", descriptor.CONTROL).AppendLine();
                                                sb.AppendFormat("TNO = {0}", descriptor.TNO).AppendLine();
                                                sb.AppendFormat("POINT = {0}", descriptor.POINT).AppendLine();
                                                sb.AppendFormat("Min = {0}", descriptor.Min).AppendLine();
                                                sb.AppendFormat("Sec = {0}", descriptor.Sec).AppendLine();
                                                sb.AppendFormat("Frame = {0}", descriptor.Frame).AppendLine();
                                                sb.AppendFormat("HOUR = {0}", descriptor.HOUR).AppendLine();
                                                sb.AppendFormat("PHOUR = {0}", descriptor.PHOUR).AppendLine();
                                                sb.AppendFormat("PMIN = {0}", descriptor.PMIN).AppendLine();
                                                sb.AppendFormat("PSEC = {0}", descriptor.PSEC).AppendLine();
                                                sb.AppendFormat("PFRAME = {0}", descriptor.PFRAME).AppendLine();
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                        case 5:
                            {
                                switch (descriptor.POINT)
                                {
                                    case 0xB0:
                                        {
                                            sb.AppendFormat("Start of next possible program in the recordable area of the disc: {3:X2}:{0:X2}:{1:X2}:{2:X2}", descriptor.Min, descriptor.Sec, descriptor.Frame, descriptor.HOUR).AppendLine();
                                            sb.AppendFormat("Maximum start of outermost Lead-out in the recordable area of the disc: {3:X2}:{0:X2}:{1:X2}:{2:X2}", descriptor.PMIN, descriptor.PSEC, descriptor.PFRAME, descriptor.PHOUR).AppendLine();
                                            break;
                                        }
                                    case 0xB1:
                                        {
                                            sb.AppendFormat("Number of skip interval pointers: {0:X2}", descriptor.PMIN).AppendLine();
                                            sb.AppendFormat("Number of skip track pointers: {0:X2}", descriptor.PSEC).AppendLine();
                                            break;
                                        }
                                    case 0xB2:
                                    case 0xB3:
                                    case 0xB4:
                                        {
                                            sb.AppendFormat("Skip track {0}", descriptor.Min).AppendLine();
                                            sb.AppendFormat("Skip track {0}", descriptor.Sec).AppendLine();
                                            sb.AppendFormat("Skip track {0}", descriptor.Frame).AppendLine();
                                            sb.AppendFormat("Skip track {0}", descriptor.Zero).AppendLine();
                                            sb.AppendFormat("Skip track {0}", descriptor.PMIN).AppendLine();
                                            sb.AppendFormat("Skip track {0}", descriptor.PSEC).AppendLine();
                                            sb.AppendFormat("Skip track {0}", descriptor.PFRAME).AppendLine();
                                            break;
                                        }
                                    case 0xC0:
                                        {
                                            sb.AppendFormat("Optimum recording power: 0x{0:X2}", descriptor.Min).AppendLine();
                                            sb.AppendFormat("Start time of the first Lead-in area in the disc: {3:X2}:{0:X2}:{1:X2}:{2:X2}", descriptor.PMIN, descriptor.PSEC, descriptor.PFRAME, descriptor.PHOUR).AppendLine();
                                            break;
                                        }
                                    case 0xC1:
                                        {
                                            sb.AppendFormat("Copy of information of A1 from ATIP found");
                                            sb.AppendFormat("Min = {0}", descriptor.Min).AppendLine();
                                            sb.AppendFormat("Sec = {0}", descriptor.Sec).AppendLine();
                                            sb.AppendFormat("Frame = {0}", descriptor.Frame).AppendLine();
                                            sb.AppendFormat("Zero = {0}", descriptor.Zero).AppendLine();
                                            sb.AppendFormat("PMIN = {0}", descriptor.PMIN).AppendLine();
                                            sb.AppendFormat("PSEC = {0}", descriptor.PSEC).AppendLine();
                                            sb.AppendFormat("PFRAME = {0}", descriptor.PFRAME).AppendLine();
                                            break;
                                        }
                                    case 0xCF:
                                        {
                                            sb.AppendFormat("Start position of outer part lead-in area: {3:X2}:{0:X2}:{1:X2}:{2:X2}", descriptor.PMIN, descriptor.PSEC, descriptor.PFRAME, descriptor.PHOUR).AppendLine();
                                            sb.AppendFormat("Stop position of inner part lead-out area: {3:X2}:{0:X2}:{1:X2}:{2:X2}", descriptor.Min, descriptor.Sec, descriptor.Frame, descriptor.HOUR).AppendLine();
                                            break;
                                        }
                                    default:
                                        {
                                            if (descriptor.POINT >= 0x01 && descriptor.POINT <= 0x40)
                                            {
                                                sb.AppendFormat("Start time for interval that should be skipped: {0:X2}:{1:X2}:{2:X2}", descriptor.PMIN, descriptor.PSEC, descriptor.PFRAME).AppendLine();
                                                sb.AppendFormat("Ending time for interval that should be skipped: {0:X2}:{1:X2}:{2:X2}", descriptor.Min, descriptor.Sec, descriptor.Frame).AppendLine();
                                            }
                                            else
                                            {
                                                sb.AppendFormat("ADR = {0}", descriptor.ADR).AppendLine();
                                                sb.AppendFormat("CONTROL = {0}", descriptor.CONTROL).AppendLine();
                                                sb.AppendFormat("TNO = {0}", descriptor.TNO).AppendLine();
                                                sb.AppendFormat("POINT = {0}", descriptor.POINT).AppendLine();
                                                sb.AppendFormat("Min = {0}", descriptor.Min).AppendLine();
                                                sb.AppendFormat("Sec = {0}", descriptor.Sec).AppendLine();
                                                sb.AppendFormat("Frame = {0}", descriptor.Frame).AppendLine();
                                                sb.AppendFormat("HOUR = {0}", descriptor.HOUR).AppendLine();
                                                sb.AppendFormat("PHOUR = {0}", descriptor.PHOUR).AppendLine();
                                                sb.AppendFormat("PMIN = {0}", descriptor.PMIN).AppendLine();
                                                sb.AppendFormat("PSEC = {0}", descriptor.PSEC).AppendLine();
                                                sb.AppendFormat("PFRAME = {0}", descriptor.PFRAME).AppendLine();
                                            }
                                            break;
                                        }
                                }
                                break;
                            }
                    }
                }
            }

            return sb.ToString();
        }

        public static string Prettify(byte[] CDFullTOCResponse)
        {
            CDFullTOC? decoded = Decode(CDFullTOCResponse);
            return Prettify(decoded);
        }
    }
}
