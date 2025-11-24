using albums_api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace albums_api.Controllers
{
    [Route("albums")]
    [ApiController]
    /// <summary>
    /// Provides endpoints to retrieve album information with optional sorting.
    /// </summary>
    public class AlbumController : ControllerBase
    {
        // GET: api/album
        [HttpGet]
        public IActionResult Get([FromQuery] string? sortBy = null, [FromQuery] int? year = null)
        {
            // Retrieve albums (filtered by year if specified)
            var albums = year.HasValue ? Album.GetByYear(year.Value) : Album.GetAll();

            if (!string.IsNullOrEmpty(sortBy))
            {
                albums = SortAlbums(albums, sortBy);
            }

            return Ok(albums);
        }

        /// <summary>
        /// Sorts albums by the specified field (name/title, artist, or price)
        /// </summary>
        /// <param name="albums">List of albums to sort</param>
        /// <param name="sortBy">Field to sort by: "name", "title", "artist", or "price"</param>
        /// <returns>Sorted list of albums</returns>
        private List<Album> SortAlbums(List<Album> albums, string sortBy)
        {
            return sortBy.ToLowerInvariant() switch
            {
                "name" or "title" => albums.OrderBy(a => a.Title).ToList(),
                "artist" => albums.OrderBy(a => a.Artist).ToList(),
                "price" => albums.OrderBy(a => a.Price).ToList(),
                _ => albums // Return unsorted if invalid sort field
            };
        }

        // GET api/<AlbumController>/5
        /// <summary>
        /// Retrieves a specific album by its ID.
        /// </summary>
        /// <param name="id">The ID of the album to retrieve</param>
        /// <returns>The album with the specified ID</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var album = Album.GetById(id);
            if (album == null)
            {
                return NotFound($"Album with ID {id} not found");
            }
            return Ok(album);
        }

        // POST api/<AlbumController>
        /// <summary>
        /// Creates a new album.
        /// </summary>
        /// <param name="album">The album to create</param>
        /// <returns>The created album</returns>
        [HttpPost]
        public IActionResult Post([FromBody] Album album)
        {
            if (string.IsNullOrEmpty(album.Title) || string.IsNullOrEmpty(album.Artist))
            {
                return BadRequest("Title and Artist are required");
            }

            var createdAlbum = Album.Create(album);
            return CreatedAtAction(nameof(Get), new { id = createdAlbum.Id }, createdAlbum);
        }

        // PUT api/<AlbumController>/5
        /// <summary>
        /// Updates an existing album.
        /// </summary>
        /// <param name="id">The ID of the album to update</param>
        /// <param name="album">The updated album data</param>
        /// <returns>The updated album</returns>
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Album album)
        {
            if (string.IsNullOrEmpty(album.Title) || string.IsNullOrEmpty(album.Artist))
            {
                return BadRequest("Title and Artist are required");
            }

            var success = Album.Update(id, album);
            if (!success)
            {
                return NotFound($"Album with ID {id} not found");
            }

            var updatedAlbum = Album.GetById(id);
            return Ok(updatedAlbum);
        }

        // DELETE api/<AlbumController>/5
        /// <summary>
        /// Deletes an album by its ID.
        /// </summary>
        /// <param name="id">The ID of the album to delete</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var success = Album.Delete(id);
            if (!success)
            {
                return NotFound($"Album with ID {id} not found");
            }

            return NoContent();
        }
    }
}
