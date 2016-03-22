
using Dav.Ef.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dav.Ef
{
    class Program
    {
        static void Main(string[] args)
        {
            // NOTE: do not really do this, it's just for the demo so that database is in a consistent state.
            System.Data.Entity.Database.SetInitializer(new DropCreateDatabaseAlways<iSwoonDbContext>());

            DemoCreateAndStuff();

            DisplayResults();

            Console.ReadLine();
        }

        static void DemoCreateAndStuff()
        {
            var demo = new EfDbDemo();

            demo.CreateDateAndEvent(1, "test", new[] { 1, 3, 5 });


            demo.CreateDate(2);
            demo.CreateEvent("Test 2", new[] { 2, 3, 4, 5, 6 });

            var date = demo.GetDateByNumber(2);
            var evt = demo.GetEventByDescription("Test 2");
            demo.AddEventToDate(date, evt);

            demo.CreateDate(3);
            date = demo.GetDateByNumber(3);
            demo.AddEventToDate(date, evt);
            demo.AddEventToDateByNumberAndDescription(3, "test");
        }

        static void DisplayResults()
        {
            var demo = new EfDbDemo();

            var dates = demo.GetAllDates();
            foreach (var date in dates)
            {
                Console.WriteLine(date.DateNumber);
                // NOTE: We did not define an incldue for date.DateEvents, but it will still come back because it's lazy loaded.
                // it will hit the database on this line. This is enabled by default and requires DateEvents to be virtual
                var dateEvents = date.DateEvents;
                foreach (var dateEvent in dateEvents)
                {
                    // Again, lazy loaded. You can see why this would be bad for performance
                    Console.WriteLine("\t" + dateEvent.Event.Description);
                }
            }

            var events = demo.GetAllEvents();
            foreach (var evt in events)
            {
                Console.WriteLine(evt.Description);
                var allowedDateNumbers = demo.GetAllowedDateNumbersForEventDescription(evt.Description);

                Console.WriteLine("Allowed Numbers: " + string.Join(", ", allowedDateNumbers));
            }
        }
    }
}
