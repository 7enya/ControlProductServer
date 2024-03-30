using ServerWinForm.Data;
using ServerWinForm.Enums;
using System;
using System.Collections.Generic;
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

        private static async Task<bool> WriteWithCodeAsync(this NetworkStream stream, MessageCode code, string? message)
        {
            if (message != null)
            {
                var messageAsBytes = Encoding.UTF8.GetBytes(message);
                var buffer = new byte[1 + BYTES_FOR_MESSAGE_LENGTH + message.Length];
                var messageLengthBytes = BitConverter.GetBytes(message.Length);
                if (messageLengthBytes.Length > BYTES_FOR_MESSAGE_LENGTH)
                {
                    return false;
                }
                buffer[0] = (byte)code;
                messageLengthBytes.CopyTo(buffer, BYTES_FOR_MESSAGE_LENGTH - messageLengthBytes.Length + 1);
                messageAsBytes.CopyTo(buffer, 5);
                await stream.WriteAsync(buffer, 0, buffer.Length);
                await stream.FlushAsync();
                return true;
            }
            else
            {
                var buffer = new byte[1] { (byte) code };
                await stream.WriteAsync(buffer, 0, buffer.Length);
                return true;
            }
        }

        private static async Task<byte[]> ReadWithCodeAsync(this NetworkStream stream, bool onlyCode)
        {
            var messageCode = new byte[1];
            await stream.ReadAsync(messageCode, 0, messageCode.Length);
            if (onlyCode)
            {
                return messageCode;
            }
            var messageLengthBytes = new byte[BYTES_FOR_MESSAGE_LENGTH];
            var readBytes = 0;
            while (readBytes != 4)
            {
                readBytes += await stream.ReadAsync(messageLengthBytes, readBytes, messageLengthBytes.Length);
            }
            var message = new byte[BitConverter.ToInt32(messageLengthBytes, 0)];
            readBytes = 0;
            while (readBytes != message.Length)
            {
                readBytes += await device.TcpClient.GetStream().ReadAsync(message, readBytes, message.Length);
            }
            return message;
        }


    }
}
