using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DomainModels
{
    public class XBOX_Game : Item
    {
        // Klasse for at repræsentere et XBOX-spil, der arver fra Item-klassen.
        // Indeholder egenskaber for udgivelsesår, en liste over billeder og XBOX-modellen.
        public int yearDeployed;
        public List<string> pictures;
        public string xboxModel;
    }
}