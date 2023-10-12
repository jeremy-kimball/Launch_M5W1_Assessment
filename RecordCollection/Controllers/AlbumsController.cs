using Microsoft.AspNetCore.Mvc;
using RecordCollection.DataAccess;
using RecordCollection.Models;

namespace RecordCollection.Controllers
{
    public class AlbumsController : Controller {

        private readonly RecordCollectionContext _context;
        private readonly Serilog.ILogger _logger;

        public AlbumsController(RecordCollectionContext context, Serilog.ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var albums = _context.Albums.ToList();
            return View(albums);
        }

        [Route("/albums/{id:int}")]
        public IActionResult Show(int? id)
        {
            if (id == null)
            {
                _logger.Warning("Album Show Action - id was null");
                return NotFound();
            }

            var album = _context.Albums.FirstOrDefault(a => a.Id == id);

            if (album == null)
            {
                _logger.Warning("Album Show Action - album was null");
                return NotFound();
            }

            _logger.Information($"ID:{album.Id}| Title:{album.Title} was accessed.");
            return View(album);
        }

        public IActionResult New()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Album album)
        {
            //ensure model is entered correctly
            if(ModelState.IsValid)
            {
				_context.Albums.Add(album);
				_context.SaveChanges();

				_logger.Information($"Create Action: Success - {album.Id}:{album.Title}");

				return RedirectToAction(nameof(Index));
			}
            //route back to new pass in model including error messages
            return View("New", album);
        }

        [HttpPost]
        [Route("/albums/{id:int}")]
        public IActionResult Delete(int? id)
        {
            if(id == null)
            {
                _logger.Warning("Album Delete - Failure! id was null.");
                return RedirectToAction(nameof(Index));
            }
            var album = _context.Albums.FirstOrDefault(a => a.Id == id);
			
            if (album == null)
			{
				_logger.Warning("Album Delete - Failure! album was null.");
				return RedirectToAction(nameof(Index));
			}
			_context.Albums.Remove(album);
            _context.SaveChanges();

            _logger.Fatal($"Success! {album.Title} was removed from the database.");

            return RedirectToAction(nameof(Index));
        }
    }
}
