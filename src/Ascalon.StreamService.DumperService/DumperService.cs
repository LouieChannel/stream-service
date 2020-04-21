using Ascalon.Kafka;
using Ascalon.StreamService.DumperService.Dtos;
using Ascalon.StreamService.DumperService.Properties;
using LumenWorks.Framework.IO.Csv;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Ascalon.StreamService.DumperService
{
    public class DumperService : IDumperService
    {
        public bool Stop { get; set; }

        private static Producer _producer;
        private Queue<string> _dataFromDumper = new Queue<string>();
        private Queue<DumperInfo> _dumerInfos = new Queue<DumperInfo>();

        private static ConcurrentDictionary<int, CancellationTokenSource> _driversCanncelainToken = new ConcurrentDictionary<int, CancellationTokenSource>();

        private static DataTable _csvTable = null;
        private static object _locker = new object();

        public DumperService(Producer producer)
        {
            LoadDataToDataTable();

            _producer = producer;
        }

        private void LoadDataToDataTable()
        {
            lock (_locker)
            {
                if (_csvTable != null)
                    return;

                using var csvReader = new CsvReader(new StringReader(Resources.Terra_D1_multi_labeled_interpolated), true);
                _csvTable = new DataTable();
                _csvTable.Load(csvReader);
            }
        }

        public void InitShift(int driverId)
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;

            if (_driversCanncelainToken.ContainsKey(driverId))
                return;

            GetDataFromDataTable(driverId, token);

            ProcessDataFromDumper(token);

            SendDataToDumperService(token);

            _driversCanncelainToken.TryAdd(driverId, cancelTokenSource);
        }

        public void EndShift(int driverId)
        {
            _driversCanncelainToken.TryGetValue(driverId, out CancellationTokenSource cancellationTokenSource);

            if (cancellationTokenSource == null)
                return;

            cancellationTokenSource.Cancel();

            _driversCanncelainToken.TryRemove(driverId, out CancellationTokenSource removed);
        }

        private void GetDataFromDataTable(int driverId, CancellationToken cancellationToken)
        {
            new Task(() =>
            {
                foreach (DataRow data in _csvTable.Rows)
                {
                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return;

                        _dataFromDumper.Enqueue(@$"{data.ItemArray[1]},{data.ItemArray[2]},{data.ItemArray[3]},
                    {data.ItemArray[4]},{data.ItemArray[5]},{data.ItemArray[6]},
                    {data.ItemArray[7]},{data.ItemArray[8]},{driverId.ToString()}");
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }).Start();
        }

        private void ProcessDataFromDumper(CancellationToken cancellationToken)
        {
            new Task(() =>
            {
                while (true)
                {
                    try
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return;

                        if (Stop)
                            break;

                        _dataFromDumper.TryDequeue(out string result);

                        if (string.IsNullOrEmpty(result))
                            continue;

                        _dumerInfos.Enqueue(result.ToDumperInfo());
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }).Start();
        }

        private void SendDataToDumperService(CancellationToken cancellationToken)
        {
            new Task(async () =>
            {
                Queue<DumperInfo> queueToSendData;

                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

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
            }).Start();
        }
    }
}
