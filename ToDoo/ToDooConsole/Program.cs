using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Web;
using WcfToDoService;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.ServiceModel.Description;
using ToDoBase;

namespace ToDoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            TestWcfService();
            
        }

        // This is where we do all our stuff
        static void TestWcfService()
        {
            // The host is the object that runs services on port 8000
            WebServiceHost host = new WebServiceHost(typeof(ToDoService), new Uri("http://localhost:8000/"));
            try
            {
                // The ep endpoint is listening to http requests and servicing WCF using the IToDoService interface/data contract
                ServiceEndpoint ep = host.AddServiceEndpoint(typeof(IToDoService), new WebHttpBinding(), "");

                // Our webservice is open for business.
                // All stuff that is going on with host is "our side" and would NOT be in the code on our "customers/users/front end developers" side.
                // It is just here to get our service going so we can test it.
                host.Open();



                // From here and below, we start to simulate/test stuff from the "customers/users/front end developers" point of vew.


                // First we create a ChannelFactory object cf, which in turn will be used to create a channel for communication
                // with the WCF service
                using (ChannelFactory<IToDoService> cf = new ChannelFactory<IToDoService>(new WebHttpBinding(), "http://localhost:8000"))
                {
                    cf.Endpoint.Behaviors.Add(new WebHttpBehavior());


                    // Now we ask the "Channel Factory" to create a channel object for us.
                    // The channel object will be used to communicate with the server/service we have created above.
                    IToDoService channel = cf.CreateChannel();

                  

                    // Demonstrate the methods

                    DemoGetToDo(channel, "Charlie");

                    DemoGetToDoImportant(channel, "MrInAHurry");

                    DemoGetEstimate(channel, "MrInAHurry");

                    DemoGetToDoPriority(channel, "MrInAHurry");
                    
                    DemoCreateToDo(channel, "Charlie");

                    DemoCreateToDoCSV(channel, "MrCSVTester");

                   //  DemoRevealAllMySecrets(channel);

                    DemoGetDone(channel, "Chow");

                    DemoDeleteToDo(channel, "Michele");

                    DemoSetAndCheckIfSomethingIsDone(channel, "MrDoer");



                    // Här testar man DemoEditToDo
                    Console.WriteLine("Mata in ett ID: ");
                    string myId = Console.ReadLine();
                    Console.WriteLine("Mata in en uppgift: ");
                    string desc = Console.ReadLine();
                    Console.WriteLine("Mata in ett namn: ");
                    string name = Console.ReadLine();
                    Console.WriteLine("Mata in ett deadLine: ");
                    string deadLine = Console.ReadLine();
                    Console.WriteLine("Mata in ett estimationTime: ");
                    string estimationTime = Console.ReadLine();
                    Console.WriteLine("Mata in ett finnished: ");
                    string finnished = Console.ReadLine();
                    DemoEditToDo(channel, myId, desc, name, deadLine, estimationTime, finnished);






                }


                // Let the console application run until the user choose to terminate
                // This is because the user might want the service to keep running
                // and therefore be able to test it using other tools or applications.

                Console.WriteLine("Press <ENTER> to terminate");
                Console.ReadLine();


                // Bye bye WCF service, sleep tight!
                host.Close();
            }

            catch (CommunicationException cex)
            {
                Console.WriteLine($"An exception occurred: {cex.Message}");
                host.Abort();
            }

        }


        // Test the GetToDo WCF method
        static void DemoGetToDo(IToDoService channel, string name)
        {
            
            Console.WriteLine("Calling GetToDo via HTTP GET: ");
            
            // This is it! We are using the WCF service method GetToDo through the channel object.
            // The method is returning a list of ToDo objects.
            List<ToDo> aToDo = channel.GetToDo(name);

            // Print out some information/properties from the todo item
            ViewToDoItems(aToDo, "todolist", name);

            // Display how to access them via uri
            ViewWebInstructions($"/todo/{name}");

        }


        // Test the GetToDo WCF method
        static void DemoGetToDoImportant(IToDoService channel, string name)
        {

            Console.WriteLine();
            Console.WriteLine($"DemoGetToDoImportant clearing out test data for todolist {name}");
            DeleteToDoByName(channel, name);



            Console.WriteLine($"DemoGetToDoImportant creating test data for todolist {name}");
            ToDo rowOne = new ToDo()
            {
                Name = name,
                Description = "Inte viktigt",
                CreatedDate = DateTime.Now,
                Finnished = false,
                DeadLine = DateTime.Now.AddDays(10),
                EstimationTime = 100
            };
            channel.CreateToDo(name, rowOne);


            ToDo rowTwo = new ToDo()
            {
                Name = name,
                Description = "Viktigt!",
                CreatedDate = DateTime.Now,
                Finnished = true,
                DeadLine = DateTime.Now.AddDays(20),
                EstimationTime = 100
            };
            channel.CreateToDo(name, rowTwo);


            ToDo rowThree = new ToDo()
            {
                Name = name,
                Description = "Gör detta nu!",
                CreatedDate = DateTime.Now,
                Finnished = false,
                DeadLine = DateTime.Now.AddDays(5),
                EstimationTime = 100
            };
            channel.CreateToDo(name, rowThree);


            ToDo rowFour = new ToDo()
            {
                Name = name,
                Description = "Gör detta senare...",
                CreatedDate = DateTime.Now,
                Finnished = false,
                DeadLine = DateTime.Now.AddDays(7),
                EstimationTime = 100
            };
            channel.CreateToDo(name, rowFour);

            ToDo rowFive = new ToDo()
            {
                Name = name,
                Description = "Detta har vi gjort!",
                CreatedDate = DateTime.Now,
                Finnished = true,
                DeadLine = DateTime.Now.AddDays(2),
                EstimationTime = 100
            };
            channel.CreateToDo(name, rowFive);


            Console.WriteLine("DemoGetToDoImportant Calling GetToDo - this is all the items");
            List<ToDo> aToDo = channel.GetToDo(name);
            ViewToDoItems(aToDo, "GetToDo", name);


            Console.WriteLine("DemoGetToDoImportant Calling CountToDoImportant");
            Console.WriteLine($"There is {channel.CountToDoImportant(name)} important items in the {name} list.");
            ViewWebInstructions($"/todo/{name}/count/important");
            Console.WriteLine();

            Console.WriteLine("DemoGetToDoImportant Calling CountDoneImportant");
            Console.WriteLine($"There is {channel.CountToDoImportant(name)} DONE important items in the {name} list.");
            ViewWebInstructions($"/todo/{name}/count/important");
            Console.WriteLine();


            Console.WriteLine("DemoGetToDoImportant Calling GetToDoImportant - only important items");
            aToDo = channel.GetToDoImportant(name);
            ViewToDoItems(aToDo, "GetToDoImportant", name);
            ViewWebInstructions($"/todo/{name}/important");
            Console.WriteLine();

            Console.WriteLine("DemoGetToDoImportant Calling GetDoneImportant");
            aToDo = channel.GetDoneImportant(name);
            ViewToDoItems(aToDo, "GetDoneImportant", name);
            ViewWebInstructions($"/getdone/{name}/important");
            Console.WriteLine();

            Console.WriteLine("DemoGetToDoImportant Calling GetNotDone");
            aToDo = channel.GetNotDone(name);
            ViewToDoItems(aToDo, "GetNotDone", name);
            ViewWebInstructions($"/getnotdone/{name}");
            Console.WriteLine();


            Console.WriteLine("DemoGetToDoImportant Calling GetNotDoneImportant");
            aToDo = channel.GetNotDoneImportant(name);
            ViewToDoItems(aToDo, "GetNotDoneImportant", name);
            ViewWebInstructions($"/getnotdone/{name}/important");
            Console.WriteLine();

         




        }


        static void DemoSetAndCheckIfSomethingIsDone(IToDoService channel, string name)
        {

            Console.WriteLine("****************************************");
            Console.WriteLine("This is DemoSetAndCheckIfSomethingIsDone");
            Console.WriteLine("****************************************");
            Console.WriteLine();
            Console.WriteLine($"DemoSetAndCheckIfSomethingIsDone clearing out test data for todolist {name}");
            DeleteToDoByName(channel, name);
            Console.WriteLine($"DemoSetAndCheckIfSomethingIsDone creating test data for todolist {name}");
            ToDo rowOne = new ToDo()
            {
                Name = name,
                Description = "Inte viktigt",
                CreatedDate = DateTime.Now,
                Finnished = false,
                DeadLine = DateTime.Now.AddDays(10),
                EstimationTime = 100
            };
            channel.CreateToDo(name, rowOne);


            ToDo rowTwo = new ToDo()
            {
                Name = name,
                Description = "Viktigt!",
                CreatedDate = DateTime.Now,
                Finnished = true,
                DeadLine = DateTime.Now.AddDays(20),
                EstimationTime = 100
            };
            channel.CreateToDo(name, rowTwo);


            ToDo rowThree = new ToDo()
            {
                Name = name,
                Description = "Gör detta nu!",
                CreatedDate = DateTime.Now,
                Finnished = false,
                DeadLine = DateTime.Now.AddDays(5),
                EstimationTime = 100
            };
            channel.CreateToDo(name, rowThree);


            ToDo rowFour = new ToDo()
            {
                Name = name,
                Description = "Gör detta senare...",
                CreatedDate = DateTime.Now,
                Finnished = false,
                DeadLine = DateTime.Now.AddDays(7),
                EstimationTime = 100
            };
            channel.CreateToDo(name, rowFour);

            ToDo rowFive = new ToDo()
            {
                Name = name,
                Description = "Detta har vi gjort!",
                CreatedDate = DateTime.Now,
                Finnished = true,
                DeadLine = DateTime.Now.AddDays(2),
                EstimationTime = 100
            };
            channel.CreateToDo(name, rowFive);


            Console.WriteLine($"DemoSetAndCheckIfSomethingIsDone fetching list of stuff to do for {name}");
            var someStuffToDo = channel.GetToDo(name);

            Console.WriteLine($"DemoSetAndCheckIfSomethingIsDone printing all that is not done for {name}");
            var someNotDoneStuff = channel.GetNotDone(name);
            ViewToDoItems(someNotDoneStuff, "DemoSetAndCheckIfSomethingIsDone", name);
            

            Console.WriteLine($"DemoSetAndCheckIfSomethingIsDone printing all that IS done for {name}");
            var someDoneStuff = channel.GetDone(name);
            ViewToDoItems(someDoneStuff, "DemoSetAndCheckIfSomethingIsDone", name);

            Console.WriteLine($"DemoSetAndCheckIfSomethingIsDone trying if items are Finnished = true for {name}");
            foreach(var yetAnotherTodo in someStuffToDo)
                Console.WriteLine($"ID: {yetAnotherTodo.Id} Is it finished? {channel.IsToDoDone(name, yetAnotherTodo.Id.ToString())}");

            Console.WriteLine($"DemoSetAndCheckIfSomethingIsDone trying if items are Finnished = false for {name}");
            foreach (var yetAnotherTodo in someStuffToDo)
            {
                Console.WriteLine($"ID: {yetAnotherTodo.Id} Is it NOT finished? {channel.IsToDoNotDone(name, yetAnotherTodo.Id.ToString())}");
                ViewWebInstructions($"/todo/{name}/{yetAnotherTodo.Id.ToString()}/notdone");
            }


            Console.WriteLine($"DemoSetAndCheckIfSomethingIsDone setting all previously DONE items to finnished=false for {name}");
            foreach (var yetAnotherTodo in someDoneStuff)
                channel.MarkToDoNotDone(name, yetAnotherTodo.Id.ToString());
                
            
                Console.WriteLine($"DemoSetAndCheckIfSomethingIsDone setting all previously NOT DONE items to finnished=true for {name}");
            foreach (var yetAnotherTodo in someNotDoneStuff)
                channel.MarkToDoDone(name, yetAnotherTodo.Id.ToString());


            Console.WriteLine($"DemoSetAndCheckIfSomethingIsDone trying if items are Finnished = true for {name}");
            foreach (var yetAnotherTodo in someStuffToDo)
            {
                Console.WriteLine($"ID: {yetAnotherTodo.Id} Is it finished? {channel.IsToDoDone(name, yetAnotherTodo.Id.ToString())}");
                ViewWebInstructions($"/todo/{name}/{yetAnotherTodo.Id.ToString()}/done");
            }

                Console.WriteLine($"DemoSetAndCheckIfSomethingIsDone trying if items are Finnished = false for {name}");
            foreach (var yetAnotherTodo in someStuffToDo)
                Console.WriteLine($"ID: {yetAnotherTodo.Id} Is it NOT finished? {channel.IsToDoNotDone(name, yetAnotherTodo.Id.ToString())}");


        }



        static void DemoGetEstimate(IToDoService channel, string name)
        {
            Console.WriteLine();
            Console.WriteLine("DemoGetEstimate Calling GetEstimate - for all the items");
            var estimate = channel.GetEstimate(name);
            Console.WriteLine($"It will take {estimate.TotalTime} minutes to finish all the items in {name}");
            Console.WriteLine($"so it will be done at {estimate.CompletedAt.ToString("yyyy-MM-dd HH:mm")}");
            ViewWebInstructions($"/todo/{name}/estimate");

            Console.WriteLine();
            Console.WriteLine("DemoGetEstimate Calling GetEstimateImportant - for IMPORTANT items");
            estimate = channel.GetEstimateImportant(name);
            Console.WriteLine($"It will take {estimate.TotalTime} minutes to finish all the IMPORTANT items in {name}");
            Console.WriteLine($"so it will be done at {estimate.CompletedAt.Date.ToString("yyyy-MM-dd HH:mm")}");
            ViewWebInstructions($"/todo/{name}/estimate/important");


            Console.WriteLine();
            Console.WriteLine("DemoGetEstimate Calling GetEstimateNotDone - for all the items");
            estimate = channel.GetEstimateNotDone(name);
            Console.WriteLine($"It will take {estimate.TotalTime} minutes to finish all the NOT DONE items in {name}");
            Console.WriteLine($"so it will be done at {estimate.CompletedAt.ToString("yyyy-MM-dd HH:mm")}");
            ViewWebInstructions($"/getnotdone/{name}/estimate");

            Console.WriteLine();
            Console.WriteLine("DemoGetEstimate Calling GetEstimateNotDoneImportant - for IMPORTANT items");
            estimate = channel.GetEstimateNotDoneImportant(name);
            Console.WriteLine($"It will take {estimate.TotalTime} minutes to finish all the NOT DONE IMPORTANT items in {name}");
            Console.WriteLine($"so it will be done at {estimate.CompletedAt.Date.ToString("yyyy-MM-dd HH:mm")}");
            ViewWebInstructions($"/getnotdone/{name}/estimate/important");



        }

        static void DemoGetToDoPriority(IToDoService channel, string name)
        {
            Console.WriteLine();
            Console.WriteLine("DemoGetToDoPriority Calling GetToDoPriority - for all the items");
            var aToDo = channel.GetToDoPriority(name);
            ViewToDoItems(aToDo, "GetToDoPriority", name);
            ViewWebInstructions($"/todo/{name}/priority");

            Console.WriteLine();
            Console.WriteLine("DemoGetToDoPriority Calling GetToDoPriorityImportant - for IMPORTANT items");
            aToDo = channel.GetToDoPriorityImportant(name);
            ViewToDoItems(aToDo, "GetToDoPriorityImportant", name);
            ViewWebInstructions($"/todo/{name}/priority/important");



        }


        // Test the GetDone WCF method
        static void DemoGetDone(IToDoService channel, string name)
        {
            // While testing, a lot of todo lists will be created.
            // This will clean up the database
            DeleteToDoByName(channel, name);

            ToDo rowOne = new ToDo()
            {
                Name = name,
                Description = "Sover",
                CreatedDate = DateTime.Now,
                Finnished = false,
                DeadLine = DateTime.Now,
                EstimationTime = 100
            };
            channel.CreateToDo(name, rowOne);


            ToDo rowTwo = new ToDo()
            {
                Name = name,
                Description = "Dreglar",
                CreatedDate = DateTime.Now,
                Finnished = true,
                DeadLine = DateTime.Now,
                EstimationTime = 100
            };
            channel.CreateToDo(name, rowTwo);


            ToDo rowThree = new ToDo()
            {
                Name = name,
                Description = "Skejtar",
                CreatedDate = DateTime.Now,
                Finnished = false,
                DeadLine = DateTime.Now,
                EstimationTime = 100
            };
            channel.CreateToDo(name, rowThree);


            ToDo rowFour = new ToDo()
            {
                Name = name,
                Description = "Minglar",
                CreatedDate = DateTime.Now,
                Finnished = true,
                DeadLine = DateTime.Now,
                EstimationTime = 100
            };
            channel.CreateToDo(name, rowFour);


            Console.WriteLine("Calling GetDone via HTTP GET: ");

            // This is it! We are using the WCF service method GetToDo through the channel object.
            // The method is returning a list of ToDo objects.
            List<ToDo> aToDo = channel.GetDone(name);

            // Print out some information/properties from the todo item
            ViewToDoItems(aToDo, "donelist", name);

            // This is instructions to the user of our console application that the information
            // can also be retrieved with an internet browser.
            ViewWebInstructions($"/GetDone/{name}");

        }



        //  Test the CreateToDo WCF method.
        static void DemoCreateToDo( IToDoService channel, string name)
        {
            var aNewToDoItem = new ToDo();
            DateTime tempDateTime;

            // The DateTime.Tryparse will return TRUE if successful
            // or FALSE if it went wrong.
            // If it was successful, the converted date will be put into the tempDateTime variable
            var weWereAbleToConvertTheStringToDateTime = DateTime.TryParse("2016-01-01", out tempDateTime);

            if (weWereAbleToConvertTheStringToDateTime)
            {
                aNewToDoItem.DeadLine = tempDateTime; // This will happen if the conversion went well
            }
            else
            {
                aNewToDoItem.DeadLine = DateTime.Now; // This is our fallback if the conversion failed
            }

            // And here we set the rest of the properties for the ToDo-item we are about to create
            aNewToDoItem.CreatedDate = DateTime.Now;
            aNewToDoItem.Name = name;
            aNewToDoItem.Finnished = false;   // Finnished...rotflmao!
            aNewToDoItem.EstimationTime = 60;
            aNewToDoItem.Description = "Kärna smör!";


            // While testing, a lot of todo lists will be created.
            // This will clean up the database
            DeleteToDoByName(channel, name);



            // Here we do the call to actually create the ToDo-item using the WCF CreateToDO method
            if (channel.CreateToDo(name, aNewToDoItem) != -1)
            {
                // This is the success message. It should be successful.
                Console.WriteLine($"Vi lyckades lägga till ett todo item med namn {aNewToDoItem.Name}");
            }
            else
            {
                // This happens if the CreateToDo WVF method is unhappy and return a FALSE. 
                Console.WriteLine($"Vi misslyckades med att lägga till ett todo item med namn {aNewToDoItem.Name}");
            }


            // To demonstrate, we try to create another ToDO-item, but because the CreateToDO method
            // will refuse to add Items if the name parameter (Chaplin in this case) is not the same
            // as the aNewToDoItem.Name (CHarlie in this case)
            if (channel.CreateToDo(name+"other", aNewToDoItem) != -1)
            {
                Console.WriteLine($"Vi lyckades lägga till ett todo item med namn {aNewToDoItem.Name}");
            }
            else
            {
                // Because of our "mistake" explained above, this will happen.
                Console.WriteLine($"Vi misslyckades med att lägga till ett todo item med namn {aNewToDoItem.Name}");
            }
        }

        // Testing the CreateToDoCSV method
        static void DemoCreateToDoCSV(IToDoService channel, string name)
        {
            // While testing, a lot of todo lists will be created.
            // This will clean up the database
            DeleteToDoByName(channel, name);

            Console.WriteLine();
            Console.WriteLine($"Creating a new ToDo list for {name} using a comma separated list of items");


            if (channel.CreateToDoCSV(name, "skruvmejsel, hammare, såg, skruvstäd", "2016-02-01", "20"))
            {
                Console.WriteLine("All items added successfully");
            }
            else
            {
                Console.WriteLine("At least some items did not get added. Possibly because they were duplicates");
            }
            Console.WriteLine("");
            Console.WriteLine($"Using DemoGetToDo to get the list of items for {name}:");

            DemoGetToDo(channel, name);
        }

        //  Test the DeleteToDo() method.
        static void DemoDeleteToDo(IToDoService channel, string name)
        {
            // Create a ToDo item to delete
            Console.WriteLine("Creating a ToDo item");
            var toDoItem = new ToDo();
            toDoItem.DeadLine = DateTime.Now;

            toDoItem.CreatedDate = DateTime.Now;
            toDoItem.Name = name;
            toDoItem.Finnished = false;
            toDoItem.EstimationTime = 1;
            toDoItem.Description = "Delete Me!";
            channel.CreateToDo(name, toDoItem);

            // Get the last ID of that users ToDo
            List<ToDo> aToDo = channel.GetToDo(name);
            string lastId = Convert.ToString(aToDo.Last().Id);
            Console.WriteLine($"Created ToDo for {name} with ID {lastId} ");

            // Delete the last ToDo item
            channel.DeleteToDoByID(name, lastId);
            Console.WriteLine($"Item with {lastId} deleted");
        }

        //// Testing the RevealALlMySecrets WCF method
        //static void DemoRevealAllMySecrets(IToDoService channel)
        //{
        //    Console.WriteLine("Calling RevealAllMySecrets via HTTP GET: ");
        //    var returnedString = channel.RevealAllMySecrets("wrong_password");
        //    Console.WriteLine($"   Output: {returnedString}");
        //    Console.WriteLine();
        //}

        static void DeleteToDoByName(IToDoService channel, string name)
        {

            // While testing, a lot of todo lists will be created.
            // This will clean up the database
            if (channel.DeleteToDoByName(name)) Console.WriteLine($"Successfully removed all todos with name {name} from the database.");
            else Console.WriteLine($"Tried to remove all todos for {name} but failed, or someone else managed to add new todos at the same time");


        }


        // Test the EditToDo WCF method
        static void DemoEditToDo(IToDoService channel,
                                 string id,
                                 string description,
                                 string name,
                                 string deadLine,
                                 string estimationTime,
                                 string finnished)
        {

            Console.WriteLine("\n\nCalling EditToDo via HTTP PUT: ");

            bool status = false;
            status = channel.EditToDo(id, description, name, deadLine, estimationTime, finnished);

            if (status != true)
            {
                Console.WriteLine("Det gick inte att köra EditToDo");
            }
            else
            {
                Console.WriteLine("Det gick BRA att köra EditToDo");
            }

            if (id == "") { id = "%20"; } else { id = Uri.EscapeDataString(id); }
            if (description == "") { description = "%20"; } else { description = Uri.EscapeDataString(description); }
            if (name == "") { name = "%20"; } else { name = Uri.EscapeDataString(name); }
            if (deadLine == "") { deadLine = "%20"; } else { deadLine = Uri.EscapeDataString(deadLine); }
            if (estimationTime == "") { estimationTime = "%20"; } else { estimationTime = Uri.EscapeDataString(estimationTime); }
            if (finnished == "") { finnished = "%20"; } else { finnished = Uri.EscapeDataString(finnished); }
            // Display how to access them via uri
            ViewWebInstructions($"/EditToDo/{id}/{description}/{name}/{deadLine}/{estimationTime}/{finnished}");

        } // EditToDo WCF method slutar här


        // Shows instructions to the user of our console application that the information
        // can also be retrieved with an internet browser.
        static void ViewWebInstructions(string urlPart)
        {

            Console.WriteLine("");
            Console.WriteLine("This can also be accomplished by navigating to");
            Console.WriteLine($"http://localhost:8000{urlPart}");
            Console.WriteLine("in a web browser while this sample is running.");
            Console.WriteLine();

        }


        // Prints out some information/properties from the todo items
        static void ViewToDoItems(List<ToDo> aToDo, string demoMethodName, string name)
        {
            Console.WriteLine($"   Output for {demoMethodName} {name}:");

            // Print out some information/properties from the todo item
            foreach (var todoItem in aToDo)
            {
                Console.WriteLine($"ID: {todoItem.Id}");
                Console.WriteLine($"Beskrivning: {todoItem.Description}");
                Console.WriteLine($"Skapad datum: {todoItem.CreatedDate}");
                Console.WriteLine($"Deadline: {todoItem.DeadLine}");
                Console.WriteLine($"Uppskattad tid att utföra: {todoItem.EstimationTime}");
                Console.WriteLine($"Färdig? {todoItem.Finnished}");
                Console.WriteLine("");
            };

        }


    }


    
}
