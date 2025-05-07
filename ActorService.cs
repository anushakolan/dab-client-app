using Bogus;
using Microsoft.DataApiBuilder.Rest;

namespace ConsoleApp
{
	class ActorService
	{
		const string BASE_URL = "http://localhost:5000/api/{0}";
		private static readonly TableRepository<Actor> actorRepository = new(new(string.Format(BASE_URL, "Actor")));
		private static readonly List<Actor> newActors = [];
		private static List<int> userEnteredIds = [];

		public static async Task Main()
		{
			while (true)
			{
				Console.WriteLine("\n=================================================");
				Console.WriteLine("What task to you want to perform?");
				Console.WriteLine("1. Get Actor Details");
				Console.WriteLine("2. Update Actor Details");
				Console.WriteLine("3. Create Actors");
				Console.WriteLine("4. Delete Actors");
				Console.WriteLine("5. Exit");
				Console.WriteLine("=================================================\n");

				var choice = Console.ReadLine();

				switch (choice)
				{
					case "1":
						await GetActorDetails();
						break;
					case "2":
						await UpdateActorDetails();
						break;
					case "3":
						await AddNewActors();
						break;
					case "4":
						await DeleteActors();
						break;
					case "5":
						return; // Exit the loop and end the program
					default:
						Console.WriteLine("Invalid choice. Please try again.");
						break;
				}
			}

		}

		/*
		* API to get Actor details.
		*/
		static async Task GetActorDetails()
		{
			DabResponse<Actor, Actor[]> actor = await actorRepository.GetAsync();

			foreach (var eachActor in actor.Result)
			{
				Console.WriteLine($"Here is the actor Name: {eachActor.Name}");
				Console.WriteLine($"Here is the actor Id: {eachActor.Id}");
				Console.WriteLine($"Here is the actor BirthYear: {eachActor.BirthYear}");
				Console.WriteLine($"=================================================");
			}
		}

		/*
		 * API to update Actor details.
		 */
		static async Task UpdateActorDetails()
		{
			Console.WriteLine($"Updating actors name, with the year suffix");

			DabResponse<Actor, Actor[]> existingActors = await actorRepository.GetAsync();
			Console.WriteLine("These are ids of existing actors. Choose an Id to update the name with the year suffix.");
			await GetIdsFromUserInput();

			foreach (var eachActor in existingActors.Result)
			{
				if (userEnteredIds.Contains(eachActor.Id))
				{
					string suffix = $"- Born {eachActor.BirthYear}";
					string temp = eachActor.Name;
					int index = temp.IndexOf('-');
					if (index >= 0)
					{
						eachActor.Name = eachActor.Name[..index];
					}

					Actor updateActor = new()
					{
						Id = eachActor.Id,
						Name = $"{eachActor.Name}{suffix}",
						BirthYear = eachActor.BirthYear
					};

					DabResponse<Actor, Actor> response = await actorRepository.PutAsync(updateActor);

					if (response.Success)
					{
						Console.WriteLine("Actor updated successfully.");
					}
					else
					{
						Console.WriteLine($"Failed to update actor.");
					}
				}
			}


		}

		/*
		* API to create Actor details.
		*/
		static async Task AddNewActors()
		{
			GenerateNewActors();

			Console.WriteLine($"\nAdding {newActors.Count} new actors to DB");

			foreach (var eachActor in newActors)
			{
				DabResponse<Actor, Actor> response = await actorRepository.PostAsync(eachActor);

				if (response.Success)
				{
					Console.WriteLine("\nActor created successfully.");
				}
				else
				{
					Console.WriteLine($"\nFailed to create actor.");
				}
			}
		}

		/*
		* API to delete Actor details.
		*/
		static async Task DeleteActors()
		{
			Console.WriteLine("\nThese are ids of existing actors. Choose an Id to delete.\n");
			await GetIdsFromUserInput();

			DabResponse<Actor, Actor[]> existingActors = await actorRepository.GetAsync();

			foreach (var eachActor in existingActors.Result)
			{
				if (userEnteredIds.Contains(eachActor.Id))
				{
					Console.WriteLine($"\nDeleting [{eachActor.Id}].");
					DabResponse response = await actorRepository.DeleteAsync(eachActor);

					if (response.Success)
					{
						Console.WriteLine($"Actor [{eachActor.Id} {eachActor.Name}] deleted successfully.");
					}
					else
					{
						Console.WriteLine($"\nFailed to delete actor [{eachActor.Id} {eachActor.Name}].");
					}
				}
			}
		}

		/*
		* Helper function to get user input.
		*/
		static async Task GetIdsFromUserInput()
		{
			DabResponse<Actor, Actor[]> existingActors = await actorRepository.GetAsync();
			foreach (var eachActor in existingActors.Result)
			{
				Console.WriteLine($"[{eachActor.Id} {eachActor.Name}] \t");
			}

			Console.WriteLine("\nEnter a list of comma-separated integers:");

			// Read the input from the user
			string? input = Console.ReadLine();

			if (string.IsNullOrWhiteSpace(input))
			{
				Console.WriteLine("\nInput cannot be null or empty. Please try again.");
				return;
			}

			// Split the input string into an array of substrings
			string[] values = input.Split(',');

			// Convert each substring to an integer and store in a list
			userEnteredIds = [.. values.Select(int.Parse)];
		}

		/*
		* Helper function to generate new actors.
		*/
		static void GenerateNewActors()
		{
			newActors.Clear();

			Random random = new();
			int count = random.Next(1, 10);
			Console.WriteLine($"Generating {count} new actors.");

			int[] randomIds = new int[count];
			int[] randomYears = new int[count];
			for (int i = 0; i < count; i++)
			{
				randomIds[i] = random.Next(0, int.MaxValue);
				randomYears[i] = random.Next(1960, 2015);
			}

			// Create a Faker instance for generating random names
			var faker = new Faker();

			for (int i = 0; i < count; i++)
			{
				Actor actor = new()
				{
					Id = randomIds[i],
					Name = faker.Name.FullName(),
					BirthYear = randomYears[i]
				};

				newActors.Add(actor);
			}
		}
	}
}