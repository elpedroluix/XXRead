using System;
using System.Collections.Generic;
using System.Text;

namespace XStory.BL.Web.Helpers
{
    public static class StaticUtils
    {
        public static Dictionary<string, string> CategoryNameDictionary = new Dictionary<string, string>()
        {
            { "histoire-categorie-small-1" , "Avec plusieurs femmes" },
            { "histoire-categorie-small-2" , "Avec plusieurs hommes" },
            { "histoire-categorie-small-3" , "Divers" },
            { "histoire-categorie-small-4" , "Erotique" },
            { "histoire-categorie-small-5" , "Gay" },
            { "histoire-categorie-small-6" , "Hétéro" },
            { "histoire-categorie-small-7" , "Inceste" },
            { "histoire-categorie-small-8" , "Lesbienne" },
            { "histoire-categorie-small-9" , "Orgie / Partouze" },
            { "histoire-categorie-small-10" , "SM / Fétichisme" },
            { "histoire-categorie-small-11" , "Trash" },
            { "histoire-categorie-small-12" , "Travesti / Trans" },
            { "histoire-categorie-small-13" , "Voyeur / Exhibition" },
            { "histoire-categorie-small-14" , "Zoophilie" }
        };

        public static Dictionary<string, string> CategoryFromUrlDictionary = new Dictionary<string, string>()
        {
            { "histoires-erotiques,h%E9t%E9ro,1.html" , ",hetero,1" },
            { "histoires-erotiques,hetero,1.html" , ",hetero,1" },
            { "histoires-erotiques,lesbienne,2.html" , ",lesbienne,2" },
            { "histoires-erotiques,gay,3.html" , ",gay,3" },
            { "histoires-erotiques,voyeur+-+exhibition,4.html" , ",voyeur+-+exhibition,4" },
            { "histoires-erotiques,avec+plusieurs+femmes,5.html" , ",avec+plusieurs+femmes,5" },
            { "histoires-erotiques,avec+plusieurs+hommes,6.html" , ",avec+plusieurs+hommes,6" },
            { "histoires-erotiques,orgie+-+partouze,7.html" , ",orgie+-+partouze,7" },
            { "histoires-erotiques,sm+-+f%E9tichisme,8.html" , ",sm+-+f%E9tichisme,8" },
            { "histoires-erotiques,sm+-+fetichisme,8.html" , ",sm+-+fetichisme,8" },
            { "histoires-erotiques,divers,9.html" , ",divers,9" },
            { "histoires-erotiques,zoophilie,10.html" , ",zoophilie,10" },
            { "histoires-erotiques,travesti+-+trans,11.html" , ",travesti+-+trans,11" },
            { "histoires-erotiques,inceste,12.html" , ",inceste,12" },
            { "histoires-erotiques,trash,13.html" , ",trash,13" },
            { "histoires-erotiques,erotique,14.html" , ",erotique,14" },
            { "" , "" }

        };


    }
}
