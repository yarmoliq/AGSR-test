using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Bogus;
using PatientApiConsole.Models;

namespace PatientApiConsole
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient { BaseAddress = new Uri("https://localhost:44355") };

        static async Task Main(string[] args)
        {
            var patients = GeneratePatients(100);
            foreach (var patient in patients)
            {
                await PostPatientAsync(patient);
            }
            Console.WriteLine("All patients have been added.");
        }

        static List<Patient> GeneratePatients(int count)
        {
            var genderOptions = new[] { Gender.Male, Gender.Female, Gender.Other, Gender.Unknown };
            var faker = new Faker("en");

            var patients = new List<Patient>();
            for (int i = 0; i < count; i++)
            {
                var name = new Name
                {
                    Use = "official",
                    Family = faker.Name.LastName(),
                    Given = new List<string> { faker.Name.FirstName(), faker.Name.FirstName() }
                };
                var patient = new Patient
                {
                    Name = name,
                    Gender = faker.PickRandom(genderOptions),
                    BirthDate = faker.Date.Past(5, DateTime.Now).Date,
                    Active = faker.Random.Bool()
                };
                patients.Add(patient);
            }

            return patients;
        }

        static async Task PostPatientAsync(Patient patient)
        {
            var response = await client.PostAsJsonAsync("/api/Patients", patient);
            response.EnsureSuccessStatusCode();
            Console.WriteLine($"Added patient: {patient.Name.Family}");
        }
    }
}
