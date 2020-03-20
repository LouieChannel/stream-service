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
