namespace Environmental_Monitor
{
    /// <summary>
    /// Represents the deserialized JSON
    /// </summary>
    public class UdpMessage
    {
        /// <summary>
        /// The temp in farenheit sensed by the pi
        /// </summary>
        public double temperatureF { get; set; }

        /// <summary>
        /// The temp in celsius sensed by the pi
        /// </summary>
        public double temperatureC { get; set; }

        /// <summary>
        /// The humidity percentage sensed by the pi
        /// </summary>
        public double humidity { get; set; }
    }
}
