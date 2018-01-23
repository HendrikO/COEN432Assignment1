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
            this.Tables = tables;
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
            int totalSeats = this.Tables.Count * this.Tables[0].NumberOfSeats;
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
            foreach (var guest1 in this.GuestList)
            {
                Table tableOfGuest1 = guest1.TableSeated;
                foreach (var guest2 in this.GuestList)
                {
                    if (!Object.ReferenceEquals(guest1,guest2) && guest1.Identity > 0 && guest2.Identity > 0)
                    {
                        int relationshipValue = guest1.GetRelationshipValue(guest2.Identity);

                        switch (relationshipValue)
                        {
                            case 1:
                                if (tableOfGuest1.AreNextTo(guest1, guest2))
                                {
                                    penalty += 15;
                                }
                                else
                                {
                                    if (tableOfGuest1.AreSameTable(guest1, guest2))
                                    {
                                        penalty += 10;
                                    }
                                }
                                break;
                            case 2:
                                if (tableOfGuest1.AreNextTo(guest1, guest2))
                                {
                                        penalty += 15;
                                }
                                break;
                            case 3:
                                break;
                            case 4:
                                if (!tableOfGuest1.AreSameTable(guest1, guest2))
                                {
                                    penalty += 10;
                                }
                                break;
                            case 5:
                                if (!tableOfGuest1.AreNextTo(guest1, guest2) && tableOfGuest1.AreSameTable(guest1, guest2))
                                {
                                    penalty += 15;
                                }
                                else
                                {
                                    if (!tableOfGuest1.AreNextTo(guest1, guest2) && !tableOfGuest1.AreSameTable(guest1, guest2))
                                    {
                                        penalty += 20;
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
        /// Seats the guests.
        /// </summary>
        private void SeatTheGuests()
        {
            // Start by giving every table and ID number
            for (int i = 0; i < this.Tables.Count; ++i)
            {
                Tables[i].Identity = i;
            }

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
                this.Tables[i].Identity = i;
                var seat = this.Tables[i].FirstSeat.NextSeat;
                this.Tables[i].FirstSeat.Occupant = this.GuestList[guestIndex];
                guestIndex++;

                while (!object.ReferenceEquals(seat, this.Tables[i].FirstSeat) && guestIndex < this.GuestList.Count)
                {
                    seat.Occupant = this.GuestList[guestIndex];
                    this.GuestList[guestIndex].TableSeated = this.Tables[i];
                    guestIndex++;
                    seat = seat.NextSeat;
                }
            }
        }
    }
}
