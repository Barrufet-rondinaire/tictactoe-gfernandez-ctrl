﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

 
namespace TicTacToe;

class Program
{
    private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri("http://localhost:8080/") };
    private const string JugadorDesqualificada = "Espanya";

    
        static async Task Main(string[] args)
        {
            var participants = await client.GetFromJsonAsync<List<string>>("jugadors");
            var partides = await client.GetFromJsonAsync<List<string>>("partides");
            
            Dictionary<string, string> jugadors = new();
            HashSet<string> desqualificats = new();
            Dictionary<string, int> victòries = new();
            
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



    }
      
    
    
}