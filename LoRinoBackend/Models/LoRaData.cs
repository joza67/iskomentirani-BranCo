using System.Collections.Generic;

namespace LoRinoBackend.Models
{
    // Class representing LoRaWAN data received from end devices
    public class LoRaData
    {
        // Unique identifier for the LoRa data entry
        public int Id { get; set; }

        // Message ID for the LoRaWAN message, can be null
        public string? MsgId { get; set; }

        // Information about the end device that sent the message
        public EndDevice EndDeviceData { get; set; }

        // FPort is the port number used in the LoRaWAN message
        public int FPort { get; set; }

        // FCntDown is the frame counter for downlink messages, can be null
        public int? FCntDown { get; set; }

        // FCntUp is the frame counter for uplink messages, can be null
        public int? FCntUp { get; set; }

        // ADR (Adaptive Data Rate) flag, indicating if ADR is enabled
        public bool Adr { get; set; }

        // Indicates if the message was confirmed, can be null
        public bool? Confirmed { get; set; }

        // Indicates if the message was encrypted, can be null
        public bool? Encrypted { get; set; }

        // Payload of the LoRaWAN message
        public string Payload { get; set; }

        // Timestamp indicating when the message was received
        public long RecvTime { get; set; }

        // ClassB flag, indicating if the message is related to Class B operation, can be null
        public bool? ClassB { get; set; }

        // Delayed flag, indicating if the message was delayed, can be null
        public bool? Delayed { get; set; }

        // Uplink frequency in Hz, can be null
        public float? UlFrequency { get; set; }

        // Modulation type used in the LoRaWAN message, can be null
        public string? Modulation { get; set; }

        // Data rate used in the LoRaWAN message, can be null
        public string? DataRate { get; set; }

        // Coding rate used in the LoRaWAN message, can be null
        public string? CodingRate { get; set; }

        // Number of gateways that received the message, can be null
        public int? GwCnt { get; set; }

        // List of gateway information entries related to this LoRaWAN message
        public List<GwInfo> GwInfoData { get; set; }
    }

    // Class representing the end device that sent the LoRaWAN message
    public class EndDevice
    {
        // Unique identifier for the end device
        public int Id { get; set; }

        // Unique identifier for the end device, used in LoRaWAN communication
        public string DevEui { get; set; }

        // Device address, can be null
        public string? DevAddr { get; set; }

        // Cluster information associated with the end device, can be null
        public Cluster? ClusterData { get; set; }
    }

    // Class representing gateway information related to a LoRaWAN message
    public class GwInfo
    {
        // Unique identifier for the gateway information entry
        public int Id { get; set; }

        // Unique identifier for the gateway
        public string GwEui { get; set; }

        // Radio frequency region of the gateway, can be null
        public string? RfRegion { get; set; }

        // Signal strength of the received message at the gateway, can be null
        public int? Rssi { get; set; }

        // Signal-to-noise ratio of the received message at the gateway, can be null
        public double? Snr { get; set; }

        // Latitude of the gateway, can be null
        public double? Latitude { get; set; }

        // Longitude of the gateway, can be null
        public double? Longitude { get; set; }

        // Altitude of the gateway, can be null
        public int? Altitude { get; set; }

        // Channel number used by the gateway, can be null
        public int? Channel { get; set; }

        // Radio identifier used by the gateway, can be null
        public int? RadioId { get; set; }
    }

    // Class representing cluster information related to an end device
    public class Cluster
    {
        // Unique identifier for the cluster, can be null
        public int? Id { get; set; }

        // Cluster ID, can be null
        public int? ClusterId { get; set; }
    }
}
