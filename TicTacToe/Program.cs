using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;

namespace TicTacToeME
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:8080/")
        };

        static async Task Main(string[] args)
        {
            Dictionary<string, string> jugadors = new();
            HashSet<string> desqualificats = new();
            Dictionary<string, int> victories = new();

            try
            {
               
                var participants = await client.GetFromJsonAsync<List<string>>("jugadors/");
                if (participants == null) throw new Exception("No s'han trobat participants.");

                Regex regexJugador = new(@"participant.([A-Z]+\w+ [A-Z-'a-z]+\w+).*representa(nt)? (a |de )([A-Z-a-z]+\w+)");
                Regex regexDesqualificat = new(@"participant.([A-Z]+\w+ [A-Z-'a-z]+\w+).*desqualifica(da|t)");

                foreach (var entrada in participants)
                {
                    Match match = regexJugador.Match(entrada);
                    if (match.Success)
                    {
                        string nom = match.Groups[1].Value;
                        string pais = match.Groups[4].Value;
                        jugadors[nom] = pais;
                    }
                    else
                    {
                        Match matchDesq = regexDesqualificat.Match(entrada);
                        if (matchDesq.Success)
                        {
                            string nom = matchDesq.Groups[1].Value;
                            desqualificats.Add(nom);
                        }
                    }
                }

                
                for (int i = 0; i < 10000; i++)
                {
                    try
                    {
                        var partida = await client.GetFromJsonAsync<Dictionary<string, object>>($"partida/{i}");
                        if (partida == null) break;

                        string jugador1 = partida["jugador1"]?.ToString();
                        string jugador2 = partida["jugador2"]?.ToString();
                        var tauler = (List<string>)partida["tauler"];

                        if (desqualificats.Contains(jugador1) || desqualificats.Contains(jugador2))
                            continue;

                        string guanyador = GuanyadorPerDeterminar(tauler);
                        if (guanyador == "Jugador 1") guanyador = jugador1;
                        else if (guanyador == "Jugador 2") guanyador = jugador2;

                        if (!string.IsNullOrEmpty(guanyador) && !desqualificats.Contains(guanyador))
                        {
                            if (!victories.ContainsKey(guanyador))
                                victories[guanyador] = 0;

                            victories[guanyador]++;
                        }
                    }
                    catch (HttpRequestException ex)
                    {
                        Console.WriteLine($"Error HTTP en la partida {i}: {ex.Message}");
                        break; 
                    }
                }

                
                int maxVictories = victories.Values.DefaultIfEmpty(0).Max();
                var guanyadors = victories.Where(v => v.Value == maxVictories).ToList();
                
                Console.WriteLine("Participants de la competició:");
                foreach (var jugador in jugadors)
                    Console.WriteLine($" {jugador.Key} - {jugador.Value}");

                Console.WriteLine("\nGuanyadors de la competició:");
                if (guanyadors.Count > 0)
                {
                    foreach (var guanyador in guanyadors)
                        Console.WriteLine($"{guanyador.Key} ({jugadors.GetValueOrDefault(guanyador.Key, "País desconegut")}) amb {guanyador.Value} victòries");
                }
                else
                {
                    Console.WriteLine("No hi ha cap guanyador.");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error en comunicar amb el servidor: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        static string GuanyadorPerDeterminar(List<string> tauler)
        {
            int[][] combinacionsGuanyadores = new int[][]
            {
                new[] { 0, 1, 2 }, new[] { 3, 4, 5 }, new[] { 6, 7, 8 },
                new[] { 0, 3, 6 }, new[] { 1, 4, 7 }, new[] { 2, 5, 8 },
                new[] { 0, 4, 8 }, new[] { 2, 4, 6 }
            };

            foreach (var combinacio in combinacionsGuanyadores)
            {
                char c1 = tauler[combinacio[0]][0];
                char c2 = tauler[combinacio[1]][0];
                char c3 = tauler[combinacio[2]][0];

                if (c1 == c2 && c2 == c3 && c1 != '.')
                    return c1 == 'O' ? "Jugador 1" : "Jugador 2";
            }
            return "";
        }
    }
}
