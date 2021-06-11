using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReversiRestApi.Model {
    public class SpelBordConverter : JsonConverter<Kleur[,]> {
        public override Kleur[,] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Kleur[,] bord, JsonSerializerOptions options) {
            writer.WriteStartObject();
            for (int x = 0; x < 8; x++) {
                for (int y = 0; y < 8; y++) {
                    Kleur kleur = bord[y, x];
                    writer.WriteNumber(x + "," + y, kleur.GetHashCode());
                }
            }

            writer.WriteEndObject();
        }
    }

    public class Spel : ISpel {
        public int ID { get; set; }
        public string Omschrijving { get; set; }
        public string Token { get; set; }
        public string Speler1Token { get; set; }
        public string Speler2Token { get; set; }

        [NotMapped]
        public Status Status {
            get {
                if (Speler2Token == null) return Status.Wachtend;
                return Afgelopen() ? Status.Klaar : Status.Bezig;
            }
        }

        [NotMapped] private Kleur[,] _bord;
        
        [NotMapped]
        [JsonConverter(typeof(SpelBordConverter))]
        public Kleur[,] Bord { get => _bord; set => BordAsString = _convert(value); }

        [JsonIgnore]
        public string BordAsString {
            get => _convert(_bord);

            set => _bord = _convert(value);
        }
        
        private static string _convert(Kleur[,] bord) {
            string s = "";
            for (int y = 0; y < 8; y++) {
                for (int x = 0; x < 8; x++) {
                    Kleur kleur = bord[x, y];
                    int v;
                    switch (kleur) {
                        default:
                        case Kleur.Geen:
                            v = 0;
                            break;
                        case Kleur.Wit:
                            v = 1;
                            break;
                        case Kleur.Zwart:
                            v = 2;
                            break;
                    }

                    s += v;
                }

                s += ":";
            }

            return s;
        }
        
        private static Kleur[,] _convert(string str) {
            Kleur[,] bord = new Kleur[8, 8];

            int x = 0;
            foreach (string line in str.Split(":")) {
                int y = 0;
                foreach (char c in line.ToCharArray()) {
                    Kleur kleur;
                    
                    switch (c) {
                        default:
                        case '0':
                            kleur = Kleur.Geen;
                            break;
                        case '1':
                            kleur = Kleur.Wit;
                            break;
                        case '2':
                            kleur = Kleur.Zwart;
                            break;
                    }

                    bord[y, x] = kleur;
                    
                    y++;
                }

                x++;
            }

            return bord;
        }
        
        public Kleur AandeBeurt { get; set; }

        public Spel() {
            AandeBeurt = Kleur.Zwart;
            Bord = new Kleur[8, 8];
            SetupStartPositie();
        }

        public bool Pas() {
            if (!GeenMogelijkheden()) return false;
            Next();
            return true;
        }

        public bool Afgelopen() {
            if (VolBord()) return true;
            if (!GeenMogelijkheden()) return false;
            Next();
            return GeenMogelijkheden();
        }

        private bool VolBord() {
            foreach (Kleur kleur in Bord) {
                if (kleur == Kleur.Geen) return false;
            }

            return true;
        }

        private bool GeenMogelijkheden() {
            for (int y = 0; y < 8; y++) {
                for (int x = 0; x < 8; x++) {
                    if (Bord[x, y] != Kleur.Geen) continue;
                    if (ZetMogelijk(x, y)) return false;
                }
            }

            return true;
        }

        public Kleur OverwegendeKleur() {
            int wit = 0, zwart = 0;
            foreach (Kleur kleur in Bord) {
                if (kleur == Kleur.Wit) wit++;
                else if (kleur == Kleur.Zwart) zwart++;
            }

            return wit >= zwart ? wit == zwart ? Kleur.Geen : Kleur.Wit : Kleur.Zwart;
        }

        [NotMapped]
        private static readonly int[][] Directions = {
            new[] {1, 0}, new[] {0, 1}, new[] {-1, 0}, new[] {0, -1},
            new[] {1, 1}, new[] {-1, -1}, new[] {-1, 1}, new[] {1, -1}
        };

        public bool ZetMogelijk(int rijZet, int kolomZet) {
            if (rijZet < 0 || rijZet >= 8 || kolomZet < 0 || kolomZet >= 8) return false;

            if (Bord[rijZet, kolomZet] != Kleur.Geen) return false;
            
            return Directions
                .Select(dir => CheckDirection(rijZet, kolomZet, dir, AandeBeurt))
                .Aggregate((b1, b2) => b1 || b2);
        }

        private bool CheckDirection(int rijZet, int kolomZet, int[] dir, Kleur doel) {
            bool first = true;
            while (true) {
                rijZet += dir[0];
                kolomZet += dir[1];

                if (rijZet < 0 || rijZet >= 8 || kolomZet < 0 || kolomZet >= 8) return false;

                Kleur kleur = Bord[rijZet, kolomZet];
                if (kleur == Kleur.Geen) return false;
                if (kleur == doel) return !first;
                first = false;
            }
        }

        public bool DoeZet(int rijZet, int kolomZet) {
            if (!ZetMogelijk(rijZet, kolomZet)) return false;

            foreach (int[] dir in Directions) {
                bool valid = CheckDirection(rijZet, kolomZet, dir, AandeBeurt);
                if (!valid) continue;

                int x = rijZet, y = kolomZet;

                do {
                    Bord[x, y] = AandeBeurt;
                    x += dir[0];
                    y += dir[1];
                } while (Bord[x, y] != AandeBeurt);
            }

            Next();
            return true;
        }

        private void Next() { AandeBeurt = AandeBeurt == Kleur.Wit ? Kleur.Zwart : Kleur.Wit; }

        private void SetupStartPositie() {
            for (int y = 0; y < 8; y++) {
                for (int x = 0; x < 8; x++) {
                    Bord[x, y] = Kleur.Geen;
                }
            }

            Bord[3, 3] = Kleur.Wit;
            Bord[3, 4] = Kleur.Zwart;
            Bord[4, 3] = Kleur.Zwart;
            Bord[4, 4] = Kleur.Wit;
        }
    }
}