using System;
using System.Collections.Generic;
using System.Globalization;

namespace Ascalon.StreamService.DumperService.Dtos
{
    public static class Extensions
    {
        public static DumperInfo ToDumperInfo(this string dumperData)
        {
            var separateData = dumperData.Split(',');

            return new DumperInfo()
            {
                Array = new List<float>() 
                {
                    float.Parse(separateData[0], new NumberFormatInfo()),
                    float.Parse(separateData[1], new NumberFormatInfo()),
                    float.Parse(separateData[2], new NumberFormatInfo()),
                    float.Parse(separateData[3].Replace("\r\n", "").TrimStart(), new NumberFormatInfo()),
                    float.Parse(separateData[4], new NumberFormatInfo()),
                    float.Parse(separateData[5], new NumberFormatInfo()),
                    float.Parse(separateData[6].Replace("\r\n", "").TrimStart(), new NumberFormatInfo())
                },
                Label = separateData[7],
                IpAddress = separateData[8]
            };
        }
    }
}
