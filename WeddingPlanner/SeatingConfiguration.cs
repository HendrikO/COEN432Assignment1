using System;
using System.Collections.Generic;

namespace WeddingPlanner
{
    /// <summary>
    /// Seating configuration.
    /// </summary>
    public class SeatingConfiguration
    {
        /// <summary>
        /// Random table index generator.
        /// </summary>
        private static Random randomTable = new Random();

        /// <summary>
        /// The tables.
        /// </summary>
        public List<Table> Tables;

        /// <summary>
        /// The guest list.
        /// </summary>
        public List<Person> GuestList;

        /// <summary>
        /// Gets or sets the fitness.
        /// </summary>
        /// <value>The fitness.</value>
        public int Fitness { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:WeddingPlanner.SeatingConfiguration"/> class.
        /// </summary>
        public SeatingConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:WeddingPlanner.SeatingConfiguration"/> class.
        /// </summary>
        /// <param name="tables">Tables.</param>
        /// <param name="guests">Guests.</param>
        public SeatingConfiguration(List<Table> tables, Person[] guests)
        {
            this.Tables = new List<Table>(tables);
            this.GuestList = new List<Person>(guests);
            this.SeatTheGuestsInOrder();
            this.EvaluateFitness();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:WeddingPlanner.SeatingConfiguration"/> class.
        /// </summary>
        /// <param name="tables">Tables.</param>
        /// <param name="guests">Guests.</param>
        public SeatingConfiguration(List<Table> tables, List<Person> guests)
        {
            this.Tables = new List<Table>(tables);
            this.GuestList = new List<Person>(guests);
            this.SeatTheGuests();
            this.EvaluateFitness();
        }

        /// <summary>
        /// Tos the permutation form.
        /// </summary>
        /// <returns>The permutation form.</returns>
        public Person[] ToPermutationForm()
        {
            List<Person> permutation = new List<Person>();
            int emptySeatID = -1;

            foreach (var table in this.Tables)
            {
                var seat = table.FirstSeat;

                do
                {
                    // Empty seats is represented by a person with negative ID
                    if (seat.Occupant == null)
                    {
                        Person emptySeat = new Person
                        {
                            Identity = emptySeatID
                        };
                        emptySeatID--;
                        permutation.Add(emptySeat);
                    }
                    else
                    {
                        permutation.Add(seat.Occupant);
                    }
                    seat = seat.NextSeat;
                }
                while (!object.ReferenceEquals(seat, table.FirstSeat));
            }

            return permutation.ToArray();
        }

        /// <summary>
        /// Evaluates the fitness.
        /// </summary>
        /// <returns>The fitness.</returns>
        private void EvaluateFitness()
        {
            int penalty = 0;

            for (int i = 0; i < this.GuestList.Count; ++i)
            {
                int currentGuestID = this.GuestList[i].Identity;
                Table guestTable = this.GetTableOfGuest(currentGuestID);

                for (int j = 0; j < this.GuestList.Count; ++j)
                {
                    int comparedGuestID = this.GuestList[j].Identity;

                    // stop if same guest or if one guest is an empty seat
                    if (currentGuestID != comparedGuestID && 
                        !(currentGuestID < 0) &&
                        !(comparedGuestID < 0))
                    {
                        int relationshipValue = GuestList[i].GetRelationshipValue(comparedGuestID);

                        switch (relationshipValue)
                        {
                            
                            case 1:
                                if (guestTable.AreNextTo(currentGuestID, comparedGuestID))
                                {
                                    penalty += 15;
                                }
                                else
                                {
                                    if (guestTable.AreSameTable(comparedGuestID))
                                    {
                                        penalty += 10;
                                    }
                                }
                                break;
                            case 2:
                                if (guestTable.AreNextTo(currentGuestID, comparedGuestID))
                                {
                                    penalty += 15;
                                }
                                break;
                            case 3:
                                break;
                            case 4:
                                if (!guestTable.AreSameTable(comparedGuestID))
                                {
                                    penalty += 10;
                                }
                                break;
                            case 5:
                                if (!guestTable.AreSameTable(comparedGuestID))
                                {
                                    penalty += 20;
                                }
                                else
                                {
                                    if (!guestTable.AreNextTo(currentGuestID, comparedGuestID))
                                    {
                                        penalty += 15;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
            }

            this.Fitness = penalty;
        }

        /// <summary>
        /// Gets the table of guest.
        /// </summary>
        /// <returns>The table of guest.</returns>
        /// <param name="guestID">Guest identifier.</param>
        public Table GetTableOfGuest(int guestID)
        {
            Table table = null;

            for (int i = 0; i < this.Tables.Count; ++i)
            {
                var seat = this.Tables[i].FirstSeat;
                do
                {
                    if (seat.Occupant.Identity == guestID)
                    {
                        table = this.Tables[i];
                    }
                    seat = seat.NextSeat;
                }
                while (!ReferenceEquals(seat, this.Tables[i].FirstSeat));
            }

            return table;
        }

        /// <summary>
        /// Seats the guests.
        /// </summary>
        private void SeatTheGuests()
        {
            // Go through the list and place guests randomly
            foreach (var guest in GuestList)
            {
                int tableIndex = 0;
                do
                {
                    tableIndex = randomTable.Next(Tables.Count);
                }
                while (!this.Tables[tableIndex].IsSeatAvailable());

                this.Tables[tableIndex].SitGuest(guest);
            }
        }

        /// <summary>
        /// Seats the guests in order.
        /// </summary>
        private void SeatTheGuestsInOrder()
        {
            int guestIndex = 0;
            // Start by giving every table and ID number
            for (int i = 0; i < this.Tables.Count; ++i)
            {
                var seat = this.Tables[i].FirstSeat.NextSeat;
                this.Tables[i].FirstSeat.Occupant = this.GuestList[guestIndex];
                guestIndex++;

                while (!object.ReferenceEquals(seat, this.Tables[i].FirstSeat) && guestIndex < this.GuestList.Count)
                {
                    seat.Occupant = this.GuestList[guestIndex];
                    guestIndex++;
                    seat = seat.NextSeat;
                }
            }
        }

        /// <summary>
        /// Outputs the tables.
        /// </summary>
        public void OutputTables()
        {
            string guestIDs = string.Empty;

            Console.WriteLine("Each line represents a table:");
            foreach (var table in this.Tables)
            {
                var seat = table.FirstSeat;
                do
                {
                    guestIDs = guestIDs + seat.Occupant.Identity + " ";
                    seat = seat.NextSeat;
                }
                while (!ReferenceEquals(seat, table.FirstSeat));
                Console.WriteLine(guestIDs);
                guestIDs = string.Empty;
            }
        }
    }
}
