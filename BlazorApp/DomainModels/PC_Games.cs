using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DomainModels
{
    // Klassen PC_Game arver fra klassen Item og repræsenterer en PC-spil.
    public class PC_Game : Item
    {
        public int yearDeployed;
        public List<string> pictures;
        public string operatingSystem;
    }
}