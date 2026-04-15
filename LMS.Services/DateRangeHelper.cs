using Domain.Models.Entities;
using Domain.Models.Exceptions;
using LMS.Shared.DTOs;

namespace LMS.Services
{
    internal class StartEnd : IComparable
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Context { get; set; } = "";
        public bool Persistent { get; set; }

        public void Throw(PersistentDataInconsistencyException.Diagnostic diag, string context = "")
        {
            context = string.IsNullOrEmpty(context) ? Context : context!;
            if (Persistent)
            {
                throw new PersistentDataInconsistencyException(diag, context);
            }
            string dstr = diag.ToString("F");
            throw new BadRequestException($"{dstr} at {context}");
        }

        public StartEnd(DateTime start, DateTime end, string context, bool persistent)
        {
            Start = start;
            End = end;
            Context = context;
            Persistent = persistent;
            if (End < Start) Throw(
                PersistentDataInconsistencyException.Diagnostic.NegativeTimeSpan);
        }
        private static string MakeContext(Course c)
        {
            return (c.Id == 0) ? $"CourseId=new" : $"CourseId={c.Id}";
        }
        private static string MakeContext(Module m)
        {
            if (m.Id == 0) return $"CourseId={m.CourseId} ModuleId=new";
            return $"CourseId={m.CourseId} moduleId={m.Id}";
        }
        private static string MakeContext(Activity a)
        {
            if (a.Id == 0)
            {
                return $"CourseId={a.Module.CourseId}"
                    + $"ModuleId={a.ModuleId} ActivityId=new";
            }
            return $"CourseId={a.Module.CourseId}"
                + $"ModuleId={a.ModuleId} ActivityId={a.Id}";
        }
        public StartEnd(Course c, bool? persistent = null)
            : this(
                  c.StartDate,
                  c.EndDate,
                  MakeContext(c),
                  persistent ?? (c.Id != 0))
        { }
        public StartEnd(Module m, bool? persistent = null)
            : this(
                  m.StartDate,
                  m.EndDate,
                  MakeContext(m),
                  persistent ?? (m.Id != 0))
        { }
        public StartEnd(Activity a, bool? persistent = null)
            : this(
                  a.StartDate,
                  a.EndDate,
                  MakeContext(a),
                  persistent ?? (a.Id != 0))
        { }
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
            Throw(
                PersistentDataInconsistencyException.Diagnostic.Overlapping,
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
                Context,
                persistent: false);
        }
    }
    internal class DateRangeHelper
    {
        private StartEnd bounds_;
        private DateTime freeStart_;
        private string freeStartContext_ = "";
        private List<StartEnd> intervals_;
        private List<StartEnd> freeIntervals_ = new();

        public void Throw(PersistentDataInconsistencyException.Diagnostic diag, string context2)
        {
            bounds_.Throw(diag, $"{bounds_.Context}: {context2}");
        }
        private DateRangeHelper(StartEnd bounds, List<StartEnd> intervals)
        {
            bounds_ = bounds;
            freeStart_ = bounds_.Start;
            intervals_ = intervals;
            intervals_.Sort();
            for (int i = 0; i < intervals_.Count; ++i)
            {
                var x = intervals_[i];
                if (!bounds_.Includes(x))
                {
                    Throw(
                        PersistentDataInconsistencyException.Diagnostic.OutOfTimeBounds,
                        x.Context);
                }
                for (int j = 0; j < i; ++j)
                {
                    x.CheckNotOverlapping(intervals_[j]);
                }
                StartEnd se = new StartEnd(
                    freeStart_,
                    x.Start,
                    $"{freeStartContext_}->x.Context",
                    persistent: false);
                freeStart_ = x.End;
                freeStartContext_ = x.Context;
                if (!se.IsTooSmall()) freeIntervals_.Add(se);
            }
            StartEnd seEnd = new StartEnd(
                freeStart_,
                bounds.End,
                $"{freeStartContext_}->",
                persistent: false);
            if (!seEnd.IsTooSmall()) freeIntervals_.Add(seEnd);
        }

        public DateRangeHelper(Course course)
            : this(
                 new StartEnd(course),
                 course.Modules.Select(m => new StartEnd(m)).ToList())
        { }
        public DateRangeHelper(Module module)
            : this(
                 new StartEnd(module),
                 module.Activities.Select(a => new StartEnd(a)).ToList())
        { }
        public DateRangeResponseDto? GetDateRange(DateRangeRequestDto request)
        {
            if (request == null)
            {
                return null;
            }

            foreach (var f in freeIntervals_)
            {
                DateTime startingAfter = OneOf(request.StartingAfter, f.Start);
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
            if (!bounds_.Includes(se)) se.Throw(
                PersistentDataInconsistencyException
                .Diagnostic
                .OutOfTimeBounds,
                bounds_.Context);
            foreach (var x in intervals_)
            {
                if (x.Overlaps(se)) se.Throw(
                    PersistentDataInconsistencyException
                    .Diagnostic
                    .Overlapping,
                    x.Context);
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
        public void CheckNewBounds(StartEnd bounds)
        {
            foreach (var x in intervals_)
            {
                if (!bounds.Includes(x)) bounds.Throw(
                    PersistentDataInconsistencyException
                    .Diagnostic
                    .OutOfTimeBounds,
                    x.Context);
            }
        }
        public void CheckIntervalChange(StartEnd oldInt, StartEnd newInt)
        {
            if (!bounds_.Includes(newInt)) newInt.Throw(
                PersistentDataInconsistencyException
                .Diagnostic
                .OutOfTimeBounds,
                bounds_.Context);
            bool foundOldInt = false;
            foreach (var x in intervals_)
            {
                if (x.Start == oldInt.Start && x.End == oldInt.End)
                {
                    foundOldInt = true;
                    continue;
                }
                if (newInt.Overlaps(x)) newInt.Throw(
                    PersistentDataInconsistencyException
                    .Diagnostic.Overlapping,
                    x.Context);
            }
            if (foundOldInt) return;
            throw new NotFoundException($"{oldInt} not in {bounds_.Context}");
        }
    }
}
