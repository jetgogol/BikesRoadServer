using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikesRoadServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoadsController : ControllerBase
    {
        public readonly RoadContext _context;
        private readonly ILogger<WeatherForecastController> _logger;
        public RoadsController(RoadContext context, ILogger<WeatherForecastController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public string GetNewTraces()
        {
            Road road = new Road("Empty", new Road.MapPoint[1] { new Road.MapPoint() });
            _context.Add(road);
            _context.SaveChanges();
            return road.StringId;
        }

        [HttpPost]
        public ActionResult<Road> PostNewRoad([FromForm] string description, [FromForm] string mapPointsArray)
        {
			// Map points array x,y + x,y + ...
            List<Road.MapPoint> mapPoints = new List<Road.MapPoint>();
            foreach (string mapPoint in mapPointsArray.Split("+"))
            {
                string[] coordinates = mapPoint.Split(",");
                mapPoints.Add(new Road.MapPoint()
                {
                    PosX = float.Parse(coordinates[0]),
                    PosY = float.Parse(coordinates[1])
                });
            }
            Road PostRoad = new Road(description, mapPoints.ToArray());
            
            Road roadToAdd = new Road(PostRoad.Description, PostRoad.MapPoints);
            _context.RoadItems.Add(roadToAdd);
            _context.SaveChanges();

            return Created(PostRoad.StringId, PostRoad);
        }
        [HttpPost("{id}")]
        public ActionResult PostPhotosToRoad(string id)
        {
            int dbID = _context.RoadItems.ToList().FindIndex(r => r.StringId == id);
            if (dbID == -1)
            {
                return NotFound();
            }

            List<byte[]> imagesToSave = new List<byte[]>();
            for (int i = 0; i < Request.Form.Files.Count; i++)
            {
                Road road = _context.RoadItems.ToArray()[dbID];
                IFormFile file = Request.Form.Files[i];
                string savePath = road.PhotosFolder;
                string fullPath = Path.Combine(savePath, $"{road.PhotosCount}.jpg");
                StreamSaver.SaveStreamToFile(fullPath, file.OpenReadStream());
            }

            //TODO: Save images here
            //foreach (IFormFile image in images)
            //{
            //    Console.WriteLine(image.FileName);
            //    Console.WriteLine(image.Name);
            //}

            //long roadId = PostRoad.Id;
            //images[0];

            return Ok();
        }

        /// <summary>
        /// Get road json by id
        /// </summary>
        /// <param name="id">road hash id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<Road> GetById(string id)
        {
            Console.WriteLine(id);
            Road road = _context.RoadItems.ToList().Find(r => id == r.StringId);
            if (road == null)
            {
                return NotFound();
            }

            return Ok(road.ToJson());
        }
        [HttpGet("{id}/{index}")]
        public IActionResult GetPhotoById(string id, int index)
        {
            string path = $"Storge/{id}/{index}.jpg";
            if (System.IO.File.Exists(path))
            {
                byte[] bytes = System.IO.File.ReadAllBytes(path);
                return File(bytes, "image/jpeg");
            }

            return NotFound();
        }
    }
}
