using Dav.Ef.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dav.Ef
{
    public class EfDbDemo
    {
        private readonly iSwoonDbContext _db;

        public EfDbDemo()
        {
            _db = new iSwoonDbContext();
        }

        public IEnumerable<Date> GetAllDates()
        {
            // No properties will initially be populated
            return _db.Dates.ToList();
        }

        public Date GetDateByNumber(int dateNumber)
        {
            var date = _db.Dates.SingleOrDefault(d => d.DateNumber == dateNumber);
            if (date == null)
            {
                throw new Exception("The date number was not found in the database.");
            }
            return date;
        }

        public void CreateDate(int dateNumber)
        {
            if (_db.Dates.Any(d => d.DateNumber == dateNumber))
            {
                throw new ArgumentException("Date number already exists");
            }
            _db.Dates.Add(new Date
            {
                DateNumber = dateNumber
            });
            _db.SaveChanges();
        }

        public Date CreateDateAndEvent(int dateNumber, string description, int[] allowedDateNumbers)
        {
            if (!allowedDateNumbers.Contains(dateNumber))
            {
                throw new ArgumentException("Hey idiot  you didn't include the date you're adding as an allowed number");
            }
            
            var evt = new Event
            {
                Description = description,
                AllowedDateNumbers = allowedDateNumbers.Select(n => new EventAllowedDateNumber { DateNumber = n }).ToList()
            };

            var date = new Date
            {
                DateNumber = dateNumber,
                DateEvents = new[] { new DateEvent { Event = evt } }
            };

            _db.Dates.Add(date);
            _db.SaveChanges();

            return date;
        }

        public bool AddEventToDate(Date date, Event evt)
        {
            if (evt.AllowedDateNumbers.Any(adn => adn.DateNumber == date.DateNumber))
            {
                // explicit load when the entity already exists
                _db.Entry(date).Collection(nameof(date.DateEvents)).Load();
                if (date.DateEvents.Any(de => de.EventId == evt.Id))
                {
                    throw new ArgumentException("date already has that event");
                }

                _db.DateEvents.Add(new DateEvent { Date = date, Event = evt });
                _db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AddEventToDateByNumberAndDescription(int dateNumber, string eventDescription)
        {
            return AddEventToDate(
                GetDateByNumber(dateNumber),
                GetEventByDescription(eventDescription));
        }

        public IEnumerable<Event> GetAllEvents()
        {
            return _db.Events.ToList();
        }

        public Event GetEventByDescription(string description)
        {
            // This will throw an exception on its own if it does not exist
            return _db.Events
                .Include(e => e.AllowedDateNumbers) // explicit loading instead of lazy
                .Single(e => e.Description == description);
        }

        public IEnumerable<Event> GetEventsAvailbleForDateNumber(int dateNumber)
        {
            return _db.Events.Where(e => e.AllowedDateNumbers.Any(adn => adn.DateNumber == dateNumber)).ToList();
        }

        public int[] GetAllowedDateNumbersForEventDescription(string description)
        {
            var evt = GetEventByDescription(description);
            // lazy loading
            var eventDateNumbers = evt.AllowedDateNumbers.ToList();
            return eventDateNumbers.Select(edn => edn.DateNumber).ToArray();
        }

        public void CreateEvent(string description, int[] dateNums)
        {
            var allowedDateNumbers = new List<EventAllowedDateNumber>();
            foreach (var num in dateNums)
            {
                allowedDateNumbers.Add(new EventAllowedDateNumber
                {
                    DateNumber = num,
                });
            }

            var evt = new Event
            {
                Description = description,
                AllowedDateNumbers = allowedDateNumbers
            };

            _db.Events.Add(evt);
            _db.SaveChanges();
        }
    }
}
