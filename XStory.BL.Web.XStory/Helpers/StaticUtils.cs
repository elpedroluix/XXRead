using System;
using System.Collections.Generic;
using System.Text;

namespace XStory.BL.Web.XStory.Helpers
{
	public static class StaticUtils
	{

		public static Dictionary<string, string> CategorySubChaptersDictionary = new Dictionary<string, string>()
		{
			{ "histoire-categorie-small-1" , "Hétéro" },
			{ "histoire-categorie-small-2" , "Lesbienne" },
			{ "histoire-categorie-small-3" , "Gay" },
			{ "histoire-categorie-small-4" , "Voyeur / Exhibition" },
			{ "histoire-categorie-small-5" , "Avec plusieurs femmes" },
			{ "histoire-categorie-small-6" , "Avec plusieurs hommes" },
			{ "histoire-categorie-small-7" , "Orgie / Partouze" },
			{ "histoire-categorie-small-8" , "SM / Fétichisme" },
			{ "histoire-categorie-small-9" , "Divers" },
			{ "histoire-categorie-small-10" , "Zoophilie" },
			{ "histoire-categorie-small-11" , "Travesti / Trans" },
			{ "histoire-categorie-small-12" , "Inceste" },
			{ "histoire-categorie-small-13" , "Trash" },
			{ "histoire-categorie-small-14" , "Erotique" },
			{ "" , "" }
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

		public static Dictionary<string, string> CategorySubChaptersToCategoryUrlDictionary = new Dictionary<string, string>()
		{
			{ "histoire-categorie-small-1" , "histoires-erotiques,hetero,1.html" },
			{ "histoire-categorie-small-2" , "histoires-erotiques,lesbienne,2.html" },
			{ "histoire-categorie-small-3" , "histoires-erotiques,gay,3.html" },
			{ "histoire-categorie-small-4" , "histoires-erotiques,voyeur+-+exhibition,4.html" },
			{ "histoire-categorie-small-5" , "histoires-erotiques,avec+plusieurs+femmes,5.html" },
			{ "histoire-categorie-small-6" , "histoires-erotiques,avec+plusieurs+hommes,6.html" },
			{ "histoire-categorie-small-7" , "histoires-erotiques,orgie+-+partouze,7.html" },
			{ "histoire-categorie-small-8" , "histoires-erotiques,sm+-+fetichisme,8.html" },
			{ "histoire-categorie-small-9" , "histoires-erotiques,divers,9.html" },
			{ "histoire-categorie-small-10" , "histoires-erotiques,zoophilie,10.html" },
			{ "histoire-categorie-small-11" , "histoires-erotiques,travesti+-+trans,11.html" },
			{ "histoire-categorie-small-12" , "histoires-erotiques,inceste,12.html" },
			{ "histoire-categorie-small-13" , "histoires-erotiques,trash,13.html" },
			{ "histoire-categorie-small-14" , "histoires-erotiques,erotique,14.html" },
			{ "" , "" }
		};
	}
}
