using System; // Importing base class library, including fundamental classes and base classes that define commonly-used value and reference data types

namespace LoRinoBackend.Functions
{
    ///<summary>
    /// Class for converting Unix time (also known as Epoch time, POSIX time, seconds since epoch, or UNIX Epoch time).
    /// A system for describing the number of milliseconds that have elapsed since the Unix Epoch,
    /// excluding leap seconds. The Unix epoch is 00:00:00 UTC on 1 January 1970 (an arbitrary date).
    ///</summary>
    public static class Unix
    {
        ///<summary>
        /// Function to convert UnixTimeStamp to DateTime.
        /// Takes one parameter of type long, representing the time stamp.
        /// Returns the described time in DateTime format.
        ///</summary>
        public static DateTime ToDateTime(this long unixTimeStamp)
        {
            // Unix timestamp is (milli)seconds past epoch
            // Initialize DateTime to Unix epoch (1 January 1970, 00:00:00 UTC)
            //System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            // Add milliseconds from unixTimeStamp and convert to local time
            //dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            //return dtDateTime;

            // Directly return the calculated DateTime
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                   .AddMilliseconds(unixTimeStamp).ToLocalTime();
        }

        ///<summary>
        /// Function to convert STRING UnixTimeStamp to DateTime.
        /// Takes one parameter of type long, representing the time stamp.
        /// Returns the described time in DateTime format.
        ///</summary>
        public static DateTime ToDateTime(this long unixTimeStamp, bool milis = true)
        {
            // Unix timestamp is (milli)seconds past epoch
            // Initialize DateTime to Unix epoch (1 January 1970, 00:00:00 UTC)
            //System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            // Add milliseconds from unixTimeStamp and convert to local time
            //dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            //return dtDateTime;

            if (milis) // If milis is true, convert using milliseconds
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local)
                       .AddMilliseconds(unixTimeStamp).ToLocalTime();
            }
            else // Otherwise, convert using seconds
            {
                return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local)
                       .AddSeconds(unixTimeStamp).ToLocalTime();
            }
        }

        ///<summary>
        /// Function to convert DateTime to UnixTimeStamp.
        /// Takes one parameter DateTime, representing the time.
        /// Returns the described time in UnixTimeStamp format, as a variable of type long.
        ///</summary>
        public static long ToUnixTimeStamp(this DateTime dateTime)
        {
            // Calculate the Unix timestamp as the total milliseconds since the Unix epoch
            //long unixTimeStamp = (long)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            //return unixTimeStamp;

            // Directly return the calculated Unix timestamp
            return (long)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
        }
    }
}
