using ServerWinForm.Enums;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

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
                var messageLengthAsBytes = BitConverter.GetBytes((short)(1 + messageAsBytes.Length));
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

        public static async Task<byte[]> ReadMessageAsync(this NetworkStream stream, TcpClient client)
        {
            //CancellationTokenSource tokenSource = new CancellationTokenSource();
            //CancellationToken token = new CancellationToken();
            var messageLengthAsBytes = new byte[BYTES_FOR_MESSAGE_LENGTH];
            var readBytes = 0;
            //Task<int>? readMessageTask;
            //Task? completedTask;

            //while (readBytes != BYTES_FOR_MESSAGE_LENGTH)
            //{


            //    Debug.WriteLine($"Readed bytes = {readBytes}");
            //    readMessageTask = stream.ReadAsync(messageLengthAsBytes, readBytes, messageLengthAsBytes.Length, token);
            //    TimeSpan timeOut = TimeSpan.FromSeconds(7);
            //    var timeoutTask = Task.Delay(timeOut);
            //    completedTask = await Task.WhenAny(timeoutTask, readMessageTask);
            //    //readBytes += await stream.ReadAsync(messageLengthAsBytes, readBytes, messageLengthAsBytes.Length);
            //    if (completedTask == timeoutTask)
            //    {
            //        tokenSource.Cancel();
            //        try
            //        {
            //            stream.WriteByte(1);
            //        }
            //        catch (IOException)
            //        {
            //            throw new SocketException();
            //        }
            //        tokenSource.Dispose();
            //        tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            //        Debug.WriteLine("СНОВА ЖДУ СООБЩЕНИЯ");
            //        continue;
            //        //if (stream.Wr)
            //        //{
            //        //    tokenSource.Dispose();
            //        //    tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            //        //    Debug.WriteLine("СНОВА ЖДУ СООБЩЕНИЯ");
            //        //    continue;
            //        //}
            //        //else throw new SocketException();
            //    }
            //    else
            //    {
            //        Debug.WriteLine("УСПЕЛ ПРОЧИТАТЬ");
            //        readBytes += readMessageTask.Result;
            //        if (readBytes == 0)
            //        {
            //            Debug.WriteLine("НУ ПИЗДЕЦ");
            //            throw new SocketException();
            //        }
            //    }
            //}

            while (readBytes != BYTES_FOR_MESSAGE_LENGTH)
            {
                readBytes += await stream.ReadAsync(messageLengthAsBytes, readBytes, messageLengthAsBytes.Length);
                if (readBytes == 0)
                {
                    Debug.WriteLine("НУ ПИЗДЕЦ");
                    throw new SocketException();
                }
            }
            Debug.Write("\nMessage length (bytes) -> ");
            foreach (byte b in messageLengthAsBytes)
            {
                Debug.Write($"{b} ");
            }
            Debug.WriteLine("\n");
            var message = new byte[BitConverter.ToInt16(messageLengthAsBytes, 0)];
            Debug.WriteLine($"Message Length -> {message.Length}");
            readBytes = 0;
            try
            {
                while (readBytes != message.Length)
                {
                    readBytes += await stream.ReadAsync(message, readBytes, message.Length);
                    if (readBytes == 0)
                        throw new SocketException();
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
            while ((await stream.ReadAsync(buffer, 0, buffer.Length)) != 0) ;
        }
    }
}