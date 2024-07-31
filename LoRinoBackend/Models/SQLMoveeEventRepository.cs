using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LoRinoBackend.Models
{
    public class SQLMoveeEventRepository : IMoveeEventRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SQLMoveeEventRepository> _logger;

        public SQLMoveeEventRepository(AppDbContext context,
                                    ILogger<SQLMoveeEventRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public int GetActiveAlarmsByLocationId(int locationId)
        {
            var activeAlarms = _context.MoveeEventFrame.Where(a => a.LocationId == locationId && a.IsAcked == false);

            int alarmCount = 0;
            foreach (var alarm in activeAlarms)
            {
                alarmCount = alarmCount + alarm.AlarmCount;
            }
            return alarmCount;
        }

        public int GetEventIdFromDataFrameId(int id)
        {
            return _context.MoveeDataFrame.FirstOrDefault(a => a.Id == id).MoveeEventFrameId;
        }

        public int GetEventIdFromGuid(string guid)//bug
        {
            return _context.MoveeEventFrame.FirstOrDefault(a => a.Guid == guid).Id;
        }

        public string GetGuidFromEventId(int eventId)
        {
            return _context.MoveeEventFrame.FirstOrDefault(a => a.Id == eventId).Guid;
        }

        public List<MoveeEventFrame> GetAllActiveEvents()
        {
            var activeEvents = _context.MoveeEventFrame.Where(a => a.IsAcked == false && a.LocationId != 0).ToList();

            return activeEvents;
        }

        public List<MoveeEventFrame> GetAllConfirmedEvents()
        {
            var activeEvents = _context.MoveeEventFrame.Where(a => a.IsAcked == true && a.IsCleared == false && a.LocationId != 0).ToList();

            return activeEvents;
        }

        public int CountAllActiveEvents()
        {

            var activeEvents = _context.MoveeEventFrame.Where(a => a.IsAcked == false && a.LocationId != 0).Count();

            return activeEvents;
        }

        public int CountAllConfirmedEvents()
        {
            var confirmedEvents = _context.MoveeEventFrame.Where(a => a.IsAcked == true && a.IsCleared == false && a.LocationId != 0).Count();

            return confirmedEvents;
        }

        public int AddEvent(MoveeEventFrame newData)
        {
            try
            {
                _context.MoveeEventFrame.Add(newData);
                _context.SaveChanges();
                return newData.Id;
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Problem with saving data");
                _logger.LogInformation(newData.ToString());
                _logger.LogInformation(ex.Message);
                return -1;
            }
        }

        public ICollection<MoveeEventFrame> GetAllData()
        {
            var events = _context.MoveeEventFrame.ToList();
            if (events != null)
            {
                return events;
            }
            return null;
            
        }
        //            return context.MoveeDataFrame.Include(ed => ed.Device).ThenInclude(dt => dt.DeviceType).GroupBy(d => d.Device.DevEui);


        public void UpdateAlarmCount(string guid, int count)
        {
            var updateAlarmCount = _context.MoveeEventFrame.Find(guid);
            updateAlarmCount.AlarmCount = count;
            _context.SaveChanges();
        }

        public void UpdateAckedMoveeEvent(int id, MoveeEventFrame moveeEventFrame)
        {
            var eventForUpdate = _context.MoveeEventFrame.Find(id);
            eventForUpdate.EventAckBy = moveeEventFrame.EventAckBy;
            eventForUpdate.EventAckTime = moveeEventFrame.EventAckTime;
            eventForUpdate.AckMessage = moveeEventFrame.AckMessage;
            eventForUpdate.IsAcked = true;
            _context.MoveeEventFrame.Update(eventForUpdate);
            _context.SaveChanges();
        }

        //public void UpdateSilentMoveeEvent(int id, MoveeEventFrame moveeEventFrame)
        //{
        //    var eventForUpdate = _context.MoveeEventFrame.Find(id);
        //    eventForUpdate.IsSilent = true;
        //    _context.MoveeEventFrame.Update(eventForUpdate);
        //    _context.SaveChanges();
        //}

        public void UpdateClearedMoveeEvent(int id, MoveeEventFrame moveeEventFrame)
        {
            var eventForUpdate = _context.MoveeEventFrame.Find(id);
            eventForUpdate.EventClearBy = moveeEventFrame.EventClearBy;
            eventForUpdate.EventClearTime = moveeEventFrame.EventClearTime;
            eventForUpdate.ClearMessage = moveeEventFrame.ClearMessage;
            eventForUpdate.IsCleared = true;
            _context.MoveeEventFrame.Update(eventForUpdate);
            _context.SaveChanges();
        }

        public void AckMoveeAlarmsByEventId(int id, MoveeEventFrame moveeEventFrame)
        {
            var alarms = _context.MoveeDataFrame.Where(a => a.MoveeEventFrameId == id);
            if (alarms != null)
            {
                foreach (var item in alarms)
                {
                    item.AckId = moveeEventFrame.EventAckBy;
                    item.AckMsg = true;
                    item.AckTime = moveeEventFrame.EventAckTime;
                }
                _context.SaveChanges();
            }
        }

        public string GetLocationNameFromEventId(int id)
        {
            var locations = _context.Location.ToList();
            var devices = _context.Device.ToList();
            var moveeEventFrames = _context.MoveeEventFrame.ToList();
            var moveeDataFrames = _context.MoveeDataFrame.ToList();

            string data = (from d in devices
                           join l in locations on d.LocationId equals l.Id
                           join mdf in moveeDataFrames on d.Id equals mdf.DeviceId
                           join mef in moveeEventFrames on mdf.MoveeEventFrameId equals mef.Id
                           where mef.Id == id
                           select l.Name).FirstOrDefault();

            return data;
        }

        public Location GetLocationFromEventId(int id)
        {
            var locations = _context.Location.ToList();
            var devices = _context.Device.ToList();
            var moveeEventFrames = _context.MoveeEventFrame.ToList();
            var moveeDataFrames = _context.MoveeDataFrame.ToList();

            var data = (from d in devices
                           join l in locations on d.LocationId equals l.Id
                           join mdf in moveeDataFrames on d.Id equals mdf.DeviceId
                           join mef in moveeEventFrames on mdf.MoveeEventFrameId equals mef.Id
                           where mef.Id == id
                           select l).FirstOrDefault();

            return data;
        }

        public List<MoveeEventFrame> GetEventsWithCompanyId(int companyId)
        {
            var locations = _context.Location.ToList();
            var moveeEventFrames = _context.MoveeEventFrame.ToList();

            var data = (from e in moveeEventFrames
                        join l in locations on e.LocationId equals l.Id
                           where l.CompanyId == companyId
                        select e).ToList();

            return data;
        }

        public List<MoveeEventFrame> GetEventsFromLocationId(int id)
        {
            var events = _context.MoveeEventFrame.Where(a => a.LocationId == id).ToList();
            return events;
        }

        public MoveeEventFrame GetMoveeEventById(int id)
        {
            var moveeEvent = _context.MoveeEventFrame.FirstOrDefault(a => a.Id == id);
            return moveeEvent;
        }

        public List<MoveeEventFrame> GetEventsWithTagId(int tagId)
        {
            var events = _context.MoveeEventFrame.ToList();
            List<MoveeEventFrame> moveeEvents = new List<MoveeEventFrame>();
            var eventTags = _context.MoveeEventTag.Where(a => a.MoveeTagId == tagId).Where(a => a.Active == true);
            foreach (var item in eventTags)
            {
                moveeEvents.Add(events.FirstOrDefault(a => a.Id == item.MoveeEventFrameId));
            }
            return moveeEvents;

        }
        public List<MoveeEventFrame> GetEventsWithTagIds(int[] tagIds)
        {
            var events = _context.MoveeEventFrame.ToList();
            List<MoveeEventFrame> moveeEvents = new List<MoveeEventFrame>();
            var eventTags = _context.MoveeEventTag.Where(a => tagIds.Contains(a.MoveeTagId)).Where(a => a.Active == true);
            foreach (var item in eventTags)
            {
                moveeEvents.Add(events.FirstOrDefault(a => a.Id == item.MoveeEventFrameId));
            }
            return moveeEvents;

        }

        public List<MoveeEventFrame> QueryStringFilter(string s, int recordsPerPage, int recordsForPageNo, bool orderByDesc, int[] tagIds, int companyId)
        {
            var filter = _context.MoveeEventFrame.OrderByDescending(a => a.Id).Where(a => a.Id != 1).ToList();

            // Filter By Name
            if (!string.IsNullOrEmpty(s))
            {
                filter = filter
                   .Where
                    (

                        p => p.AckMessage.Contains(s, StringComparison.CurrentCultureIgnoreCase) ||
                        p.ClearMessage.Contains(s, StringComparison.CurrentCultureIgnoreCase)

                    ).ToList();
            }

            // Filter By Tag
            if (tagIds.Any())
            {
                filter = GetEventsWithTagIds(tagIds);
            }

            // Filter By Company
            if (companyId != 0)
            {
                filter = GetEventsWithCompanyId(companyId).Where(a => a.Id != 1).ToList();
            }

            // Page
            if (recordsPerPage > 0 && recordsForPageNo > 0)
            {
                filter = filter.Skip((recordsForPageNo - 1) * recordsPerPage).Take(recordsPerPage).ToList();
            }

            // Sort
            if (orderByDesc)
            {
                filter = filter.OrderByDescending(r => r.EventCreationTime).ToList();
            }
            else
            {
                filter = filter.OrderBy(r => r.EventCreationTime).ToList();
            }

            return filter;
        }

        public int GetEventsCount()
        {
            return _context.MoveeEventFrame.Where(a => a.Id != 1).Count();
        }
    }
}