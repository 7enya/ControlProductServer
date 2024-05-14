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
        private static int BYTES_FOR_MESSAGE_LENGTH = 2;

        // максимальное количество однобайтных символов в кодироке UTF-8, которые вмещаются в BYTES_FOR_MESSAGE_LENGTH 
        private static int MAX_MESSAGE_SYMB_SIZE = ((int)Math.Pow(2, BYTES_FOR_MESSAGE_LENGTH * 8)) - 2;

        public static async Task<bool> WriteMessageAsync(this NetworkStream stream, MessageCode code, string? message)
        {
            byte[] buffer; 
            if (message != null)
            {
                if (message.Length > MAX_MESSAGE_SYMB_SIZE)
                {
                    return false;
                }
                var messageAsBytes = Encoding.UTF8.GetBytes(message);
                if (messageAsBytes.Length > MAX_MESSAGE_SYMB_SIZE)
                {
                    return false;
                }
                buffer = new byte[BYTES_FOR_MESSAGE_LENGTH + 1 + messageAsBytes.Length];
                var messageLengthAsBytes = BitConverter.GetBytes( (short)(1 + messageAsBytes.Length));
                messageLengthAsBytes.CopyTo(buffer, 0);
                buffer[BYTES_FOR_MESSAGE_LENGTH] = (byte)code;
                messageAsBytes.CopyTo(buffer, BYTES_FOR_MESSAGE_LENGTH + 1);
                await stream.WriteAsync(buffer, 0, buffer.Length);
                await stream.FlushAsync();
                Debug.WriteLine($"Sent package -> length: {messageAsBytes.Length + 1} bytes, code: {code}, message: {message}");
                return true;
            }
            else
            {
                var messageLengthAsBytes = BitConverter.GetBytes((short)1);
                buffer = new byte[messageLengthAsBytes.Length + 1];
                messageLengthAsBytes.CopyTo(buffer, 0);
                buffer[BYTES_FOR_MESSAGE_LENGTH] = (byte)code;
                await stream.WriteAsync(buffer, 0, buffer.Length);
                await stream.FlushAsync();
                Debug.WriteLine($"Sent package -> code: {code}");
                return true;
            }
        }

        public static async Task<byte[]> ReadMessageAsync(this NetworkStream stream)
        {
            var messageLengthAsBytes = new byte[BYTES_FOR_MESSAGE_LENGTH];
            var readBytes = 0;
            while (readBytes != BYTES_FOR_MESSAGE_LENGTH)
            {
                readBytes += await stream.ReadAsync(messageLengthAsBytes, readBytes, messageLengthAsBytes.Length);
            }
            var message = new byte[BitConverter.ToInt16(messageLengthAsBytes, 0)];
            readBytes = 0;
            try
            {
                while (readBytes != message.Length)
                {
                    readBytes += await stream.ReadAsync(message, readBytes, message.Length);
                }
                if (message.Length == 1)
                    Debug.WriteLine($"Accepted package -> length: {message.Length}, code: {(MessageCode)message[0]}");
                else
                    Debug.WriteLine($"Accepted package -> length: {message.Length}, code: {(MessageCode)message[0]}, message: {Encoding.UTF8.GetString(message, 1, message.Length - 1)}");
                return message;
            } 
            catch (ArgumentOutOfRangeException)
            {
                throw new FormatException();
            }
        }

        public static async Task ClearStreamAsync(this NetworkStream stream)
        {
            byte[] buffer = new byte[100];
            while ((await stream.ReadAsync(buffer, 0, buffer.Length)) != 0);
        }
    }
}
