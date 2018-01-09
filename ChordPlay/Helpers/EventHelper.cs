using ChordPlay.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;

namespace ChordPlay.Helpers
{
    public static class EventHelper
    {
        public static async Task<List<Place>> GetPlaces()
        {
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(new Uri("http://planer.info.pl/api/rest/places.json"));
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<Place>>(json);

            return result.Where(x => x.name != null).Distinct(new PlaceComparer()).ToList();
        }

        public static async Task<List<Event>> GetEvents(int id)
        {
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(new Uri("http://planer.info.pl/api/rest/events.json?place="+id));
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<Event>>(json);

            return result.ToList();
        }

        class PlaceComparer : IEqualityComparer<Place>
        {
            public bool Equals(Place x, Place y)
            {
                return x.name == y.name;
            }

            public int GetHashCode(Place obj)
            {
                if (Object.ReferenceEquals(obj, null)) return 0;

                int hashPlaceName = obj.name == null ? 0 : obj.name.GetHashCode();

                return hashPlaceName;
            }
        }
    }
}
