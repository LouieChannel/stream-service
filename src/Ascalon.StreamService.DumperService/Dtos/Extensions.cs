using System;
using System.Collections.Generic;
using System.Text;

namespace Ascalon.StreamService.DumperService.Dtos
{
    public static class Extensions
    {
        public static DumperInfo ToDumperInfo(this string dumperData)
        {
            var separateData = dumperData.Split(',');

            return new DumperInfo()
            {
                Id = separateData[0],
                Gfx = separateData[1],
                Gfy = separateData[2],
                Gfz = separateData[3],
                Wx = separateData[4],
                Wy = separateData[5],
                Wz = separateData[6],
                Speed = separateData[7],
                Label = separateData[8]
            };
        }
    }
}
