using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using LoRinoBackend.ViewModels;
using System.Diagnostics.Tracing;
using Microsoft.Extensions.Logging;

namespace LoRinoBackend.Models
{
    public class SQLMoveeEventTagRepository : IMoveeEventTagRepository
    {
        private readonly AppDbContext contex;
        private readonly UserManager<ApplicationUser> userManager;
        private IHttpContextAccessor httpContextAccessor;

        public SQLMoveeEventTagRepository(AppDbContext contex, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this.contex = contex;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            //_user = userManager.Users.Include(c => c.Company).FirstOrDefault();

            //var _company = _user.Email; 
        }

        public string GetCompanyNameFromTagId(int id)
        {
            return contex.Company.FirstOrDefault(a => a.Id == id).Name;
        }

        public IEnumerable<MoveeTag> GetMoveeTags()
        {
            return contex.MoveeTag.ToList();
        }

        public IEnumerable<MoveeTag> GetActiveMoveeTags()
        {
            return contex.MoveeTag.Where(a => a.Active == true).ToList();
        }

        public MoveeTag GetMoveeTagById(int id)
        {
            return contex.MoveeTag.FirstOrDefault(a => a.Id == id);
        }

        public void CreateMoveeTag(MoveeTag moveeTag)
        {
            contex.MoveeTag.Add(moveeTag);
            contex.SaveChanges();
        }

        public MoveeTag DeleteMoveeTag(MoveeTag moveeTag)
        {
                MoveeTag tagForDelete = contex.MoveeTag.Find(moveeTag.Id);
                if (tagForDelete != null)
                {
                tagForDelete.Active = false;
                    contex.SaveChanges();
                }
                return tagForDelete;
        }

        public MoveeTag UpdateMoveeTag(MoveeTag moveeTag)
        {
            var tagForUpdate = contex.MoveeTag.Attach(moveeTag);
            tagForUpdate.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            contex.SaveChanges();
            return tagForUpdate.Entity;
        }

        public IEnumerable<MoveeEventTag> GetAllMoveeEventTags()
        {
            return contex.MoveeEventTag.ToList();
        }

        public MoveeEventTag GetMoveeEventTagById(int id)
        {
            var moveeEventTag = contex.MoveeEventTag.FirstOrDefault(c => c.Id == id);

            return moveeEventTag;
        }

        public void UpdateMoveeEventTag(MoveeEventTag moveeEventTag)
        {
            contex.MoveeEventTag.Update(moveeEventTag);
            contex.SaveChanges();
        }

        public void CreateMoveeEventTag(MoveeEventTag moveeEventTag, string by, long time)
        {
            contex.MoveeEventTag.Add(moveeEventTag);
            contex.SaveChanges();

            LogAddedEventTag(moveeEventTag.MoveeEventFrameId, moveeEventTag.Id, by, time);
        }

        public MoveeEventTag RemoveMoveeEventTag(MoveeEventTag moveeEventTag, string by, long time)
        {
            MoveeEventTag tagForDelete = contex.MoveeEventTag.Find(moveeEventTag.Id);
            if (tagForDelete != null)
            {
                LogRemovedEventTag(moveeEventTag.MoveeEventFrameId, moveeEventTag.Id, by, time);
                tagForDelete.Active = false;
                contex.SaveChanges();
            }
            return tagForDelete;

        }

        public void LogAddedEventTag(int eventId, int eventTagId, string by, long time)
        {
            EventTagLog eventTagLog = new EventTagLog()
            {
                MoveeEventTagId = eventTagId,
                Action = "Dodan",
                EventTagBy = by,
                EventTagTime = time,
                EventId = eventId
            };
            contex.EventTagLog.Add(eventTagLog);
            contex.SaveChanges();
        }

        public void LogRemovedEventTag(int eventId, int eventTagId, string by, long time)
        {
            EventTagLog eventTagLog = new EventTagLog()
            {
                MoveeEventTagId = eventTagId,
                Action = "Obrisan",
                EventTagBy = by,
                EventTagTime = time,
                EventId = eventId
            };
            contex.EventTagLog.Add(eventTagLog);
            contex.SaveChanges();
        }

        public List<SelectListItem> TagsForDropDownList(List<int> ints, int companyId)
        {
            return contex.MoveeTag.Where(a => a.Active == true && a.CompanyId == companyId && !ints.Contains(a.Id) ).Select(c => new SelectListItem()
            {
                Value = c.Id.ToString(),
                Text = c.Name,
            }).ToList();
        }

        public List<SelectListItem> AllTagsForDropDownList(int companyId)
        {
            return contex.MoveeTag.Where(a => a.Active == true && a.CompanyId == companyId).Select(c => new SelectListItem()
            {
                Value = c.Id.ToString(),
                Text = c.Name,
            }).ToList();
        }

        public List<MoveeTag> TagsList()
        {
            return contex.MoveeTag.ToList();
        }

        public List<int> GetMyDeletedTagLogs(int eventId, string userId)
        {
            var moveeEventTags = contex.MoveeEventTag.Where(a => a.Active == false && a.MoveeEventFrameId == eventId).ToList();
            var list = contex.EventTagLog.Where(a => a.EventTagBy == userId && a.EventId == eventId && a.Action == "Obrisan").ToList();
            List<int> tags = new List<int>();
            var qs_inner_join = from v in list
                                join u in moveeEventTags
                                on v.MoveeEventTagId equals u.Id
                                select new
                                {
                                    Id = u.MoveeTagId,
                                };

            foreach (var item in qs_inner_join)
            {
                tags.Add(item.Id);
            }
            return tags;
        }

        public List<int> GetMyAddedTagLogs(int eventId, string userId)
        {
            var moveeEventTags = contex.MoveeEventTag.Where(a => a.Active == true && a.MoveeEventFrameId == eventId).ToList();
            var list = contex.EventTagLog.Where(a => a.EventTagBy == userId && a.EventId == eventId && a.Action == "Dodan").ToList();
            List<int> tags = new List<int>();
            var qs_inner_join = from v in list
                                join u in moveeEventTags
                                on v.MoveeEventTagId equals u.Id
                                select new
                                {
                                    Id = u.MoveeTagId,
                                };

            foreach (var item in qs_inner_join)
            {
                tags.Add(item.Id);
            }
            return tags;
        }

        public string GetTagOwner(int moveeEventTagId)
        {
            string owner = contex.EventTagLog.OrderBy(a => a.Id).LastOrDefault(a => a.MoveeEventTagId == moveeEventTagId).EventTagBy;
            return owner;
        }

    }
}