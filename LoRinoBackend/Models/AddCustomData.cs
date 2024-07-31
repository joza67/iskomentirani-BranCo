// Import necessary namespaces
using LoRinoBackend.Functions; // Import functions from the LoRinoBackend.Functions namespace
using Newtonsoft.Json.Linq; // Import JSON handling capabilities from Newtonsoft.Json.Linq
using System; // Import the System namespace for basic functionalities
using System.Collections.Generic; // Import collections like List
using System.IO; // Import I/O functionalities for file handling
using System.Linq; // Import LINQ functionalities for data querying
using System.Net; // Import networking functionalities
using System.Net.Mail; // Import functionalities for sending email
using System.Threading.Tasks; // Import asynchronous programming functionalities

namespace LoRinoBackend.Models
{
    // Class responsible for adding custom data
    public class AddCustomData
    {
        // Private fields for repository dependencies
        private readonly IMoveeDataRepository _moveeDataRepository;
        private readonly IMoveeEventRepository _moveeEventRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ILocationRepository _locationRepository;

        // Constructor to inject dependencies
        public AddCustomData(
                            IMoveeDataRepository moveeDataRepository,
                            IMoveeEventRepository moveeEventRepository,
                            IDeviceRepository deviceRepository,
                            ILocationRepository locationRepository
                            )
        {
            // Initialize repository dependencies
            _moveeDataRepository = moveeDataRepository;
            _moveeEventRepository = moveeEventRepository;
            _deviceRepository = deviceRepository;
            _locationRepository = locationRepository;
        }

        // Asynchronous method to decode device data
        public async Task<DB_Info> decodeDevice(string devEui, int fPort, long recvTime, string payload)
        {
            // Initialize default DB_Info object with default values
            DB_Info customData = new DB_Info
            {
                ReturnId = 0,
                _DevEui = "0000000000000000",
                _DataType = 0,
                _EmailNotify = false,
                _TimerStarted = 0,
                _CompanyId = 0,
                _LocationId = 0
            };

            // Retrieve all data from the device repository
            var Movee = _deviceRepository.GetAllData();

            // Return default customData if devEui is a default value
            if (devEui == "0000000000000000")
                return customData;

            // Check if the device exists in the repository
            if (Movee.Where(d => d.DevEui.Contains(devEui)).Any())
            {
                // If exists, decode Movee data
                return await DecodeMoveeAsync(devEui, fPort, recvTime, payload);
            }

            // Return default customData if no matching device is found
            return customData;
        }

        // Private asynchronous method to decode Movee data
        private async Task<DB_Info> DecodeMoveeAsync(string devEui, int _fPort, long recvTime, string payload)
        {
            // Initialize DB_Info object with provided values
            DB_Info dB_Info = new DB_Info
            {
                ReturnId = 0,
                _DevEui = devEui,
                _DataType = 0,
                _EmailNotify = false,
                _TimerStarted = 0,
                _CompanyId = 0,
                _LocationId = 0
            };

            // Variables to store decoded data
            int _ret = 0;
            double temperature;
            double battery;
            int dataType;
            int gx = 0;
            int gy = 0;
            int gz = 0;

            // Convert payload to byte array
            byte[] rawData = payload.HexStringToByte();

            // Decode battery and temperature values from payload
            battery = ((3.6 - 2.8) / 255 * rawData[0] + 2.8);
            temperature = rawData[1];
            dataType = rawData[2];

            // Decode accelerometer data if dataType is 4 (shock event)
            if (dataType == 4)
            {
                gx = rawData[3] * 256 + rawData[4];
                gx = (gx < 16000) ? gx : 65536 - gx;

                gy = rawData[5] * 256 + rawData[6];
                gy = (gy < 16000) ? gy : 65536 - gy;

                gz = rawData[7] * 256 + rawData[8];
                gz = (gz < 16000) ? gz : 65536 - gz;
            }

            // Get device information from repository
            var device = _deviceRepository.GetData(devEui);
            // Get all locations from repository
            var locations = _locationRepository.GetAllLocations();

            // Initialize list of users
            List<string> users = new List<string>();

            // Update device's DevEui
            device.DevEui = devEui;

            // Create a new MoveeDataFrame object with the decoded data
            MoveeDataFrame moveeDataFrame = new()
            {
                Device = device,
                RecvTime = recvTime,
                Temperature = temperature,
                Battery = battery,
                DataType = dataType,
                Gx = gx,
                Gy = gy,
                Gz = gz,
                AckMsg = (dataType == 1 ? true : false)
            };

            // Handle specific data types and update database
            if (dataType == 1 || dataType == 2 || dataType == 4)
            {
                moveeDataFrame.MoveeEventFrameId = 1;
                var _retObj = _moveeDataRepository.Add(moveeDataFrame);
                _ret = _retObj.Id;
                dB_Info.ReturnId = _retObj.Id;
            }

            // Set email notification flag for shock events
            if (dataType == 4)
            {
                dB_Info._EmailNotify = true;
            }

            // Update DB_Info object with device and location information
            dB_Info._LocationId = device.Location.Id;
            dB_Info._LocationName = device.Location.Name;
            dB_Info._CompanyId = device.Company.Id;
            dB_Info._DataType = dataType;
            dB_Info._Locations = locations;
            dB_Info._MailUsers = await _locationRepository.GetAllMailUsersFromLocationId(device.Location.Id);

            // Return the updated DB_Info object
            return dB_Info;
        }

        // Private method to generate email body from a template
        private string mailBody(string uName, string devEui)
        {
            string body = string.Empty;
            // Reading the HTML template for email

            using (StreamReader reader = new StreamReader("./wwwroot/mailTemplateAlarm.html"))
            {
                body = reader.ReadToEnd(); // Read the entire template file
            }

            // Replace placeholders with actual values
            body = body.Replace("{UserName}", uName);
            body = body.Replace("{device}", devEui);

            return body; // Return the final email body
        }
    }
}
