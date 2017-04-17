using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace JobSearch.Extensions
{
    public static class GeopointExtensions
    {
        private const double Circle = 2*Math.PI;
        private const double DegreesToRadian = Math.PI/180.0;
        private const double RadianToDegrees = 180.0/Math.PI;
        private const double EarthRadius = 6378137.0;

        public static IList<BasicGeoposition> GetCirclePoints(this Geopoint center,
            int radius, int nrOfPoints = 50)
        {
            var locations = new List<BasicGeoposition>();
            double latA = center.Position.Latitude*DegreesToRadian;
            double lonA = center.Position.Longitude*DegreesToRadian;
            double angularDistance = radius/EarthRadius;

            double sinLatA = Math.Sin(latA);
            double cosLatA = Math.Cos(latA);
            double sinDistance = Math.Sin(angularDistance);
            double cosDistance = Math.Cos(angularDistance);
            double sinLatAtimeCosDistance = sinLatA*cosDistance;
            double cosLatAtimeSinDistance = cosLatA*sinDistance;

            double step = Circle/nrOfPoints;
            for (double angle = 0; angle < Circle; angle += step)
            {
                var lat = Math.Asin(sinLatAtimeCosDistance + cosLatAtimeSinDistance*
                                    Math.Cos(angle));
                var dlon = Math.Atan2(Math.Sin(angle)*cosLatAtimeSinDistance,
                    cosDistance - sinLatA*Math.Sin(lat));
                var lon = ((lonA + dlon + Math.PI)%Circle) - Math.PI;

                locations.Add(new BasicGeoposition
                {
                    Latitude = lat*RadianToDegrees,
                    Longitude = lon*RadianToDegrees
                });
            }
            return locations;
        }
    }
}
