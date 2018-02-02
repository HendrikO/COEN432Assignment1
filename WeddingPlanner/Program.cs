using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WeddingPlanner
{
    public class Program
    {
        /// <summary>
        /// The size of the table.
        /// </summary>
        public static double tableSize;

        /// <summary>
        /// The number of guests.
        /// </summary>
        public static double numberOfGuests;

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            // Initialize the parameters
            InitializeParameters();

            // Create Population
            InitializePopulation(20, out List<SeatingConfiguration> population);

            // Start the algorithm
            StartEvolution(ref population, false);

            // Create Population
            InitializePopulation(20, out List<SeatingConfiguration> populationDiverse);

            // Start the algorithm
            StartEvolution(ref populationDiverse, true);

            Console.WriteLine(string.Format("Diversity no enhancement: {0}", CalculateDiversity(population)));
            Console.WriteLine(string.Format("Diversity with enhancement: {0}", CalculateDiversity(populationDiverse)));
        }

        /// <summary>
        /// Calculates the diversity.
        /// </summary>
        /// <returns>The diversity.</returns>
        /// <param name="population">Population.</param>
        private static int CalculateDiversity(List<SeatingConfiguration> population)
        {
            int diversity = 0;
            for (int i = 0; i < 5; ++i)
            {
                for (int j = i + 1; j < 5; ++j)
                {
                    diversity = diversity + GeneticAlgorithm.Instance.MeasureDiversity(population[i], population[j]);
                }
            }

            return diversity;
        }

        /// <summary>
        /// Initializes the parameters.
        /// </summary>
        private static void InitializeParameters()
        {
            // Read settings.txt file to get size of table and number of guest
            // Get file path
            var directory = Directory.GetCurrentDirectory();
            var file = directory.Replace("WeddingPlanner", "HelpFiles") + "/settings.txt";
            var lines = File.ReadAllLines(file);

            // Read first line which contains table size
            // k: <tablesize>
            tableSize = Convert.ToInt32(lines[0].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1]);

            // Read second line which contains number of guests
            // n: <numberofguests>
            numberOfGuests = Convert.ToInt32(lines[1].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries)[1]);
        }

        /// <summary>
        /// Initializes the guest list.
        /// </summary>
        /// <param name="guestList">Guest list.</param>
        public static void InitializeGuestList(List<Person> guestList)
        {
            // Read guests.csv file to get size of table and number of guest
            // Get file path
            var directory = Directory.GetCurrentDirectory();
            var file = directory.Replace("WeddingPlanner", "HelpFiles") + "/guests.csv";

            var fileContent = File.ReadAllLines(file);
            var guestIdenties = fileContent[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            // Loop through every guest row by row
            for (int i = 0; i < guestIdenties.Length; ++i)
            {
                // Create a new guest and assign its ID
                Person person = new Person();
                person.Identity = Convert.ToInt32(guestIdenties[i]);

                var row = fileContent[i + 1].Split(new char[] { ',' });

                // Loop through all of the values
                // First value is the current guest's identity
                for (int j = 1; j < row.Length; ++j)
                {
                    // If no value then its comparing the same guest
                    if (!string.IsNullOrEmpty(row[j]))
                    {
                        person.Relationships.Add(
                        new KeyValuePair<int, int>(
                            j,
                            Convert.ToInt32(row[j])));
                    }
                }
                guestList.Add(person);
            }

            // Add empty seats
            double numberOfTables = numberOfGuests / tableSize;
            numberOfTables = Math.Ceiling(numberOfTables);
            double numEmptySeats = numberOfTables * tableSize - numberOfGuests;

            int emptySeatID = -1;
            for (int i = 0; i < numEmptySeats; ++i)
            {
                Person empty = new Person();
                empty.Identity = emptySeatID--;
                guestList.Add(empty);
            }
        }

        /// <summary>
        /// Initializes the tables.
        /// </summary>
        /// <param name="tables">Tables.</param>
        public static void InitializeTables(List<Table> tables)
        {
            // Calculate the number of tables needed
            double numberOfTables = numberOfGuests / tableSize;
            numberOfTables = Math.Ceiling(numberOfTables);

            for (int i = 0; i < numberOfTables; ++i)
            {
                tables.Add(new Table((int)tableSize));
            }
        }

        /// <summary>
        /// Creates the population.
        /// </summary>
        /// <param name="populationSize">Population size.</param>
        /// <param name="configs">Configs.</param>
        private static void InitializePopulation(int populationSize, out List<SeatingConfiguration> configs)
        {
            configs = new List<SeatingConfiguration>();

            for (int i = 0; i < populationSize; ++i)
            {
                // Create list of tables
                List<Table> tables = new List<Table>();
                InitializeTables(tables);

                // Create the guest list
                List<Person> guestList = new List<Person>();
                InitializeGuestList(guestList);

                // Create a config
                SeatingConfiguration config = new SeatingConfiguration(tables, guestList);

                // Add it to the population
                configs.Add(config);
            }
        }

        /// <summary>
        /// Starts the evolution.
        /// </summary>
        private static void StartEvolution(ref List<SeatingConfiguration> population, bool isDiversityEnhanced)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            int bestFitnessAchieved = population[0].Fitness;
            int generation = 0;
            int iterations = 0;

            Console.WriteLine(string.Format("Generation: {0} fitness: {1}", generation, bestFitnessAchieved));

            while (bestFitnessAchieved != 0 || iterations == 5000)
            {
                if (isDiversityEnhanced)
                {
                    GeneticAlgorithm.Instance.NextGenerationDiversityPreservation(ref population);
                }
                else
                {
                    GeneticAlgorithm.Instance.NextGeneration(ref population);    
                }

                generation++;
                iterations++;

                if (population[0].Fitness < bestFitnessAchieved)
                {
                    bestFitnessAchieved = population[0].Fitness;
                    iterations = 0;
                    Console.WriteLine(string.Format("Generation: {0}", generation));
                    Console.WriteLine(string.Format("Fitness: {0}", bestFitnessAchieved));
                }
            }

            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("Complete! ");
            Console.WriteLine("RunTime " + elapsedTime + Environment.NewLine);
        }
    }
}
