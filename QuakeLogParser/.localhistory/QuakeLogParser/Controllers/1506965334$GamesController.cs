using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using QuakeLogParser.Models;
using System.Data.Entity;

namespace QuakeLogParser.Controllers
{
    [RoutePrefix("api/v1/games")]
    public class GamesController : ApiController
    {
        QuakeEntities bd = new QuakeEntities();

        [HttpGet]
        [Route("bd")]
        public HttpResponseMessage ObterInfoBd()
        {
            var partidas = QuakeLogParser();

            if (bd.game.Count() <= 0)
            {

                foreach (var partida in partidas)
                {
                    bd.game.Add(partida);

                    foreach (var jogador in partida.player)
                    {
                        bd.player.Add(jogador);
                    }
                }

                bd.SaveChanges();

            }

            var games = bd.game.Include(m => m.player).ToList();

            var result  = from g in games select new { Game = g.Name, TotalKills = g.TotalKills, TotalPlayers = g.player.Count(), Players = from p in g.player select new { name = p.Name, kills = p.Kills} };

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        [Route("file")]
        public HttpResponseMessage ObterInfoArquivo()
        {
            return Request.CreateResponse(HttpStatusCode.OK, QuakeLogParser());
        }
        
        public List<game> QuakeLogParser()
        {
            var count = 1;
            var from = "";
            var to = "";

            string quakeLog = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\App_Data\games.log.txt");

            var getGames = quakeLog.Split(new string[] { "------------------------------------------------------------" }, StringSplitOptions.None);

            List<game> games = new List<game>();

            foreach (var gameInfo in getGames)
            {
                if (gameInfo.Contains("InitGame:"))
                {
                    var g = new game();
                    g.player = new List<player>();
                    g.TotalKills = 0;

                    var getPlayers = gameInfo.Split(new string[] { "ClientUserinfoChanged:" }, StringSplitOptions.None);

                    foreach (var getplayer in getPlayers)
                    {
                        if (!getplayer.Contains("InitGame:"))
                        {
                            from = @" n\";
                            to = @"\t\";
                            var player = new player();
                            player.Name = FindString(getplayer, from, to);
                            if (!g.player.Any(model => model.Name.Equals(player.Name)))
                            {
                                player.Kills = 0;
                                g.player.Add(player);
                            }
                        }
                    }

                    var getKills = gameInfo.Split(new string[] { "Kill:" }, StringSplitOptions.None);
                    foreach (var kill in getKills)
                    {
                        if (!kill.Contains("InitGame:"))
                        {
                            g.TotalKills++;

                            from = ": ";
                            to = " killed";
                            var killerPlayer = FindString(kill, from, to);
                            if (!killerPlayer.Contains("<world>"))
                            {
                                var ja = g.player.Where(model => model.Name.Equals(killerPlayer)).FirstOrDefault();
                                if (ja != null)
                                {
                                    ja.Kills++;
                                }
                            }
                        }


                        if (kill.Contains("<world>"))
                        {
                            g.KillsByWorld++;
                            from = "<world> killed ";
                            to = " by";
                            var worldKilledJogador = FindString(kill, from, to);
                            var j = g.player.Where(model => model.Name.Equals(worldKilledJogador)).FirstOrDefault();
                            if (j != null && j.Kills != 0)
                            {
                                j.Kills--;
                            }
                        }
                    }
                    g.Name = string.Format("game_{0}", count++);
                    games.Add(g);
                }
            }
            return games;
        }


        private string FindString(string stringBase, string from, string to)
        {
            var stringFound = stringBase.Split(new string[] { from }, StringSplitOptions.None)[1]
                      .Split(new string[] { to }, StringSplitOptions.None)[0];

            return stringFound;
        }
    }
}
