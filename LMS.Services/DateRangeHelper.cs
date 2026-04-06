using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Services
{
    internal class StartEnd : IComparable
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Context { get; set; } = "";

        private StartEnd() {}

        public static StartEnd NewModule(DateTime start, DateTime end, int courseId)
        {
            string context = $"CourseId={courseId} moduleId=new";
            string diag = PersistentDataInconsistencyException
                .Diagnostic
                .NegativeTimeSpan
                .ToString("F");
            if (end < start) throw new BadRequestException($"{diag} at {context}");
            return new StartEnd { Start = start, End = end, Context = context };
        }

        public static StartEnd NewActivity(DateTime start, DateTime end, int courseId, int moduleId)
        {
            string context = $"CourseId={courseId} ModuleId={moduleId} ActivityId=new";
            string diag = PersistentDataInconsistencyException
                .Diagnostic
                .NegativeTimeSpan
                .ToString("F");
            if (end < start) throw new BadRequestException($"{diag} at {context}");
            return new StartEnd { Start = start, End = end, Context = context };
        }

        public StartEnd(DateTime start, DateTime end, string context)
        {
            Start = start;
            End = end;
            Context = context;
            if (End < Start) throw new PersistentDataInconsistencyException(
                PersistentDataInconsistencyException
                    .Diagnostic
                    .NegativeTimeSpan,
                Context);
        }

        public StartEnd(Course c)
            : this(c.StartDate, c.EndDate, $"CourseId={c.Id}") {}
        public StartEnd(Module m) 
            : this(
                  m.StartDate, 
                  m.EndDate, 
                  $"CourseId={m.CourseId} ModuleId={m.Id}") {}
        public StartEnd(Activity a)
            : this(
                  a.StartDate,
                  a.EndDate,
                  $"CourseId={a.Module.CourseId}"
                    +$" ModuleId={a.ModuleId}" 
                    +$" ActivityId={a.Id}") {}
        public bool Includes(StartEnd other)
        {
            if (Start > other.Start) return false;
            if (End < other.End) return false;
            return true;
        }
        public bool Overlaps(StartEnd other)
        {
            if (Start > other.Start) return other.Overlaps(this);
            return End > other.Start;
        }
        public void CheckNotOverlapping(StartEnd other)
        {
            if (!Overlaps(other)) return;
            throw new PersistentDataInconsistencyException(
                PersistentDataInconsistencyException
                    .Diagnostic
                    .Overlapping,
                $"{Context} : {other.Context}");

        }

        public int CompareTo(object? obj)
        {
            if (obj == null) return 1;
            StartEnd? se = obj as StartEnd;
            if (se == null) return 1;
            CheckNotOverlapping(se);
            return Start.CompareTo(se.Start);
        }
        public static readonly TimeSpan MinimumInterval
            = new TimeSpan(0, 30, 0);  // 30 minutes.
        public bool IsTooSmall()
        {
            return (End - Start) < MinimumInterval;
        }
        public StartEnd? Fit(DateTime startingAfter, DateTime endingBefore, TimeSpan duration)
        {
            if (duration < MinimumInterval) return null;
            if ((startingAfter + duration) > End) return null;
            if ((endingBefore - duration) < Start) return null;
            return new StartEnd(
                startingAfter,
                startingAfter + duration,
                Context);
        }
    }
    internal class DateRangeHelper
    {
        private StartEnd bounds_;
        private DateTime freeStart_;
        private string freeStartContext_ = "";
        private List<StartEnd> intervals_;
        private List<StartEnd> freeIntervals_ = new();
        private DateRangeHelper(StartEnd bounds, List<StartEnd> intervals)
        {
            bounds_ = bounds;
            freeStart_ = bounds_.Start;
            intervals_ = intervals;
            intervals_.Sort();
            for (int i = 0; i < intervals_.Count; ++i)
            {
                if (!bounds.Includes(intervals_[i])) {
                    throw new PersistentDataInconsistencyException(
                        PersistentDataInconsistencyException
                            .Diagnostic
                            .OutOfTimeBounds,
                        intervals_[i].Context);
                }
                var x = intervals_[i];
                for (int j = 0; j < i; ++j)
                {
                    x.CheckNotOverlapping(intervals_[j]);
                }
                StartEnd se = new StartEnd(
                    freeStart_,
                    x.Start,
                    $"{freeStartContext_}->x.Context");
                freeStart_ = x.End;
                freeStartContext_ = x.Context;
                if (!se.IsTooSmall()) freeIntervals_.Add(se);
            }
            StartEnd seEnd = new StartEnd(
                freeStart_,
                bounds.End,
                $"{freeStartContext_}->");
            if (!seEnd.IsTooSmall()) freeIntervals_.Add(seEnd);
        }

        public DateRangeHelper(Course course)
            :this(
                 new StartEnd(course),
                 course.Modules.Select(m => new StartEnd(m)).ToList())
        {}
        public DateRangeHelper(Module module)
            :this(
                 new StartEnd(module),
                 module.Activities.Select(a => new StartEnd(a)).ToList())
        {}
        public DateRangeResponseDto? GetDateRange(DateRangeRequestDto request)
        { 
            foreach (var f in freeIntervals_)
            {
                DateTime startingAfter = OneOf(request.StartingAfter,f.Start);
                DateTime endingBefore = OneOf(request.EndingBefore, f.End);
                StartEnd? se = f.Fit(startingAfter, 
                    endingBefore,
                    request.Duration);
                if (se != null) return new DateRangeResponseDto
                {
                    Start = se!.Start,
                    End = se!.End
                };
            }
            return null;
        }
        public void CheckNew(StartEnd se)
        {
            if (!bounds_.Includes(se)) throw new BadRequestException("time parameters");
            foreach (var x in intervals_)
            {
                if (x.Overlaps(se)) throw new BadRequestException("time parameters, overlapping");
            }
        }
        static public DateTime OneOf(DateTime? t1, DateTime t2)
        {
            if (t1 == null) return t2;
            DateTime zero = default;
            if (t1.Value == zero) return t2;
            return t1.Value;
        }
        static public bool Absent(DateTime? t)
        {
            if (t == null) return true;
            DateTime zero = default;
            if (t == zero) return true;
            return false;
        }

    }
}
