using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace LoRinoBackend.Models
{
    public class SQLMoveeEventCommentRepository : IMoveeEventCommentRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SQLMoveeEventRepository> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private IHttpContextAccessor _httpContextAccessor;

        public SQLMoveeEventCommentRepository(AppDbContext context, ILogger<SQLMoveeEventRepository> logger, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public List<MoveeEventComment> GetAllCommentsByEventId(int eventId)
        {
            var comments = _context.MoveeEventComment.Where(a => a.MoveeEventFrameId == eventId).ToList();
            return comments;
        }

        public MoveeEventComment AddComment(MoveeEventComment moveeEventComment)
        {
                _context.MoveeEventComment.Add(moveeEventComment);
            _context.SaveChanges();
            return moveeEventComment;
        }
        
        public MoveeEventComment GetCommentDetails(int id)
        {
            var comment = _context.MoveeEventComment.FirstOrDefault(a => a.Id == id);


            return comment;
        }

        public MoveeEventComment DeleteComment(int commentId)
        {
            MoveeEventComment comment = _context.MoveeEventComment.Find(commentId);
            if (comment != null)
            {
                _context.MoveeEventComment.Remove(comment);
                _context.SaveChanges();
            }
            return comment;
        }

        public MoveeEventComment Update(MoveeEventComment commentChanges)
        {
            var comment = _context.MoveeEventComment.Attach(commentChanges);
            comment.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return commentChanges;
        }
        public IEnumerable<MoveeTag> GetMoveeTags()
        {
            return _context.MoveeTag.ToList();
        }

        public IEnumerable<MoveeEventTag> GetAllMoveeEventTagsByEventId(int eventId)
        {
            return _context.MoveeEventTag.Where(a => a.MoveeEventFrameId == eventId).ToList();
        }

        public IEnumerable<EventTagLog> GetAllEventTagsLogByEventId(int eventId)
        {
            return _context.EventTagLog.Where(a => a.EventId == eventId).ToList();
        }

        public List<LogViewModel> GetLogs(int eventId)
        {
            var comments = GetAllCommentsByEventId(eventId);

            var eventTagsLog = GetAllEventTagsLogByEventId(eventId);

            var moveeEventTags = GetAllMoveeEventTagsByEventId(eventId);

            var tags = GetMoveeTags();

            var moveeEvent = _context.MoveeEventFrame.FirstOrDefault(a => a.Id == eventId);


            List<LogViewModel> logs = new List<LogViewModel>();
            if (comments != null)
            {
                foreach (var item in comments)
                {
                    LogViewModel log = new LogViewModel();
                    log.LogTime = item.EventCommentTime;
                    log.LogBy = item.EventCommentBy;
                    log.MessageType = MessageType.Komentar;
                    log.Message = item.Comment;
                    logs.Add(log);
                }
            }

            if (eventTagsLog != null)
            {
                foreach (var item in eventTagsLog)
                {
                    int messageId = moveeEventTags.FirstOrDefault(a => a.Id == item.MoveeEventTagId).MoveeTagId;
                    LogViewModel log = new LogViewModel();
                    log.LogTime = item.EventTagTime;
                    log.LogBy = item.EventTagBy;
                    log.MessageType = MessageType.Tag;
                    log.Action = item.Action;
                    log.Message = tags.FirstOrDefault(a => a.Id == messageId).Name;
                    logs.Add(log);
                }
            }

            if (!string.IsNullOrEmpty(moveeEvent.AckMessage))
            {
                LogViewModel log = new LogViewModel();
                log.LogTime = moveeEvent.EventAckTime;
                log.LogBy = moveeEvent.EventAckBy;
                log.MessageType = MessageType.Potvrda;
                log.Message = moveeEvent.AckMessage;
                logs.Add(log);
            }

            if (!string.IsNullOrEmpty(moveeEvent.ClearMessage))
            {
                LogViewModel log = new LogViewModel();
                log.LogTime = moveeEvent.EventClearTime;
                log.LogBy = moveeEvent.EventClearBy;
                log.MessageType = MessageType.Zatvaranje;
                log.Message = moveeEvent.ClearMessage;
                logs.Add(log);
            }


            return logs;
        }
    }
}