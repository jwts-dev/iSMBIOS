﻿
namespace iTin.Core.Hardware.Specification.Smbios
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;

    using Dmi.Property;

    using Helpers;
    using Helpers.Enumerations;

    // Type 005: Memory Controller Information.
    // •——————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
    // |          Spec.                                                                                                   |
    // | Offset   Version     Name                Length      deviceProperty       Description                            |
    // •——————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
    // | 00h      2.0+        Type                BYTE        5           Memory Controller Indicator                     |
    // •——————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
    // | 01h      2.0+        Length              BYTE        Varies      0fh + (2 * x) for version 2.0                   |
    // |                                                                  10h + (2 * x) for version 2.1 and later         |
    // |                                                                  Where x is the 0Eh field value.                 |
    // •——————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
    // | 02h      2.0+        Handle              WORD        Varies                                                      |
    // •——————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
    // | 04h      2.0+        Error Detecting     BYTE        ENUM        Note: Please see, GetErrorDetectingMethod       |
    // |                      Method                                                                                      |
    // •——————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
    // | 05h      2.0+        Error               BYTE        Bit Field   Note: Please see, GetErrorDetectingMethod       |
    // |                      Correcting                                                                                  |
    // |                      Capability                                                                                  |
    // •——————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
    // | 06h      2.0+        Supported           BYTE        ENUM        Note: Please see, GetControllerInterleave       |
    // |                      Interleave                                                                                  |
    // •——————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
    // | 07h      2.0+        Current Interleave  BYTE        ENUM        Note: Please see, GetControllerInterleave       |
    // •——————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
    // | 08h      2.0+        Maximum             BYTE        Varies (n)  The size of the largest memory module           |
    // |                      Memory                                      supported (per slot), specified as n,           |
    // |                      Module Size                                 where 2**n is the maximum size in MB.           |
    // |                                                                  The maximum amount of memory supported by       |
    // |                                                                  this controller is that deviceProperty times    |
    // |                                                                  the number of slots, as specified in offset     |
    // |                                                                  0Eh of this structure.                          |
    // •——————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
    // | 09h      2.0+        Supported Speeds    WORD        Bit Field   Note: Please see, GetControllerSupportedSpeeds  |
    // •——————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
    // | 0Bh      2.0+        Supported           WORD        Bit Field   Note: Please see, GetControllerSupportedTypes   |
    // |                      Memory Types                                                                                |
    // •——————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
    // | 0Dh      2.0+        Memory              BYTE        Bit Field   This field describes the requiered voltages     |
    // |                      Module                                      for every memory module.                        |
    // |                      Voltage                                                                                     |
    // |                                                                  Bits 07:03 - Reserved, must be zero             |
    // |                                                                  Bit     02 - 2.9V                               |
    // |                                                                  Bit     01 - 3.3V                               |
    // |                                                                  Bit     00 - 5V                                 |
    // |                                                                                                                  |
    // |                                                                  Setting of multiple bits indicates that         |
    // |                                                                  the sockets are configurable.                   |
    // |                                                                  Note: Please see, GetControllerModuleVoltage    |
    // •——————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
    // | 0Eh      2.0+        Number of           BYTE        Varies      Defines how many of the Memory Module           |
    // |                      Associated                                  Information blocks are controlled by this       |
    // |                      Memory Slots                                controller.                                     |
    // |                      (x)                                                                                         |
    // •——————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
    // | 0Fh to   2.0+        Memory              x           Varies      A list of memory information structure          | 
    // | 0Fh +                Module              WORDs                   handles controlled by this controller.          |
    // | (2*x)-1              Configuration                               deviceProperty in offset 0Eh (x) defines the    |
    // |                      Handles                                     count.                                          |
    // •——————————————————————————————————————————————————————————————————————————————————————————————————————————————————•
    // | 0Fh +    2.1+        Enabled Error       BYTE        Bit Field   Identifies the error-correcting                 |
    // | (2*x)                Correcting                                  capabilities that were enabled when the         |
    // |                      Capabilities                                structure was built.                            |
    // |                                                                  Note: Please see, GetErrorCorrectingCapability  |
    // •——————————————————————————————————————————————————————————————————————————————————————————————————————————————————•

    /// <inheritdoc />
    /// <summary>
    /// Specialization of the <see cref="T:iTin.Core.Hardware.Specification.Smbios.SmbiosBaseType" /> class that contains the logic to decode the Memory Controller Information (Type 5, Obsolete) structure.
    /// </summary>
    sealed class SmbiosType005 : SmbiosBaseType
    {
        #region constructor/s

        #region [public] SmbiosType005(SmbiosStructureHeaderInfo, int): Initializes a new instance of the class by specifying the structure information and the SMBIOS version
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:iTin.Core.Hardware.Specification.Smbios.SmbiosType005" /> class by specifying the structure information and the <see cref="T:iTin.Core.Hardware.Specification.SMBIOS" /> version.
        /// </summary>
        /// <param name="smbiosStructureHeaderInfo">Raw information of the current structure.</param>
        /// <param name="smbiosVersion">Current <see cref="T:iTin.Core.Hardware.Specification.SMBIOS" /> version.</param>
        public SmbiosType005(SmbiosStructureHeaderInfo smbiosStructureHeaderInfo, int smbiosVersion) : base(smbiosStructureHeaderInfo, smbiosVersion)
        {
        }
        #endregion

        #endregion

        #region private properties

        #region [private] (byte) ErrorDetectingMethod: Gets a value representing the 'Error Detecting Method' field
        /// <summary>
        /// Gets a value representing the <c>Error Detecting Method</c> field.
        /// </summary>
        /// <value>
        /// Property value.
        /// </value>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte ErrorDetectingMethod => GetByte(0x04);
        #endregion

        #region [private] (byte) ErrorCorrectingCapabilities: Gets a value representing the 'Error Correcting Capabilities' field
        /// <summary>
        /// Gets a value representing the <c>Error Correcting Capabilities</c> field.
        /// </summary>
        /// <value>
        /// Property value.
        /// </value>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte ErrorCorrectingCapabilities => GetByte(0x05);
        #endregion

        #region [private] (byte) SupportedInterleave: Gets a value representing the 'Supported Interleave' field
        /// <summary>
        /// Gets a value representing the <c>Supported Interleave</c> field.
        /// </summary>
        /// <value>
        /// Property value.
        /// </value>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte SupportedInterleave => HeaderInfo.RawData[0x06];
        #endregion

        #region [private] (byte) CurrentInterleave: Gets a value representing the 'Current Interleave' field
        /// <summary>
        /// Gets a value representing the <c>Current Interleave</c> field.
        /// </summary>
        /// <value>
        /// Property value.
        /// </value>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte CurrentInterleave => GetByte(0x07);
        #endregion

        #region [private] (int) MaximumMemoryModuleSize: Gets a value representing the 'Maximum Memory Module Size' field
        /// <summary>
        /// Gets a value representing the <c>Maximum Memory Module Size</c> field.
        /// </summary>
        /// <value>
        /// Property value.
        /// </value>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int MaximumMemoryModuleSize => (int)Math.Pow(2, GetByte(0x08));
        #endregion

        #region [private] (int) SupportedSpeeds: Gets a value representing the 'Supported Speeds' field
        /// <summary>
        /// Gets a value representing the <c>Supported Speeds</c> field.
        /// </summary>
        /// <value>
        /// Property value.
        /// </value>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int SupportedSpeeds => GetWord(0x09);
        #endregion

        #region [private] (int) SupportedMemoryTypes: Gets a value representing the 'Supported Memory Types' field
        /// <summary>
        /// Gets a value representing the <c>Supported Memory Types</c> field.
        /// </summary>
        /// <value>
        /// Property value.
        /// </value>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int SupportedMemoryTypes => GetWord(0x0b);
        #endregion

        #region [private] (byte) MemoryModuleVoltages: Gets a value representing the 'Memory Module Voltages' field
        /// <summary>
        /// Gets a value representing the <c>Memory Module Voltages</c> field.
        /// </summary>
        /// <value>
        /// Property value.
        /// </value>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte MemoryModuleVoltages => GetByte(0x0d);
        #endregion

        #region [private] (byte) NumberMemorySlots: Gets a value representing the 'Number Memory Slots' field
        /// <summary>
        /// Gets a value representing the '<c>Number Memory Slots</c>'.
        /// </summary>
        /// <deviceProperty>
        /// Property value.
        /// </deviceProperty>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private byte NumberMemorySlots => HeaderInfo.RawData[0x0e];
        #endregion

        #endregion

        #region protected override methods

        #region [protected] {override} (object) GetValueTypedProperty(PropertyKey): Gets a value that represents the value of the specified property
        /// <inheritdoc />
        /// <summary>
        /// Gets a value that represents the value of the specified property.
        /// </summary>
        /// <param name="propertyKey">Property key to be obtained.</param>
        /// <returns>
        /// Property value
        /// </returns>
        protected override object GetValueTypedProperty(PropertyKey propertyKey)
        {
            object value = null;
            SmbiosType005Property propertyId = (SmbiosType005Property)propertyKey.PropertyId;

            switch (propertyId)
            {
                #region [0x04] - [v2.0] - [Error Detecting Method] - [String]
                case SmbiosType005Property.ErrorDetectingMethod:
                    value = GetErrorDetectingMethod(ErrorDetectingMethod);
                    break;
                #endregion

                #region [0x05] - [v2.0] - [Error Correcting Capabilities] - [String]
                case SmbiosType005Property.ErrorCorrectingCapabilities:
                    value = GetErrorCorrectingCapability(ErrorCorrectingCapabilities);
                    break;
                #endregion

                #region [0x06] - [v2.0] - [Supported Interleave] - [String]
                case SmbiosType005Property.SupportedInterleave:
                    value = GetControllerInterleave(SupportedInterleave);
                    break;
                #endregion

                #region [0x07] - [v2.0] - [Current Interleave] - [String]
                case SmbiosType005Property.CurrentInterleave:
                    value = GetControllerInterleave(CurrentInterleave);
                    break;
                #endregion

                #region [0x08] - [v2.0] - [Maximum Memory Module Size] - [Int32]
                case SmbiosType005Property.MaximumMemoryModuleSize:
                    value = MaximumMemoryModuleSize;
                    break;
                #endregion

                #region [0x09] - [v2.0] - [Supported Speeds] - [ReadOnlyCollection<String>]
                case SmbiosType005Property.SupportedSpeeds:
                    value = GetControllerSupportedSpeeds(SupportedSpeeds);
                    break;
                #endregion

                #region [0x0b] - [v2.0] - [Supported Memory Types] - [ReadOnlyCollection<String>]
                case SmbiosType005Property.SupportedMemoryTypes:
                    value = GetControllerSupportedTypes(SupportedMemoryTypes);
                    break;
                #endregion

                #region [0x0d] - [v2.0] - [Memory Module Voltages] - [ReadOnlyCollection<String>]
                case SmbiosType005Property.MemoryModuleVoltages:
                    value = GetControllerModuleVoltages(MemoryModuleVoltages);
                    break;
                #endregion

                #region [0x0e] - [v2.0] - [Number Of Associated Memory Slots] - [Int32]
                case SmbiosType005Property.NumberMemorySlots:
                    value = NumberMemorySlots;
                    break;
                #endregion

                #region [0x0f + (2*x)-1] - [v2.0] - [Memory Module Configuration Handles] - [MemoryControllerContainedElementCollection]
                case SmbiosType005Property.ContainedMemoryModules:
                    byte n = NumberMemorySlots;
                    if (n != 0x00)
                    {
                        int m = n << 1;
                        var containedElementsArray = new byte[m];
                        Array.Copy(HeaderInfo.RawData, 0x0f, containedElementsArray, 0, m);

                        IEnumerable<int> containedElements = GetContainedMemoryModules(containedElementsArray);
                        value = new MemoryControllerContainedElementCollection(containedElements);
                    }
                    break;
                #endregion

                #region [0x0f + (2*x)] - [v2.1] - [Enabled Error Correcting Capabilities] - [String]
                case SmbiosType005Property.EnabledErrorCorrectingCapabilities:
                    byte nn = NumberMemorySlots;
                    if (nn != 0x00)
                    {
                        int m = nn << 1;
                        if (HeaderInfo.Length >= 0x10 + m)
                        {
                            value = GetErrorCorrectingCapability(HeaderInfo.RawData[0x0f + m]);
                        }
                    }
                    break;                
                #endregion
            }

            return value;
        }
        #endregion

        #region [protected] {override} (void) Parse(SmbiosPropertiesTable): Gets the property collection for this structure
        /// <inheritdoc />
        /// <summary>
        /// Gets the property collection for this structure.
        /// </summary>
        /// <param name="properties">Collection of properties of this structure.</param>
        protected override void Parse(SmbiosPropertiesTable properties)
        {
            #region validate parameter/s
            SentinelHelper.ArgumentNull(properties);
            #endregion

            #region values
            properties.Add(DmiProperty.MemoryController.ErrorDetectingMethod, GetErrorDetectingMethod(ErrorDetectingMethod));
            properties.Add(DmiProperty.MemoryController.ErrorCorrectingCapabilities, GetErrorCorrectingCapability(ErrorCorrectingCapabilities));
            properties.Add(DmiProperty.MemoryController.SupportedInterleave, GetControllerInterleave(SupportedInterleave));
            properties.Add(DmiProperty.MemoryController.CurrentInterleave, GetControllerInterleave(CurrentInterleave));
            properties.Add(DmiProperty.MemoryController.MaximumMemoryModuleSize, MaximumMemoryModuleSize);
            properties.Add(DmiProperty.MemoryController.SupportedSpeeds, GetControllerSupportedSpeeds(SupportedSpeeds));
            properties.Add(DmiProperty.MemoryController.SupportedMemoryTypes, GetControllerSupportedTypes(SupportedMemoryTypes));
            properties.Add(DmiProperty.MemoryController.MemoryModuleVoltages, GetControllerModuleVoltages(MemoryModuleVoltages));

            byte n = NumberMemorySlots;
            properties.Add(DmiProperty.MemoryController.NumberMemorySlots, n);

            if (n != 0x00)
            {
                int m = n << 1;
                byte[] containedElementsArray = new byte[m];
                Array.Copy(HeaderInfo.RawData, 0x0f, containedElementsArray, 0, m);

                IEnumerable<int> containedElements = GetContainedMemoryModules(containedElementsArray);
                properties.Add(DmiProperty.MemoryController.ContainedMemoryModules, new MemoryControllerContainedElementCollection(containedElements));

                if (HeaderInfo.Length >= 0x10 + m)
                {
                    properties.Add(DmiProperty.MemoryController.EnabledErrorCorrectingCapabilities, GetErrorCorrectingCapability(HeaderInfo.RawData[0x0f + m]));
                }
            }
            #endregion
        }
        #endregion

        #endregion

        #region BIOS Specification 2.7.1 (26/01/2011)

        #region [private] {static} (string) GetControllerInterleave(byte): Gets a string representing the interpolation method
        /// <summary>
        /// Gets a string representing the interpolation method.
        /// </summary>
        /// <param name="code">Value to analyze</param>
        /// <returns>
        /// The interpolation method.
        /// </returns>
        private static string GetControllerInterleave(byte code)
        {
            string[] deviceProperty =
            {
                "Other",   // 0x01
                "Unknown",
                "1 way",
                "2 way",
                "4 way",
                "8 way",
                "16 way"   // 0x07                                      
            };

            if (code >= 0x01 && code <= 0x07)
            {
                return deviceProperty[code - 0x01];
            }

            return SmbiosHelper.OutOfSpec;
        }
        #endregion

        #region [private] {static} (IEnumerable<int>) GetContainedMemoryModules(IList<byte>): Gets the list of memory devices that control this memory controller
        /// <summary>
        /// Gets the list of memory devices that control this memory controller.
        /// </summary>
        /// <param name="rawdevicePropertyArray">Raw information.</param>
        /// <returns>
        /// Collection of items contained in this memory controller.
        /// </returns>
        private static IEnumerable<int> GetContainedMemoryModules(IList<byte> rawdevicePropertyArray)
        {
            Collection<int> containedElements = new Collection<int>();
            for (byte i = 0; i < rawdevicePropertyArray.Count; i++)
            {
                containedElements.Add(rawdevicePropertyArray.ToArray().GetWord(i));
                i++;
            }

            return containedElements;
        }
        #endregion

        #region [private] {static} (ReadOnlyCollection<string>) GetControllerModuleVoltages(int): Returns a collection of voltages supported by the memory device
        /// <summary>
        /// Returns a collection of voltages supported by the memory device.
        /// </summary>
        /// <param name="code">Value to analyze</param>
        /// <returns>
        /// Collection of voltages supported by the memory device.
        /// </returns>            
        private static ReadOnlyCollection<string> GetControllerModuleVoltages(byte code)
        {
            string[] deviceProperty =
            {
                "5.0", // 0
                "3.3",
                "2.9"  // 2                                     
            };

            List<string> items = new List<string>();

            bool isLegacyMode = code.CheckBit(Bits.Bit07);
            if (isLegacyMode)
            {
                double voltage = (double)(code & 0x7f) / 10;
                items.Add($"{voltage}");
            }
            else
            {
                for (byte i = 0; i <= 2; i++)
                {
                    bool addVoltage = code.CheckBit(i);
                    if (addVoltage)
                    {
                        items.Add(deviceProperty[i]);
                    }
                }
            }

            return items.AsReadOnly();
        }
        #endregion

        #region [private] {static} (ReadOnlyCollection<string>) GetControllerSupportedSpeeds(int): Gets a collection of supported speeds
        /// <summary>
        /// Gets a collection of supported speeds.
        /// </summary>
        /// <param name="code">Value to analyze</param>
        /// <returns>
        /// Gets a collection of supported speeds.
        /// </returns>            
        private static ReadOnlyCollection<string> GetControllerSupportedSpeeds(int code)
        {
            string[] deviceProperty =
            {
                "Other", // 0x00
                "Unknown",
                "70",
                "60",
                "50" // 0x04                                              
            };

            List<string> items = new List<string>();
            for (byte i = 0x00; i <= 0x04; i++)
            {                    
                bool addSpeed = code.CheckBit(i);
                if (addSpeed)
                {
                    items.Add(deviceProperty[i]);
                }
            }

            return items.AsReadOnly();
        }
        #endregion

        #region [private] {static} (ReadOnlyCollection<string>) GetControllerSupportedTypes(int): Gets a collection of supported memory types
        /// <summary>
        /// Gets a collection of supported memory types.
        /// </summary>
        /// <param name="code">Value to analyze</param>
        /// <returns>
        /// Collection of supported memory types.
        /// </returns>            
        private static ReadOnlyCollection<string> GetControllerSupportedTypes(int code)
        {
            string[] deviceProperty =
            {
                "Other",            // 0x00
                "Unknown",
                "Standard",
                "Fast Page Mode",
                "EDO",
                "Parity",
                "ECC",
                "SIMM",
                "DIMM",
                "Burst EDO",
                "SDRAM"             // 0x0a                                              
            };

            List<string> items = new List<string>();
            for (byte i = 0x00; i <= 0x0a; i++)
            {
                bool addType = code.CheckBit(i);
                if (addType)
                {
                    items.Add(deviceProperty[i]);
                }
            }

            return items.AsReadOnly();
        }
        #endregion

        #region [private] {static} (string) GetErrorCorrectingCapability(byte): Gets a string representing the error correction method
        /// <summary>
        /// Gets a string representing the error correction method.
        /// </summary>
        /// <param name="code">Value to analyze</param>
        /// <returns>
        /// The error correction method.
        /// </returns>            
        private static string GetErrorCorrectingCapability(byte code)
        {
            string[] items =
            {
                "Other",                         // 0x01
                "Unknown",
                "None",
                "Single bit error correcting",
                "Double bit error correcting",
                "Error Scrubbing"                // 0x05
            };

            string deviceProperty = string.Empty;
            for (byte i = 0; i <= 0x05; i++)
            {
                bool addItem = code.CheckBit(i);
                if (addItem)
                {
                    deviceProperty = items[i];
                }
            }

            return deviceProperty;
        }
        #endregion

        #region [private] {static} (string) GetErrorDetectingMethod(byte): Gets a string representing the error detection method
        /// <summary>
        /// Gets a string representing the error detection method.
        /// </summary>
        /// <param name="code">Value to analyze</param>
        /// <returns>
        /// The error detection method.
        /// </returns>            
        private static string GetErrorDetectingMethod(byte code)
        {
            string[] deviceProperty =
            {
                "Other",          // 0x01
                "Unknown",
                "None",
                "8-bit Parity",
                "32-bit ECC",
                "64-bit ECC",
                "128-bit ECC",
                "CRC"             // 0x08
            };

            if (code >= 0x01 && code <= 0x08)
            {
                return deviceProperty[code - 0x01];
            }

            return SmbiosHelper.OutOfSpec;
        }
        #endregion

        #endregion
    }
}
