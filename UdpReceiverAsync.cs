using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Environmental_Monitor
{
    /// <summary>
    /// Responsible for asynchronous operations in receiving data over UDP.
    /// </summary>
    public class UdpReceiverAsync
    {
        /// <summary>
        /// Represents the port the device communicates on.
        /// </summary>
        private int ListenPort = 11000;

        /// <summary>
        /// Signals cancellation of an async task, represented as a token source.
        /// Throws a TaskCanceledException (sends cancellation signal).
        /// </summary>
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Calls the token source to throw a TaskCanceledException.
        /// </summary>
        public void StopListening() { cancellationTokenSource.Cancel(); }

        /// <summary>
        /// An asynchronous method that receives information from a specified port using UDP.
        /// </summary>
        /// <param name="listenPort">Represents the port the device communicates on.</param>
        /// <returns>A Task that returns a deserialized <see cref="UdpMessage"/> or null on error/cancel.</returns>
        public async Task<UdpMessage?> ReceiveAsync(int listenPort = 11000)
        {
            Logger logger = new Logger("logs/udp.log");
            this.ListenPort = listenPort;

            // Create an endpoint with any IP address on the specified port
            var ipEndPoint = new IPEndPoint(IPAddress.Any, ListenPort);

            // Creates a UDP client using the IP endpoint and disposes it when finished
            using var udpClient = new UdpClient(ipEndPoint);
            logger.Info($"Listening for UDP packets on port {ListenPort}...");

            try
            {
                // Wait asynchronously for a single UDP packet (cancellable)
                var received = await udpClient.ReceiveAsync(cancellationTokenSource.Token);
                logger.Info($"Message received from {received.RemoteEndPoint}.");

                // UDP transmits bytes, not text, into a raw byte array as a buffer
                // Must be decoded using UTF-8 into a readable string
                string message = Encoding.UTF8.GetString(received.Buffer);
                logger.Info($"Message encoded using UTF-8.");

                // Reformats the invalid JSON to a valid format (temperatureF is missing quotation marks)
                string formattedMessage = message.Replace("temperatureF:", "\"temperatureF\":");

                // Deserialize to a UdpMessage; return null if deserialization fails
                UdpMessage? udpMessage = JsonSerializer.Deserialize<UdpMessage>(formattedMessage);
                if (udpMessage == null)
                {
                    logger.Error("Deserialized UDP message was null.");
                    return null;
                }

                logger.Info($"Message deserialized from JSON string.");

                // Accounts for "bad messages" from the pi
                if (udpMessage.temperatureF == 32 && udpMessage.temperatureC == 0 && udpMessage.humidity == 0)
                {
                    logger.Info($"Message received has default values; disregard.");
                }
                else
                {
                    logger.Info($"Temp F: {udpMessage.temperatureF}, Temp C: {udpMessage.temperatureC}, Humidity: {udpMessage.humidity}");
                }

                return udpMessage;
            }
            // Catches the more specific version of a TaskCanceledException
            catch (OperationCanceledException ex) { logger.Error($"UDP reception cancelled: {ex.Message}"); return null; }

            // SocketException is thrown whenever a network socket operation fails at the system or network level
            catch (SocketException ex) { logger.Error($"Socket error: {ex.Message}"); return null; }

            // JsonSerializer could return null
            catch (JsonException ex) { logger.Error($"JSON error: {ex.Message}"); return null; }

            // Generic exception catch
            catch (Exception ex) { logger.Error(ex.Message); return null; }
        }
    }
}