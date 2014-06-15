/***************************************************************************
The Disc Image Chef
----------------------------------------------------------------------------
 
Filename       : Main.cs
Version        : 1.0
Author(s)      : Natalia Portillo
 
Component      : Main program loop.

Revision       : $Revision$
Last change by : $Author$
Date           : $Date$
 
--[ Description ] ----------------------------------------------------------
 
Contains the main program loop.
 
--[ License ] --------------------------------------------------------------
 
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

----------------------------------------------------------------------------
Copyright (C) 2011-2014 Claunia.com
****************************************************************************/
//$Id$

using System;
using System.Collections.Generic;
using DiscImageChef.ImagePlugins;
using DiscImageChef.PartPlugins;
using DiscImageChef.Plugins;
using System.Reflection;

namespace DiscImageChef
{
    class MainClass
    {
        static PluginBase plugins;
        public static bool isDebug;
        public static bool isVerbose;

        public static void Main(string[] args)
        {
            plugins = new PluginBase();

            string invokedVerb = "";
            object invokedVerbInstance = null;

            var options = new Options();
            if (!CommandLine.Parser.Default.ParseArguments(args, options,
                (verb, subOptions) =>
            {
                // if parsing succeeds the verb name and correct instance
                // will be passed to onVerbCommand delegate (string,object)
                invokedVerb = verb;
                invokedVerbInstance = subOptions;
            }))
            {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            object[] attributes = typeof(MainClass).Assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            string AssemblyTitle = ((AssemblyTitleAttribute) attributes[0]).Title;
            attributes = typeof(MainClass).Assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            Version AssemblyVersion = typeof(MainClass).Assembly.GetName().Version;
            string AssemblyCopyright  = ((AssemblyCopyrightAttribute) attributes[0]).Copyright;

            Console.WriteLine("{0} {1}", AssemblyTitle, AssemblyVersion);
            Console.WriteLine("{0}", AssemblyCopyright);
            Console.WriteLine();

            switch (invokedVerb)
            {
                case "analyze":
                    AnalyzeSubOptions AnalyzeOptions = (AnalyzeSubOptions)invokedVerbInstance;
                    isDebug = AnalyzeOptions.Debug;
                    isVerbose = AnalyzeOptions.Verbose;
                    Analyze(AnalyzeOptions);
                    break;
                case "compare":
                    CompareSubOptions CompareOptions = (CompareSubOptions)invokedVerbInstance;
                    isDebug = CompareOptions.Debug;
                    isVerbose = CompareOptions.Verbose;
                    Compare(CompareOptions);
                    break;
                case "checksum":
                    ChecksumSubOptions ChecksumOptions = (ChecksumSubOptions)invokedVerbInstance;
                    isDebug = ChecksumOptions.Debug;
                    isVerbose = ChecksumOptions.Verbose;
                    Checksum(ChecksumOptions);
                    break;
                case "verify":
                    VerifySubOptions VerifyOptions = (VerifySubOptions)invokedVerbInstance;
                    isDebug = VerifyOptions.Debug;
                    isVerbose = VerifyOptions.Verbose;
                    Verify(VerifyOptions);
                    break;
                case "formats":
                    ListFormats();
                    break;
                default:
                    throw new ArgumentException("Should never arrive here!");
            }
        }

        static void ListFormats()
        {
            plugins.RegisterAllPlugins();

            Console.WriteLine("Supported images:");
            foreach (KeyValuePair<string, ImagePlugin> kvp in plugins.ImagePluginsList)
                Console.WriteLine(kvp.Value.Name);
            Console.WriteLine();
            Console.WriteLine("Supported filesystems:");
            foreach (KeyValuePair<string, Plugin> kvp in plugins.PluginsList)
                Console.WriteLine(kvp.Value.Name);
            Console.WriteLine();
            Console.WriteLine("Supported partitions:");
            foreach (KeyValuePair<string, PartPlugin> kvp in plugins.PartPluginsList)
                Console.WriteLine(kvp.Value.Name);
        }

        static void Compare(CompareSubOptions options)
        {
            if (isDebug)
            {
                Console.WriteLine("--debug={0}", options.Debug);
                Console.WriteLine("--verbose={0}", options.Verbose);
                Console.WriteLine("--input1={0}", options.InputFile1);
                Console.WriteLine("--input2={0}", options.InputFile2);
            }
            throw new NotImplementedException("Comparing not yet implemented.");
        }

        static void Checksum(ChecksumSubOptions options)
        {
            if (isDebug)
            {
                Console.WriteLine("--debug={0}", options.Debug);
                Console.WriteLine("--verbose={0}", options.Verbose);
                Console.WriteLine("--separated-tracks={0}", options.SeparatedTracks);
                Console.WriteLine("--whole-disc={0}", options.WholeDisc);
                Console.WriteLine("--input={0}", options.InputFile);
                Console.WriteLine("--crc32={0}", options.DoCRC32);
                Console.WriteLine("--md5={0}", options.DoMD5);
                Console.WriteLine("--sha1={0}", options.DoSHA1);
                Console.WriteLine("--fuzzy={0}", options.DoFuzzy);
            }
            throw new NotImplementedException("Checksumming not yet implemented.");
        }

        static void Verify(VerifySubOptions options)
        {
            if (isDebug)
            {
                Console.WriteLine("--debug={0}", options.Debug);
                Console.WriteLine("--verbose={0}", options.Verbose);
                Console.WriteLine("--input={0}", options.InputFile);
                Console.WriteLine("--verify-disc={0}", options.VerifyDisc);
                Console.WriteLine("--verify-sectors={0}", options.VerifySectors);
            }
            throw new NotImplementedException("Verifying not yet implemented.");
        }

        static void Analyze(AnalyzeSubOptions options)
        {
            if (isDebug)
            {
                Console.WriteLine("--debug={0}", options.Debug);
                Console.WriteLine("--verbose={0}", options.Verbose);
                Console.WriteLine("--input={0}", options.InputFile);
                Console.WriteLine("--filesystems={0}", options.SearchForFilesystems);
                Console.WriteLine("--partitions={0}", options.SearchForPartitions);
            }

            plugins.RegisterAllPlugins();

            List<string> id_plugins;
            Plugin _plugin;
            string information;
            bool checkraw = false;
            ImagePlugin _imageFormat;
			
            try
            {
                _imageFormat = null;

                // Check all but RAW plugin
                foreach (ImagePlugin _imageplugin in plugins.ImagePluginsList.Values)
                {
                    if(_imageplugin.PluginUUID != new Guid("12345678-AAAA-BBBB-CCCC-123456789000"))
                    {
                        if (_imageplugin.IdentifyImage(options.InputFile))
                        {
                            _imageFormat = _imageplugin;
                            Console.WriteLine("Image format identified by {0}.", _imageplugin.Name);
                            break;
                        }
                    }
                }

                // Check only RAW plugin
                if (_imageFormat == null)
                {
                    foreach (ImagePlugin _imageplugin in plugins.ImagePluginsList.Values)
                    {
                        if(_imageplugin.PluginUUID == new Guid("12345678-AAAA-BBBB-CCCC-123456789000"))
                        {
                            if (_imageplugin.IdentifyImage(options.InputFile))
                            {
                                _imageFormat = _imageplugin;
                                Console.WriteLine("Image format identified by {0}.", _imageplugin.Name);
                                break;
                            }
                        }
                    }
                }

                // Still not recognized
                if (_imageFormat == null)
                {
                    Console.WriteLine("Image format not identified, not proceeding.");
                    return;
                }

                try
                {
                    if (!_imageFormat.OpenImage(options.InputFile))
                    {
                        Console.WriteLine("Unable to open image format");
                        Console.WriteLine("No error given");
                        return;
                    }

                    if (isDebug)
                    {
                        Console.WriteLine("DEBUG: Correctly opened image file.");
                        Console.WriteLine("DEBUG: Image without headers is {0} bytes.", _imageFormat.GetImageSize());
                        Console.WriteLine("DEBUG: Image has {0} sectors.", _imageFormat.GetSectors());
                        Console.WriteLine("DEBUG: Image identifies disk type as {0}.", _imageFormat.GetDiskType());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to open image format");
                    Console.WriteLine("Error: {0}", ex.Message);
                    return;
                }

                Console.WriteLine("Image identified as {0}.", _imageFormat.GetImageFormat());

                if (options.SearchForPartitions)
                {
                    List<Partition> partitions = new List<Partition>();
                    string partition_scheme = "";
					
                    // TODO: Solve possibility of multiple partition schemes (CUE + MBR, MBR + RDB, CUE + APM, etc)
                    foreach (PartPlugin _partplugin in plugins.PartPluginsList.Values)
                    {
                        List<Partition> _partitions;

                        if (_partplugin.GetInformation(_imageFormat, out _partitions))
                        {
                            partition_scheme = _partplugin.Name;
                            partitions = _partitions;
                            break;
                        }
                    }

                    if (_imageFormat.ImageHasPartitions())
                    {
                        partition_scheme = _imageFormat.GetImageFormat();
                        partitions = _imageFormat.GetPartitions();
                    }
					
                    if (partition_scheme == "")
                    {
                        if(MainClass.isDebug)
                            Console.WriteLine("DEBUG: No partitions found");
                        if (!options.SearchForFilesystems)
                        {
                            Console.WriteLine("No partitions founds, not searching for filesystems");
                            return;
                        }
                        checkraw = true;
                    }
                    else
                    {
                        Console.WriteLine("Partition scheme identified as {0}", partition_scheme);
                        Console.WriteLine("{0} partitions found.", partitions.Count);
						
                        for (int i = 0; i < partitions.Count; i++)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Partition {0}:", partitions[i].PartitionSequence);	
                            Console.WriteLine("Partition name: {0}", partitions[i].PartitionName);	
                            Console.WriteLine("Partition type: {0}", partitions[i].PartitionType);	
                            Console.WriteLine("Partition start: sector {0}, byte {1}", partitions[i].PartitionStartSector, partitions[i].PartitionStart);	
                            Console.WriteLine("Partition length: {0} sectors, {1} bytes", partitions[i].PartitionSectors, partitions[i].PartitionLength);	
                            Console.WriteLine("Partition description:");	
                            Console.WriteLine(partitions[i].PartitionDescription);
							
                            if (options.SearchForFilesystems)
                            {
                                Console.WriteLine("Identifying filesystem on partition");
								
                                Identify(_imageFormat, out id_plugins, partitions[i].PartitionStartSector);
                                if (id_plugins.Count == 0)
                                    Console.WriteLine("Filesystem not identified");
                                else if (id_plugins.Count > 1)
                                {
                                    Console.WriteLine(String.Format("Identified by {0} plugins", id_plugins.Count));
									
                                    foreach (string plugin_name in id_plugins)
                                    {
                                        if (plugins.PluginsList.TryGetValue(plugin_name, out _plugin))
                                        {
                                            Console.WriteLine(String.Format("As identified by {0}.", _plugin.Name));
                                            _plugin.GetInformation(_imageFormat, partitions[i].PartitionStartSector, out information);
                                            Console.Write(information);
                                        }
                                    }
                                }
                                else
                                {
                                    plugins.PluginsList.TryGetValue(id_plugins[0], out _plugin);
                                    Console.WriteLine(String.Format("Identified by {0}.", _plugin.Name));
                                    _plugin.GetInformation(_imageFormat, partitions[i].PartitionStartSector, out information);
                                    Console.Write(information);
                                }
                            }
                        }
                    }
                }
				
                if (checkraw)
                {
                    Identify(_imageFormat, out id_plugins, 0);
                    if (id_plugins.Count == 0)
                        Console.WriteLine("Filesystem not identified");
                    else if (id_plugins.Count > 1)
                    {
                        Console.WriteLine(String.Format("Identified by {0} plugins", id_plugins.Count));
						
                        foreach (string plugin_name in id_plugins)
                        {
                            if (plugins.PluginsList.TryGetValue(plugin_name, out _plugin))
                            {
                                Console.WriteLine(String.Format("As identified by {0}.", _plugin.Name));
                                _plugin.GetInformation(_imageFormat, 0, out information);
                                Console.Write(information);
                            }
                        }
                    }
                    else
                    {
                        plugins.PluginsList.TryGetValue(id_plugins[0], out _plugin);
                        Console.WriteLine(String.Format("Identified by {0}.", _plugin.Name));
                        _plugin.GetInformation(_imageFormat, 0, out information);
                        Console.Write(information);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Error reading file: {0}", ex.Message));
                if (isDebug)
                    Console.WriteLine(ex.StackTrace);
            }
        }

        static void Identify(ImagePlugin imagePlugin, out List<string> id_plugins, ulong partitionOffset)
        {
            id_plugins = new List<string>();
			
            foreach (Plugin _plugin in plugins.PluginsList.Values)
            {
                if (_plugin.Identify(imagePlugin, partitionOffset))
                    id_plugins.Add(_plugin.Name.ToLower());
            }
        }
    }
}
