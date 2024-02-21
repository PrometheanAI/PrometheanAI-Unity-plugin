using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace PrometheanAI.Modules.TCPServer
{
    public class PrometheanTcpAssetDataClient
    {
        private const string k_prometheanAddress = "127.0.0.1";
        private const int k_prometheanAssetDataPort = 1315;
        private Socket m_socket;

        public PrometheanTcpAssetDataClient() => ConnectServer(); 

        /// <summary> 	
        /// Setup socket connection	
        /// </summary> 	
        private void ConnectServer() {
            Debug.Log("Connect to an additional socket!");
            var endPoint = new IPEndPoint(IPAddress.Parse(k_prometheanAddress), k_prometheanAssetDataPort);
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_socket.Connect(endPoint);
        }

        /// <summary>
        /// Send data to an additional socket
        /// </summary>
        /// <param name="data"></param>
        public void Send(string data) {
            try {
                m_socket.Send(Encoding.UTF8.GetBytes(data));
                m_socket.Close();
            }
            catch (Exception e) {
                Debug.LogWarning($"Exception: {e}");
            }
        }
    }
}