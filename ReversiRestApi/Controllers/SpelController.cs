using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ReversiRestApi.Model;
using ReversiRestApi.Repositories;

namespace ReversiRestApi.Controllers {
    [ApiController]
    [Route("api/spel")]
    public class SpelController : ControllerBase {
        private readonly ISpelRepository iRepository;
        public SpelController(ISpelRepository repository) { iRepository = repository; }

        // GET api/spel
        [HttpGet]
        public ActionResult<IEnumerable<Spel>> GetSpelOmschrijvingenVanSpellenMetWachtendeSpeler() {
            return Ok(
                iRepository.GetSpellen().Where(spel => spel.Speler2Token == null)
            );
        }

        [HttpPost]
        public ActionResult<Spel> AddSpel([FromForm] string spelerToken, [FromForm] string omschrijving) {
            Spel spel = new Spel();
            spel.Token = Guid.NewGuid().ToString();
            spel.Speler1Token = spelerToken;
            spel.Omschrijving = omschrijving;
            iRepository.AddSpel(spel);
            return Ok(spel);
        }

        [HttpGet("/api/spel/{id}")]
        public ActionResult<Spel> GetSpel(string id) {
            Spel spel = iRepository.GetSpel(id);
            if (spel == null) return NotFound();
            return Ok(spel);
        }

        [HttpDelete("/api/spel/{id}")]
        public ActionResult<Spel> DeleteSpel(string id, [FromQuery] string token) {
            if (token == null) return Unauthorized();
            Spel spel = iRepository.GetSpel(id);
            if (spel == null) return NotFound();
            if ((spel.AandeBeurt == Kleur.Wit ? spel.Speler1Token : spel.Speler2Token) != token) return Unauthorized("Niet jou beurt");
            
            iRepository.Delete(spel);
            iRepository.Save();
            
            return Ok(spel);
        }

        [HttpPut("/api/spel/{id}/join")]
        public ActionResult<Spel> JoinSpel(string id, [FromQuery] string token) {
            if (token == null) return Unauthorized();
            
            Spel spel = iRepository.GetSpel(id);
            if (spel == null) return NotFound();

            if (spel.Status != Status.Wachtend) return BadRequest("Dit spel is al bezig");
            if (spel.Speler2Token != null) return BadRequest("Geen ruimte in dit spel");
            spel.Speler2Token = token;
            
            iRepository.Save();
            
            return Ok(spel);
        }

        [HttpPut("/api/spel/{id}/zet")]
        public ActionResult<Spel> DoeEenZet(string id, [FromQuery] string token, [FromQuery] int rij, [FromQuery] int kolom) {
            if (token == null) return Unauthorized();

            Spel spel = iRepository.GetSpel(id);
            if (spel == null) return NotFound();
            if (spel.Status != Status.Bezig) return BadRequest("Dit potje is niet bezig");
            
            if ((spel.AandeBeurt == Kleur.Wit ? spel.Speler1Token : spel.Speler2Token) != token) return Unauthorized("Niet jou beurt");
            if (!spel.DoeZet(rij, kolom)) return BadRequest("Geen valide zet");
            
            iRepository.Save();
            
            return Ok(spel);
        }

        [HttpGet("/api/player/{id}")]
        public ActionResult<IEnumerable<Spel>> GetSpellenVanSpeler(string id) {
            return Ok(
                iRepository.GetSpellen().Where(spel => spel.Speler1Token == id || spel.Speler2Token == id)
            );
        }
    }
}