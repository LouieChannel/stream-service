using System;
using System.Net;
using System.Text;
using System.Net.Http;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Ascalon.StreamService.DumperService.Dtos;

namespace Ascalon.StreamService.DumperService
{
    public class DumperService : IDumperService
    {
        public bool Stop { get; set; }
        private static HttpClient _httpClient;
        private static DumperServiceConfig _dumperServiceConfig;
        private Queue<string> _dataFromDumper = new Queue<string>();
        private Queue<DumperInfo> _dumerInfos = new Queue<DumperInfo>();

        public DumperService(IHttpClientFactory clientFactory, IOptionsMonitor<DumperServiceConfig> optionsMonitor)
        {
            _dumperServiceConfig = optionsMonitor.CurrentValue;
            _httpClient = clientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(_dumperServiceConfig.Host);
        }

        public void GetDataFromDumper()
        {

            UdpClient receiver = new UdpClient(5005);
            IPEndPoint remoteIp = null;

            try
            {
                while (true)
                {
                    if (Stop)
                        break;

                    byte[] data = receiver.Receive(ref remoteIp);
                    _dataFromDumper.Enqueue(Encoding.UTF8.GetString(data));
                }
            }
            finally
            {
                receiver.Close();
            }
        }

        public void ProcessDataFromDumper()
        {
            while (true)
            {
                if (Stop)
                    break;

                _dataFromDumper.TryDequeue(out string result);

                if (string.IsNullOrEmpty(result))
                    continue;

                _dumerInfos.Enqueue(result.ToDumperInfo());
            }
        }

        public async void SendDataToDumperService()
        {
            Queue<DumperInfo> queueToSendData;

            while (true)
            {
                if (Stop)
                    break;

                queueToSendData = new Queue<DumperInfo>();

                while (true)
                {
                    _dumerInfos.TryDequeue(out DumperInfo dumperInfo);

                    if (dumperInfo == null)
                        continue;

                    queueToSendData.Enqueue(dumperInfo);

                    if (queueToSendData.Count == 10)
                        break;
                }

                await _httpClient.PostAsync($"dumper/", new StringContent(JsonConvert.SerializeObject(queueToSendData, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), System.Text.Encoding.UTF8, "application/json"));
            }
        }
    }
}
