using DomainModels;
using Npgsql;
using System.Buffers;
using System;
using BlazorApp;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Components;
using System.Security.Cryptography.X509Certificates;
using NpgsqlTypes;

namespace Service
{
    public class DatabaseService
    {
        public string connectionString;
        public List<Item> allItems;
        public List<User> allUsers;
        public string CurrentUserID { get; private set; }

        //Gemmer et CurrentUserID, som er det id på personen som er loggetin.
        public void SetCurrentUserID(string userID)
        {
            CurrentUserID = userID;
        }

        //er det smart at den beder om alle bruger ved opstart??

        //Bruger funktionerne der henter alle produkter og alle bruger fra databasen.
        public DatabaseService(string connectionString) { this.connectionString = connectionString; this.allItems = GetAllData(); this.allUsers = GetAllUser(); }

        //Henter informationen fra alle produkterne i databasen og gemmer dem som objekter i listen "allData".
        public List<Item> GetAllData()
        {
            List<Item> allData = new List<Item>();

            //Denne using tager kontakt til PostgresSQL databasen.
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                //SQL kommando for valget af den information jeg skal bruger og fra hvilken liste, informationen skal trækkes fra.
                string sql = "SELECT * FROM items";

                //Denne using laver et object, der repræsenter en SQL kommando/funktion der skal udføres mod PostgresSQL databasen.
                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    //Denne using udføre SQL kommandoen mod databasen og henter resultatet. SQL kommandoen er defineret i using ovenover.
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        //Loop der læser informationen på databasen. Vil læse alle objekterne et af gangen, til den har været igennem dem alle sammen.
                        while (reader.Read())
                        {
                            //Henter spillets type, converter data til en string og lave det til stort. Brugers for at gøre det sorter bare gennem klassen af objektet der tilføjes til listen
                            var Type = reader["type"].ToString().ToUpper();

                            //Vis spillet er af typen "PC" skal den gemme spillet med klassen "PC_Game".
                            if (Type == "PC")
                            {
								//Læser, converter og gemmer informationen fra et spil på databasen som et object på listen "allData".
								allData.Add(new PC_Game()
                                {
                                    itemID = Convert.ToInt32(reader["itemid"]),
                                    gameName = reader["gamename"].ToString(),
                                    genre = reader["genre"].ToString(),
                                    price = Convert.ToInt32(reader["price"]),
                                    manufacture = reader["manufacture"].ToString(),
                                    condition = reader["condition"].ToString(),
                                    description = reader["description"].ToString(),
                                    userID = Convert.ToInt32(reader["userid"]),
                                    type = reader["type"].ToString()
                                });
                            }

							//Vis spillet er af typen "PS" skal den gemme spillet med klassen "PS_Game".
							else if (Type == "PS")
                            {
								//Læser, converter og gemmer informationen fra et spil på databasen som et object på listen "allData".
								allData.Add(new PS_Game()
                                {
                                    itemID = Convert.ToInt32(reader["itemid"]),
                                    gameName = reader["gamename"].ToString(),
                                    genre = reader["genre"].ToString(),
                                    price = Convert.ToInt32(reader["price"]),
                                    manufacture = reader["manufacture"].ToString(),
                                    condition = reader["condition"].ToString(),
                                    description = reader["description"].ToString(),
                                    userID = Convert.ToInt32(reader["userid"]),
                                    type = reader["type"].ToString()
                                });
                            }

							//Vis spillet er af typen "XBOX" skal den gemme spillet med klassen "XBOX_Game".
							else if (Type == "XBOX")
                            {
								//Læser, converter og gemmer informationen fra et spil på databasen som et object på listen "allData".
								allData.Add(new XBOX_Game()
                                {
                                    itemID = Convert.ToInt32(reader["itemid"]),
                                    gameName = reader["gamename"].ToString(),
                                    genre = reader["genre"].ToString(),
                                    price = Convert.ToInt32(reader["price"]),
                                    manufacture = reader["manufacture"].ToString(),
                                    condition = reader["condition"].ToString(),
                                    description = reader["description"].ToString(),
                                    userID = Convert.ToInt32(reader["userid"]),
                                    type = reader["type"].ToString()
                                });
                            }
                        }
                        return allData;
                    }
                }
            }
        }

        //Henter informationen for alle brugerne i databasen og gemmer dem som objekter i listen "allUser". 
        public List<User> GetAllUser()
        {
            // Opretter en tom liste til at gemme brugerdata
            List<User> allUser = new List<User>();

            //Oprette forbindelse til databasen via NPGSQL
			using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                //Starter Forbindelsen
                connection.Open();

                //Gemmer sql kommandoen i en variabel
				string sql = "SELECT * FROM users";

                //Med forbindelsen til databasen indsætter vi kommandoen
				using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    //Med NPGSQL læser vi dataen fra Users table'et
					using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        //Mens Readern læser
						while (reader.Read())
                        {
                            //Her tilføjer vi Brugeren som er hentet fra databasen til listen med strukturen neden under
							allUser.Add(new User()
                            {
                                name = reader["name"].ToString(),
                                userID = Convert.ToInt32(reader["id"]),
                                phoneNumber = reader["phonenumber"].ToString(),
                                email = reader["email"].ToString(),
                                password = reader["password"].ToString(),
                                city = reader["city"].ToString()
                            });
                        }
                        //Returner dataen til allUser
                        return allUser;
                    }
                }
            }
        }

        //Tilføjer et PC spil til databasen.
        public int AddPCGameToDatabase(PC_Game pcGameToBeCreated)
        {
			// Initialiser newItemId til -1 (eller enhver standardværdi)
			int newItemId = -1; 

            int useridToInsert = Convert.ToInt32(CurrentUserID);

			//Denne using tager kontakt til PostgresSQL databasen.
			using var connection = new NpgsqlConnection(connectionString);
            {
                connection.Open();

                //Denne using laver et object, der ikke har en SQL kommando og en forbindelse.
                using (var cmd = new NpgsqlCommand())
                {
                    //Tildeler forbindelsen til vores NpgSQL objekt.
                    cmd.Connection = connection;

                    //if statement tjekker om spillet der bliver tilføjet er et objekt af klassen PC_Game.
                    if (pcGameToBeCreated is PC_Game pcGame)
                    {
                        //SQL kommandoen der skal køres mod Postgres databasen gemt i en variable.
                        string insertCommand = $@"INSERT INTO items(type, gamename, genre, price, manufacture, condition, description, userid)
                                    VALUES('PC', '{pcGame.gameName}','{pcGame.genre}','{pcGame.price}','{pcGame.manufacture}','{pcGame.condition}','{pcGame.description}','{useridToInsert}')
                                    RETURNING itemid"; //Inkluder RETURNING id for at få ID'et af den nyligt indsatte række.

                        //Sætter SQL kommandoen til NpgSQL objektet, der skal køres mod databasen.
						cmd.CommandText = insertCommand;

						//Brug ExecuteScalar til at få ID'et af den nyligt indsatte række.
						newItemId = (int)cmd.ExecuteScalar();

						//Du kan eller måske ikke have brug for at opdatere allItems.
						this.allItems = GetAllData(); 
                    }
                }
            }
            return newItemId;
        }

		//Tilføjer et PS spil til databasen.
		public int AddPSGameToDatabase(PS_Game psGameToBeCreated)
        {
            int newItemId = -1;

            int useridToInsert = Convert.ToInt32(CurrentUserID);

            using var connection = new NpgsqlConnection(connectionString);
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    if (psGameToBeCreated is PS_Game psGame)
                    {
                        string insertCommand = $@"INSERT INTO items(type, gamename, genre, price, manufacture, condition, description, userid)
                                            VALUES('PS', '{psGame.gameName}','{psGame.genre}','{psGame.price}','{psGame.manufacture}','{psGame.condition}','{psGame.description}','{useridToInsert}')
                                            RETURNING itemid";
                        cmd.CommandText = insertCommand;
                        newItemId = (int)cmd.ExecuteScalar();
                        this.allItems = GetAllData();
                    }
                }
            }
            return newItemId;
        }

		//Tilføjer et PS spil til databasen.
		public int AddXboxGameToDatabase(XBOX_Game XboxGameToBeCreated)
        {
            int newItemId = -1;

            int useridToInsert = Convert.ToInt32(CurrentUserID);

            using var connection = new NpgsqlConnection(connectionString);
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    if (XboxGameToBeCreated is XBOX_Game XboxGame)
                    {
                        string insertCommand = $@"INSERT INTO items(type, gamename, genre, price, manufacture, condition, description, userid)
                                            VALUES('XBOX', '{XboxGame.gameName}','{XboxGame.genre}','{XboxGame.price}','{XboxGame.manufacture}','{XboxGame.condition}','{XboxGame.description}','{useridToInsert}')
                                            RETURNING itemid";
                        cmd.CommandText = insertCommand;
                        newItemId = (int)cmd.ExecuteScalar();
                        this.allItems = GetAllData();
                    }
                }
            }
            return newItemId;
        }

        //Tilføjer en bruger til databasen.
        public void AddUserToDatabase(string name, string email, string password, string phonenumber, string city)
        {
            using var connection = new NpgsqlConnection(connectionString);
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    string insertCommand = @"INSERT INTO users(name, email, password, phonenumber, city, favorites)
                                VALUES(@Name, @Email, @Password, @phonenumber,@City, ARRAY[]::integer[])";
                    cmd.CommandText = insertCommand;

                    //Er en måde og tilføje værdierne til "VALUES" i SQL kommandoen. 
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Phonenumber", phonenumber);
                    cmd.Parameters.AddWithValue("@City", city);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Henter alle produkter fra databasen der er oprettet af en bestemt bruger.
        public List<Item> GetListedSalesForUser(int userID)
        {
            List<Item> itemsForUser = new List<Item>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string sql = $@"SELECT * FROM items WHERE userid = {userID}";

                using (var command = new NpgsqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var type = reader["type"].ToString().ToUpper();
                            if (type == "PC")
                            {
                                itemsForUser.Add(new PC_Game()
                                {
                                    itemID = Convert.ToInt32(reader["itemid"]),
                                    gameName = reader["gamename"].ToString(),
                                    genre = reader["genre"].ToString(),
                                    price = Convert.ToInt32(reader["price"]),
                                    manufacture = reader["manufacture"].ToString(),
                                    condition = reader["condition"].ToString(),
                                    description = reader["description"].ToString(),
                                    type = reader["type"].ToString()
                                });
                            }
                            else if (type == "PS")
                            {
                                itemsForUser.Add(new PS_Game()
                                {
                                    itemID = Convert.ToInt32(reader["itemid"]),
                                    gameName = reader["gamename"].ToString(),
                                    genre = reader["genre"].ToString(),
                                    price = Convert.ToInt32(reader["price"]),
                                    manufacture = reader["manufacture"].ToString(),
                                    condition = reader["condition"].ToString(),
                                    description = reader["description"].ToString(),
                                    type = reader["type"].ToString()
                                });
                            }
                            else if (type == "XBOX")
                            {
                                itemsForUser.Add(new XBOX_Game()
                                {
                                    itemID = Convert.ToInt32(reader["itemid"]),
                                    gameName = reader["gamename"].ToString(),
                                    genre = reader["genre"].ToString(),
                                    price = Convert.ToInt32(reader["price"]),
                                    manufacture = reader["manufacture"].ToString(),
                                    condition = reader["condition"].ToString(),
                                    description = reader["description"].ToString(),
                                    type = reader["type"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            return itemsForUser;
        }

        //Sletter produkt med bestemt itemID.
        public void DeleteItem(int itemID)
        {
            using var connection = new NpgsqlConnection(connectionString);
            {
                connection.Open();

                //Laver SQL kommandoen for at slette et produkt fra listen "items" med et bestemt "itemid".
                string deleteItemCommand = @"DELETE FROM items WHERE itemid = @ItemID";

                using (var cmd = new NpgsqlCommand(deleteItemCommand, connection))
                {
                    //Tilføje værdien af "itemid" til SQL kommandoen.
                    cmd.Parameters.AddWithValue("@ItemID", itemID);
                    cmd.ExecuteNonQuery();
                }
                //Valgmulighed, opdater listen "allItems", for at fjerne det slettede objekt.
                this.allItems = GetAllData();
            }
        }

        //Søgefunktionen igennem databasen, den søger kun igennem title.
        public List<Item> SearchProducts(string searchText)
        {
            List<Item> filteredItems = new List<Item>();
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM items WHERE LOWER(gamename) LIKE LOWER(@SearchText)";
                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@SearchText", $"%{searchText}%");
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var type = reader["type"].ToString().ToUpper();
                            if (type == "PC")
                            {
                                filteredItems.Add(new PC_Game()
                                {
                                    itemID = Convert.ToInt32(reader["itemid"]),
                                    gameName = reader["gamename"].ToString(),
                                    genre = reader["genre"].ToString(),
                                    price = Convert.ToInt32(reader["price"]),
                                    manufacture = reader["manufacture"].ToString(),
                                    condition = reader["condition"].ToString(),
                                    description = reader["description"].ToString(),
                                    userID = Convert.ToInt32(reader["userid"])
                                });
                            }
                            else if (type == "PS")
                            {
                                filteredItems.Add(new PS_Game()
                                {
                                    itemID = Convert.ToInt32(reader["itemid"]),
                                    gameName = reader["gamename"].ToString(),
                                    genre = reader["genre"].ToString(),
                                    price = Convert.ToInt32(reader["price"]),
                                    manufacture = reader["manufacture"].ToString(),
                                    condition = reader["condition"].ToString(),
                                    description = reader["description"].ToString(),
                                    userID = Convert.ToInt32(reader["userid"])
                                });
                            }
                            else if (type == "XBOX")
                            {
                                filteredItems.Add(new XBOX_Game()
                                {
                                    itemID = Convert.ToInt32(reader["itemid"]),
                                    gameName = reader["gamename"].ToString(),
                                    genre = reader["genre"].ToString(),
                                    price = Convert.ToInt32(reader["price"]),
                                    manufacture = reader["manufacture"].ToString(),
                                    condition = reader["condition"].ToString(),
                                    description = reader["description"].ToString(),
                                    userID = Convert.ToInt32(reader["userid"])
                                });
                            }
                        }
                    }
                }
            }
            return filteredItems;
        }

        //Henter bruger detaljerne fra databasen ud fra et bestemt ID.
        public List<User> GetSellerDetailsFromUsers(int userID)
        {
            List<User> seller = new List<User>();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string sql = $@"SELECT * FROM users WHERE id = {userID}";

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            seller.Add(new User()
                            {
                                name = reader["name"].ToString(),
                                userID = Convert.ToInt32(reader["id"]),
                                phoneNumber = reader["phonenumber"].ToString(),
                                email = reader["email"].ToString(),
                                city = reader["city"].ToString()
                            });
                        }
                        return seller;
                    }
                }
            }
        }

        //Henter brugerens favoriseret produkter gennem bestemt bruger ID.
        public List<int> GetFavoritesByUserID(int id)
        {
            List<int> favorites = new List<int>();

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                string sql = $@"SELECT array_to_string(favorites, ',') AS favorites 
                      FROM users 
                      WHERE id={id}";

                using (NpgsqlCommand command = new NpgsqlCommand(sql, connection))
                {
                    using (NpgsqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            foreach (var item in reader["favorites"].ToString().Split(',').Where(s => !string.IsNullOrWhiteSpace(s)))
                            {
                                favorites.Add(Convert.ToInt32(item));
                            }
                        }
                        return favorites;
                    }
                }
            }
        }

        //Opdater bruger information på databasen, ud fra et bestemt bruger ID.
        public void UpdateUserInDatabase(int userID, string name, string email, string password, string phoneNumber, string city)
        {
            using var connection = new NpgsqlConnection(connectionString);
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    string updateCommand = @"UPDATE users SET name = @Name, email = @Email, password = @password, phonenumber = @PhoneNumber, city = @City WHERE id = @UserID";
                    cmd.CommandText = updateCommand;

                    cmd.Parameters.Add(new NpgsqlParameter("@Name", NpgsqlDbType.Text) { Value = name });
                    cmd.Parameters.Add(new NpgsqlParameter("@Email", NpgsqlDbType.Text) { Value = email });
                    cmd.Parameters.Add(new NpgsqlParameter("@password", NpgsqlDbType.Text) { Value = password });
                    cmd.Parameters.Add(new NpgsqlParameter("@PhoneNumber", NpgsqlDbType.Text) { Value = phoneNumber });
                    cmd.Parameters.Add(new NpgsqlParameter("@City", NpgsqlDbType.Text) { Value = city });
                    cmd.Parameters.Add(new NpgsqlParameter("@UserID", NpgsqlDbType.Integer) { Value = userID });

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Sletter brugeren på databasen ud fra et bestemt bruger ID. Produkter der er oprettet af denne bruger, bliver automatisk slettet på databasen pga reference nøglen har en "ON DELETE CASCADE" kommando.
        public void DeleteUserAndItsItems(int userID)
        {
            using var connection = new NpgsqlConnection(connectionString);
            {
                connection.Open();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = connection;
                    string deleteUserCommand = @"DELETE FROM users WHERE id = @UserID";
                    cmd.CommandText = deleteUserCommand;
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Tilføje produktets "itemid" til brugerens favorite liste, ud fra et bestemt bruger ID.
        public void AddItemToFavoriteInUsers(int userid, int itemid)
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                string insertFav = $@"UPDATE users
                                      SET favorites = favorites || ARRAY[{itemid}]
                                      WHERE id = {userid}";
                cmd.CommandText = insertFav;
                cmd.ExecuteNonQuery();
            }
        }

        //Sletter produkt "itemid" fra brugerens favorite liste, ud fra et bestemt bruger ID.
        public void DeleteItemFromFavorites(int userid, int itemid)
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();

            using (var cmd = new NpgsqlCommand())
            {
                cmd.Connection = connection;
                string insertFav = $@"UPDATE users
                                      SET favorites = array_remove(favorites, {itemid})
                                      WHERE id = {userid}";
                cmd.CommandText = insertFav;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
