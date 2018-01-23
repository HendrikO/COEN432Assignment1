﻿using System;
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
            // Sort the list
            population = population.OrderBy(x => x.Fitness).ToList();

            // Cross Selection with 2 fittest as parents
            this.BreedParents(population[0], population[1], out SeatingConfiguration firstChild, out SeatingConfiguration secondChild);


            // New population has the 5 fittests configs out of the current 7
            population.Add(firstChild);
            population.Add(secondChild);
            population = population.OrderBy(x => x.Fitness).ToList();
            population.RemoveAt(population.Count - 1);
            population.RemoveAt(population.Count - 1);
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

        //private Person[] 

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

            // Choose a random index to start
            int index = randomSeat.Next(arraySize);
            int swathSize = arraySize / 2;

            // Copy Swath from mother into child
            for (int i = 0; i < swathSize; ++i)
            {
                child[index] = mother[index];
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
                    child[index] = father[fatherIndex];
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
    }
}
