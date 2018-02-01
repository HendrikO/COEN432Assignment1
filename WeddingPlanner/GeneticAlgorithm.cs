using System;
using System.Collections.Generic;
using System.Linq;

namespace WeddingPlanner
{
    /// <summary>
    /// Genetic algorithm.
    /// </summary>
    public class GeneticAlgorithm
    {
        /// <summary>
        /// The random seat.
        /// </summary>
        private static Random randomSeat = new Random();

        /// <summary>
        /// The instance.
        /// </summary>
        private static GeneticAlgorithm instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:WeddingPlanner.GeneticAlgorithm"/> class.
        /// </summary>
        private GeneticAlgorithm()
        {}

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static GeneticAlgorithm Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GeneticAlgorithm();
                }

                return instance;
            }
        }

        /// <summary>
        /// Nexts the generation.
        /// </summary>
        /// <param name="population">Population.</param>
        public void NextGeneration(ref List<SeatingConfiguration> population)
        {
            // Parent selection
            // TODO: Use a constant instead of a hard coded value
            this.ParentSelection(population, out List<SeatingConfiguration> parents, 10);

            // Create children
            this.GenerateChildren(parents, out List<SeatingConfiguration> children);

            // Survival selection
            this.SurvivalSelection(ref population, children);
        }

        /// <summary>
        /// Nexts the generation diversity preservation.
        /// </summary>
        /// <param name="population">Population.</param>
        public void NextGenerationDiversityPreservation(ref List<SeatingConfiguration> population)
        {
            // Select parents
            this.ParentSelection(population, out List<SeatingConfiguration> parents, 10);

            // Generate children and survival selection in one
            this.CrowdingDiversity(ref population, parents);

        }

        private void CrowdingDiversity(ref List<SeatingConfiguration> population, List<SeatingConfiguration> parents)
        {
            // Iterate through the parents by picking two at random until all have been picked
            while (parents.Count != 0)
            {
                // Pick two parents at random and remove them list
                Random randomIndex = new Random();

                SeatingConfiguration parentA = parents[randomIndex.Next(parents.Count)];
                parents.Remove(parentA);
                SeatingConfiguration parentB = parents[randomIndex.Next(parents.Count)];
                parents.Remove(parentB);

                // Breed these two parents
                this.BreedParents(parentA, parentB, out SeatingConfiguration childA, out SeatingConfiguration childB);

                // Calculate the distance values using diversity metric
                // d(p1,o1) + d(p2,o2) < d(p1,o2) + d(p2,o1)
                int dPACA = this.MeasureDiversity(parentA, childA);
                int dPACB = this.MeasureDiversity(parentA, childB);
                int dPBCA = this.MeasureDiversity(parentB, childA);
                int dPBCB = this.MeasureDiversity(parentB, childB);

                // Create the competition
                if (dPACA + dPBCB < dPACB + dPBCA)
                {
                    // PA with CA
                    // PB with CB
                    // Compare the fitness
                    if (childA.Fitness < parentA.Fitness)
                    {
                        // Child is more fit, so add to pop and remove parent
                        population.Remove(parentA);
                        population.Add(childA);
                    }

                    if (childB.Fitness < parentB.Fitness)
                    {
                        population.Remove(parentB);
                        population.Add(childB);
                    }
                }
                else
                {
                    // PA with CB
                    // PB with CA
                    if (childB.Fitness < parentA.Fitness)
                    {
                        population.Remove(parentA);
                        population.Add(childB);
                    }

                    if (childA.Fitness < parentB.Fitness)
                    {
                        population.Remove(parentB);
                        population.Add(childA);
                    }
                }
            }

            // Order the population based on absolute rank
            population = population.OrderBy(x => x.Fitness).ToList();
        }

        /// <summary>
        /// Parents the selection.
        /// </summary>
        /// <param name="population">Population.</param>
        /// <param name="parents">Parents.</param>
        /// <param name="numParents">Number parents.</param>
        private void ParentSelection(List<SeatingConfiguration> population, out List<SeatingConfiguration> parents, int numParents)
        {
            parents = new List<SeatingConfiguration>();

            // TournamentSelection
            Random randomIndex = new Random();

            for (int i = 0; i < numParents; ++i)
            {
                // Pick 3 out of the population at random
                int i0 = randomIndex.Next(population.Count);
                int i1 = randomIndex.Next(population.Count);
                int i2 = randomIndex.Next(population.Count);

                // Find out which one has the highest fitness
                List<SeatingConfiguration> tournamentChosen = new List<SeatingConfiguration>
                {
                    population[i0],
                    population[i1],
                    population[i2]
                };

                tournamentChosen = tournamentChosen.OrderBy(x => x.Fitness).ToList();

                // Add winner to parents
                parents.Add(tournamentChosen[0]);
            }
        }

        /// <summary>
        /// Generates the children.
        /// </summary>
        /// <param name="parents">Parents.</param>
        /// <param name="children">Children.</param>
        private void GenerateChildren(List<SeatingConfiguration> parents, out List<SeatingConfiguration> children)
        {
            children = new List<SeatingConfiguration>();

            for (int i = 0; i < parents.Count - 1; ++i)
            {
                this.BreedParents(parents[i], parents[i + 1],
                                  out SeatingConfiguration firstChild,
                                  out SeatingConfiguration secondChild);
                children.Add(firstChild);
                children.Add(secondChild);
            }
        }

        /// <summary>
        /// Survivals the selection.
        /// </summary>
        /// <param name="population">Population.</param>
        /// <param name="children">Number new children.</param>
        private void SurvivalSelection(ref List<SeatingConfiguration> population, List<SeatingConfiguration> children)
        {
            // Add children to the pool
            foreach (var child in children)
            {
                population.Add(child);
            }

            // Order the population based on absolute rank
            population = population.OrderBy(x => x.Fitness).ToList();

            // Remove same amount as added
            for (int i = 0; i < children.Count; ++i)
            {
                population.RemoveAt(population.Count - 1);
            }
        }

        /// <summary>
        /// Breeds the parents.
        /// </summary>
        /// <param name="mother">Mother.</param>
        /// <param name="father">Father.</param>
        /// <param name="firstChild">First child.</param>
        /// <param name="secondChild">Second child.</param>
        private void BreedParents(SeatingConfiguration mother, SeatingConfiguration father, out SeatingConfiguration firstChild, out SeatingConfiguration secondChild)
        {
            firstChild = null;
            secondChild = null;

            // Create list of tables for both children
            List<Table> tablesFirst = new List<Table>();
            Program.InitializeTables(tablesFirst);
            List<Table> tablesSecond = new List<Table>();
            Program.InitializeTables(tablesSecond);

            // Get the permutation form of parents
            var motherPermutation = mother.ToPermutationForm();
            var fatherPermutation = father.ToPermutationForm();

            // Perform  order 1 crossover
            Person[] firstChildPermutation;
            Person[] secondChildPermutation;
            this.Order1Crossover(motherPermutation, fatherPermutation, out firstChildPermutation);
            this.Order1Crossover(fatherPermutation, motherPermutation, out secondChildPermutation);

            // Perform mutation
            this.SwapMutation(firstChildPermutation);
            this.SwapMutation(secondChildPermutation);

            // Create the seating configuration
            firstChild = new SeatingConfiguration(tablesFirst, firstChildPermutation);
            secondChild = new SeatingConfiguration(tablesSecond, secondChildPermutation);
        }

        /// <summary>
        /// Order1s the crossover.
        /// </summary>
        /// <param name="mother">Mother.</param>
        /// <param name="father">Father.</param>
        /// <param name="child">Child.</param>
        private void Order1Crossover(Person[] mother, Person[] father, out Person[] child)
        {
            int arraySize = mother.Length;

            child = new Person[arraySize];

            // Create the guest list
            List<Person> guestList = new List<Person>();
            Program.InitializeGuestList(guestList);

            // Choose a random index to start
            int index = randomSeat.Next(arraySize);
            int swathSize = arraySize / 2;

            // Copy Swath from mother into child
            for (int i = 0; i < swathSize; ++i)
            {
                child[index] = this.GetGuest(guestList, mother[index].Identity);
                index++;

                if (index == arraySize)
                {
                    index = 0;
                }
            }

            // Copy over father alleles
            int allelesLeft = arraySize - swathSize;
            int fatherIndex = index;
            while (allelesLeft != 0)
            {
                // check to see if child does not already contain father allele
                if (!GeneticAlgorithm.DoesContainAllele(child, father[fatherIndex]))
                {
                    child[index] = this.GetGuest(guestList, father[fatherIndex].Identity);
                    index++;
                    fatherIndex++;
                    allelesLeft--;
                }
                // if not increment father index
                else
                {
                    fatherIndex++;
                }

                // Turn around if index is too big
                index = index == arraySize ? 0 : index;
                fatherIndex = fatherIndex == arraySize ? 0 : fatherIndex;
            }
        }

        /// <summary>
        /// Doeses the contain allele.
        /// </summary>
        /// <returns><c>true</c>, if contain allele was doesed, <c>false</c> otherwise.</returns>
        /// <param name="child">Child.</param>
        /// <param name="allele">Allele.</param>
        private static bool DoesContainAllele(Person[] child, Person allele)
        {
            for (int i = 0; i < child.Length; ++i)
            {
                if (child[i] != null && child[i].Identity == allele.Identity)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Swaps the mutation.
        /// </summary>
        /// <param name="child">Child.</param>
        private void SwapMutation(Person[] child)
        {
            //TODO: Currently hardcoded to swap 2 pairs, should be a percentage of permutation size
            int index1 = randomSeat.Next(child.Length);
            int index2 = randomSeat.Next(child.Length);
            int index3 = randomSeat.Next(child.Length);
            int index4 = randomSeat.Next(child.Length);


            var temp = child[index1];
            child[index1] = child[index2];
            child[index2] = temp;

            var temp1 = child[index3];
            child[index3] = child[index4];
            child[index4] = temp1;
        }

        /// <summary>
        /// Gets the guest.
        /// </summary>
        /// <returns>The guest.</returns>
        /// <param name="guestList">Guest list.</param>
        /// <param name="id">Identifier.</param>
        private Person GetGuest(List<Person> guestList, int id)
        {
            foreach (var guest in guestList)
            {
                if (guest.Identity == id)
                {
                    return guest;
                }
            }

            return null;
        }

        /// <summary>
        /// Measures the diversity.
        /// </summary>
        /// <returns>The diversity.</returns>
        /// <param name="seatingA">Seating a.</param>
        /// <param name="seatingB">Seating b.</param>
        public int MeasureDiversity(SeatingConfiguration seatingA, SeatingConfiguration seatingB)
        {
            int diversity = 0;

            foreach (var guest in seatingA.GuestList)
            {
                bool seatingMatched = false;
                int positionMatched = 0; // 0 for nextToR and 1 for nextToL

                int idGuestRightA = seatingA.GetTableOfGuest(guest.Identity).IdOfNextGuest(guest.Identity) < 0 ? 
                                            0 : seatingA.GetTableOfGuest(guest.Identity).IdOfNextGuest(guest.Identity);
                int idGuestLeftA = seatingA.GetTableOfGuest(guest.Identity).IdOfPreviousGuest(guest.Identity) < 0 ?
                                           0 : seatingA.GetTableOfGuest(guest.Identity).IdOfPreviousGuest(guest.Identity);
                int idGuestRightB = seatingB.GetTableOfGuest(guest.Identity).IdOfNextGuest(guest.Identity) < 0 ?
                                            0 : seatingB.GetTableOfGuest(guest.Identity).IdOfNextGuest(guest.Identity);
                int idGuestLeftB = seatingB.GetTableOfGuest(guest.Identity).IdOfPreviousGuest(guest.Identity) < 0 ?
                                           0 : seatingB.GetTableOfGuest(guest.Identity).IdOfPreviousGuest(guest.Identity);

                int temp = 0;

                // Do not bother with empty seats
                if (!(guest.Identity < 0))
                {
                    
                    if (idGuestRightA != idGuestRightB && idGuestRightA != idGuestLeftB)
                    {
                        diversity++;
                    }
                    else
                    {
                        seatingMatched = true;
                        if (idGuestRightA == idGuestRightB)
                        {
                            positionMatched = 0;
                        }
                        else
                        {
                            positionMatched = 1;
                        }
                    }

                    if (!seatingMatched)
                    {
                        if (idGuestLeftA != idGuestRightB && idGuestLeftA != idGuestLeftB)
                        {
                            diversity++;
                        }
                    }
                    else
                    {
                        if (positionMatched == 0 && idGuestLeftA != idGuestLeftB)
                        {
                            diversity++;
                        }
                        else if (positionMatched == 1 && idGuestLeftA != idGuestRightB)
                        {
                            diversity++;
                        }
                    }

                    // Diversity if guests are same table
                    // Get table of guest
                    Table tableGuestA = seatingA.GetTableOfGuest(guest.Identity);
                    Table tableGuestB = seatingB.GetTableOfGuest(guest.Identity);
                    List<int> idsA = tableGuestA.GetIdsOfSittingGuests();
                    idsA.Remove(guest.Identity);

                    foreach (int id in idsA)
                    {
                        if (!(id < 0) && tableGuestB.IsGuestSitting(id))
                        {
                            temp++;
                        }
                    }

                    int commonEmpty = Math.Min(tableGuestA.GetNumberOfEmptySeats(), tableGuestB.GetNumberOfEmptySeats());
                    temp = temp + commonEmpty;

                    diversity = diversity + (tableGuestA.NumberOfSeats - 1 - temp);
                }
                    
            }

            return diversity;
        }
    }
}
