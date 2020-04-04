namespace Ascalon.StreamService.DumperService.Dtos
{
    public static class Extensions
    {
        public static DumperInfo ToDumperInfo(this string dumperData)
        {
            var separateData = dumperData.Split(',');

            return new DumperInfo()
            {
                Gfx = separateData[0],
                Gfy = separateData[1],
                Gfz = separateData[2],
                Wx = separateData[3],
                Wy = separateData[4],
                Wz = separateData[5],
                Speed = separateData[6],
                Label = separateData[7],
                IpAddress = separateData[8]
            };
        }
    }
}
