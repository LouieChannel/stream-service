using Ascalon.Kafka;
using Ascalon.StreamService.DumperService.Dtos;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Ascalon.StreamService.DumperService
{
    public class DumperService : IDumperService
    {
        public bool Stop { get; set; }

        private static Producer _producer;
        private Queue<string> _dataFromDumper = new Queue<string>();
        private Queue<DumperInfo> _dumerInfos = new Queue<DumperInfo>();

        private Thread getData;
        private Thread processData;
        private Thread sendData;

        public DumperService(Producer producer)
        {
            _producer = producer;
            
            getData = new Thread(new ThreadStart(GetDataFromDumper));
            processData = new Thread(new ThreadStart(ProcessDataFromDumper));
            sendData = new Thread(new ThreadStart(SendDataToDumperService));

            getData.Start();
            processData.Start();
            sendData.Start();
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
                    _dataFromDumper.Enqueue(Encoding.UTF8.GetString(data)+$",{remoteIp.Address}");
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

                await _producer.Produce(null, queueToSendData, "post_dumper_data");
            }
        }
    }
}
