using SWW.GStats.BusinessLogic.DTO;
using SWW.GStats.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SWW.GStats.BusinessLogic.Services
{
    public static class ServicesExtentions
    {
        private const string GameModesSeparator = ",";

        public static EndpointAdvertising ToDto(this Endpoint endpoint) {
            if (endpoint == null) return null;

            return new EndpointAdvertising {
                name = endpoint.Name,
                gameModes = endpoint.GameModes.Split(GameModesSeparator)
            };
        }

        public static void FromDto(this Endpoint endpoint, EndpointAdvertising data) {
            endpoint.Name = data.name;
            endpoint.GameModes = data.gameModes == null ? null : string.Join(",", data.gameModes);
        }


        public static MatchItem ToDto(this Match match)
        {
            return new MatchItem {
                map = match.Map,
                fragLimit = match.FragLimit,
                gameMode = match.GameMode,
                timeElapsed = match.TimeElapsed,
                timeLimit = match.TimeLimit,
                scoreboard = match.Scoreboard.Select(x => x.ToDto()).ToArray()
            };
        }


        public static Match ToData(this MatchItem dto, Endpoint endpoint, DateTime timestamp) {
            var totalPlayers = dto.scoreboard.Length;
            return new Match {
                EndpointId = endpoint.Id,
                Timestamp = timestamp,
                Map = dto.map,
                FragLimit = dto.fragLimit,
                GameMode = dto.gameMode,
                TimeElapsed = dto.timeElapsed,
                TimeLimit = dto.timeLimit,
                PlayersCount = totalPlayers,
                Scoreboard = dto.scoreboard.Select((x,ind) => x.ToData(CalcScoreboardPercent(ind, totalPlayers)) ).ToArray()
            };
        }

        public static ScoreboardItem ToDto(this Scoreboard scoreboard) {
            return new ScoreboardItem {
                name = scoreboard.Name,
                deaths = scoreboard.Deaths,
                frags = scoreboard.Frags,
                kills = scoreboard.Kills,
            };
        }

        public static Scoreboard ToData(this ScoreboardItem dto, float rating) {
            return new Scoreboard {
                Name = dto.name,
                Deaths = dto.deaths,
                Frags = dto.deaths,
                Kills = dto.kills,
                Rating = rating
            };
        }

        public static MatchReportItem ToMatchReportItem(this Match match)
        {
            return new MatchReportItem {
                server = match.EndpointId,
                timestamp = match.Timestamp,
                results = match.ToDto()
            };
        }

        public static void CollectGroupStats<T>(this Dictionary<T, int> dictionary, T key) {
            if (!dictionary.TryGetValue(key, out int val)) {
                val = 0;
            }
            dictionary[key] = val+1;
        }

        public static float CalcScoreboardPercent(int position, int total) {
            if (total == 1) return 100F;
            return 100F * (1F - (float)position / (total - 1));
        }
    }
}
