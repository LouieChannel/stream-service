namespace Ascalon.StreamService.DumperService
{
    public interface IDumperService
    {
        void InitShift(int driverId);

        void EndShift(int driverId);
    }
}
