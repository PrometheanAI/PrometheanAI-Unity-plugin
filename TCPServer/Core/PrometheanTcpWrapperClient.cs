using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using PrometheanAI.Modules.Core.Services;
using PrometheanAI.Modules.Utils;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer
{
    /// <summary>
    /// Class which is responsible for interaction with PrometheanAI via TCP ports
    /// </summary>
    public class PrometheanTcpWrapperClient
    {
        private const string k_prometheanAddress = "127.0.0.1";
        private const int k_prometheanUE4Port = 1316;
        private volatile NetworkStream m_networkStream;
        private Thread m_receivedSocketThread;
        private CommandsService m_commandsService;
        private Socket m_socket;
        private Socket m_client;

        public PrometheanTcpWrapperClient() {
            ConnectServer();
            m_commandsService = new CommandsService();
        }

        /// <summary> 	
        /// Setup socket connection	
        /// </summary> 	
        private void ConnectServer() {
            try {
                Debug.Log($"start socket");
                m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_socket.Bind(new IPEndPoint(IPAddress.Parse(k_prometheanAddress), k_prometheanUE4Port));
                m_socket.Listen(10);
                m_receivedSocketThread = new Thread(Listen) {
                    IsBackground = true
                };
                m_receivedSocketThread.Start();
            }
            catch (Exception e) {
                Debug.LogError($"Server connection is failed! Message: {e.Message}");
            }
        }

        /// <summary> 	
        /// Runs in background m_receivedSocketThread. Listens for incoming data. 	
        /// </summary>     
        private void Listen() {
            while (true) {
                try {
                    m_client = m_socket.Accept();
                    m_networkStream = new NetworkStream(m_client);
                    if (m_networkStream.CanRead) {
                        var receiveBuffer = new byte[1024];
                        var data = new StringBuilder();
                        do {
                            var length = m_networkStream.Read(receiveBuffer, 0, receiveBuffer.Length);
                            data.AppendFormat("{0}", Encoding.UTF8.GetString(receiveBuffer, 0, length));
                        } while (m_networkStream.DataAvailable);

                        Debug.Log("Received command " + data);
                        Parse(data.ToString());
                    }
                    else {
                        Debug.LogError("Can't read from the disposed networkStream.");
                    }
                }
                catch (Exception e) { // IOException or SocketException
                    Debug.Log("Exception message: " + e.Message);
                    Debug.Log("Exception stackTrace: " + e.StackTrace);
                }
            }
        }

        /// <summary>
        /// Used to parse incoming data into Promethean AI commands
        /// </summary>
        /// <param name="data"></param>
        private void Parse(string data) {
            var commandsList = data.Split(new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);

            //custom check for learn command 
            var isLearnCommand = false;
            foreach (var item in commandsList) {
                if (item.StartsWith("get_asset_data")) {
                    isLearnCommand = true;
                    break;
                }
            }

            foreach (var rawCommandData in commandsList) {
                CommandUtility.ParseCommandParameters(rawCommandData, out var commandToken, out var commandParametersString); //TODO commandParametersString can be empty string. Check before split in next line
                var commandParametersList = commandParametersString.Split(' ').ToList();                                      //TODO create analogue method for converting commands parameters into list -> c++ CommandParameterString.ParseIntoArray(CommandParameterList, TEXT(" "), true);
                m_commandsService.Parse(rawCommandData, commandToken, commandParametersList, (state, outData) => {
                    switch (state) {
                        case CommandHandleProcessState.Success:
                            if (!outData.Equals(string.Empty) && !isLearnCommand) {
                                Send(outData);
                            }

                            break;
                        case CommandHandleProcessState.SingleState:
                            if (!outData.Equals(string.Empty)) {
                                SendSingleMessage(outData);
                                Send("Done");
                            }

                            break;
                        case CommandHandleProcessState.Failed:
                            //Parse is failed.
                            //Doesn't close socket because can exists another handler
                            break;
                    }
                });
            }
        }

        /// <summary>
        /// Send data in single message to server using networkStream 	
        /// </summary>
        /// <param name="data"></param>
        private void SendSingleMessage(string data) {
            new PrometheanTcpAssetDataClient().Send(data);
        }

        /// <summary>
        /// Send data to server using networkStream 	
        /// </summary>
        /// <param name="data"></param>
        private void Send(string data) {
            try {
                Debug.Log("Sending data: " + data);
                if (m_networkStream.CanWrite) {
                    var writeBuffer = Encoding.UTF8.GetBytes(data);
                    m_networkStream.Write(writeBuffer, 0, writeBuffer.Length);
                    m_client.Close();
                }
                else {
                    Debug.LogError("Can't write to the disposed networkStream.");
                }
            }
            catch (Exception e) {
                Debug.Log("Exception message: " + e.Message);
                Debug.Log("Exception stackTrace: " + e.StackTrace);
            }
        }
    }
}