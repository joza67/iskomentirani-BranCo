// Importing necessary namespaces for .NET functionalities
using System; // Provides basic types and functions, like DateTime
using System.Collections.Generic; // Provides generic collection types like List<T>
using System.Linq; // Provides LINQ functionality for querying collections
using System.Threading.Tasks; // Provides support for asynchronous programming

namespace LoRinoBackend.Models
{
    // Class representing received LoRa data from a device
    public class RecievedLoraData
    {
        // Unique identifier for the received data
        public string Id { get; set; }

        // Object representing the end device that sent the data
        public RecievedEndDevice EndDevice { get; set; }

        // Port number used for communication
        public int FPort { get; set; }

        // Frame counter for downlink messages
        public int FCntDown { get; set; }

        // Frame counter for uplink messages
        public int FCntUp { get; set; }

        // Adaptive Data Rate flag
        public bool Adr { get; set; }

        // Indicates whether the message is confirmed
        public bool Confirmed { get; set; }

        // Indicates whether the message is encrypted
        public bool Encrypted { get; set; }

        // Payload data in string format
        public string Payload { get; set; }

        // Time when the data was received (in Unix time format)
        public long RecvTime { get; set; }

        // Indicates whether the device is in Class B mode
        public bool ClassB { get; set; }

        // Indicates whether the message was delayed
        public bool Delayed { get; set; }

        // Uplink frequency used for communication
        public float UlFrequency { get; set; }

        // Modulation type used for communication
        public string Modulation { get; set; }

        // Data rate used for communication
        public string DataRate { get; set; }

        // Coding rate used for communication
        public string CodingRate { get; set; }

        // Number of gateways that received the message
        public int GwCnt { get; set; }

        // List of gateway information that received the message
        public List<RecievedGwInfo> GwInfo { get; set; }
    }

    // Class representing the end device from which the data was received
    public class RecievedEndDevice
    {
        // Unique identifier for the end device
        public int Id { get; set; }

        // Unique identifier for the device (Extended Unique Identifier)
        public string DevEui { get; set; }

        // Address of the device
        public string DevAddr { get; set; }

        // Object representing the cluster to which the device belongs
        public RecievedCluster Cluster { get; set; }
    }

    // Class representing information about the gateways that received the data
    public class RecievedGwInfo
    {
        // Unique identifier for the gateway info record
        public int Id { get; set; }

        // Unique identifier for the gateway (Extended Unique Identifier)
        public string GwEui { get; set; }

        // RF region where the gateway is located
        public string RfRegion { get; set; }

        // Received Signal Strength Indicator (RSSI) value
        public int Rssi { get; set; }

        // Signal-to-Noise Ratio (SNR) value
        public double Snr { get; set; }

        // Latitude where the gateway is located
        public double Latitude { get; set; }

        // Longitude where the gateway is located
        public double Longitude { get; set; }

        // Altitude where the gateway is located
        public int Altitude { get; set; }

        // Channel used by the gateway for communication
        public int Channel { get; set; }

        // Identifier for the radio used by the gateway
        public int RadioId { get; set; }
    }

    // Class representing a cluster to which the end device belongs
    public class RecievedCluster
    {
        // Unique identifier for the cluster
        public int Id { get; set; }
    }
}
