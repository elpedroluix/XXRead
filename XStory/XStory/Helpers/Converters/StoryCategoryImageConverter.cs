using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace XStory.Helpers.Converters
{
    /// <summary>
    /// Converts CategoryUrl to Category Image
    /// </summary>
    public class StoryCategoryImageConverter : IValueConverter
    {
        public static Dictionary<string, ImageSource> CategoryImageDictionary = new Dictionary<string, ImageSource>()
        {
            { "histoires-erotiques,hetero,1.html" , ImageSource.FromFile("hetero_1") },
            { "histoires-erotiques,lesbienne,2.html" , ImageSource.FromFile("lesbienne_2") },
            { "histoires-erotiques,gay,3.html" , ImageSource.FromFile("gay_3") },
            { "histoires-erotiques,voyeur+-+exhibition,4.html" , ImageSource.FromFile("voyeur_exhibition_4") },
            { "histoires-erotiques,avec+plusieurs+femmes,5.html" , ImageSource.FromFile("avec_plusieurs_femmes_5") },
            { "histoires-erotiques,avec+plusieurs+hommes,6.html" , ImageSource.FromFile("avec_plusieurs_hommes_6") },
            { "histoires-erotiques,orgie+-+partouze,7.html" , ImageSource.FromFile("orgie_partouze_7") },
            { "histoires-erotiques,sm+-+fetichisme,8.html" , ImageSource.FromFile("sm_fetichisme_8") },
            { "histoires-erotiques,divers,9.html" , ImageSource.FromFile("divers_9") },
            { "histoires-erotiques,zoophilie,10.html" , ImageSource.FromFile("zoophilie_10") },
            { "histoires-erotiques,travesti+-+trans,11.html" , ImageSource.FromFile("travesti_trans_11") },
            { "histoires-erotiques,inceste,12.html" , ImageSource.FromFile("inceste_12") },
            { "histoires-erotiques,trash,13.html" , ImageSource.FromFile("trash_13") },
            { "histoires-erotiques,erotique,14.html" , ImageSource.FromFile("erotique_14") }
        };
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return CategoryImageDictionary[(string)value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
