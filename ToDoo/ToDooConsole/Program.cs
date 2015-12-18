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

                    string returnedString; // To be used with methods that return a string



                    //
                    // Test the GetToDo WCF method
                    //

                    Console.WriteLine("Calling GetToDo via HTTP GET: ");
                    var name = "Charlie";

                    // This is it! We are using the WCF service method GetToDo through the channel object.
                    // The method is returning a list of ToDo objects.
                    List<ToDo> aToDo = channel.GetToDo(name);

                    // Now we just have to output some text to the console so that it looks like we have
                    // been busy doing a lot of stuff...
                    Console.WriteLine($"   Output for todolist {name}:");

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
                                                           

                    // This is instructions to the user of our console application that the information
                    // can also be retrieved with an internet browser.
                    Console.WriteLine("");
                    Console.WriteLine("This can also be accomplished by navigating to");
                    Console.WriteLine($"http://localhost:8000/GetToDo/{name}");
                    Console.WriteLine("in a web browser while this sample is running.");

                    Console.WriteLine("");



                    //
                    //  Test the CreateToDo WCF method.
                    //

                    var aNewToDoItem = new ToDo();
                    DateTime tempDateTime;

                    // The DateTime.Tryparse will return TRUE if successful
                    // or FALSE if it went wrong.
                    // If it was successful, the converted date will be put into the tempDateTime variable
                    var weWereAbleToConvertTheStringToDateTime = DateTime.TryParse("2016-01-01", out tempDateTime);

                    if(weWereAbleToConvertTheStringToDateTime)
                    {
                        aNewToDoItem.DeadLine = tempDateTime; // This will happen if the conversion went well
                    }
                    else
                    {
                        aNewToDoItem.DeadLine = DateTime.Now; // This is our fallback if the conversion failed
                    }

                    // And here we set the rest of the properties for the ToDo-item we are about to create
                    aNewToDoItem.CreatedDate = DateTime.Now;
                    aNewToDoItem.Name = "Charlie";
                    aNewToDoItem.Finnished = false;   // Finnished...rotflmao!
                    aNewToDoItem.EstimationTime = 60;
                    aNewToDoItem.Description = "Kärna smör!";


                    // Here we do the call to actually create the ToDo-item using the WCF CreateToDO method
                    if ( channel.CreateToDo("Charlie",aNewToDoItem))
                    {
                        // This is the success message. It should be successful.
                        Console.WriteLine($"Vi lyckades lägga till ett todo item med namn {aNewToDoItem.Name}" );
                    }
                    else
                    {
                        // This happens if the CreateToDo WVF method is unhappy and return a FALSE. 
                        Console.WriteLine($"Vi misslyckades med att lägga till ett todo item med namn {aNewToDoItem.Name}" );
                    }


                    // To demonstrate, we try to create another ToDO-item, but because the CreateToDO method
                    // will refuse to add Items if the name parameter (Chaplin in this case) is not the same
                    // as the aNewToDoItem.Name (CHarlie in this case)
                    if (channel.CreateToDo("Chaplin", aNewToDoItem))
                    {
                        Console.WriteLine($"Vi lyckades lägga till ett todo item med namn {aNewToDoItem.Name}");
                    }
                    else
                    {
                        // Because of our "mistake" explained above, this will happen.
                        Console.WriteLine($"Vi misslyckades med att lägga till ett todo item med namn {aNewToDoItem.Name}");
                    }





                    //
                    // Testing the RevealALlMySecrets WCF method
                    //

                    Console.WriteLine("Calling RevealAllMySecrets via HTTP GET: ");
                    returnedString = channel.RevealAllMySecrets("wrong_password");
                    Console.WriteLine($"   Output: {returnedString}");
                    Console.WriteLine();

                                        
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

    }


    
}
