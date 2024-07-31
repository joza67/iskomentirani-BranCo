namespace LoRinoBackend.Logging
{
    public class LoRinoBackendFileLoggerOptions // Class to define configuration options for the file logger
    {
        public virtual string FilePath { get; set; } // Property to specify the file path for the log file
        public virtual string FolderPath { get; set; } // Property to specify the folder path where the log file will be stored
    }
}
