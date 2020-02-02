﻿
namespace iTin.Core.Hardware.Specification.Dmi
{
    using Property;

    using Smbios;
    using Smbios.Property;

    /// <inheritdoc/>
    /// <summary>
    /// Specialization of the <see cref="DmiBaseType{T}"/> class that contains the logic to decode the 64-Bit Memory Error Information (Type 33) structure.
    /// For more information, please see <see cref="SmbiosType033"/>.
    /// </summary>
    internal sealed class DmiType033 : DmiBaseType<SmbiosType033>
    {
        #region constructor/s

        #region [public] DmiType033(SmbiosType033, int): Initializes a new instance of the class by specifying the structure information and the SMBIOS version
        /// <inheritdoc/>
        /// <summary>
        /// Initializes a new instance of the <see cref="DmiType033"/> class by specifying the structure information and the <see cref="SMBIOS"/> version.
        /// </summary>
        /// <param name="smbiosStructure">Formatted structure information.</param>
        /// <param name="smbiosVersion">Current <see cref="SMBIOS"/> version.</param>
        public DmiType033(SmbiosType033 smbiosStructure, int smbiosVersion) : base(smbiosStructure, smbiosVersion)
        {
        }
        #endregion

        #endregion

        #region protected override methods

        #region [protected] {override} (void) PopulateProperties(DmiClassPropertiesTable): Populates the property collection for this structure
        /// <inheritdoc/>
        /// <summary>
        /// Populates the property collection for this structure.
        /// </summary>
        /// <param name="properties">Collection of properties of this structure.</param>
        protected override void PopulateProperties(DmiClassPropertiesTable properties)
        {
            object errorType = SmbiosStructure.GetPropertyValue(SmbiosProperty.BitMemoryError64.ErrorType);
            if (errorType != null)
            {
                properties.Add(DmiProperty.BitMemoryError64.ErrorType, errorType);
            }

            object errorGranularity = SmbiosStructure.GetPropertyValue(SmbiosProperty.BitMemoryError64.ErrorGranularity);
            if (errorGranularity != null)
            {
                properties.Add(DmiProperty.BitMemoryError64.ErrorGranularity, errorGranularity);
            }

            object errorOperation = SmbiosStructure.GetPropertyValue(SmbiosProperty.BitMemoryError64.ErrorOperation);
            if (errorOperation != null)
            {
                properties.Add(DmiProperty.BitMemoryError64.ErrorOperation, errorOperation);
            }

            object vendorSyndromeProperty = SmbiosStructure.GetPropertyValue(SmbiosProperty.BitMemoryError64.VendorSyndrome);
            if (vendorSyndromeProperty != null)
            {
                long vendorSyndrome = (int) vendorSyndromeProperty;
                if (vendorSyndrome != 0x00000000)
                {
                    properties.Add(DmiProperty.BitMemoryError64.VendorSyndrome, vendorSyndrome);
                }
            }

            object busErrorAddressProperty = SmbiosStructure.GetPropertyValue(SmbiosProperty.BitMemoryError64.BusErrorAddress);
            if (busErrorAddressProperty != null)
            {
                ulong busErrorAddress = (ulong) busErrorAddressProperty;
                if (busErrorAddress != 0x8000000000000000)
                {
                    properties.Add(DmiProperty.BitMemoryError64.BusErrorAddress, busErrorAddress);
                }
            }

            object deviceErrorAddressProperty = SmbiosStructure.GetPropertyValue(SmbiosProperty.BitMemoryError64.DeviceErrorAddress);
            if (deviceErrorAddressProperty != null)
            {
                ulong deviceErrorAddress = (ulong)deviceErrorAddressProperty;
                if (deviceErrorAddress != 0x8000000000000000)
                {
                    properties.Add(DmiProperty.BitMemoryError64.DeviceErrorAddress, deviceErrorAddress);
                }
            }

            object errorResolutionProperty = SmbiosStructure.GetPropertyValue(SmbiosProperty.BitMemoryError64.ErrorResolution);
            if (errorResolutionProperty != null)
            {
                ulong errorResolution = (ulong)errorResolutionProperty;
                if (errorResolution != 0x80000000)
                {
                    properties.Add(DmiProperty.BitMemoryError64.ErrorResolution, errorResolution);
                }
            }
        }
        #endregion

        #endregion
    }
}
