using System;
using System.Collections.Generic;

namespace WeddingPlanner
{
    public class Table
    {
        /// <summary>
        /// The random seat.
        /// </summary>
        private static Random randomSeat = new Random();

        /// <summary>
        /// Gets or sets the first seat.
        /// </summary>
        /// <value>The first seat.</value>
        public Seat FirstSeat { get; set; }

        /// <summary>
        /// Gets or sets the number of seats.
        /// </summary>
        /// <value>The number of seats.</value>
        public int NumberOfSeats { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:WeddingPlanner.Table"/> class.
        /// </summary>
        public Table()
        {
            this.NumberOfSeats = 5;
            this.FirstSeat = new Seat();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:WeddingPlanner.Table"/> class.
        /// </summary>
        /// <param name="numberOfSeats">Number of seats.</param>
        public Table(int numberOfSeats)
        {
            this.NumberOfSeats = numberOfSeats;
            this.InitializeTable();
        }

        /// <summary>
        /// Is there an available seat.
        /// </summary>
        /// <returns><c>true</c>, if seat is available, <c>false</c> otherwise.</returns>
        public bool IsSeatAvailable()
        {
            var seat = this.FirstSeat;
            for (int i = 0; i < this.NumberOfSeats; ++i)
            {
                if (seat.Occupant == null)
                {
                    return true;
                }

                seat = seat.NextSeat;
            }

            return false;
        }

        /// <summary>
        /// Sits the guest.
        /// </summary>
        /// <param name="guest">Guest.</param>
        public void SitGuest(Person guest)
        {
            var seat = this.FirstSeat;
            do
            {
                // How many hops to take
                int hops = randomSeat.Next(this.NumberOfSeats);

                for (int i = 0; i < hops; ++i)
                {
                    seat = seat.NextSeat;
                }
            } while (seat.Occupant != null);

            seat.Occupant = guest;
        }

        /// <summary>
        /// Ises the next to.
        /// </summary>
        /// <returns><c>true</c>, if next to was ised, <c>false</c> otherwise.</returns>
        /// <param name="guest1">Guest1.</param>
        /// <param name="guest2">Guest2.</param>
        public bool AreNextTo(int guest1, int guest2)
        {
            var seat = this.FirstSeat;
            do
            {
                // find where guest1 is sitting
                if (seat.Occupant.Identity == guest1)
                {
                    if (seat.NextSeat.Occupant.Identity == guest2 ||
                       seat.PreviousSeat.Occupant.Identity == guest2)
                    {
                        return true;
                    }
                }
                seat = seat.NextSeat;
            }
            while (!ReferenceEquals(seat, this.FirstSeat));

            return false;
        }

        /// <summary>
        /// Ares the same table.
        /// </summary>
        /// <returns><c>true</c>, if same table was ared, <c>false</c> otherwise.</returns>
        /// <param name="guest2">Guest2.</param>
        public bool AreSameTable( int guest2)
        {
            var seat = this.FirstSeat;
            do
            {
                // find where guest1 is sitting
                if (seat.Occupant.Identity == guest2)
                {
                    return true;
                }
                seat = seat.NextSeat;
            }
            while (!ReferenceEquals(seat, this.FirstSeat));

            return false;
        }

        /// <summary>
        /// Initializes the table.
        /// </summary>
        private void InitializeTable()
        {
            for (int i = 0; i < this.NumberOfSeats; ++i)
            {
                Seat seat = new Seat
                {
                    SeatID = i
                };
                this.AddSeat(seat);
            }
        }

        /// <summary>
        /// Adds the seat.
        /// </summary>
        /// <param name="newSeat">New seat.</param>
        private void AddSeat(Seat newSeat)
        {
            if (this.FirstSeat == null)
            {
                this.FirstSeat = newSeat;
            }
            else
            {
                // Insert seat next to first seat
                if (this.FirstSeat.NextSeat == null)
                {
                    this.FirstSeat.NextSeat = newSeat;
                    newSeat.NextSeat = this.FirstSeat;
                    this.FirstSeat.PreviousSeat = newSeat;
                    newSeat.PreviousSeat = this.FirstSeat;
                }
                else
                {
                    var temp = this.FirstSeat.NextSeat;
                    this.FirstSeat.NextSeat = newSeat;
                    temp.PreviousSeat = newSeat;
                    newSeat.PreviousSeat = this.FirstSeat;
                    newSeat.NextSeat = temp;
                }
            }
        }

        /// <summary>
        /// Ises the guest sitting.
        /// </summary>
        /// <returns><c>true</c>, if guest sitting was ised, <c>false</c> otherwise.</returns>
        /// <param name="guestID">Guest identifier.</param>
        public bool IsGuestSitting(int guestID)
        {
            var seat = this.FirstSeat;
            do
            {
                // find where guest1 is sitting
                if (seat.Occupant.Identity == guestID)
                {
                    return true;
                }
                seat = seat.NextSeat;
            }
            while (!ReferenceEquals(seat, this.FirstSeat));

            return false;
        }

        /// <summary>
        /// Identifiers the of next guest.
        /// </summary>
        /// <returns>The of next guest.</returns>
        /// <param name="guestID">Guest identifier.</param>
        public int IdOfNextGuest(int guestID)
        {
            int nextGuestID = 0;

            var seat = this.FirstSeat;
            do
            {
                // find where guest1 is sitting
                if (seat.Occupant.Identity == guestID)
                {
                    nextGuestID = seat.NextSeat.Occupant.Identity;
                }
                seat = seat.NextSeat;
            }
            while (!ReferenceEquals(seat, this.FirstSeat));

            return nextGuestID;
        }

        /// <summary>
        /// Identifiers the of previous guest.
        /// </summary>
        /// <returns>The of previous guest.</returns>
        /// <param name="guestID">Guest identifier.</param>
        public int IdOfPreviousGuest(int guestID)
        {
            int previousGuestID = 0;

            var seat = this.FirstSeat;
            do
            {
                // find where guest1 is sitting
                if (seat.Occupant.Identity == guestID)
                {
                    previousGuestID = seat.PreviousSeat.Occupant.Identity;
                }
                seat = seat.NextSeat;
            }
            while (!ReferenceEquals(seat, this.FirstSeat));

            return previousGuestID;
        }

        /// <summary>
        /// Gets the identifiers of sitting guests.
        /// </summary>
        /// <returns>The identifiers of sitting guests.</returns>
        public List<int> GetIdsOfSittingGuests()
        {
            List<int> ids = new List<int>();

            var seat = this.FirstSeat;
            do
            {
                ids.Add(seat.Occupant.Identity);
                seat = seat.NextSeat;
            }
            while (!ReferenceEquals(seat, this.FirstSeat));

            return ids;
        }

        /// <summary>
        /// Gets the number of empty seats.
        /// </summary>
        /// <returns>The number of empty seats.</returns>
        public int GetNumberOfEmptySeats()
        {
            int numEmpty = 0;

            var seat = this.FirstSeat;
            do
            {
                if (seat.Occupant.Identity < 0)
                {
                    numEmpty++;
                }
                seat = seat.NextSeat;
            }
            while (!ReferenceEquals(seat, this.FirstSeat));

            return numEmpty;
        }

        /// <summary>
        /// Seat.
        /// </summary>
        public class Seat
        {
            /// <summary>
            /// Gets or sets the seat identifier.
            /// </summary>
            /// <value>The seat identifier.</value>
            public int SeatID { get; set; }

            /// <summary>
            /// Gets or sets the occupant.
            /// </summary>
            /// <value>The occupant.</value>
            public Person Occupant { get; set; }

            /// <summary>
            /// Gets or sets the next seat.
            /// </summary>
            /// <value>The next seat.</value>
            public Seat NextSeat { get; set; }

            /// <summary>
            /// Gets or sets the previous seat.
            /// </summary>
            /// <value>The previous seat.</value>
            public Seat PreviousSeat { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:WeddingPlanner.Table.Seat"/> class.
            /// </summary>
            public Seat()
            {
            }
        }
    }
}
