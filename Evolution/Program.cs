using System;
using System.Collections.Generic;
using System.Linq;

namespace EvolutionSimulationWithNN
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Parameter der Simulation
            int populationSize = 100;
            int genomeLength = 10;
            int generations = 100;
            double mutationRate = 0.01;

            // Erzeuge die Startpopulation
            Population population = new Population(populationSize, genomeLength, mutationRate);

            for (int generation = 0; generation < generations; generation++)
            {
                // Simuliere zufällige Umweltbedingungen
                double environmentFactor = EnvironmentCondition();

                // Population basierend auf den Umweltbedingungen weiterentwickeln
                population.Evolve(environmentFactor);
                Console.WriteLine($"Generation {generation + 1}: Max Fitness = {population.GetMaxFitness()}");
            }
        }

        // Eine Funktion, die zufällige Umweltbedingungen simuliert
        static double EnvironmentCondition()
        {
            // Beispiel: zufälliger Wert zwischen -1 und 1, der die Umweltbedingungen verändert
            Random random = new Random();
            return random.NextDouble() * 2 - 1;
        }
    }

    public class Individual
    {
        public double[] Genome { get; private set; }
        public double Fitness { get; private set; } // Deklariere die Fitness-Eigenschaft

        private static Random random = new Random();

        public Individual(int genomeLength)
        {
            Genome = new double[genomeLength];
            // Initialisiere Genome mit zufälligen Werten zwischen -1 und 1
            for (int i = 0; i < genomeLength; i++)
                Genome[i] = random.NextDouble() * 2 - 1;
            CalculateFitness(0); // Initiale Fitness berechnen
        }

        public Individual(double[] genome)
        {
            Genome = genome;
            CalculateFitness(0); // Fitness beim Erstellen des Individuums berechnen
        }

        // Fitness basierend auf Anpassung an die Umwelt berechnen
        public void CalculateFitness(double environmentFactor)
        {
            // Neuronales Netzwerk verwenden, um eine Entscheidung zu treffen
            double networkOutput = NeuralNetworkOutput(environmentFactor);

            // Fitness berechnen, die die Anpassung an die Umwelt belohnt
            Fitness = 1.0 - Math.Abs(networkOutput - environmentFactor);
        }

        // Einfaches neuronales Netzwerk zur Berechnung der Anpassungsfähigkeit
        private double NeuralNetworkOutput(double input)
        {
            double sum = 0;
            for (int i = 0; i < Genome.Length; i++)
            {
                sum += Genome[i] * input;
            }
            return Math.Tanh(sum); // Aktivierungsfunktion (Tanh zur Normalisierung zwischen -1 und 1)
        }

        // Mutieren des Genoms
        public Individual Mutate(double mutationRate)
        {
            double[] newGenome = (double[])Genome.Clone();
            for (int i = 0; i < newGenome.Length; i++)
            {
                if (random.NextDouble() < mutationRate)
                {
                    // Mutation: Kleiner zufälliger Wert hinzufügen oder subtrahieren
                    newGenome[i] += (random.NextDouble() * 2 - 1) * 0.1; // Anpassung an Mutation
                }
            }
            return new Individual(newGenome);
        }
    }

    public class Population
    {
        public List<Individual> Individuals { get; private set; }
        private double mutationRate;
        private Random random = new Random();

        public Population(int populationSize, int genomeLength, double mutationRate)
        {
            Individuals = new List<Individual>();
            this.mutationRate = mutationRate;

            for (int i = 0; i < populationSize; i++)
                Individuals.Add(new Individual(genomeLength));
        }

        // Evolution einer Generation, angepasst an die Umweltbedingungen
        public void Evolve(double environmentFactor)
        {
            // Fitness berechnen basierend auf der aktuellen Umwelt
            foreach (var individual in Individuals)
            {
                individual.CalculateFitness(environmentFactor);
            }

            // Selektiere die besten Individuen zur Fortpflanzung (z.B. 50%)
            List<Individual> selectedIndividuals = Individuals
                .OrderByDescending(ind => ind.Fitness)
                .Take(Individuals.Count / 2)
                .ToList();

            // Erzeuge die neue Generation
            List<Individual> newGeneration = new List<Individual>();

            // Fülle die Population durch Fortpflanzung der ausgewählten Individuen
            while (newGeneration.Count < Individuals.Count)
            {
                // Zwei zufällig ausgewählte Eltern
                Individual parent1 = selectedIndividuals[random.Next(selectedIndividuals.Count)];
                Individual parent2 = selectedIndividuals[random.Next(selectedIndividuals.Count)];

                // Erzeuge ein Kind und mutiere es
                Individual child = Crossover(parent1, parent2).Mutate(mutationRate);
                newGeneration.Add(child);
            }

            Individuals = newGeneration;
        }

        // Einfacher Crossover
        private Individual Crossover(Individual parent1, Individual parent2)
        {
            double[] childGenome = new double[parent1.Genome.Length];
            for (int i = 0; i < childGenome.Length; i++)
            {
                childGenome[i] = random.NextDouble() < 0.5 ? parent1.Genome[i] : parent2.Genome[i];
            }
            return new Individual(childGenome);
        }

        // Maximal erreichte Fitness in der Population
        public double GetMaxFitness()
        {
            return Individuals.Max(ind => ind.Fitness);
        }
    }
}
