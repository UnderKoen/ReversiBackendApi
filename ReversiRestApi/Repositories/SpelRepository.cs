using System.Collections.Generic;
using ReversiRestApi.Model;

namespace ReversiRestApi.Repositories {
    public class SpelRepository : ISpelRepository {
        // Lijst met tijdelijke spellen
        public List<Spel> Spellen { get; set; }

        public SpelRepository() {
            Spel spel1 = new Spel();
            spel1.Speler1Token = "abcdef";
            spel1.Omschrijving = "Potje snel reveri, dus niet lang nadenken";
            spel1.Token = "1";
            Spellen = new List<Spel> {spel1};
        }

        public void AddSpel(Spel spel) { Spellen.Add(spel); }

        public List<Spel> GetSpellen() { return Spellen; }

        public Spel GetSpel(string spelToken) { return Spellen.Find(spel => spel.Token == spelToken); }

        public void Delete(Spel spel) { Spellen.Remove(spel); }

        public void Save() { }
    }
}