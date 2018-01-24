using System;
using System.Collections.Generic;
namespace WeddingPlanner
{
    /// <summary>
    /// Class that respresents a guest.
    /// </summary>
    public class Person
    {
        /// <summary>
        /// Gets or sets the identity.
        /// </summary>
        /// <value>The identity.</value>
        public int Identity { get; set; }

        /// <summary>
        /// Gets or sets the relationships.
        /// </summary>
        /// <value>The relationships.</value>
        public List<KeyValuePair<int,int>> Relationships { get; set; }

        /// <summary>
        /// Gets or sets the table seated.
        /// </summary>
        /// <value>The table seated.</value>
        public Table TableSeated { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:WeddingPlanner.Person"/> class.
        /// </summary>
        public Person()
        {
            this.Relationships = new List<KeyValuePair<int, int>>();
        }

        /// <summary>
        /// Gets the relationship value.
        /// </summary>
        /// <returns>The relationship value.</returns>
        /// <param name="guestIdentity">Guest identity.</param>
        public int GetRelationshipValue(int guestIdentity)
        {
            return Relationships.Find(x => x.Key == guestIdentity).Value;
        }
    }
}
