using System;
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
        /// The identity.
        /// </summary>
        //public int Identity;

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
            //this.FirstSeat = new Seat();
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
