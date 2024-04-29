using DomainModels;

namespace Service
{
    public class FiltreTypes
    {
        //Filtrer til PC spil.
        public List<Item> FiltreToPC_Games(List<Item> games)
        {
            List<Item> Games = new List<Item>();

            foreach (var game in games)
            {
                if (game is PC_Game pcGame)
                {
                    Games.Add(pcGame);
                }
            }
            return Games;
        }

		//Filtrer til PS spil.
		public List<Item> FiltreToPS_Games(List<Item> games)
        {
            List<Item> Games = new List<Item>();

            foreach (var game in games)
            {
                if (game is PS_Game psGame)
                {
                    Games.Add(psGame);
                }
            }
            return Games;
        }

		//Filtrer til XBOX spil.
		public List<Item> FiltreToXBOX_Games(List<Item> games)
        {
            List<Item> Games = new List<Item>();

            foreach (var game in games)
            {
                if (game is XBOX_Game xboxGame)
                {
                    Games.Add(xboxGame);
                }
            }
            return Games;
        }
    }
}
