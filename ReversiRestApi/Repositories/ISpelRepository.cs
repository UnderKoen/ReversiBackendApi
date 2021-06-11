using System.Collections.Generic;
using ReversiRestApi.Model;

namespace ReversiRestApi.Repositories {
    public interface ISpelRepository {
        void AddSpel(Spel spel);
        public List<Spel> GetSpellen();
        Spel GetSpel(string spelToken);
        void Delete(Spel spel);
        void Save();
    }
}