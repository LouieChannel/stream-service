using System.Threading.Tasks;
using System.Collections.Generic;
using Ascalon.StreamService.DumperService.Dtos;

namespace Ascalon.StreamService.DumperService
{
    public interface IDumperService
    {
        bool Stop { get; set; }

        void GetDataFromDumper();

        void ProcessDataFromDumper();

        void SendDataToDumperService();
    }
}
