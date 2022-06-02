using System.IO;
using System.Xml.Serialization;

namespace Persistence
{
    /// <summary>
    /// A DataAccess osztály valósítja meg a fájlokban történő tárolást
    /// </summary>
    public class DataAccess
    {
        /// <summary>
        /// Játék serialize-val törtéő mentése
        /// </summary>
        /// <param name="filename">Fájl nevének azonosítója</param>
        /// <param name="table">menteni való játék tábla azonosítója</param>
        public void SaveGame(string filename, GameTable table)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GameTable));
            if (filename == "") return;
            TextWriter writer = new StreamWriter(filename);

            var gameTable = table.GetTable(); 

            serializer.Serialize(writer, gameTable);
            writer.Close();
        }

        /// <summary>
        /// Serialize-val való betöltés
        /// </summary>
        /// <param name="filename">Betötlendő fájl azonosítója</param>
        /// <returns>Betöltött fájl</returns>
        public FileStream LoadGame(string filename)
        {
            FileStream stream = new FileStream(filename, FileMode.Open);

            return stream;
        }
    }
}
