using System.Collections.Generic;
using System;

public static class EventsGenerator
{
    public static Event FirstEvent()
    {
        var tupleList = new List<Tuple<string, int, string>>
        {
          new Tuple<string, int, string>("Allons-y", 13, "Bien dit!\nRends-toi où tout à commencer."),
          new Tuple<string, int, string>("Pas mon problème...", -1, "En laissant la terre dans cet état, le monde ne change pas...\n\nVous avez donc échoué dans votre mission...")
        };


        return new Event(0, "Départ", "La troisième guerre mondial a éclatée il y a maintenant 10 ans! Le monde est déchiré par les bombes. Il nous faut changer le cours de l'histoire. Votre objectif est donc de remonter le temps pour trouver une meilleure fin pour l'humanité!", tupleList); ;
    }
}