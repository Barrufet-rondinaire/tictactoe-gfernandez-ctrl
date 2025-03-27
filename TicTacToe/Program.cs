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
    static void Main(string[] args)
    {
        var LlistaDeJugadors = client.GetFromJsonAsync<List<string>>("jugadors").Result;
        
        Regex rg = new Regex(@"participant.([A-Z]+\w+ [A-Z-'a-z]+\w+).*representa(nt)? (a |de )([A-Z-a-z]+\w+)");
        
        Regex rga = new Regex(@"participant.([A-Z]+\w+ [A-Z-'a-z]+\w+).*desqualifica(da|t)");

        foreach (var jugadors in LlistaDeJugadors)
        {
            Match m = rga.Match(jugadors);
            string nom = m.Groups[1].Value;
            string pais = m.Groups[2].Value;
            if (nom != "Anhoa Ojeda")
            {
            jugador.Add (nom, pais);
            Console.WriteLine ($"El jugador és {nom} i el seu pais és {pais}");
            }
        }
        
    }
    
}