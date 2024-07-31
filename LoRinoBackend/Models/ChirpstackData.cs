// Define the namespace for the models
namespace LoRinoBackend.Models
{
    // Class representing the Chirpstack data model
    public class ChirpstackData
    {
        public string? deduplicationId { get; set; } // Unique ID for deduplication
        public string? time { get; set; } // Time of the event
        public DeviceInfo? deviceInfo { get; set; } // Information about the device
        public string? devAddr { get; set; } // Device address
        public bool adr { get; set; } // Adaptive data rate flag
        public int? dr { get; set; } // Data rate
        public int? fCnt { get; set; } // Frame counter
        public int fPort { get; set; } // Frame port
        public bool? confirmed { get; set; } // Confirmation flag
        public string? data { get; set; } // Data payload
        public objectData? @object { get; set; } // Object data containing sensor information
        public RxInfo[]? rxInfo { get; set; } // Array of received information
        public TxInfo? txInfo { get; set; } // Transmission information
    }

    // Class representing device information
    public class DeviceInfo
    {
        public string? tenantId { get; set; } // Tenant ID
        public string? tenantName { get; set; } // Tenant name
        public string? applicationId { get; set; } // Application ID
        public string? applicationName { get; set; } // Application name
        public string? deviceProfileId { get; set; } // Device profile ID
        public string? deviceProfileName { get; set; } // Device profile name
        public string? deviceName { get; set; } // Device name
        public string? devEui { get; set; } // Device EUI (unique identifier)
        public string? deviceClassEnabled { get; set; } // Enabled device class
        public object? tags { get; set; } // Additional tags associated with the device
    }

    // Class representing object data (sensor data)
    public class objectData
    {
        public object gy { get; set; } // Gyroscope Y-axis data
        public double temperature { get; set; } // Temperature value
        public string recvTime { get; set; } // Time when data was received
        public double dataType { get; set; } // Type of data
        public object gx { get; set; } // Gyroscope X-axis data
        public bool ackMsg { get; set; } // Acknowledgment message flag
        public double battery { get; set; } // Battery level
        public object gz { get; set; } // Gyroscope Z-axis data
    }

    // Class representing received information
    public class RxInfo
    {
        public string? gatewayId { get; set; } // Gateway ID
        public int? uplinkId { get; set; } // Uplink ID
        public string? gwTime { get; set; } // Gateway time
        public string? nsTime { get; set; } // Network server time
        public int? rssi { get; set; } // Received signal strength indicator
        public double? snr { get; set; } // Signal-to-noise ratio
        public int? channel { get; set; } // Channel number
        public ChirpstackLocation? location { get; set; } // Location information
        public string? context { get; set; } // Context information
        public Metadata? metadata { get; set; } // Additional metadata
        public string? crcStatus { get; set; } // CRC status
    }

    // Class representing location information
    public class ChirpstackLocation
    {
        public double? latitude { get; set; } // Latitude coordinate
        public double? longitude { get; set; } // Longitude coordinate
    }

    // Class representing metadata information
    public class Metadata
    {
        public string? regionConfigId { get; set; } // Region configuration ID
        public string? regionCommonName { get; set; } // Common name of the region
    }

    // Class representing transmission information
    public class TxInfo
    {
        public int? frequency { get; set; } // Frequency of the transmission
        public Modulation? modulation { get; set; } // Modulation information
    }

    // Class representing modulation information
    public class Modulation
    {
        public Lora? lora { get; set; } // LoRa modulation details
    }

    // Class representing LoRa modulation details
    public class Lora
    {
        public int? bandwidth { get; set; } // Bandwidth of the signal
        public int? spreadingFactor { get; set; } // Spreading factor
        public string? codeRate { get; set; } // Coding rate
    }
}
