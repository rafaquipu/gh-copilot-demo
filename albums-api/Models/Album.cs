using System;
using System.Collections.Generic;
using System.Linq;

namespace albums_api.Models
{
    public record Album(int Id, string Title, Artist Artist, int Year, double Price, string Image_url)
    {
        private static List<Album> _albums = new List<Album>(){
            new Album(1, "You, Me and an App Id", new Artist("Daprize", new DateTime(1990, 1, 1), "Seattle, WA"), 2023, 10.99, "https://aka.ms/albums-daprlogo"),
            new Album(2, "Seven Revision Army", new Artist("The Blue-Green Stripes", new DateTime(1985, 5, 12), "London, UK"), 2022, 13.99, "https://aka.ms/albums-containerappslogo"),
            new Album(3, "Scale It Up", new Artist("KEDA Club", new DateTime(1992, 3, 8), "Berlin, Germany"), 2021, 13.99, "https://aka.ms/albums-kedalogo"),
            new Album(4, "Lost in Translation", new Artist("MegaDNS", new DateTime(1988, 7, 22), "Tokyo, Japan"), 2020, 12.99,"https://aka.ms/albums-envoylogo"),
            new Album(5, "Lock Down Your Love", new Artist("V is for VNET", new DateTime(1995, 11, 30), "San Francisco, CA"), 2019, 12.99, "https://aka.ms/albums-vnetlogo"),
            new Album(6, "Sweet Container O' Mine", new Artist("Guns N Probeses", new DateTime(1980, 9, 17), "New York, NY"), 2018, 14.99, "https://aka.ms/albums-containerappslogo")
         };

        public static List<Album> GetAll()
        {
            return _albums;
        }

        public static Album? GetById(int id)
        {
            return _albums.FirstOrDefault(a => a.Id == id);
        }

        public static List<Album> GetByYear(int year)
        {
            return _albums.Where(a => a.Year == year).ToList();
        }

        public static Album Create(Album album)
        {
            var newId = _albums.Any() ? _albums.Max(a => a.Id) + 1 : 1;
            var newAlbum = album with { Id = newId };
            _albums.Add(newAlbum);
            return newAlbum;
        }

        public static bool Update(int id, Album updatedAlbum)
        {
            var index = _albums.FindIndex(a => a.Id == id);
            if (index == -1) return false;
            
            _albums[index] = updatedAlbum with { Id = id };
            return true;
        }

        public static bool Delete(int id)
        {
            var album = _albums.FirstOrDefault(a => a.Id == id);
            if (album == null) return false;
            
            _albums.Remove(album);
            return true;
        }
    }
}
