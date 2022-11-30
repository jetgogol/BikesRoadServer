using Newtonsoft.Json;
using System;
using System.IO;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace BikesRoadServer
{
    public class Road
    {
        public static Road FromJson(string Json)
        {
            return JsonConvert.DeserializeObject<Road>(Json);
        }
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        //В ходе работы опустил идентификаторы с long до int
        public static int LastID { get; private set; } = 0;
        public string PhotosFolder => $"Storge/{StringId}";
        public Road(string description, MapPoint[] mapPoints)
        {
            Id = ++LastID;
            Directory.CreateDirectory(PhotosFolder);
            Description = description;
            MapPoints = mapPoints;
        }
        public Road()
        {
        }

        //Finding info
        [JsonIgnore]
        public int Id { get; set; }
        public string StringId => GetHashString(Id);

        //Info
        //TODO: Add map info
        //TODO: Add users info profile somehow...
        public string Description { get; set; }

        [JsonIgnore]
        public string MapPointsJson { get; set; } = "[{\"PosX\":0.0,\"PosY\":0.0}]";
        [NotMapped]
        [JsonIgnore]
        public MapPoint[] MapPoints
        {
            get
            {
                return JsonConvert.DeserializeObject<MapPoint[]>(MapPointsJson);
            }
            set
            {
                string json = JsonConvert.SerializeObject(value);
                MapPointsJson = json;
            }
        }
        //TODO: Поденлил через jsonignore методы запроса и методы в
        //бд, но возможно это повлияет на хохранение, надо смотреть
        public int TotalPoints => MapPoints.Length;

        //User Progress
        public int CompletedPoints { get; set; }
        public int PhotosCount => Directory.GetFiles(PhotosFolder).Count();
        /*
        public string[] Photos => GetPhotos();
        string[] GetPhotos() //TODO: somewhat it returns null
        {
            string[] photos = Directory.GetFiles(PhotosFolder);
            if (photos == null)
            {
                photos = new string[0];
            }

            return photos;
        }
        */

        //Directory.GetFiles(PhotosFolder);
        //public float Progress => TotalPoints / CompletedPoints;

        //Compute SHA256 string
        string GetHashString(long Id)
        {
            string output = string.Empty;
            using (SHA256 hash = SHA256.Create())
            {
                Encoding utf8 = Encoding.UTF8;
                byte[] result = hash.ComputeHash(utf8.GetBytes(Id.ToString()));
                foreach (Byte b in result)
                {
                    output += b.ToString("x2");
                }
            }
            return output.Substring(48); //Sub to 16 to make request shorter
        }

        public class MapPoint
        {
            public float PosX { get; set; }
            public float PosY { get; set; }
        }
    }
}
