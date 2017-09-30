using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using QuakeLogParser.Models;

namespace QuakeLogParser.Controllers
{
    [RoutePrefix("api/v1/games")]
    public class GamesController : ApiController
    {
        [HttpGet]
        [Route("")]
        public HttpResponseMessage ObterPartidas()
        {
            return Request.CreateResponse(HttpStatusCode.OK, QuakeLogParser());
        }

        private List<Game> QuakeLogParser()
        {
            var count = 1;
            var from = "";
            var to = "";

            string quakeLog = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\App_Data\games.log.txt");

            var getGames = quakeLog.Split(new string[] { "------------------------------------------------------------" }, StringSplitOptions.None);

            List<Game> games = new List<Game>();

            foreach (var gameInfo in getGames)
            {
                if (gameInfo.Contains("InitGame:"))
                {
                    var g = new Game();
                    g.Players = new List<Player>();

                    var getPlayers = gameInfo.Split(new string[] { "ClientUserinfoChanged:" }, StringSplitOptions.None);

                    foreach (var getplayer in getPlayers)
                    {
                        if (!getplayer.Contains("InitGame:"))
                        {
                            from = @" n\";
                            to = @"\t\";
                            var jogador = new Player();
                            jogador.Name = FindString(getplayer, from, to);
                            if (!g.Players.Any(model => model.Name.Equals(jogador.Name)))
                            {
                                g.Players.Add(jogador);
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
                                var ja = g.Players.Where(model => model.Name.Equals(killerPlayer)).FirstOrDefault();
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
                            var j = g.Players.Where(model => model.Name.Equals(worldKilledJogador)).FirstOrDefault();
                            if (j != null /*&& j.Kills != 0*/)
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
