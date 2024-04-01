using ServerWinForm.Data;
using ServerWinForm.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServerWinForm.Extensions
{
    public static class NetworkStreamExtension
    {
        private const int BYTES_FOR_MESSAGE_LENGTH = 4;
        private const int MAX_MESSAGE_SIZE = 104_857_600; // 100 MB

        public static async Task<bool> WriteMessageAsync(this NetworkStream stream, MessageCode code, string? message)
        {
            if (message != null)
            {
                var messageAsBytes = Encoding.UTF8.GetBytes(message);
                var buffer = new byte[1 + BYTES_FOR_MESSAGE_LENGTH + messageAsBytes.Length];
                var messageLengthBytes = BitConverter.GetBytes(messageAsBytes.Length);
                if (messageAsBytes.Length > MAX_MESSAGE_SIZE)
                {
                    return false;
                }
                buffer[0] = (byte)code;
                messageLengthBytes.CopyTo(buffer, 1);
                messageAsBytes.CopyTo(buffer, 5);
                await stream.WriteAsync(buffer, 0, buffer.Length);
                await stream.FlushAsync();
                Debug.WriteLine($"Sent package -> code: {code}, length: {message.Length}, message: {message}");
                return true;
            }
            else
            {
                var buffer = new byte[1] { (byte) code };
                await stream.WriteAsync(buffer, 0, buffer.Length);
                await stream.FlushAsync();
                Debug.WriteLine($"Sent package -> code: {code}");
                return true;
            }
        }

        public static async Task<byte[]> ReadMessageAsync(this NetworkStream stream)
        {
            var messageLengthBytes = new byte[BYTES_FOR_MESSAGE_LENGTH];
            var readBytes = 0;
            while (readBytes != 4)
            {
                readBytes += await stream.ReadAsync(messageLengthBytes, readBytes, messageLengthBytes.Length);
            }
            var message = new byte[BitConverter.ToInt32(messageLengthBytes, 0)];
            readBytes = 0;
            try
            {
                while (readBytes != message.Length)
                {
                    readBytes += await stream.ReadAsync(message, readBytes, message.Length);
                }
                Debug.WriteLine($"Accepted package -> length: {message.Length}, message: {Encoding.UTF8.GetString(message)}");
                return message;
            } 
            catch (ArgumentOutOfRangeException)
            {
                throw new FormatException();
            }
        }

        public static async Task<MessageCode> ReadCodeAsync(this NetworkStream stream)
        {
            var messageCode = new byte[1];
            await stream.ReadAsync(messageCode, 0, messageCode.Length);
            Debug.WriteLine($"Accepted code: {(MessageCode) messageCode[0]}");
            return (MessageCode) messageCode[0];
        }



    }
}
