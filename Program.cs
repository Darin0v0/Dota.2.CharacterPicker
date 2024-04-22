using System;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient();
            var api = "https://api.opendota.com/api/heroes";
            string jsonResponse = await client.GetStringAsync(api);
            List<Hero> heroes = JsonConvert.DeserializeObject<List<Hero>>(jsonResponse);

            Console.WriteLine("Jakiego typu bohatera (typ ataku)? Melee, Ranged");
            var attackType = Console.ReadLine().Trim().ToLower();

            Console.WriteLine("Jaki atrybut głównym atakiem? int, str, agi");
            var primaryAttr = Console.ReadLine().Trim().ToLower();

            Console.WriteLine("Jaka rola? Durable, Initiator, Nuker, Escape, Carry, Disabler, Support");
            var role = Console.ReadLine().Trim().ToLower();

            var filteredHeroes = heroes.Where(h =>
                (h.AttackType.ToLower() == attackType || attackType == "all") &&
                (h.PrimaryAttr.ToLower() == primaryAttr || primaryAttr == "all") &&
                h.Roles.Any(r => r.ToLower().Contains(role) || role == "all")).ToList();

            if (!filteredHeroes.Any())
            {
                Console.WriteLine("Nie znaleziono bohaterów spełniających podane kryteria.");
                return;
            }

            var random = new Random();
            var selectedHero = filteredHeroes[random.Next(filteredHeroes.Count)];
            Console.WriteLine($"Wylosowany bohater: {selectedHero.LocalizedName}");

            // Fetching items data for the selected hero
            string itemApiResponse = await client.GetStringAsync($"https://api.opendota.com/api/heroes/{selectedHero.Id}/itemPopularity");
            HeroItems heroItems = JsonConvert.DeserializeObject<HeroItems>(itemApiResponse);

            if (heroItems?.StartGameItems != null)
            {
                Console.WriteLine("Polecane przedmioty na początek gry:");
                foreach (var item in heroItems.StartGameItems.Keys)
                {
                    Console.WriteLine(item);
                }
            }
            else
            {
                Console.WriteLine("Brak danych o przedmiotach na początek gry.");
            }
            Console.WriteLine("");
            Console.WriteLine("Chcesz sprawdzić swoje konto? 1-Tak 2-Nie");
            int Input = int.Parse(Console.ReadLine());
            if(Input == 1){

            }
            else
            {
                Console.WriteLine("ok");
            }
        }
    }

    public class Hero
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("localized_name")]
        public string LocalizedName { get; set; }
        [JsonProperty("primary_attr")]
        public string PrimaryAttr { get; set; }
        [JsonProperty("attack_type")]
        public string AttackType { get; set; }
        [JsonProperty("roles")]
        public List<string> Roles { get; set; }
    }

    public class HeroItems
    {
        [JsonProperty("start_game_items")]
        public Dictionary<string, int> StartGameItems { get; set; }
    }
}
